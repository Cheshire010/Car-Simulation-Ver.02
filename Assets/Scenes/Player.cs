using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;                   // 이동 속도
    public float jumpForce = 5f;                   // 점프 힘
    public Transform groundCheck;                  // 바닥 체크 위치
    public float groundDistance = 0.4f;            // 바닥 체크 거리
    public LayerMask groundMask;                   // 바닥 레이어 마스크
    public GameObject box;                         // 플레이어가 위치할 기준 박스

    public Transform cameraTransform;              // 카메라 Transform (자식)
    public Step step;                              // Step 스크립트 참조

    private Rigidbody rb;
    private bool isGrounded;

    public float crouchOffsetY = -1.8f;            // 앉았을 때 카메라를 얼마나 내릴지 (상대값)
    public float crouchSpeed = 5f;                 // 카메라 위치 이동 속도

    private Vector3 originalCameraLocalPos;        // 원래 카메라 위치 저장
    private Vector3 targetCameraLocalPos;          // 목표 카메라 위치 (앉거나 서있을 때)

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // 카메라가 안 할당되어 있으면 메인 카메라 자동 할당
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }

        // 원래 카메라 위치 저장
        originalCameraLocalPos = cameraTransform.localPosition;
        targetCameraLocalPos = originalCameraLocalPos;

        // 박스 위에 플레이어 위치시키기
        if (box != null)
        {
            Bounds bounds = box.GetComponent<Collider>().bounds;
            Vector3 center = bounds.center;
            transform.position = new Vector3(center.x, bounds.min.y + 1f, center.z);
        }

        // 처음 바라보는 방향 설정
        transform.LookAt(new Vector3(0, transform.position.y, 0));
    }

    void Update()
    {
        // 마우스 클릭 시 레이캐스트 발사
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f))
            {
                Debug.DrawLine(ray.origin, hit.point, Color.red);
                Debug.Log("Ray hit: " + hit.collider.name);
                step.CheckAction(hit.collider);
            }
            else
            {
                Debug.DrawRay(ray.origin, ray.direction * 100f, Color.green);
            }
        }

        // 이동 입력 처리
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = transform.right * h + transform.forward * v;
        Vector3 moveVelocity = move * moveSpeed;
        rb.velocity = new Vector3(moveVelocity.x, rb.velocity.y, moveVelocity.z);

        // 바닥 체크 및 점프 처리
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        // Shift 키를 누르고 있으면 카메라 위치를 낮춘다
        if (Input.GetMouseButton(1))
        {
            Debug.Log("앉아");
            targetCameraLocalPos = originalCameraLocalPos + new Vector3(0, crouchOffsetY, 0);
        }
        else
        {
            Debug.Log("일어나");
            targetCameraLocalPos = originalCameraLocalPos;
        }

        // 카메라 위치를 부드럽게 변경
        cameraTransform.localPosition = Vector3.Lerp(
            cameraTransform.localPosition,
            targetCameraLocalPos,
            Time.deltaTime * crouchSpeed
        );
    }
}
