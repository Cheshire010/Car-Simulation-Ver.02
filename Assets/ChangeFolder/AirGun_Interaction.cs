using UnityEngine;
using UnityEngine.UI;
using static TireScene_RayCast;

public class AirGun_Interaction : MonoBehaviour, IInteractable
{
    public Text chatText;
    public Text subtitleText;  // 진행도 텍스트
    private int progress = 30;
    public GameObject objectToHide;

    [Header("대화 메시지")]
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

    [Header("근처 접근 메시지 사운드")]
    public AudioSource nearbyAudioSource;
    public AudioClip[] nearbyAudioClips;

    [Header("대화 메시지 사운드")]
    public AudioSource chatAudioSource;
    public AudioClip[] chatAudioClips;

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
            StopNearbySound();
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
            PlayNearbySound(currentIndex); // 근처 접근 사운드
            currentIndex++;
        }
        else
        {
            // 마지막 메시지까지 보여준 후 자동 닫기
            isNearby = false;
            isShowingMessages = false;
            if (nearbyPanel != null) nearbyPanel.SetActive(false);
            StopNearbySound();
        }
    }

    void PlayNearbySound(int index)
    {
        if (nearbyAudioSource == null || nearbyAudioClips == null || index >= nearbyAudioClips.Length) return;

        if (nearbyAudioSource.isPlaying)
            nearbyAudioSource.Stop();

        nearbyAudioSource.clip = nearbyAudioClips[index];
        nearbyAudioSource.Play();
    }

    void StopNearbySound()
    {
        if (nearbyAudioSource != null && nearbyAudioSource.isPlaying)
            nearbyAudioSource.Stop();
    }

    // 대화 메시지 재생 (TireScene_ChatManager에서 호출해야 함)
    public void ShowNextChatMessage(int chatIndex)
    {
        if (messagesToSend != null && chatIndex < messagesToSend.Length)
        {
            chatText.text = messagesToSend[chatIndex];
            PlayChatSound(chatIndex); // 사운드도 같이 재생
        }
    }

    void PlayChatSound(int index)
    {
        if (chatAudioSource == null || chatAudioClips == null || index >= chatAudioClips.Length) return;

        if (chatAudioSource.isPlaying)
            chatAudioSource.Stop();

        chatAudioSource.clip = chatAudioClips[index];
        chatAudioSource.Play();
    }

    void StopChatSound()
    {
        if (chatAudioSource != null && chatAudioSource.isPlaying)
            chatAudioSource.Stop();
    }

    public void Interact()
    {
        // 근처 UI가 아직 열려 있으면 닫기
        if (nearbyPanel != null && nearbyPanel.activeSelf)
        {
            nearbyPanel.SetActive(false);
            isNearby = false;
            isShowingMessages = false;
            StopNearbySound();
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
                    chatManager.StartChat(messagesToSend, chatAudioClips); // 자신의 사운드 클립 전달
                                                                           // this를 넘겨서 ShowNextChatMessage 호출 가능
                    hasTriggeredChat = true;
                }
            }
        }
    }
}
