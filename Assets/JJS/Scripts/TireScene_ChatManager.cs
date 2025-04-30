using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using static TireScene_RayCast;

public class TireScene_ChatManager : MonoBehaviour
{
    [Header("UI 설정")]
    [SerializeField] private GameObject chatPanel;
    [SerializeField] private Text chatText;
    [SerializeField] private Text pressText;

    [Header("채팅 데이터")]
    public string[] chatMessages;
    public AudioClip[] soundClips;
    public bool playOnStart = true;

    private Queue<string> chatQueue = new Queue<string>();
    private AudioSource audioSource;
    private bool isChatting = false;
    private int currentSoundIndex = 0;

    public static bool IsChatting { get; private set; }

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        ValidateUIComponents();
    }

    // UI 컴포넌트가 제대로 할당되었는지 확인
    void ValidateUIComponents()
    {
        if (chatPanel == null) Debug.LogError("Chat Panel이 인스펙터에 할당되지 않았습니다!");
        if (chatText == null) Debug.LogError("Chat Text가 인스펙터에 할당되지 않았습니다!");
        if (pressText == null) Debug.LogError("Press Text가 인스펙터에 할당되지 않았습니다!");
    }

    void Start()
    {
        Debug.Log($"[ChatManager] Start() 호출됨. playOnStart: {playOnStart}, chatMessages.Count: {chatMessages?.Length}");

        if (chatPanel != null) chatPanel.SetActive(false);
        if (pressText != null) pressText.gameObject.SetActive(false);

        // 반드시 1프레임 뒤에 실행 (UI 초기화 보장)
        if (playOnStart && chatMessages != null && chatMessages.Length > 0)
        {
            StartCoroutine(DelayedStartChat());
        }
        else if (playOnStart)
        {
            Debug.LogWarning("[ChatManager] playOnStart가 활성화되었지만 채팅 메시지가 없습니다!");
        }
    }

    IEnumerator DelayedStartChat()
    {
        // 1프레임 대기 (UI 초기화 보장)
        yield return null;
        StartChat(chatMessages);
    }

    public void StartChat(string[] messages)
    {
        if (messages == null || messages.Length == 0)
        {
            Debug.LogError("[ChatManager] 빈 메시지 배열로 채팅을 시작할 수 없습니다!");
            return;
        }

        if (soundClips != null && messages.Length != soundClips.Length)
        {
            Debug.LogWarning("[ChatManager] 메시지와 사운드 클립의 개수가 다릅니다!");
        }

        chatQueue.Clear();
        currentSoundIndex = 0;

        foreach (string msg in messages)
        {
            chatQueue.Enqueue(msg);
        }

        if (chatPanel != null) chatPanel.SetActive(true);
        if (pressText != null) pressText.gameObject.SetActive(true);

        isChatting = true;
        IsChatting = true;

        // 첫 메시지 표시
        ShowNextChat();

        if (JYJ_RaycastInteractor.Instance != null)
            JYJ_RaycastInteractor.Instance.enabled = false;
    }

    void Update()
    {
        if (!isChatting) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShowNextChat();
        }
    }

    private void ShowNextChat()
    {
        if (chatQueue.Count == 0)
        {
            EndChat();
            return;
        }

        string nextMsg = chatQueue.Dequeue();
        if (chatText != null)
        {
            chatText.text = nextMsg;

            // 강제 UI 갱신 (UI가 비활성화였다가 활성화될 때 렌더링 문제 방지)
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(chatText.rectTransform);

            // 혹시 모를 렌더링 문제 대비, 1프레임 뒤에도 다시 갱신
            StartCoroutine(ForceUITextUpdate());
        }
        PlayCurrentSound();
    }

    IEnumerator ForceUITextUpdate()
    {
        yield return null;
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(chatText.rectTransform);
    }

    void PlayCurrentSound()
    {
        if (soundClips == null || currentSoundIndex >= soundClips.Length) return;

        if (audioSource.isPlaying)
            audioSource.Stop();

        audioSource.clip = soundClips[currentSoundIndex];
        audioSource.Play();
        currentSoundIndex++;
    }

    private void EndChat()
    {
        if (chatPanel != null) chatPanel.SetActive(false);
        if (pressText != null) pressText.gameObject.SetActive(false);
        isChatting = false;
        IsChatting = false;

        if (JYJ_RaycastInteractor.Instance != null)
            JYJ_RaycastInteractor.Instance.enabled = true;
    }
}
