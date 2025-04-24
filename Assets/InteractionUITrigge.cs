using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionUITrigger : MonoBehaviour, RaycastInteractor.IInteractable
{
    public GameObject uiPanel; // 띄울 UI 패널 (Canvas 안에 있는 패널 등)

    public void Interact()
    {
        if (uiPanel != null)
            uiPanel.SetActive(true);

        // 플레이어 움직임 비활성화
        if (PlayerMove.Instance != null)
        {
            PlayerMove.Instance.enabled = false;
        }

        // 마우스 커서 보이게 + 잠금 해제
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
