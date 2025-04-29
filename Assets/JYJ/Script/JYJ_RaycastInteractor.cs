using UnityEngine;

public class JYJ_RaycastInteractor : MonoBehaviour
{
    public static JYJ_RaycastInteractor Instance { get; private set; }

    [Header("상호작용 설정")]
    public string targetTag = "Interactable"; // 감지할 태그 이름
    public float detectDistance = 2f;         // 감지 거리(미터)
    public Color highlightColor = Color.red;  // 하이라이트 색상 (빨강)

    private Renderer lastRenderer;            // 마지막으로 하이라이트된 오브젝트의 Renderer
    private Color[] originalColors;           // 모든 머티리얼의 원래 색상 저장

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
                // 모든 머티리얼의 원래 색상 저장 및 빨강으로 변경
                originalColors = new Color[rend.materials.Length];
                for (int i = 0; i < rend.materials.Length; i++)
                {
                    originalColors[i] = rend.materials[i].color;
                    rend.materials[i].color = highlightColor;
                }
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
        if (lastRenderer != null && originalColors != null)
        {
            for (int i = 0; i < lastRenderer.materials.Length; i++)
            {
                if (i < originalColors.Length)
                    lastRenderer.materials[i].color = originalColors[i];
            }
            lastRenderer = null;
            originalColors = null;
        }
    }

    void OnDisable()
    {
        RemoveHighlight();
    }
}
