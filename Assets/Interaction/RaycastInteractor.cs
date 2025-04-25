using UnityEngine;

public class RaycastInteractor : MonoBehaviour
{
    public static RaycastInteractor Instance { get; private set; }

    [Header("상호작용 설정")]
    public string targetTag = "Interactable"; // 감지할 태그 이름
    public float detectDistance = 2f;         // 감지 거리(미터)
    public Color highlightColor = Color.red;  // 하이라이트 색상

    private Renderer lastRenderer;            // 마지막으로 하이라이트된 오브젝트의 Renderer
    private Color originalColor;              // 원래 색상 저장
    //void Awake()
    //{
    //    if (Instance == null)
    //    {
    //        Instance = this;
    //        DontDestroyOnLoad(gameObject);
    //    }
    //    else
    //    {
    //        Destroy(gameObject);
    //    }
    //}
    void Update()
    {
        HandleRaycastInteraction();
    }

    public interface IInteractable
    {
        void Interact();
    }

    void HandleRaycastInteraction()
    {
        RaycastHit[] hits = Physics.RaycastAll(
            Camera.main.transform.position,
            Camera.main.transform.forward,
            detectDistance
        );

        bool detected = false;
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag(targetTag))
            {
                // 가장 가까운 오브젝트만 처리
                detected = true;
                ProcessHit(hit);
                break; // 첫 번째로 발견된 대상에서 멈춤
            }
        }

        if (!detected)
        {
            RemoveHighlight();
        }
    }

    void ProcessHit(RaycastHit hit)
    {
        Renderer rend = hit.collider.GetComponent<Renderer>();
        if (rend != null)
        {
            if (rend != lastRenderer)
            {
                RemoveHighlight();
                lastRenderer = rend;
                originalColor = rend.material.color;
                rend.material.color = highlightColor;
            }

            if (Input.GetMouseButtonDown(0))
            {
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Interact();
                }
            }
        }
    }



    void RemoveHighlight()
    {
        if (lastRenderer != null)
        {
            lastRenderer.material.color = originalColor;
            lastRenderer = null;
        }
    }

    void OnDisable()
    {
        RemoveHighlight();
    }
}
