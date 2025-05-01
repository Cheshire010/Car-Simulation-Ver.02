using UnityEngine;
using UnityEngine.UI;
using static JYJ_RaycastInteractor;
using System.Collections;

public class JYJ_InteractWash : MonoBehaviour, IInteractable
{
    [Header("애니메이터 설정")]
    public Animator washerAnimator;
    public string animationName = "Washer";

    [Header("파티클 설정")]
    public ParticleSystem particleEffect;

    [Header("UI 설정")]
    public GameObject chatPanel;
    public Text messageText;
    public string[] completionMessage = { "테이블에 문제가 생성되었습니다. 문제를 풀어주십시오." };

    [Header("활성화 오브젝트")]
    public GameObject tableObject;

    [Header("사운드 설정")]
    public AudioSource audioSource;
    public AudioClip[] soundClips;

    [Header("깜빡임 설정")]
    [SerializeField] private float blinkInterval = 0.5f;
    [SerializeField] private Color blinkColor = Color.red;
    [SerializeField] private float emissionIntensity = 2f;

    private Coroutine messageCoroutine;
    private bool isMessageActive = false;

    // 깜빡임 관련 변수
    private Renderer objectRenderer;
    private Material[] materialInstances;
    private Color[] originalEmissionColors;
    private Coroutine blinkCoroutine;
    private bool isBlinking = false;

    void OnEnable()
    {
        InitBlinkMaterials();
        StartBlinking();
    }

    void OnDisable()
    {
        StopBlinking();
        RestoreOriginalEmissionColors();
    }

    void InitBlinkMaterials()
    {
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer != null)
        {
            Material[] originalMats = objectRenderer.materials;
            materialInstances = new Material[originalMats.Length];
            originalEmissionColors = new Color[originalMats.Length];
            for (int i = 0; i < originalMats.Length; i++)
            {
                materialInstances[i] = new Material(originalMats[i]);
                if (materialInstances[i].HasProperty("_EmissionColor"))
                {
                    originalEmissionColors[i] = materialInstances[i].GetColor("_EmissionColor");
                    materialInstances[i].EnableKeyword("_EMISSION");
                }
                else
                {
                    originalEmissionColors[i] = Color.black;
                }
            }
            objectRenderer.materials = materialInstances;
        }
    }

    void StartBlinking()
    {
        if (!isBlinking && materialInstances != null)
        {
            blinkCoroutine = StartCoroutine(BlinkEffect());
            isBlinking = true;
        }
    }

    void StopBlinking()
    {
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }
        isBlinking = false;
    }

    void RestoreOriginalEmissionColors()
    {
        if (materialInstances != null && originalEmissionColors != null)
        {
            for (int i = 0; i < materialInstances.Length; i++)
            {
                if (materialInstances[i].HasProperty("_EmissionColor"))
                {
                    materialInstances[i].SetColor("_EmissionColor", originalEmissionColors[i]);
                }
            }
        }
    }

    private IEnumerator BlinkEffect()
    {
        int state = 0;
        while (true)
        {
            for (int i = 0; i < materialInstances.Length; i++)
            {
                if (materialInstances[i].HasProperty("_EmissionColor"))
                {
                    Color targetEmission = (state % 2 == 0)
                        ? blinkColor * emissionIntensity
                        : originalEmissionColors[i];
                    materialInstances[i].SetColor("_EmissionColor", targetEmission);
                }
            }
            state++;
            yield return new WaitForSeconds(blinkInterval);
        }
    }

    public void Interact()
    {
        // 깜빡임 중지 및 원래 색상 복구
        StopBlinking();
        RestoreOriginalEmissionColors();

        if (washerAnimator == null)
            washerAnimator = GetComponent<Animator>();

        washerAnimator?.Play(animationName, -1, 0f);
        StartCoroutine(HandleParticle());
    }

    private IEnumerator HandleParticle()
    {
        yield return new WaitForSeconds(1f);

        if (particleEffect != null)
        {
            particleEffect.gameObject.SetActive(true);
            particleEffect.Play();
            yield return new WaitForSeconds(5f);
            particleEffect.Stop();
            particleEffect.gameObject.SetActive(false);
        }

        if (messageCoroutine != null) StopCoroutine(messageCoroutine);
        messageCoroutine = StartCoroutine(ShowMessages());
    }

    IEnumerator ShowMessages()
    {
        SetActiveRecursively(chatPanel, true);
        if (messageText != null)
        {
            messageText.gameObject.SetActive(true);
            messageText.enabled = true;
        }

        isMessageActive = true;

        for (int i = 0; i < completionMessage.Length; i++)
        {
            messageText.text = completionMessage[i];
            PlayChatSound(i);

            bool spacePressed = false;
            float timer = 0;

            while (timer < 3f && !spacePressed)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    spacePressed = true;
                }
                timer += Time.deltaTime;
                yield return null;
            }

            if (i == completionMessage.Length - 1 && spacePressed)
            {
                break;
            }
        }

        EndMessage();
    }

    void EndMessage()
    {
        if (!isMessageActive) return;
        isMessageActive = false;

        SetActiveRecursively(chatPanel, false);
        if (messageText != null)
        {
            messageText.text = "";
            messageText.enabled = false;
            messageText.gameObject.SetActive(false);
        }

        if (tableObject != null) tableObject.SetActive(true);
        gameObject.SetActive(false);

        if (messageCoroutine != null)
        {
            StopCoroutine(messageCoroutine);
            messageCoroutine = null;
        }
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

    public void PlayChatSound(int index)
    {
        if (audioSource == null || soundClips == null) return;
        if (index < 0 || index >= soundClips.Length) return;

        audioSource.Stop();
        audioSource.PlayOneShot(soundClips[index]);
    }
}
