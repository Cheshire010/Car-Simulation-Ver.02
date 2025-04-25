using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using static RaycastInteractor;

public class InteractionEnbayCap : MonoBehaviour, IInteractable
{
    [Header("후드 설정")]
    public Animator hoodAnimator;
    public float interactionCooldown = 0.5f;
    private bool canInteract = true;
    private bool isHoodOpen = false;

    [Header("채팅 설정")]
    public ChatManager chatManager;
    public string[] useItemDialogue = {
        "후드가 열렸습니다!",
        "차량 부품을 확인할 수 있습니다.",
        "세척을 시작하세요."
    };

    private bool isFirstOpen = true;

    public void Interact()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!canInteract) return;
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
            chatManager.StartChat("UseItem", useItemDialogue);
            isFirstOpen = false;
        }
    }

    IEnumerator InteractionCooldown()
    {
        canInteract = false;
        yield return new WaitForSeconds(interactionCooldown);
        canInteract = true;
    }
}
