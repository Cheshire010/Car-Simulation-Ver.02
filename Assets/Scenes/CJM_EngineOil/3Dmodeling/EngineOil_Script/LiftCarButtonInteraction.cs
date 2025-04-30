using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Collider), typeof(Renderer))]
public class LiftCarButtonInteraction : MonoBehaviour
{
    [Header("컨트롤러 참조")]
    public LiftController liftController;
    public CarController carController;

    [Header("UI 설정")]
    public GameObject messagePanel;
    public Text messageText;
    public float messageDuration = 2f;

    [Header("채팅 데이터")]
    public string[] chatMessages; // 채팅 메시지 배열
    public AudioClip soundClip;   // 모든 채팅에 사용할 단일 사운드

    [Header("사운드 설정")]
    public AudioSource audioSource;

    [Header("하이라이트 색상")]
    public Color highlightColor = Color.magenta;

    // 내부 상태
    private Collider col;
    private Renderer rend;
    private Color originalColor;
    private bool isHighlighted = false;
    private bool isShowingMessage = false;
    private Coroutine currentHideCoroutine;
    private int currentChatIndex = 0; // 현재 채팅 인덱스

    void Awake()
    {
        col = GetComponent<Collider>();
        col.isTrigger = false;

        rend = GetComponent<Renderer>();
        originalColor = rend.material.color;

        if (messagePanel != null)
            messagePanel.SetActive(false);
        //if (messageText != null)
        //    messageText.gameObject.SetActive(false);
    }

    void Update()
    {
        UpdateHighlight();
        HandleInput();
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isShowingMessage)
        {
            if (currentChatIndex < chatMessages.Length - 1)
            {
                currentChatIndex++;
                ShowChatMessage(currentChatIndex);

                // 타이머 재설정
                if (currentHideCoroutine != null)
                    StopCoroutine(currentHideCoroutine);
                currentHideCoroutine = StartCoroutine(HideMessageAfterDelay());
            }
            else
            {
                HideMessage();
            }
        }
    }

    void UpdateHighlight()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool hitMe = Physics.Raycast(ray, out var hit, Mathf.Infinity) && hit.collider == col;

        if (hitMe && !isHighlighted)
        {
            rend.material.color = highlightColor;
            isHighlighted = true;
        }
        else if (!hitMe && isHighlighted)
        {
            rend.material.color = originalColor;
            isHighlighted = false;
        }
    }

    private bool hasInteracted = false; // 최초 인터랙션 여부 플래그 추가

    void OnMouseDown()
    {
        if (liftController == null || carController == null)
        {
            Debug.LogWarning("컨트롤러가 할당되지 않았습니다.", this);
            return;
        }

        // 사운드 재생 (버튼 클릭 시 - 매번 실행)
        if (audioSource != null && soundClip != null)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(soundClip);
        }

        // 최초 인터랙션 시에만 채팅 시작
        if (!hasInteracted)
        {
            StartChat();
            hasInteracted = true;
        }

        // 기능 실행 (매번 실행)
        carController.upDuration = liftController.upDuration;
        carController.downDuration = liftController.downDuration;
        liftController.ToggleLift();
        carController.ToggleCar();
    }

    void StartChat()
    {
        currentChatIndex = 0;
        if (chatMessages != null && chatMessages.Length > 0)
        {
            // 첫 메시지 즉시 표시
            messagePanel.SetActive(true);
            messageText.gameObject.SetActive(true);
            ShowChatMessage(currentChatIndex);
        }
        else
        {
            Debug.LogWarning("표시할 채팅 메시지가 없습니다!", this);
        }
    }

    void ShowChatMessage(int index)
    {
        if (index >= chatMessages.Length) return;

        // UI 요소 활성화
        if (messagePanel != null)
        {
            messagePanel.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(messagePanel.GetComponent<RectTransform>());
        }

        if (messageText != null)
        {
            messageText.text = chatMessages[index];
            messageText.gameObject.SetActive(true);
            Canvas.ForceUpdateCanvases(); // 강제 UI 갱신
        }
        // 채팅 사운드 재생
        if (audioSource != null && soundClip != null)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(soundClip);
        }

        isShowingMessage = true;

        // 기존 코루틴 정지
        if (currentHideCoroutine != null)
            StopCoroutine(currentHideCoroutine);

        // 자동 종료 코루틴 시작
        currentHideCoroutine = StartCoroutine(HideMessageAfterDelay());
    }

    IEnumerator HideMessageAfterDelay()
    {
        yield return new WaitForSeconds(messageDuration);
        HideMessage();
    }

    void HideMessage()
    {
        if (messagePanel != null)
            messagePanel.SetActive(false);
        if (messageText != null)
            messageText.gameObject.SetActive(false);

        isShowingMessage = false;
        currentChatIndex = 0; // 채팅 인덱스 초기화
    }
}
