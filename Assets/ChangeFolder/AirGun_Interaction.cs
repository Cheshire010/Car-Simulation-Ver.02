using UnityEngine;
using UnityEngine.UI;

public class AirGun_Interaction : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private float detectRange = 1.5f;
    [SerializeField] private string playerTag = "Player";

    [Header("UI Settings")]
    [SerializeField] private GameObject messagePanel; // 안내 패널
    [SerializeField] private Text messageText;        // 안내 텍스트
    [SerializeField] private Text percentText;        // 퍼센트 텍스트
    [SerializeField] private float messageInterval = 2f;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] messageSounds;      // 안내 사운드
    [SerializeField] private AudioClip[] completeSounds;     // 완료 후 사운드(2개)

    [Header("Messages")]
    [SerializeField] private string[] messages;              // 안내 메시지
    [SerializeField] private string[] completeMessages;      // 완료 후 메시지(2개)

    private int currentIndex = 0;
    private bool isPlayerInRange = false;
    private float messageTimer = 0f;
    private bool isMessaging = false;

    // 진행도
    private int percent = 30;
    private bool isCompleted = false;
    private int completeIndex = 0;
    private bool isCompleteMessaging = false;
    private bool hasCompleted = false; // 완료 메시지 최초 1회만
    private bool hasInitialMessagePlayed = false; // 초기 메시지 최초 1회만

    [Header("종료 후 활성화 오브젝트")]
    public GameObject activateOnComplete;

    void Update()
    {
        CheckPlayerProximity();

        // 근접 안내 메시지 자동 넘김
        if (isMessaging)
        {
            messageTimer += Time.deltaTime;

            if (messageTimer >= messageInterval)
            {
                messageTimer = 0f;
                ShowNextMessage();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                ForceStopMessages();
            }
        }

        // 완료 후 메시지/사운드: 스페이스바로 수동 넘김
        if (isCompleteMessaging && Input.GetKeyDown(KeyCode.Space))
        {
            ShowNextCompleteMessage();
        }

        // 클릭으로 퍼센트 진행
        if (isPlayerInRange && !isCompleted && Input.GetMouseButtonDown(0))
        {
            TryIncreasePercent();
        }
    }

    void CheckPlayerProximity()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectRange);
        bool wasInRange = isPlayerInRange;
        isPlayerInRange = false;

        foreach (Collider col in colliders)
        {
            if (col.CompareTag(playerTag) && !col.isTrigger)
            {
                isPlayerInRange = true;
                break;
            }
        }

        // 상태 변경 처리
        if (isPlayerInRange && !wasInRange) StartMessages();
        if (!isPlayerInRange && wasInRange)
        {
            ForceStopMessages();
            ForceStopCompleteMessages();
        }
    }

    void StartMessages()
    {
        if (messages.Length == 0 || hasInitialMessagePlayed) return;
        hasInitialMessagePlayed = true;

        currentIndex = 0;
        messageTimer = 0f;
        isMessaging = true;
        if (messagePanel != null) messagePanel.SetActive(true);
        if (messageText != null) messageText.gameObject.SetActive(true);
        ShowNextMessage();
        UpdatePercentText();
    }

    void ShowNextMessage()
    {
        if (currentIndex >= messages.Length)
        {
            ForceStopMessages();
            return;
        }

        if (messageText != null)
        {
            messageText.text = messages[currentIndex];
            messageText.gameObject.SetActive(true);
        }

        if (currentIndex < messageSounds.Length && messageSounds[currentIndex] != null)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(messageSounds[currentIndex]);
        }

        currentIndex++;
    }

    void ForceStopMessages()
    {
        isMessaging = false;
        if (messagePanel != null) messagePanel.SetActive(false);
        if (messageText != null) messageText.gameObject.SetActive(false);
        audioSource.Stop();
        currentIndex = 0;
        messageTimer = 0f;
    }

    void TryIncreasePercent()
    {
        if (percent >= 80) return;
        percent += 10;
        if (percent > 80) percent = 80;
        UpdatePercentText();

        if (percent == 80)
        {
            isCompleted = true;
            ForceStopMessages();
            StartCompleteMessages();
        }
    }

    void UpdatePercentText()
    {
        if (percentText != null)
        {
            percentText.text = $"공기압 : {percent}%";
            percentText.gameObject.SetActive(true);
        }
    }

    // 완료 후 메시지/사운드 순차 출력 (최초 1회만)
    void StartCompleteMessages()
    {
        if (completeMessages.Length == 0 || hasCompleted) return;
        hasCompleted = true;

        completeIndex = 0;
        isCompleteMessaging = true;
        if (messagePanel != null) messagePanel.SetActive(true);
        if (messageText != null) messageText.gameObject.SetActive(true);
        ShowNextCompleteMessage();
    }

    void ShowNextCompleteMessage()
    {
        if (completeIndex >= completeMessages.Length)
        {
            ForceStopCompleteMessages();
            return;
        }

        if (messageText != null)
        {
            messageText.text = completeMessages[completeIndex];
            messageText.gameObject.SetActive(true);
        }

        if (completeIndex < completeSounds.Length && completeSounds[completeIndex] != null)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(completeSounds[completeIndex]);
        }

        completeIndex++;
    }

    void ForceStopCompleteMessages()
    {
        isCompleteMessaging = false;
        if (messagePanel != null) messagePanel.SetActive(false);
        if (messageText != null) messageText.gameObject.SetActive(false);
        audioSource.Stop();
        completeIndex = 0;

        // 마지막 대화 후 오브젝트 활성화
        if (activateOnComplete != null)
            activateOnComplete.SetActive(true);
    }

    // 에디터에서 감지 범위 시각화
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0.5f, 0, 0.3f);
        Gizmos.DrawSphere(transform.position, detectRange);
    }
}
