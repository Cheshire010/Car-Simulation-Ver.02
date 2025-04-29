using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class EngineOilPickup : MonoBehaviour
{
    [Header("잡힐 위치")]
    public Transform holdPoint;

    [Header("원래 놓일 테이블 선반(부모)")]
    public Transform tableParent;

    [Header("픽업 설정")]
    public float pickupRange = 3f;

    // 원래 테이블 위 로컬 위치, 회전, 스케일 저장
    Vector3 tableLocalPos;
    Quaternion tableLocalRot;
    Vector3 tableLocalScale;

    Rigidbody rb;
    Collider col;
    bool isHeld = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        col.isTrigger = false;
        rb.useGravity = true;
        rb.isKinematic = false;

        if (tableParent == null)
        {
            Debug.LogError("tableParent 를 할당하세요!", this);
            return;
        }

        // 일단 부모를 tableParent 로 바꿔서 로컬값(위치/회전/스케일)을 저장
        var oldParent = transform.parent;
        transform.SetParent(tableParent, true);

        tableLocalPos = transform.localPosition;
        tableLocalRot = transform.localRotation;
        tableLocalScale = transform.localScale;

        // 원래대로
        transform.SetParent(oldParent, true);
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
        if (Physics.Raycast(ray, out var hit, pickupRange) &&
            hit.collider == col &&
            holdPoint != null)
        {
            isHeld = true;
            rb.isKinematic = true;
            rb.useGravity = false;
            col.isTrigger = true;

            // 카메라 앞 HoldPoint 자식으로 붙인다
            transform.SetParent(holdPoint, false);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;  // HoldPoint 에 스케일 영향받지 않도록

            Debug.Log("Picked up");
        }
    }

    void Drop()
    {
        isHeld = false;
        rb.isKinematic = false;
        rb.useGravity = true;
        col.isTrigger = false;

        // 다시 테이블 선반 자식으로 붙이고, 
        transform.SetParent(tableParent, false);

        // 저장해 둔 로컬 값으로 복원
        transform.localPosition = tableLocalPos;
        transform.localRotation = tableLocalRot;
        transform.localScale = tableLocalScale;

        Debug.Log("Dropped back to table");
    }
}
