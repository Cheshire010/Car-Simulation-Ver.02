using UnityEngine;

public class OilFilterRaycastPicker : MonoBehaviour
{
    [Header("레이캐스트 최대 거리")]
    public float maxDistance = 5f;

    [Header("필터 전용 레이어 (필터 오브젝트들을 이 레이어에 배치하세요)")]
    public LayerMask filterLayer;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, filterLayer))
            {
                // 레이캐스트가 필터 레이어의 콜라이더에 맞았다!
                var filterObj = hit.collider.gameObject;
                Debug.Log($"Oil Filter clicked: {filterObj.name}");

                // 여기서 필터 교체 로직 호출
                // 예: filterObj.GetComponent<OilFilterCoverInteraction>()?.OnFilterClicked();
            }
        }
    }
}
