using UnityEngine;

public class RaycastInteractor : MonoBehaviour
{
    public static RaycastInteractor Instance { get; private set; }

    [Header("상호작용 설정")]
    public string targetTag = "Interactable"; // 감지할 태그 이름
    public float detectDistance = 2f;         // 감지 거리(미터)
    public Color highlightColor = Color.red;  // 하이라이트 색상

    private Renderer lastRenderer;            // 마지막으로 하이라이트된 오브젝트의 Renderer
    private Color[] originalColors;           // 머티리얼별 원래 색상 저장
    private MaterialPropertyBlock mpb;        // 머티리얼 프로퍼티 블록

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

        mpb = new MaterialPropertyBlock();
    }

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
            detectDistance,
            LayerMask.GetMask("Default") // 오브젝트가 Default 레이어에 있어야 함!
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

                    int matCount = rend.sharedMaterials.Length;
                    originalColors = new Color[matCount];

                    for (int i = 0; i < matCount; i++)
                    {
                        mpb.Clear();
                        rend.GetPropertyBlock(mpb, i);

                        // 머티리얼 프로퍼티블록에 색상이 없으면 머티리얼에서 직접 가져옴
                        Color originalColor = Color.white;
                        if (mpb.HasProperty("_Color"))
                        {
                            originalColor = mpb.GetColor("_Color");
                        }
                        else
                        {
                            originalColor = rend.sharedMaterials[i].HasProperty("_Color")
                                ? rend.sharedMaterials[i].color
                                : Color.white;
                        }
                        originalColors[i] = originalColor;

                        mpb.SetColor("_Color", highlightColor);
                        rend.SetPropertyBlock(mpb, i);
                    }
                }

                // 좌클릭 시 상호작용
                if (Input.GetMouseButtonDown(0))
                {
                    IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                    if (interactable != null)
                    {
                        interactable.Interact();
                        RemoveHighlight(); // 상호작용 후 즉시 하이라이트 해제
                    }
                }
            }
        }
        else
        {
            RemoveHighlight();
        }
    }

    public void RemoveHighlight()
    {
        if (lastRenderer != null && originalColors != null)
        {
            int matCount = lastRenderer.sharedMaterials.Length;
            for (int i = 0; i < matCount; i++)
            {
                mpb.Clear();
                lastRenderer.GetPropertyBlock(mpb, i);

                // 원래 색상 복구
                mpb.SetColor("_Color", originalColors[i]);
                lastRenderer.SetPropertyBlock(mpb, i);
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
