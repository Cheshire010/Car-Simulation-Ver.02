using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Animator), typeof(Collider), typeof(AudioSource))]
public class EngineOilCapInteraction : MonoBehaviour
{
    [Header("움직일 오일 객체")]
    public Transform engineOil;

    [Header("뚜껑 기준 이동 오프셋 (열렸을 때)")]
    public Vector3 moveOffset = new Vector3(0f, 0f, -0.2f);
    public float moveDuration = 0.8f;

    [Header("Chat Settings")]
    public GameObject chatPanel;
    public Text chatText;
    public string[] chatMessages;
    public AudioClip[] soundClips;

    [Header("파티클 효과")]
    public ParticleSystem oilParticleEffect; // Inspector에서 할당

    private Queue<string> messageQueue;
    private Queue<AudioClip> soundQueue;
    private AudioSource audioSource;
    private bool isChatting = false;

    // 내부 참조
    private Animator _anim;
    private Collider _col;
    private Vector3 _origPos;
    private Quaternion _origRot;
    private bool isOpen = false;
    private bool hasInteracted = false; // 최초 인터랙션 여부

    void Awake()
    {
        _anim = GetComponent<Animator>();
        _col = GetComponent<Collider>();
        audioSource = GetComponent<AudioSource>();
        _col.isTrigger = true;

        _origPos = engineOil.position;
        _origRot = engineOil.rotation;
    }

    void Update()
    {
        // 스페이스 키로 다음 대화
        if (isChatting && Input.GetKeyDown(KeyCode.Space))
        {
            PlayNextChat();
        }

        // 오일 객체 클릭 감지
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // engineOil 오브젝트를 클릭했을 때만 파티클 재생
            if (Physics.Raycast(ray, out hit) && hit.transform == engineOil)
            {
                PlayOilParticle();
            }
        }
    }

    void OnMouseDown()
    {
        if (isOpen)
        {
            _anim.SetTrigger("Close_Cap");
            StartCoroutine(MoveOilCoroutine(false));
        }
        else
        {
            _anim.SetTrigger("Open_Cap");
            StartCoroutine(MoveOilCoroutine(true));

            // 최초 인터랙션 시에만 채팅 시작
            if (!hasInteracted)
            {
                StartChat();
                hasInteracted = true;
            }
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
            Debug.LogWarning("메시지와 사운드 개수가 다릅니다!");
            return;
        }

        messageQueue = new Queue<string>(chatMessages);
        soundQueue = new Queue<AudioClip>(soundClips);

        if (chatPanel != null)
            chatPanel.SetActive(true);

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

        // UI 강제 갱신 (혹시 모를 렌더링 이슈 방지)
        if (chatText != null)
        {
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(chatText.rectTransform);
        }
    }

    void EndChat()
    {
        if (chatPanel != null)
            chatPanel.SetActive(false);
        isChatting = false;
        if (audioSource.isPlaying)
            audioSource.Stop();
    }

    IEnumerator MoveOilCoroutine(bool opening)
    {
        yield return new WaitForSeconds(0.5f);

        Vector3 startPos = engineOil.position;
        Quaternion startRot = engineOil.rotation;

        Vector3 endPos = opening ? transform.TransformPoint(moveOffset) : _origPos;
        Quaternion endRot = opening ? transform.rotation : _origRot;

        float t = 0f;
        while (t < moveDuration)
        {
            t += Time.deltaTime;
            float f = Mathf.SmoothStep(0f, 1f, t / moveDuration);
            engineOil.position = Vector3.Lerp(startPos, endPos, f);
            engineOil.rotation = Quaternion.Slerp(startRot, endRot, f);
            yield return null;
        }

        engineOil.position = endPos;
        engineOil.rotation = endRot;
    }

    // 오일 파티클 재생 메서드
    void PlayOilParticle()
    {
        if (oilParticleEffect != null)
        {
            oilParticleEffect.Play();
        }
    }
}
