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
        RaycastHit hit;
        bool detected = Physics.Raycast(
            Camera.main.transform.position,
            Camera.main.transform.forward,
            out hit,
            detectDistance
        );

        if (detected && hit.collider.CompareTag(targetTag))
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

                // 좌클릭 시 상호작용
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
        else
        {
            RemoveHighlight();
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
