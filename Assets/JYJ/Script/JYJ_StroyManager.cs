using System.Collections.Generic;
using UnityEngine;

public class JYJ_StroyManager : MonoBehaviour
{
    [Header("제거할 Box Collider들")]
    public List<BoxCollider> collidersToRemove;

    [Header("활성화할 버튼 스크립트")]
    public JYJ_InteractionEnableButton interactionEnableButton;

    private Dictionary<GameObject, BoxColliderData> colliderDataMap = new Dictionary<GameObject, BoxColliderData>();

    void Start()
    {
        // 초기 박스 콜리더 데이터 저장 후 제거
        foreach (var collider in collidersToRemove)
        {
            if (collider != null)
            {
                colliderDataMap[collider.gameObject] = new BoxColliderData
                {
                    center = collider.center,
                    size = collider.size,
                    isTrigger = collider.isTrigger
                };
                Destroy(collider);
            }
        }

        // 버튼 이벤트 연결
        if (interactionEnableButton != null)
        {
            interactionEnableButton.OnEnabled += EnableInteractions;
        }
    }

    // 박스 콜리더 재생성 메서드
    public void EnableInteractions()
    {
        foreach (var entry in colliderDataMap)
        {
            GameObject obj = entry.Key;
            BoxColliderData data = entry.Value;

            BoxCollider newCollider = obj.AddComponent<BoxCollider>();
            newCollider.center = data.center;
            newCollider.size = data.size;
            newCollider.isTrigger = data.isTrigger;
        }
        colliderDataMap.Clear();
    }
}

// 박스 콜리더 데이터 저장 클래스
public class BoxColliderData
{
    public Vector3 center;
    public Vector3 size;
    public bool isTrigger;
}
