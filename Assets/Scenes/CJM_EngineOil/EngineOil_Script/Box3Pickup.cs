using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Box3Pickup : MonoBehaviour
{
    [Header("잡힐 위치 (HoldPoint)")]
    public Transform holdPoint;
    [Header("원래 놓일 부모 (테이블 선반)")]
    public Transform originalParent;
    [Header("픽업 최대 거리")]
    public float pickupRange = 3f;

    // 테이블 위에서 저장했던 로컬 트랜스폼
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

        if (originalParent == null)
        {
            Debug.LogError("originalParent 를 할당하세요!", this);
            return;
        }

        // ▶ originalParent에 붙여서 테이블 위 로컬 트랜스폼 저장
        var prevParent = transform.parent;
        transform.SetParent(originalParent, true);
        tableLocalPos = transform.localPosition;
        tableLocalRot = transform.localRotation;
        tableLocalScale = transform.localScale;
        transform.SetParent(prevParent, true);
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

            // ─────────── PIVOT-Y-LOCK PICKUP + Y 회전 ───────────
            // 1) 피벗의 '원래' 월드 Y좌표 저장
            float originalPivotY = transform.position.y;

            // 2) holdPoint 아래로 부모 설정(월드 트랜스폼 그대로 유지)
            transform.SetParent(holdPoint, true);

            // 3) X,Z는 holdPoint, Y는 originalPivotY 로 세팅
            Vector3 newPos = holdPoint.position;
            newPos.y = originalPivotY;
            transform.position = newPos;

            // 4) holdPoint 회전에 90° Y회전 더해주기
            //    (Quaternion.Euler(x, y, z) 은 월드 기준 오일러 회전)
            transform.rotation = holdPoint.rotation * Quaternion.Euler(0f, 90f, 45f);
            // ────────────────────────────────────────────────────

            Debug.Log("Box3 picked up and front rotated to camera");
        }
    }



    [Header("Drop할 레이어")]
    public LayerMask dropLayer;  // Ground 레이어만 체크

    void Drop()
    {
        isHeld = false;
        rb.isKinematic = false;
        rb.useGravity = true;
        col.isTrigger = false;

        // 1) HoldPoint 아래로 레이캐스트 ↓
        Ray down = new Ray(holdPoint.position, Vector3.down);
        if (Physics.Raycast(down, out var hit, 20f, dropLayer))
        {
            // 2) 맞은 지점으로 바로 이동
            transform.position = hit.point;
        }
        else
        {
            // 혹시 레이 맞는 게 없으면 그냥 원래대로(테이블 위) 복원
            transform.SetParent(originalParent, false);
            transform.localPosition = tableLocalPos;
            transform.localRotation = tableLocalRot;
            transform.localScale = tableLocalScale;
            Debug.LogWarning("Ground를 못 찾았습니다. 테이블 복원!");
            return;
        }

        // 3) 부모는 풀어서, 물리로 굴러가게 (원하는 경우)
        transform.SetParent(null, true);

        Debug.Log("Box3 dropped to ground at " + transform.position);
    }
}

