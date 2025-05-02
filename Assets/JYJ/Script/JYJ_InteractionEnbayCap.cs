using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using static JYJ_RaycastInteractor;
using UnityEngine.Events;

public class JYJ_InteractionEnbayCap : MonoBehaviour, IInteractable
{
    [Header("후드 설정")]
    public Animator hoodAnimator;
    public float interactionCooldown = 0.5f;
    private bool canInteract = true;
    private bool isHoodOpen = false;

    [Header("UI 설정")]
    public GameObject chatPanel;
    public Text messageText;
    public string openMessage = "후드가 열렸습니다!";
    public float messageDuration = 2.0f;

    [Header("사운드 설정")]
    public AudioSource audioSource;
    public AudioClip[] soundClips;

    [Header("유도 효과 설정")]
    [SerializeField] private Color[] glowColors = { Color.red, Color.white };
    [SerializeField] private float blinkInterval = 0.5f;
    [SerializeField] private float emissionIntensity = 2f;
    private Material[] materialInstances;
    private Renderer objectRenderer;
    private Coroutine glowCoroutine;

    private bool isFirstOpen = true;
    private bool isDialogueActive = false;

    public JYJ_BlinkOnInteract blinkTarget;
    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer != null)
        {
            Material[] originalMats = objectRenderer.materials;
            materialInstances = new Material[originalMats.Length];
            for (int i = 0; i < originalMats.Length; i++)
            {
                materialInstances[i] = new Material(originalMats[i]);
            }
            objectRenderer.materials = materialInstances;
            glowCoroutine = StartCoroutine(BlinkEffect());
        }
        else
        {
            Debug.LogWarning("Renderer 컴포넌트가 없습니다.", this);
        }
    }

    public void Interact()
    {
        if (Input.GetMouseButtonDown(0) && canInteract)
        {
            // 깜빡임 중지 및 모든 머티리얼을 흰색으로 초기화
            if (glowCoroutine != null)
            {
                StopCoroutine(glowCoroutine);
                foreach (var mat in materialInstances)
                {
                    mat.EnableKeyword("_EMISSION");
                    mat.SetColor("_EmissionColor", Color.white * emissionIntensity);
                }
            }

            OpenHoodOnce();
            TriggerUseItemEvent();
            StartCoroutine(InteractionCooldown());
        }
        if (blinkTarget != null)
            blinkTarget.Interact();
    }

    private IEnumerator BlinkEffect()
    {
        int colorIndex = 0;
        while (true)
        {
            if (materialInstances != null)
            {
                Color targetColor = glowColors[colorIndex % glowColors.Length];
                foreach (var mat in materialInstances)
                {
                    mat.EnableKeyword("_EMISSION");
                    mat.SetColor("_EmissionColor", targetColor * emissionIntensity);
                }
                colorIndex++;
            }
            yield return new WaitForSeconds(blinkInterval);
        }
    }

    void OpenHoodOnce()
    {
        if (isHoodOpen) return;
        isHoodOpen = true;
        if (hoodAnimator != null)
            hoodAnimator.SetTrigger("Open");
    }

    void TriggerUseItemEvent()
    {
        if (isFirstOpen && isHoodOpen)
        {
            StartCoroutine(ShowDialogue());
            isFirstOpen = false;
        }
    }

    IEnumerator ShowDialogue()
    {
        SetActiveRecursively(chatPanel, true);
        SetActiveRecursively(messageText.gameObject, true);

        isDialogueActive = true;

        messageText.enabled = true;

        for (int i = 0; i < soundClips.Length; i++)
        {
            var clip = soundClips[i];
            if (clip == null) continue;
            messageText.text = openMessage;
            audioSource.clip = clip;
            audioSource.Play();

            float timer = 0f;

            while (timer < messageDuration)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    break;
                }
                timer += Time.deltaTime;
                yield return null;
            }
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }

        SetActiveRecursively(chatPanel, false);
        SetActiveRecursively(messageText.gameObject, false);
        isDialogueActive = false;
    }

    IEnumerator InteractionCooldown()
    {
        canInteract = false;
        yield return new WaitForSeconds(interactionCooldown);
        canInteract = true;
    }

    void SetActiveRecursively(GameObject obj, bool active)
    {
        if (obj == null) return;
        obj.SetActive(active);
        foreach (Transform child in obj.transform)
        {
            SetActiveRecursively(child.gameObject, active);
        }
    }

    private void OnDisable()
    {
        if (materialInstances != null)
        {
            foreach (var mat in materialInstances)
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", Color.white * emissionIntensity);
            }
        }
    }

    private void OnDestroy()
    {
        if (materialInstances != null)
        {
            foreach (var mat in materialInstances)
            {
                Destroy(mat);
            }
        }
    }
}
