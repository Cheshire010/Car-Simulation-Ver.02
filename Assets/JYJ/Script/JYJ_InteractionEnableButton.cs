using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using static JYJ_RaycastInteractor;

public class JYJ_InteractionEnableButton : MonoBehaviour, IInteractable
{
    [Header("애니메이션 설정")]
    public Animator animator;
    public float interactionCooldown = 0.5f;

    [Header("상호작용 관리")]
    public JYJ_StroyManager storyManager;

    [Header("UI/사운드 설정")]
    public GameObject chatPanel;
    public Text messageText;
    public string openMessage = "상호작용이 활성화되었습니다!";
    public float messageDuration = 2.0f;
    public AudioSource audioSource;
    public AudioClip[] sounds;

    [Header("유도 효과 설정")]
    [SerializeField] private Color[] glowColors = { Color.red, Color.white }; // 빨강-흰색으로 초기화
    [SerializeField] private float blinkInterval = 0.5f; // 깜빡임 간격
    [SerializeField] private float emissionIntensity = 2f;
    private Material materialInstance;
    private Renderer objectRenderer;
    private Coroutine glowCoroutine;

    private bool canInteract = true;
    private bool hasOpened = false;
    private Coroutine messageCoroutine = null;
    private bool isMessageActive = false;

    public delegate void EnabledHandler();
    public event EnabledHandler OnEnabled;

    public void Interact()
    {
        if (!canInteract || hasOpened) return;

        if (glowCoroutine != null)
        {
            StopCoroutine(glowCoroutine);
            glowCoroutine = null;
        }

        if (materialInstance != null)
        {
            materialInstance.SetColor("_EmissionColor", Color.white);
        }

        canInteract = false;
        hasOpened = true;

        PlayOpenAnimation();
        StartCoroutine(InteractionCooldown());

        if (chatPanel != null)
            SetActiveRecursively(chatPanel, true);

        if (messageText != null)
        {
            messageText.gameObject.SetActive(true);
            messageCoroutine = StartCoroutine(ShowMessage(openMessage));
        }
        PlaySounds();

        OnEnabled?.Invoke();
    }
    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer != null)
        {
            materialInstance = new Material(objectRenderer.material);
            objectRenderer.material = materialInstance;
        }

        if (!hasOpened)
        {
            glowCoroutine = StartCoroutine(BlinkEffect());
        }
    }
    void Update()
    {
        if (isMessageActive && Input.GetKeyDown(KeyCode.Space))
        {
            EndMessage();
        }
    }

    void PlayOpenAnimation()
    {
        if (animator != null)
            animator.SetTrigger("Open");
        else
            Debug.LogWarning("Animator가 할당되지 않았습니다.", this);

        if (storyManager != null)
            storyManager.EnableInteractions();
        else
            Debug.LogWarning("StoryManager가 할당되지 않았습니다.", this);
    }

    void PlaySounds()
    {
        if (audioSource != null && sounds != null)
        {
            foreach (var clip in sounds)
            {
                if (clip != null)
                    audioSource.PlayOneShot(clip);
            }
        }
    }
    private IEnumerator BlinkEffect()
    {
        int colorIndex = 0;
        while (true)
        {
            if (materialInstance != null && glowColors.Length >= 2)
            {
                materialInstance.EnableKeyword("_EMISSION");
                Color currentColor = glowColors[colorIndex % 2];
                materialInstance.SetColor("_EmissionColor", currentColor * emissionIntensity);
                colorIndex++;
            }
            yield return new WaitForSeconds(blinkInterval);
        }
    }
    IEnumerator ShowMessage(string message)
    {
        messageText.text = message;
        messageText.enabled = true;
        isMessageActive = true;
        float timer = 0f;
        while (timer < messageDuration)
        {
            if (!isMessageActive) break;
            timer += Time.deltaTime;
            yield return null;
        }
        EndMessage();
    }

    void EndMessage()
    {
        isMessageActive = false;
        if (messageText != null)
        {
            messageText.enabled = false;
            messageText.gameObject.SetActive(false);
        }
        if (chatPanel != null)
            SetActiveRecursively(chatPanel, false); // 패널과 모든 자식 비활성화
        if (messageCoroutine != null)
        {
            StopCoroutine(messageCoroutine);
            messageCoroutine = null;
        }
    }

    IEnumerator InteractionCooldown()
    {
        yield return new WaitForSeconds(interactionCooldown);
        canInteract = true;
    }

    // 패널과 모든 자식 오브젝트 제어 함수
    void SetActiveRecursively(GameObject obj, bool active)
    {
        obj.SetActive(active);
        foreach (Transform child in obj.transform)
        {
            SetActiveRecursively(child.gameObject, active);
        }
    }
}
