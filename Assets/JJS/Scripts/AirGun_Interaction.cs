using UnityEngine;
using static TireScene_RayCast;
using UnityEngine.UI;

public class AirGun_Interaction : MonoBehaviour, IInteractable
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
    private bool hasActivatedNearby = false;

    [Header("설정")]
    public float detectRange = 5f;

    [Header("메시지 자동 넘김 설정")]
    public float messageInterval = 2f;  // 메시지 간 간격 (초)
    private float messageTimer = 0f;
    private bool isShowingMessages = false;

    void Start()
    {
        if (subtitleText != null) subtitleText.enabled = false;
        if (nearbyPanel != null) nearbyPanel.SetActive(false);
    }

    void Update()
    {
        CheckPlayerNearby();

        // 자동 메시지 넘기기
        if (isShowingMessages)
        {
            messageTimer += Time.deltaTime;
            if (messageTimer >= messageInterval)
            {
                messageTimer = 0f;
                ShowNextNearbyMessage();
            }
        }

        if (isNearby && Input.GetKeyDown(KeyCode.Space))
        {
            isNearby = false;
            if (nearbyPanel != null) nearbyPanel.SetActive(false);
            isShowingMessages = false;
        }
    }

    void CheckPlayerNearby()
    {
        if (hasActivatedNearby) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        float distance = Vector3.Distance(player.transform.position, transform.position);

        if (distance < detectRange)
        {
            isNearby = true;
            hasActivatedNearby = true;
            currentIndex = 0;

            if (nearbyPanel != null) nearbyPanel.SetActive(true);
            isShowingMessages = true;
            messageTimer = 0f;
            ShowNextNearbyMessage();
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
            // 마지막 메시지까지 보여준 후 자동 닫기
            isNearby = false;
            isShowingMessages = false;
            if (nearbyPanel != null) nearbyPanel.SetActive(false);
        }
    }

    public void Interact()
    {
        // 근처 UI가 아직 열려 있으면 닫기
        if (nearbyPanel != null && nearbyPanel.activeSelf)
        {
            nearbyPanel.SetActive(false);
            isNearby = false;
            isShowingMessages = false;
        }

        if (subtitleText != null && !subtitleText.enabled)
        {
            subtitleText.enabled = true;
        }

        progress = Mathf.Min(progress + 10, 80);

        if (subtitleText != null)
        {
            subtitleText.text = $"공기압 : {progress}%";
        }

        if (progress == 80 && !hasTriggeredChat)
        {
            if (objectToHide != null)
                objectToHide.SetActive(false);

            if (!TireScene_ChatManager.IsChatting)
            {
                TireScene_ChatManager chatManager = FindObjectOfType<TireScene_ChatManager>();
                if (chatManager != null && messagesToSend.Length > 0)
                {
                    chatManager.StartChat(messagesToSend);
                    hasTriggeredChat = true;
                }
            }
        }
    }
}