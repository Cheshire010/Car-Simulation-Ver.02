using UnityEngine;
using UnityEngine.UI;
using static JYJ_RaycastInteractor;
using System.Collections;

public class JYJ_InteractWash : MonoBehaviour, IInteractable
{
    [Header("애니메이터 설정")]
    public Animator washerAnimator;
    public string animationName = "Washer";

    [Header("파티클 설정")]
    public ParticleSystem particleEffect;

    [Header("UI 설정")]
    public GameObject chatPanel;
    public Text messageText;
    public string[] completionMessage = { "테이블에 문제가 생성되었습니다. 문제를 풀어주십시오." };

    [Header("활성화 오브젝트")]
    public GameObject tableObject;

    [Header("사운드 설정")]
    public AudioSource audioSource;
    public AudioClip[] soundClips;

    private Coroutine messageCoroutine;
    private bool isMessageActive = false;

    public void Interact()
    {
        if (washerAnimator == null)
            washerAnimator = GetComponent<Animator>();

        washerAnimator?.Play(animationName, -1, 0f);
        StartCoroutine(HandleParticle());
    }

    private IEnumerator HandleParticle()
    {
        yield return new WaitForSeconds(1f);

        if (particleEffect != null)
        {
            particleEffect.gameObject.SetActive(true);
            particleEffect.Play();
            yield return new WaitForSeconds(5f);
            particleEffect.Stop();
            particleEffect.gameObject.SetActive(false);
        }

        if (messageCoroutine != null) StopCoroutine(messageCoroutine);
        messageCoroutine = StartCoroutine(ShowMessages());

        // 기존 활성화 코드 제거
        // if (tableObject != null) tableObject.SetActive(true);
        // gameObject.SetActive(false);
    }

    IEnumerator ShowMessages()
    {
        Debug.Log("ShowMessages 시작");
        SetActiveRecursively(chatPanel, true);
        if (messageText != null)
        {
            messageText.gameObject.SetActive(true);
            messageText.enabled = true;
        }

        isMessageActive = true;

        // ▼▼▼ 핵심 로직 개선 ▼▼▼
        for (int i = 0; i < completionMessage.Length; i++)
        {
            messageText.text = completionMessage[i];
            PlayChatSound(i);

            bool spacePressed = false;
            float timer = 0;

            // 3초 대기 또는 스페이스 입력 감지
            while (timer < 3f && !spacePressed)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Debug.Log("스페이스 입력 감지");
                    spacePressed = true;
                }
                timer += Time.deltaTime;
                yield return null;
            }

            // 마지막 메시지에서 스페이스 입력 시 즉시 종료
            if (i == completionMessage.Length - 1 && spacePressed)
            {
                break;
            }
        }
        // ▲▲▲ 수정 완료 ▲▲▲

        EndMessage();
    }


    void EndMessage()
    {
        if (!isMessageActive)
        {
            Debug.Log("EndMessage: 이미 비활성화 상태");
            return;
        }
        isMessageActive = false;

        Debug.Log("패널 및 텍스트 비활성화");
        SetActiveRecursively(chatPanel, false);
        if (messageText != null)
        {
            messageText.text = "";
            messageText.enabled = false;
            messageText.gameObject.SetActive(false);
        }

        // 채팅 종료 후 오브젝트 활성화/비활성화
        if (tableObject != null) tableObject.SetActive(true);
        gameObject.SetActive(false); // 현재 오브젝트 비활성화

        if (messageCoroutine != null)
        {
            StopCoroutine(messageCoroutine);
            messageCoroutine = null;
        }
    }

    // 모든 자식 오브젝트 활성화/비활성화 함수
    void SetActiveRecursively(GameObject obj, bool active)
    {
        if (obj == null) return;
        obj.SetActive(active);
        foreach (Transform child in obj.transform)
        {
            SetActiveRecursively(child.gameObject, active);
        }
    }

    public void PlayChatSound(int index)
    {
        if (audioSource == null || soundClips == null) return;
        if (index < 0 || index >= soundClips.Length) return;

        audioSource.Stop();
        audioSource.PlayOneShot(soundClips[index]);
    }
}
