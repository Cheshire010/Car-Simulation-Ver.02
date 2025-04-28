using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using static JYJ_RaycastInteractor;

public class JYJ_InteractionEnableButton : MonoBehaviour, IInteractable
{
    [Header("애니메이션 설정")]
    public Animator animator;
    public float interactionCooldown = 0.5f;

    [Header("상호작용 관리")]
    public JYJ_StroyManager storyManager;

    private bool canInteract = true;
    private bool hasOpened = false;

    public delegate void EnabledHandler();
    public event EnabledHandler OnEnabled;

    public void Interact()
    {
        if (!canInteract || hasOpened) return;

        canInteract = false;
        hasOpened = true;

        PlayOpenAnimation();
        StartCoroutine(InteractionCooldown());

        // 이벤트 호출
        if (OnEnabled != null)
            OnEnabled();
    }

    void PlayOpenAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Open");
        }
        else
        {
            Debug.LogWarning("Animator가 할당되지 않았습니다.", this);
        }

        if (storyManager != null)
        {
            storyManager.EnableInteractions();
        }
        else
        {
            Debug.LogWarning("StoryManager가 할당되지 않았습니다.", this);
        }
    }

    IEnumerator InteractionCooldown()
    {
        yield return new WaitForSeconds(interactionCooldown);
        canInteract = true;
    }
}
