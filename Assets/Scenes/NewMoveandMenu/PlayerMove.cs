using UnityEngine;
using System.Collections;

public class PlayerMove : MonoBehaviour
{
    public static PlayerMove Instance { get; private set; }

    public float moveSpeed = 5f;
    public float mouseSensitivity = 1500f;
    public float jumpForce = 12f;

    private float xRotation = 0f;
    private Rigidbody rb;

    private bool isGrounded = true;

    // [추가] 1초간 정면 고정 관련 변수
    private bool canLook = false;
    private float lookDelay = 1f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.freezeRotation = true;
        rb.mass = 1f;
        rb.useGravity = true;

        if (GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<CapsuleCollider>();
        }

        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera.transform.SetParent(transform);
            mainCamera.transform.localPosition = new Vector3(0, 0.5f, 0);
            mainCamera.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }

        transform.rotation = Quaternion.Euler(0, 0, 0);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // [추가] 1초간 정면 고정 코루틴 실행
        StartCoroutine(EnableLookAfterDelay());
    }

    private IEnumerator EnableLookAfterDelay()
    {
        canLook = false;
        xRotation = 0f;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        if (Camera.main != null)
            Camera.main.transform.localRotation = Quaternion.Euler(0, 0, 0);

        yield return new WaitForSeconds(lookDelay);
        canLook = true;
    }

    void Update()
    {
        if (MenuManager.GameIsPaused)
            return;

        // 이동
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = transform.TransformDirection(new Vector3(h, 0, v)) * moveSpeed;
        Vector3 velocity = rb.velocity;
        velocity.x = move.x;
        velocity.z = move.z;
        rb.velocity = velocity;

        // 마우스 회전
        if (canLook)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            if (Camera.main != null)
                Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

            transform.Rotate(Vector3.up * mouseX);
        }
        else
        {
            xRotation = 0f;
            transform.rotation = Quaternion.Euler(0, 0, 0);
            if (Camera.main != null)
                Camera.main.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }

        // 점프
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
