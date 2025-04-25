using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ChatManager : MonoBehaviour
{
    [System.Serializable]
    public class ChatData
    {
        public string eventName;
        public string[] messages;
    }

    [Header("UI 설정")]
    public GameObject chatPanel;
    public Text chatText;
    public Text pressText;

    [Header("채팅 데이터")]
    public List<ChatData> chatDatas = new List<ChatData>();
    public bool playOnStart = true;

    [Header("참조 설정")]
    public PlayerMove playerController;
    public RaycastInteractor raycastInteractor;

    private Dictionary<string, string[]> chatDictionary = new Dictionary<string, string[]>();
    private Queue<string> currentChat = new Queue<string>();
    private bool isChatting = false;

    public static bool IsChatting { get; private set; }

    void Start()
    {
        foreach (ChatData data in chatDatas)
        {
            chatDictionary.Add(data.eventName, data.messages);
        }

        chatPanel.SetActive(false);
        pressText.gameObject.SetActive(false);

        // 자동 참조 (옵션)
        if (playerController == null) playerController = FindObjectOfType<PlayerMove>();
        if (raycastInteractor == null) raycastInteractor = FindObjectOfType<RaycastInteractor>();

        if (playOnStart && chatDictionary.ContainsKey("StartEvent"))
        {
            StartChat("StartEvent");
        }
    }

    public void StartChat(string eventName, string[] customMessages = null)
    {
        string[] messages = customMessages ?? (chatDictionary.ContainsKey(eventName) ? chatDictionary[eventName] : null);
        if (messages == null || messages.Length == 0) return;

        currentChat.Clear();
        foreach (string msg in messages) currentChat.Enqueue(msg);

        chatPanel.SetActive(true);
        pressText.gameObject.SetActive(true);
        isChatting = true;
        IsChatting = true;
        ShowNextChat();

        SetPlayerControl(false);
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
        if (currentChat.Count == 0)
        {
            EndChat();
            return;
        }
        chatText.text = currentChat.Dequeue();
    }

    public event System.Action OnChatEnd;

    private void EndChat()
    {
        chatPanel.SetActive(false);
        pressText.gameObject.SetActive(false);
        isChatting = false;
        IsChatting = false;

        SetPlayerControl(true);
        OnChatEnd?.Invoke();
    }

    private void SetPlayerControl(bool enable)
    {
        if (playerController != null)
            playerController.enabled = enable;
        else
            Debug.LogWarning("PlayerMove 참조 없음");

        if (raycastInteractor != null)
            raycastInteractor.enabled = enable;
        else
            Debug.LogWarning("RaycastInteractor 참조 없음");
    }
}
