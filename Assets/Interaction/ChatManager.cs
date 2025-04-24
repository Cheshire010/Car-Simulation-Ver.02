using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using static RaycastInteractor;
using System;

public class ChatManager : MonoBehaviour
{
    [Header("UI 설정")]
    public GameObject chatPanel;
    public Text chatText;
    public Text pressText; // [추가] "좌클릭을 눌러 진행" 텍스트

    [Header("채팅 데이터")]
    public string[] chatMessages;
    public bool playOnStart = true;

    private Queue<string> chatQueue = new Queue<string>();
    private bool isChatting = false;

    public static bool IsChatting { get; private set; }

    void Start()
    {
        // 채팅 관련 UI 비활성화
        chatPanel.SetActive(false);
        pressText.gameObject.SetActive(false); // [추가] 시작 시 비활성화

        // 게임 시작 시 채팅 메시지가 비어 있지 않으면 자동으로 채팅 시작
        if (playOnStart && chatMessages.Length > 0)
        {
            Debug.Log("StartChat() 호출");  // 디버깅 확인
            StartChat(chatMessages);
        }
        else
        {
            Debug.LogWarning("Chat messages are empty or playOnStart is false!");
        }
    }

    public void StartChat(string[] messages)
    {
        if (messages == null || messages.Length == 0) return;

        chatQueue.Clear();
        foreach (string msg in messages)
        {
            chatQueue.Enqueue(msg);
        }

        chatPanel.SetActive(true);
        pressText.gameObject.SetActive(true); // [추가] 활성화
        isChatting = true;
        IsChatting = true;
        ShowNextChat();

        if (PlayerMove.Instance != null)
            PlayerMove.Instance.enabled = false;

        if (RaycastInteractor.Instance != null)
            RaycastInteractor.Instance.enabled = false;
    }

    void Update()
    {
        if (!isChatting) return;

        // [수정] GetMouseButtonDown → GetKeyDown(KeyCode.Mouse0)
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
    }

    private void EndChat()
    {
        chatPanel.SetActive(false);
        pressText.gameObject.SetActive(false); // 채팅 종료 시 숨김
        isChatting = false;
        IsChatting = false;

        if (PlayerMove.Instance != null)
            PlayerMove.Instance.enabled = true;

        if (RaycastInteractor.Instance != null)
            RaycastInteractor.Instance.enabled = true;
    }
}