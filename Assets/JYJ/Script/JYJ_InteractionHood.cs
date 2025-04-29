using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using static JYJ_RaycastInteractor;

public class JYJ_InteractionHood : MonoBehaviour, IInteractable
{
    [Header("후드 설정")]
    public Animator hoodAnimator;
    public float interactionCooldown = 0.5f;
    private bool canInteract = true;
    private bool isHoodOpen = false;

    [Header("UI/사운드 설정")]
    public GameObject chatPanel;
    public Text messageText;
    public string openMessage = "후드가 열렸습니다!";
    public float messageDuration = 2.0f;
    public AudioSource audioSource;
    public AudioClip[] sounds; // 2개 이상의 사운드 클립 할당

    private Coroutine messageCoroutine;
    private bool isMessageActive = false;

    public void Interact()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!canInteract) return;
            ToggleHood();
            StartCoroutine(InteractionCooldown());
        }
    }

    void Update()
    {
        // 스페이스로 메시지 스킵
        if (isMessageActive && Input.GetKeyDown(KeyCode.Space))
        {
            EndMessage();
        }
    }

    void ToggleHood()
    {
        isHoodOpen = !isHoodOpen;
        hoodAnimator.SetTrigger(isHoodOpen ? "Open" : "Close");

        // 최초 열림 시에만 출력
        if (isHoodOpen)
        {
            // 패널과 모든 자식 활성화
            if (chatPanel != null) SetActiveRecursively(chatPanel, true);
            if (messageText != null)
            {
                messageText.gameObject.SetActive(true);
                if (messageCoroutine != null) StopCoroutine(messageCoroutine);
                messageCoroutine = StartCoroutine(ShowMessage(openMessage));
            }

            // 사운드 재생
            if (audioSource != null && sounds != null)
            {
                foreach (var clip in sounds)
                {
                    audioSource.PlayOneShot(clip);
                }
            }
        }
    }

    IEnumerator ShowMessage(string message)
    {
        isMessageActive = true;
        messageText.text = message;
        messageText.enabled = true;

        float timer = 0f;
        while (timer < messageDuration)
        {
            if (!isMessageActive) break; // 스페이스로 종료 시 루프 탈출
            timer += Time.deltaTime;
            yield return null;
        }

        EndMessage();
    }

    void EndMessage()
    {
        isMessageActive = false;
        if (messageText != null)
        {
            messageText.enabled = false;
            messageText.gameObject.SetActive(false);
        }
        if (chatPanel != null)
            SetActiveRecursively(chatPanel, false); // 패널과 모든 자식 비활성화
        if (messageCoroutine != null)
        {
            StopCoroutine(messageCoroutine);
            messageCoroutine = null;
        }
    }

    IEnumerator InteractionCooldown()
    {
        canInteract = false;
        yield return new WaitForSeconds(interactionCooldown);
        canInteract = true;
    }

    // 패널 및 모든 자식 오브젝트 활성화/비활성화 함수
    void SetActiveRecursively(GameObject obj, bool active)
    {
        obj.SetActive(active);
        foreach (Transform child in obj.transform)
        {
            SetActiveRecursively(child.gameObject, active);
        }
    }
}
