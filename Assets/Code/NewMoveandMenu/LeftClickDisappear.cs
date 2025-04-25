using UnityEngine;

public class LeftClickDisappear : MonoBehaviour, RaycastInteractor.IInteractable
{
    [Header("비활성화 시 활성화할 오브젝트")]
    public GameObject targetObject; // 활성화할 오브젝트

    public void Interact()
    {
        // 자신을 비활성화
        gameObject.SetActive(false);

        // targetObject가 할당되어 있으면 활성화
        if (targetObject != null)
        {
            targetObject.SetActive(true);
        }
    }
}
