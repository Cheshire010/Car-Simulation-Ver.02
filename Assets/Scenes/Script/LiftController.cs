using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator), typeof(Collider))]
public class LiftController : MonoBehaviour
{
    Animator animator;
    Collider col;
    bool isUp = false;
    bool isBusy = false;

    [Header("Animator Triggers")]
    public string upTrigger = "Lift_up";
    public string downTrigger = "Lift_down";

    [Header("Motion Durations (sec)")]
    public float upDuration = 2.0f;
    public float downDuration = 2.0f;

    void Awake()
    {
        animator = GetComponent<Animator>();
        col = GetComponent<Collider>();
        col.isTrigger = false;
    }

    // 외부에서 호출되는 토글 메서드
    public void ToggleLift()
    {
        if (isBusy) return;     // 모션 중엔 아무 동작도 하지 않음

        if (isUp)
        {
            animator.ResetTrigger(upTrigger);
            animator.SetTrigger(downTrigger);
            StartCoroutine(MotionLock(downDuration));
        }
        else
        {
            animator.ResetTrigger(downTrigger);
            animator.SetTrigger(upTrigger);
            StartCoroutine(MotionLock(upDuration));
        }

        isUp = !isUp;
    }

    // 지정된 시간 동안 isBusy=true → false 로 되돌리기
    IEnumerator MotionLock(float duration)
    {
        isBusy = true;
        yield return new WaitForSeconds(duration);
        isBusy = false;
    }
}
