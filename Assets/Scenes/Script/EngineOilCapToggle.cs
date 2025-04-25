using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Collider))]
public class EngineOilCapToggles : MonoBehaviour
{
    [Header("Highlight")]
    public Color highlightColor = Color.yellow;

    Animator animator;
    Collider col;
    Renderer[] rends;
    Color[] originalColors;

    bool isOpen = false;
    bool isHovering = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
        col = GetComponent<Collider>();
        col.isTrigger = false; // 물리 충돌용

        // 모든 자식 렌더러와 원래 색 저장
        rends = GetComponentsInChildren<Renderer>();
        originalColors = new Color[rends.Length];
        for (int i = 0; i < rends.Length; i++)
            originalColors[i] = rends[i].material.color;
    }

    void Update()
    {
        HandleHoverHighlight();
        HandleClickToggle();
    }

    void HandleHoverHighlight()
    {
        // 마우스 포인터 레이캐스트
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool hitMe = Physics.Raycast(ray, out RaycastHit hit, 5f) &&
                     hit.collider.transform.IsChildOf(transform);

        // hover 상태가 바뀌면 색 변경/복구
        if (hitMe != isHovering)
        {
            isHovering = hitMe;
            if (isHovering)
                ApplyHighlight();
            else
                ClearHighlight();
        }
    }

    void ApplyHighlight()
    {
        foreach (var r in rends)
            r.material.color = highlightColor;
    }

    void ClearHighlight()
    {
        for (int i = 0; i < rends.Length; i++)
            rends[i].material.color = originalColors[i];
    }

    void HandleClickToggle()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 5f) &&
            hit.collider.transform.IsChildOf(transform))
        {
            // 토글 여닫기
            if (isOpen)
                animator.SetTrigger("Close_Cap");
            else
                animator.SetTrigger("Open_Cap");

            isOpen = !isOpen;
        }
    }
}
