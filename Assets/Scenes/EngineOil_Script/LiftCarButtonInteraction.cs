using UnityEngine;

[RequireComponent(typeof(Collider))]
public class LiftCarButtonInteraction : MonoBehaviour
{
    public LiftController liftController;
    public CarController carController;

    void Awake()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = false;
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
