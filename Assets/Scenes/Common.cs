using System.Collections;
using UnityEngine;

public class Common : MonoBehaviour
{
    public Transform carDoorTransform;
    public Transform gloveBoxTransform;
    public Transform airconFilterTransform;

    private bool isActionInProgress = false;

    // 1. 회전 + 이동 (순서 선택 가능)
    public void PerformAction(Transform target, float rotationAngle, Vector3 moveOffset, bool rotateFirst, string axis = "Y", bool hideAfter = false)
    {
        if (!isActionInProgress && target != null)
        {
            StartCoroutine(PerformActionCoroutine(target, rotationAngle, moveOffset, rotateFirst, axis, hideAfter));
        }
    }

    private IEnumerator PerformActionCoroutine(Transform target, float rotationAngle, Vector3 moveOffset, bool rotateFirst, string axis, bool hideAfter)
    {
        isActionInProgress = true;

        Quaternion startRotation = target.localRotation;
        Quaternion targetRotation = GetTargetRotation(startRotation, rotationAngle, axis);

        Vector3 startPosition = target.localPosition;
        Vector3 targetPosition = startPosition + moveOffset;

        float duration = 1f;
        float elapsed = 0f;

        if (rotateFirst)
        {
            yield return RotateOverTime(target, startRotation, targetRotation, duration);
            yield return MoveOverTime(target, startPosition, targetPosition, duration);
        }
        else
        {
            yield return MoveOverTime(target, startPosition, targetPosition, duration);
            yield return RotateOverTime(target, startRotation, targetRotation, duration);
        }

        if (hideAfter)
        {
            target.gameObject.SetActive(false);
        }

        isActionInProgress = false;
    }

    // 2. 이동만
    public void PerformMoveOnly(Transform target, Vector3 moveOffset, bool hideAfter = false)
    {
        if (!isActionInProgress && target != null)
        {
            StartCoroutine(PerformMoveOnlyCoroutine(target, moveOffset, hideAfter));
        }
    }

    private IEnumerator PerformMoveOnlyCoroutine(Transform target, Vector3 moveOffset, bool hideAfter)
    {
        isActionInProgress = true;

        Vector3 startPosition = target.localPosition;
        Vector3 targetPosition = startPosition + moveOffset;

        float duration = 1f;
        float elapsed = 0f;

        yield return MoveOverTime(target, startPosition, targetPosition, duration);

        if (hideAfter)
        {
            target.gameObject.SetActive(false);
        }

        isActionInProgress = false;
    }

    // 3. 회전만
    public void PerformRotateOnly(Transform target, float rotationAngle, string axis = "Y", bool hideAfter = false)
    {
        if (!isActionInProgress && target != null)
        {
            StartCoroutine(PerformRotateOnlyCoroutine(target, rotationAngle, axis, hideAfter));
        }
    }

    private IEnumerator PerformRotateOnlyCoroutine(Transform target, float rotationAngle, string axis, bool hideAfter)
    {
        isActionInProgress = true;

        Quaternion startRotation = target.localRotation;
        Quaternion targetRotation = GetTargetRotation(startRotation, rotationAngle, axis);

        float duration = 1f;
        float elapsed = 0f;

        yield return RotateOverTime(target, startRotation, targetRotation, duration);

        if (hideAfter)
        {
            target.gameObject.SetActive(false);
        }

        isActionInProgress = false;
    }

    // 공통 보조 메서드: 회전 목표 계산
    private Quaternion GetTargetRotation(Quaternion startRotation, float rotationAngle, string axis)
    {
        switch (axis.ToUpper())
        {
            case "X":
                return Quaternion.Euler(rotationAngle, startRotation.eulerAngles.y, startRotation.eulerAngles.z);
            case "Y":
                return Quaternion.Euler(startRotation.eulerAngles.x, rotationAngle, startRotation.eulerAngles.z);
            case "Z":
                return Quaternion.Euler(startRotation.eulerAngles.x, startRotation.eulerAngles.y, rotationAngle);
            default:
                return startRotation; // 잘못된 입력이면 회전 없이
        }
    }

    // 공통 보조 메서드: 이동 애니메이션
    private IEnumerator MoveOverTime(Transform target, Vector3 start, Vector3 end, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            target.localPosition = Vector3.Lerp(start, end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        target.localPosition = end;
    }

    // 공통 보조 메서드: 회전 애니메이션
    private IEnumerator RotateOverTime(Transform target, Quaternion start, Quaternion end, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            target.localRotation = Quaternion.Slerp(start, end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        target.localRotation = end;
    }
}
