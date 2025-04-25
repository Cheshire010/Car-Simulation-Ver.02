using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using static JYJ_RaycastInteractor;

public class JYJ_InteractionEnbayCap : MonoBehaviour, IInteractable
{
    [Header("후드 설정")]
    public Animator hoodAnimator;
    public float interactionCooldown = 0.5f;
    private bool canInteract = true;
    private bool isHoodOpen = false;

    [Header("채팅 설정")]
    public JYJ_ChatManager chatManager;
    public string[] useItemDialogue = {
        "후드가 열렸습니다!",
        "차량 부품을 확인할 수 있습니다.",
        "세척을 시작하세요."
    };

    [Header("사운드 설정")] // 추가된 부분
    public AudioSource audioSource;
    public AudioClip[] soundClips;

    private bool isFirstOpen = true;

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
            // 수정된 부분: this 참조 전달
            chatManager.StartChat(useItemDialogue);
            isFirstOpen = false;
        }
    }

    // 추가된 사운드 제어 메서드
    public void PlayChatSound(int index)
    {
        if (audioSource.isPlaying)
            audioSource.Stop();

        if (soundClips != null && index < soundClips.Length)
            audioSource.PlayOneShot(soundClips[index]);
    }

    public void StopAllSounds()
    {
        if (audioSource.isPlaying)
            audioSource.Stop();
    }

    IEnumerator InteractionCooldown()
    {
        canInteract = false;
        yield return new WaitForSeconds(interactionCooldown);
        canInteract = true;
    }
}
