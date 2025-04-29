using UnityEngine;

public class JYJ_Player_Zoom : JYJ_Player_Move
{
    [Header("줌 설정")]
    public Camera playerCamera; // 플레이어 카메라
    public float zoomFOV = 30f; // 줌 시야각 (작을수록 확대)
    public float zoomSpeed = 5f; // 줌 속도

    private float originalFOV; // 원래 시야각
    private bool isZooming = false;

    void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = GetComponent<Camera>();
            if (playerCamera == null)
            {
                Debug.LogWarning("Camera 컴포넌트가 없습니다!");
                return;
            }
        }
        originalFOV = playerCamera.fieldOfView;
    }

    void Update()
    {
        base.Update(); // 부모 클래스의 이동 로직 실행

        // 우클릭 입력 감지
        if (Input.GetMouseButtonDown(1)) // 0:좌클릭, 1:우클릭
        {
            isZooming = true;
        }
        if (Input.GetMouseButtonUp(1))
        {
            isZooming = false;
        }

        // 줌 로직
        if (isZooming)
        {
            playerCamera.fieldOfView = Mathf.Lerp(
                playerCamera.fieldOfView,
                zoomFOV,
                Time.deltaTime * zoomSpeed
            );
        }
        else
        {
            playerCamera.fieldOfView = Mathf.Lerp(
                playerCamera.fieldOfView,
                originalFOV,
                Time.deltaTime * zoomSpeed
            );
        }
    }
}
