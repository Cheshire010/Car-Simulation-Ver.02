using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using static TireScene_RayCast;

public class TireScene_ChatManager : MonoBehaviour
{
    [Header("UI 설정")]
    public GameObject chatPanel;
    public Text chatText;
    public Text pressText;

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
    }

    void Start()
    {
        chatPanel.SetActive(false);
        pressText.gameObject.SetActive(false);

        if (playOnStart && chatMessages.Length > 0)
        {
            StartChat(chatMessages);
        }
    }

    public void StartChat(string[] messages)
    {
        if (messages.Length != soundClips.Length)
        {
            Debug.LogWarning("메시지와 사운드 클립 개수 불일치!");
        }

        chatQueue.Clear();
        currentSoundIndex = 0;

        foreach (string msg in messages)
        {
            chatQueue.Enqueue(msg);
        }

        chatPanel.SetActive(true);
        pressText.gameObject.SetActive(true);
        isChatting = true;
        IsChatting = true;
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

        chatText.text = chatQueue.Dequeue();
        PlayCurrentSound();
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
        chatPanel.SetActive(false);
        pressText.gameObject.SetActive(false);
        isChatting = false;
        IsChatting = false;

        if (JYJ_RaycastInteractor.Instance != null)
            JYJ_RaycastInteractor.Instance.enabled = true;
    }
}
