using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Renderer))]
public class LiftCarButtonInteraction : MonoBehaviour
{
    [Header("컨트롤러 참조")]
    public LiftController liftController;
    public CarController carController;

    [Header("하이라이트 색상")]
    public Color highlightColor = Color.magenta;

    // 내부 상태
    Collider col;
    Renderer rend;
    Color originalColor;
    bool isHighlighted = false;

    void Awake()
    {
        col = GetComponent<Collider>();
        col.isTrigger = false;

        rend = GetComponent<Renderer>();
        // 원래 컬러를 저장
        originalColor = rend.material.color;
    }

    void Update()
    {
        UpdateHighlight();
    }

    void UpdateHighlight()
    {
        // 화면 중앙(혹은 마우스)에서 레이 쏴서 이 콜라이더에 닿는지 확인
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool hitMe = Physics.Raycast(ray, out var hit, Mathf.Infinity)
                     && hit.collider == col;

        if (hitMe && !isHighlighted)
        {
            // 하이라이트
            rend.material.color = highlightColor;
            isHighlighted = true;
        }
        else if (!hitMe && isHighlighted)
        {
            // 원상복구
            rend.material.color = originalColor;
            isHighlighted = false;
        }
    }

    void OnMouseDown()
    {
        if (liftController == null || carController == null)
        {
            Debug.LogWarning("컨트롤러가 할당되지 않았습니다.", this);
            return;
        }

        // ▶ 리프트의 duration을 자동차에 그대로 복사
        carController.upDuration = liftController.upDuration;
        carController.downDuration = liftController.downDuration;

        // ▶ 두 토글을 동시 실행
        liftController.ToggleLift();
        carController.ToggleCar();
    }
}
