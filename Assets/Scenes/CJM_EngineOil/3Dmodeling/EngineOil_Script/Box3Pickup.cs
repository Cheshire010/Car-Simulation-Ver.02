using UnityEngine;
using System.Collections;
using System.Linq;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Box3Pickup : MonoBehaviour
{
    [Header("잡힐 위치 (HoldPoint)")]
    public Transform holdPoint;
    [Header("원래 놓일 부모 (테이블 선반)")]
    public Transform originalParent;
    [Header("픽업 최대 거리")]
    public float pickupRange = 3f;
    [Header("Ground로 떨어뜨릴 레이어")]
    public LayerMask dropLayer;
    [Header("PortalEntrance 태그")]
    public string entranceTag = "PortalEntrance";
    [Header("PortalEntrance에 세울 위치/회전")]
    public Vector3 entrancePosition;
    public Vector3 entranceEulerRotation; // Yaw 만 사용해도 됩니다

    // 테이블 위 로컬 트랜스폼 저장용
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

        // 테이블 자식 상태로 붙여서 로컬 트랜스폼 저장
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
            float originalPivotY = transform.position.y;
            transform.SetParent(holdPoint, true);

            Vector3 newPos = holdPoint.position;
            newPos.y = originalPivotY;
            transform.position = newPos;

            // +90° Yaw 회전 예시
            transform.rotation = holdPoint.rotation * Quaternion.Euler(0f, 90f, 0f);
            // ────────────────────────────────────────────────────

            Debug.Log("Box3 picked up and front rotated to camera");
        }
    }

    void Drop()
    {
        isHeld = false;
        rb.isKinematic = false;
        rb.useGravity = true;
        col.isTrigger = false;

        // 1) PortalEntrance 위에 있는지 확인
        Collider[] hits = Physics.OverlapSphere(transform.position, 0.1f);
        bool overEntrance = hits.Any(c => c.CompareTag(entranceTag));

        if (overEntrance)
        {
            // PortalEntrance 전용 “세워질” 위치/회전
            transform.SetParent(null, true);
            transform.position = entrancePosition;
            transform.rotation = Quaternion.Euler(entranceEulerRotation);
            transform.localScale = tableLocalScale;

            Debug.Log("Box3 erected at entrance");
            return;
        }

        // 2) Ground 레이어로 Raycast
        Ray down = new Ray(holdPoint.position, Vector3.down);
        if (Physics.Raycast(down, out var hitInfo, 20f, dropLayer))
        {
            // 콜라이더 높이 절반만큼 위로 띄워서 땅속에 파묻히지 않도록
            float halfHeight = col.bounds.extents.y;
            Vector3 dropPos = hitInfo.point + Vector3.up * halfHeight;

            transform.SetParent(null, true);
            transform.position = dropPos;
            Debug.Log($"Box3 dropped to ground at {dropPos}");
            return;
        }

        // 3) 둘 다 아니면 원래 테이블로 복원
        transform.SetParent(originalParent, false);
        transform.localPosition = tableLocalPos;
        transform.localRotation = tableLocalRot;
        transform.localScale = tableLocalScale;
        Debug.LogWarning("PortalEntrance/ground 둘 다 아니어서 테이블로 복원");
    }

}
