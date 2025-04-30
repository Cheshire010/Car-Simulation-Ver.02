using UnityEngine;

public class Main_camera : MonoBehaviour
{
    // 마우스 감도 설정
    public float mouseSensitivity = 0.005f;

    // 플레이어 본체의 Transform을 연결
    public Transform playerBody;

    // 카메라의 X축 회전 값 (위아래로 회전)
    float xRotation = 0f;

    // 카메라 변수 (유니티에서 드래그하지 않아도 자동으로 할당)
    private Camera playerCamera;

    // Start()는 게임 시작 시에 한 번만 호출
    void Start()
    {
        // 커서를 화면 중앙에 고정
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;  // 커서를 보이지 않게 설정 (선택사항)

        // playerCamera 자동 할당
        playerCamera = GetComponentInChildren<Camera>(); // 자식 객체에 카메라가 있으면 자동으로 할당
    }

    // Update()는 매 프레임마다 호출
    void Update()
    {
        // 마우스 움직임을 받아오고 감도와 deltaTime을 곱해줘서 자연스러운 회전 구현
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // 위아래 회전 (Pitch) - 카메라는 상하로만 회전
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);  // 상하 회전 범위 제한
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f); // 즉시 회전

        // 좌우 회전 (Yaw) - 플레이어 본체 회전
        playerBody.Rotate(Vector3.up * mouseX); // 즉시 회전
    }
}
