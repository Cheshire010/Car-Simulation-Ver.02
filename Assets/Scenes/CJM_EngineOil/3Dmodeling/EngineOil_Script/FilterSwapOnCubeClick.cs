using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Renderer), typeof(Collider))]
public class FilterSwapOnCubeClick : MonoBehaviour
{
    [Header("Visual highlight")]
    public Color highlightColor = Color.green;
    private Color originalColor;
    private Renderer rend;

    [Header("Filter swap settings")]
    public Transform oldFilter;
    public Transform newFilter;
    public Transform installPoint;
    public Transform newSlotPoint;
    public float moveDuration = 1f;

    [Header("UI/사운드")]
    public GameObject messagePanel;
    public Text messageText;
    public AudioSource audioSource;
    public AudioClip[] stepSounds;      // 각 단계별 사운드
    public string[] stepMessages;       // 각 단계별 메시지
    public float messageDuration = 1.5f;

    private bool isMoving = false;
    private int currentStep = 0;
    private bool isMessageActive = false;
    private Coroutine messageCoroutine;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        originalColor = rend.material.color;
        var col = GetComponent<Collider>();
        col.isTrigger = true;
        if (messagePanel != null) messagePanel.SetActive(false);
        if (messageText != null) messageText.gameObject.SetActive(false);
    }

    void Update()
    {
        // 스페이스로 메시지/단계 스킵
        if (isMessageActive && Input.GetKeyDown(KeyCode.Space))
        {
            if (messageCoroutine != null)
                StopCoroutine(messageCoroutine);
            HideMessage();
            stepSkipRequested = true;
        }
    }

    void OnMouseEnter()
    {
        rend.material.color = highlightColor;
    }

    void OnMouseExit()
    {
        rend.material.color = originalColor;
    }

    void OnMouseDown()
    {
        if (isMoving) return;
        StartCoroutine(SwapCoroutine());
    }

    // 외부에서 호출할 수 있는 공개 래퍼
    public IEnumerator DoSwap()
    {
        yield return StartCoroutine(SwapCoroutine());
    }

    // 내부 플래그로 스페이스 입력 시 단계 스킵
    private bool stepSkipRequested = false;

    private IEnumerator SwapCoroutine()
    {
        isMoving = true;

        // 1단계: oldFilter → installPoint
        yield return ShowStepMessage(0);
        yield return MoveTransform(oldFilter, installPoint.position);

        // 2단계: oldFilter 숨기고 newFilter 켜기
        oldFilter.gameObject.SetActive(false);
        newFilter.gameObject.SetActive(true);
        yield return ShowStepMessage(1);

        // 3단계: newFilter → newSlotPoint
        yield return MoveTransform(newFilter, newSlotPoint.position);
        yield return ShowStepMessage(2);

        newFilter.position = newSlotPoint.position;
        isMoving = false;
        newFilter.gameObject.SetActive(false);

        // 종료 메시지 (옵션)
        if (stepMessages.Length > 3)
            yield return ShowStepMessage(3);
    }

    IEnumerator MoveTransform(Transform target, Vector3 endPos)
    {
        Vector3 startPos = target.position;
        float t = 0f;
        stepSkipRequested = false;
        while (t < moveDuration)
        {
            if (stepSkipRequested) break;
            t += Time.deltaTime;
            target.position = Vector3.Lerp(startPos, endPos, t / moveDuration);
            yield return null;
        }
        target.position = endPos;
        stepSkipRequested = false;
    }

    IEnumerator ShowStepMessage(int step)
    {
        if (stepMessages != null && step < stepMessages.Length)
        {
            ShowMessage(stepMessages[step]);
        }
        if (stepSounds != null && step < stepSounds.Length && stepSounds[step] != null && audioSource != null)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(stepSounds[step]);
        }

        float timer = 0f;
        isMessageActive = true;
        stepSkipRequested = false;
        messageCoroutine = StartCoroutine(AutoHideMessage(messageDuration));
        while (timer < messageDuration)
        {
            if (stepSkipRequested) break;
            timer += Time.deltaTime;
            yield return null;
        }
        HideMessage();
        isMessageActive = false;
        stepSkipRequested = false;
    }

    void ShowMessage(string msg)
    {
        if (messagePanel != null) messagePanel.SetActive(true);
        if (messageText != null)
        {
            messageText.text = msg;
            messageText.gameObject.SetActive(true);
        }
    }

    IEnumerator AutoHideMessage(float duration)
    {
        yield return new WaitForSeconds(duration);
        HideMessage();
        isMessageActive = false;
    }

    void HideMessage()
    {
        if (messagePanel != null) messagePanel.SetActive(false);
        if (messageText != null) messageText.gameObject.SetActive(false);
    }
}
