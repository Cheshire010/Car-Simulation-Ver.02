using UnityEngine;
using static RaycastInteractor;
using UnityEngine.UI;

public class ChangeYellow : MonoBehaviour, IInteractable
{
    public Text subtitleText;  // 진행도 텍스트
    private int progress = 30;
    public GameObject objectToHide;

    [Header("대화 메시지")]
    [TextArea]
    public string[] messagesToSend;
    private bool hasTriggeredChat = false;

    [Header("근처 접근 시 표시할 UI")]
    public GameObject nearbyPanel;
    public Text nearbyText;
    public string[] nearbyMessages;
    private int currentIndex = 0;
    private bool isNearby = false;
    private bool hasActivatedNearby = false; // 중복 실행 방지

    [Header("설정")]
    public float detectRange = 5f;  // 근처 감지 거리

    void Start()
    {
        if (subtitleText != null) subtitleText.enabled = false;
        if (nearbyPanel != null) nearbyPanel.SetActive(false);
    }

    void Update()
    {
        CheckPlayerNearby();

        if (isNearby && Input.GetMouseButtonDown(0))
        {
            // 왼쪽 클릭 시 UI 끄기
            isNearby = false;
            if (nearbyPanel != null) nearbyPanel.SetActive(false);
        }
    }

    void CheckPlayerNearby()
    {
        if (hasActivatedNearby) return; // 이미 한 번 보여줬으면 다시 안함

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        float distance = Vector3.Distance(player.transform.position, transform.position);

        if (distance < detectRange)
        {
            isNearby = true;
            hasActivatedNearby = true;
            currentIndex = 0;

            if (nearbyPanel != null) nearbyPanel.SetActive(true);
            ShowNextNearbyMessage();
        }
    }

    public void Interact()
    {
        if (subtitleText != null && !subtitleText.enabled)
        {
            subtitleText.enabled = true;
        }

        progress = Mathf.Min(progress + 10, 80);

        if (subtitleText != null)
        {
            subtitleText.text = $"진행도 : {progress}%";
        }

        if (progress == 80 && !hasTriggeredChat)
        {
            if (objectToHide != null)
                objectToHide.SetActive(false);

            if (!ChatManager.IsChatting)
            {
                ChatManager chatManager = FindObjectOfType<ChatManager>();
                if (chatManager != null && messagesToSend.Length > 0)
                {
                    chatManager.StartChat(messagesToSend);
                    hasTriggeredChat = true;
                }
            }
        }
    }

    void ShowNextNearbyMessage()
    {
        if (nearbyMessages == null || nearbyMessages.Length == 0 || nearbyText == null) return;

        if (currentIndex < nearbyMessages.Length)
        {
            nearbyText.text = nearbyMessages[currentIndex];
            currentIndex++;
        }
        else
        {
            // 마지막 메시지까지 봤으면 더 이상 자동 진행 없음
        }
    }
}