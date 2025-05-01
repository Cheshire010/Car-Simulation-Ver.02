using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using static JYJ_RaycastInteractor;

public class JYJ_InteractionEnbayCap : MonoBehaviour, IInteractable
{
    [Header("후드 설정")]
    public Animator hoodAnimator;
    public float interactionCooldown = 0.5f;
    private bool canInteract = true;
    private bool isHoodOpen = false;

    [Header("UI 설정")]
    public GameObject chatPanel;
    public Text messageText;
    public string openMessage = "후드가 열렸습니다!"; // 단일 텍스트로 변경
    public float messageDuration = 2.0f;

    [Header("사운드 설정")]
    public AudioSource audioSource;
    public AudioClip[] soundClips;

    private bool isFirstOpen = true;
    private bool isDialogueActive = false;

    public void Interact()
    {
        if (Input.GetMouseButtonDown(0) && canInteract)
        {
            ToggleHood();
            TriggerUseItemEvent();
            StartCoroutine(InteractionCooldown());
        }
    }

    void ToggleHood()
    {
        isHoodOpen = !isHoodOpen;
        hoodAnimator.SetTrigger(isHoodOpen ? "Open" : "Close");
    }

    void TriggerUseItemEvent()
    {
        if (isFirstOpen && isHoodOpen)
        {
            StartCoroutine(ShowDialogue());
            isFirstOpen = false;
        }
    }

    IEnumerator ShowDialogue()
    {
        SetActiveRecursively(chatPanel, true);
        SetActiveRecursively(messageText.gameObject, true);

        isDialogueActive = true;
        
        messageText.enabled = true; //  텍스트 컴포넌트 활성화 추가

        // 사운드 재생 로직
        for (int i = 0; i < soundClips.Length; i++)
        {
            var clip = soundClips[i];
            if (clip == null) continue;
            messageText.text = openMessage;
            audioSource.clip = clip;
            audioSource.Play();

            float timer = 0f;
            bool skipped = false;

            while (timer < messageDuration)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    skipped = true;
                    break;
                }
                timer += Time.deltaTime;
                yield return null;
            }
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }

        SetActiveRecursively(chatPanel, false);
        SetActiveRecursively(messageText.gameObject, false);
        isDialogueActive = false;
    }



    IEnumerator InteractionCooldown()
    {
        canInteract = false;
        yield return new WaitForSeconds(interactionCooldown);
        canInteract = true;
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
}
