using UnityEngine;
using System.Collections;
using static JYJ_RaycastInteractor;

public class JYJ_InteractionHood : MonoBehaviour, IInteractable
{
    [Header("후드 설정")]
    public Animator hoodAnimator;
    public float interactionCooldown = 0.5f;
    private bool canInteract = true;
    private bool isHoodOpen = false;

    public void Interact()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!canInteract) return;
            ToggleHood();
            StartCoroutine(InteractionCooldown());
        }
    }

    void ToggleHood()
    {
        isHoodOpen = !isHoodOpen;
        hoodAnimator.SetTrigger(isHoodOpen ? "Open" : "Close");
    }

    IEnumerator InteractionCooldown()
    {
        canInteract = false;
        yield return new WaitForSeconds(interactionCooldown);
        canInteract = true;
    }
}
