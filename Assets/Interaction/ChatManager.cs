using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using static RaycastInteractor;
using System;

[RequireComponent(typeof(AudioSource))] // 오디오 소스 컴포넌트 필수 추가
public class ChatManager : MonoBehaviour
{
    [Header("UI 설정")]
    public GameObject chatPanel;
    public Text chatText;
    public Text pressText;

    [Header("채팅 데이터")]
    public string[] chatMessages;
    public AudioClip[] soundClips; // 각 메시지별 사운드 클립 배열
    public bool playOnStart = true;

    private Queue<string> chatQueue = new Queue<string>();
    private AudioSource audioSource; // 오디오 소스 참조
    private bool isChatting = false;
    private int currentSoundIndex = 0; // 현재 재생 위치 트래킹

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
        chatQueue.Clear();
        currentSoundIndex = 0; // 사운드 인덱스 초기화

        foreach (string msg in messages)
        {
            chatQueue.Enqueue(msg);
        }

        chatPanel.SetActive(true);
        pressText.gameObject.SetActive(true);
        isChatting = true;
        IsChatting = true;
        ShowNextChat();

        if (RaycastInteractor.Instance != null)
            RaycastInteractor.Instance.enabled = false;
    }

    void Update()
    {
        if (!isChatting) return;

        if (Input.GetKeyDown(KeyCode.Space)) // Space → 마우스 클릭으로 변경
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

        // 메시지 표시와 동시에 사운드 재생
        chatText.text = chatQueue.Dequeue();
        PlayCurrentSound();
        currentSoundIndex++;
    }

    void PlayCurrentSound()
    {
        if (soundClips == null || currentSoundIndex >= soundClips.Length) return;

        // 현재 재생 중인 사운드 중단
        audioSource.Stop();

        if (soundClips[currentSoundIndex] != null)
        {
            audioSource.PlayOneShot(soundClips[currentSoundIndex]);
        }
    }


    private void EndChat()
    {
        chatPanel.SetActive(false);
        pressText.gameObject.SetActive(false);
        isChatting = false;
        IsChatting = false;

        if (RaycastInteractor.Instance != null)
            RaycastInteractor.Instance.enabled = true;
    }
}
