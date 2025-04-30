using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Animator), typeof(AudioSource))]
public class BonnetInteraction : MonoBehaviour
{
    // 애니메이션 및 상태
    Animator animator;
    bool isOpen = false;
    bool isBusy = false;

    // 채팅 시스템 변수
    [Header("Chat Settings")]
    public GameObject chatPanel;
    public Text chatText;
    public Text pressText;
    public string[] chatMessages;
    public AudioClip[] soundClips;
    private Queue<string> messageQueue;
    private Queue<AudioClip> soundQueue;
    private AudioSource audioSource;
    private bool isChatting = false;

    [Header("Lock Durations (sec)")]
    public float openDuration = 1.0f;
    public float closeDuration = 1.0f;

    private bool hasInteracted = false; // 최초 인터랙션 여부

    void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // 채팅 중일 때 스페이스 키로 다음 대화
        if (isChatting && Input.GetKeyDown(KeyCode.Space))
        {
            PlayNextChat();
        }

        // 본넷 조작 (모션 중이거나 채팅 중이면 무시)
        if (isBusy || isChatting) return;
        if (!Input.GetMouseButtonDown(0)) return;

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, 100f) &&
            hit.collider.transform.IsChildOf(transform))
        {
            ToggleBonnet();

            // 최초 인터랙션 시에만 채팅 시작
            if (!hasInteracted)
            {
                StartChat();
                hasInteracted = true;
            }
        }
    }

    void ToggleBonnet()
    {
        if (isOpen)
        {
            animator.SetTrigger("Close");
            StartCoroutine(MotionLock(closeDuration));
        }
        else
        {
            animator.SetTrigger("Open");
            StartCoroutine(MotionLock(openDuration));
        }
        isOpen = !isOpen;
    }

    void StartChat()
    {
        if (chatMessages == null || chatMessages.Length == 0 ||
            soundClips == null || soundClips.Length == 0)
            return;

        if (chatMessages.Length != soundClips.Length)
        {
            Debug.LogWarning("채팅 메시지와 사운드 클립의 개수가 다릅니다!");
            return;
        }

        messageQueue = new Queue<string>(chatMessages);
        soundQueue = new Queue<AudioClip>(soundClips);

        // UI 요소 명시적 활성화
        if (chatPanel != null)
            chatPanel.SetActive(true);
        if (chatText != null)
            chatText.gameObject.SetActive(true); // 추가된 부분
        if (pressText != null)
            pressText.gameObject.SetActive(true);

        isChatting = true;
        PlayNextChat();
    }


    void PlayNextChat()
    {
        // 현재 사운드 즉시 중단
        if (audioSource.isPlaying)
            audioSource.Stop();

        // 대화 종료 조건
        if (messageQueue.Count == 0 || soundQueue.Count == 0)
        {
            EndChat();
            return;
        }

        // 다음 메시지/사운드 표시 및 재생
        string nextMsg = messageQueue.Dequeue();
        AudioClip nextClip = soundQueue.Dequeue();

        if (chatText != null)
            chatText.text = nextMsg;

        if (audioSource != null && nextClip != null)
            audioSource.PlayOneShot(nextClip);

        // 안내 텍스트 갱신
        if (pressText != null)
        {
            pressText.text = (messageQueue.Count > 0)
                ? "스페이스를 눌러 진행"
                : "스페이스를 눌러 종료";
        }

        // UI 강제 갱신 (혹시 모를 렌더링 이슈 방지)
        if (chatText != null)
        {
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(chatText.rectTransform);
        }
    }

    void EndChat()
    {
        if (chatPanel != null) chatPanel.SetActive(false);
        if (pressText != null) pressText.gameObject.SetActive(false);
        isChatting = false;
        if (audioSource.isPlaying)
            audioSource.Stop();
    }

    IEnumerator MotionLock(float duration)
    {
        isBusy = true;
        yield return new WaitForSeconds(duration);
        isBusy = false;
    }
}
