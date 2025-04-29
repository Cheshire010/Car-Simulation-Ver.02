using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Pickupable : MonoBehaviour
{
    [Header("잡힐 위치 (카메라 자식 HoldPoint)")]
    public Transform holdPoint;

    [Header("다시 놓일 부모 (테이블/선반)")]
    public Transform dropParent;

    [Header("레이캐스트 최대 거리")]
    public float pickupRange = 3f;

    // 원래 테이블 위 로컬 위치/회전/스케일 저장
    private Vector3 origLocalPos;
    private Quaternion origLocalRot;
    private Vector3 origLocalScale;

    private Rigidbody rb;
    private Collider col;
    private bool isHeld;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        col.isTrigger = false;
        rb.useGravity = true;
        rb.isKinematic = false;

        if (dropParent == null)
        {
            Debug.LogError($"[{name}] DropParent(다시 놓일 부모)를 할당하세요!", this);
            enabled = false;
            return;
        }

        // 부모를 dropParent로 바꿔서 로컬 transform 저장
        var oldP = transform.parent;
        transform.SetParent(dropParent, true);

        origLocalPos = transform.localPosition;
        origLocalRot = transform.localRotation;
        origLocalScale = transform.localScale;

        transform.SetParent(oldP, true);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!isHeld) TryPickup();
            else Drop();
        }
    }

    void TryPickup()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, pickupRange) && hit.collider == col)
        {
            if (holdPoint == null)
            {
                Debug.LogWarning($"[{name}] holdPoint를 할당해주세요!", this);
                return;
            }

            isHeld = true;
            rb.isKinematic = true;
            rb.useGravity = false;
            col.isTrigger = true;

            // ★ 부모로 붙이되 월드 트랜스폼(포지션·회전·스케일) 유지 ★
            transform.SetParent(holdPoint, true);
            // HoldPoint 월드 위치/회전으로 맞추기
            transform.position = holdPoint.position;
            transform.rotation = holdPoint.rotation;
            // 스케일은 전혀 건드리지 않으므로 원래 월드 스케일 그대로 유지됩니다.

            Debug.Log($"{name} picked up");
        }
    }


    void Drop()
    {
        isHeld = false;
        rb.isKinematic = false;
        rb.useGravity = true;
        col.isTrigger = false;

        // 월드 트랜스폼 그대로 유지하며 부모만 바꿔줍니다.
        transform.SetParent(dropParent, true);
        // 그 후 월드->로컬 변환 없이 월드 위치/회전/스케일은 origLocal*dropParent.lossyScale 계산 없이
        // 정확히 테이블 위에 놓이도록 transform.localPosition/Rotation/Scale을 미리 저장한 값으로 바꿔줍니다.
        transform.localPosition = origLocalPos;
        transform.localRotation = origLocalRot;
        transform.localScale = origLocalScale;

        Debug.Log($"{name} dropped");
    }

}
