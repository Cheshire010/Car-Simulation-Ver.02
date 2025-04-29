using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using static JYJ_RaycastInteractor;

public class JYJ_DoorController : MonoBehaviour, IInteractable
{
    [Header("후드 설정")]
    public Animator hoodAnimator;
    public float interactionCooldown = 0.5f;
    private bool canInteract = true;
    private bool isHoodOpen = false;

    [Header("사운드 설정")]
    public AudioSource audioSource;
    public AudioClip openSound;
    public AudioClip closeSound;

    [Header("UI 설정")]
    public Text messageText;
    public GameObject chatPanel; // 패널

    public string openMessage = "문이 열렸습니다!";
    public float messageDuration = 2.0f;

    private bool hasOpenedOnce = false;
    private bool isDialogueActive = false;

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
        if (isDialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            SkipCurrentDialogueAndPlayNext();
        }
    }

    void ToggleHood()
    {
        Debug.Log("ToggleHood 함수 호출됨");
        isHoodOpen = !isHoodOpen;
        hoodAnimator.SetTrigger(isHoodOpen ? "Open" : "Close");

        if (isHoodOpen && !hasOpenedOnce)
        {
            hasOpenedOnce = true;

            if (audioSource != null && openSound != null)
            {
                audioSource.Stop();
                audioSource.PlayOneShot(openSound);
            }

            if (messageText != null)
            {
                StopAllCoroutines();
                messageText.gameObject.SetActive(true);
                if (chatPanel != null) SetActiveRecursively(chatPanel, true); // 패널과 자식 모두 활성화
                StartCoroutine(ShowMessage(openMessage));
            }
        }
        else if (!isHoodOpen)
        {
            if (audioSource != null && closeSound != null)
            {
                audioSource.Stop();
                audioSource.PlayOneShot(closeSound);
            }
        }
    }

    IEnumerator ShowMessage(string message)
    {
        Debug.Log("ShowMessage 실행됨: " + message);
        messageText.text = message;
        messageText.enabled = true;
        isDialogueActive = true;
        float timer = 0f;

        while (timer < messageDuration)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                break;
            }
            timer += Time.deltaTime;
            yield return null;
        }

        messageText.enabled = false;
        isDialogueActive = false;
        messageText.gameObject.SetActive(false);
        if (chatPanel != null) SetActiveRecursively(chatPanel, false); // 패널과 자식 모두 비활성화
    }

    IEnumerator InteractionCooldown()
    {
        canInteract = false;
        yield return new WaitForSeconds(interactionCooldown);
        canInteract = true;
    }

    void SkipCurrentDialogueAndPlayNext()
    {
        if (audioSource != null)
            audioSource.Stop();

        if (messageText != null)
        {
            messageText.enabled = false;
            messageText.gameObject.SetActive(false);
        }

        if (chatPanel != null)
            SetActiveRecursively(chatPanel, false); // 패널과 자식 모두 비활성화

        isDialogueActive = false;
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
