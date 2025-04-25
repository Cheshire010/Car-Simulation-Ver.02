using UnityEngine;
using UnityEngine.UI;
using static RaycastInteractor;

[RequireComponent(typeof(AudioSource))]
public class ChangeYellow : MonoBehaviour, IInteractable
{
    public Text subtitleText;  // 진행도 텍스트
    private int progress = 30;
    public GameObject objectToHide;

    [Header("대화 메시지")]
    [TextArea]
    public string[] messagesToSend;
    public AudioClip[] messagesToSendSounds; // 메시지별 사운드

    private bool hasTriggeredChat = false;

    [Header("근처 접근 시 표시할 UI")]
    public GameObject nearbyPanel;
    public Text nearbyText;
    public string[] nearbyMessages;
    public AudioClip[] nearbyMessagesSounds; // 근접 메시지별 사운드

    private int currentIndex = 0;
    private bool isNearby = false;
    private bool hasActivatedNearby = false;

    [Header("설정")]
    public float detectRange = 5f;

    [Header("메시지 자동 넘김 설정")]
    public float messageInterval = 2f;  // 메시지 간 간격 (초)
    private float messageTimer = 0f;
    private bool isShowingMessages = false;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

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
            audioSource.Stop(); // UI 닫을 때 사운드 중단
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

            //  현재 재생 중인 사운드 중단 후 다음 사운드 재생
            audioSource.Stop();

            if (nearbyMessagesSounds != null && currentIndex < nearbyMessagesSounds.Length && nearbyMessagesSounds[currentIndex] != null)
            {
                audioSource.PlayOneShot(nearbyMessagesSounds[currentIndex]);
            }
            currentIndex++;
        }
        else
        {
            // 마지막 메시지까지 보여준 후 자동 닫기
            audioSource.Stop(); // 패널 닫힐 때 사운드 중단
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
            audioSource.Stop(); // UI 닫을 때 사운드 중단
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

        // 채팅 조건 달성 시
        if (progress == 80 && !hasTriggeredChat)
        {
            if (objectToHide != null)
                objectToHide.SetActive(false);

            // 메시지별 사운드 재생 (첫 메시지 기준)
            if (messagesToSendSounds != null && messagesToSendSounds.Length > 0 && messagesToSendSounds[0] != null)
            {
                audioSource.Stop(); // 이전 사운드 중단
                audioSource.PlayOneShot(messagesToSendSounds[0]);
            }

            if (!ChatManager.IsChatting)
            {
                ChatManager chatManager = FindObjectOfType<ChatManager>();
                if (chatManager != null && messagesToSend.Length > 0)
                {
                    // ChatManager에서 메시지별 사운드도 처리하도록 설계되어 있다면,
                    // chatManager.StartChat(messagesToSend, messagesToSendSounds); 처럼 넘겨도 됨
                    chatManager.StartChat(messagesToSend);
                    hasTriggeredChat = true;
                }
            }
        }
    }
}
