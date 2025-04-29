using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class BonnetInteraction : MonoBehaviour
{
    Animator animator;
    bool isOpen = false;  // 현재 열림 상태인지
    bool isBusy = false;  // 모션 중 클릭 잠금

    [Header("Lock Durations (sec)")]
    public float openDuration = 1.0f;  // Open_Bonnet 애니메이션 길이
    public float closeDuration = 1.0f;  // Close_Bonnet 애니메이션 길이

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isBusy) return;                 // 모션 중이면 무시
        if (!Input.GetMouseButtonDown(0)) return;

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, 100f) &&
            hit.collider.transform.IsChildOf(transform))
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
            Debug.Log(isOpen ? "Open" : "Close");
        }
    }

    // duration 동안 클릭 잠금 → 끝나면 풀어줌
    IEnumerator MotionLock(float duration)
    {
        isBusy = true;
        yield return new WaitForSeconds(duration);
        isBusy = false;
    }
}
