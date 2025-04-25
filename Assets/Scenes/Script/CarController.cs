using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator), typeof(Collider))]
public class CarController : MonoBehaviour
{
    Animator animator;
    Collider col;

    bool isUp = false;
    bool isBusy = false;

    [Header("Animator Triggers")]
    public string upTrigger = "Car_up";
    public string downTrigger = "Car_down";

    [Header("Motion Durations (sec)")]
    public float upDuration = 3.0f;
    public float downDuration = 3.0f;

    void Awake()
    {
        animator = GetComponent<Animator>();
        col = GetComponent<Collider>();
        col.isTrigger = false;
    }

    /// <summary>
    /// 버튼에서 호출될 실제 토글 메서드
    /// </summary>
    public void ToggleCar()
    {
        if (isBusy) return;

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

    IEnumerator MotionLock(float duration)
    {
        isBusy = true;
        yield return new WaitForSeconds(duration);
        isBusy = false;
    }
}
