using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizTrigger : MonoBehaviour
{
    public GameObject quizUIPanel;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == transform)
                {
                    ShowQuiz();
                }
            }
        }
    }

    void ShowQuiz()
    {
        quizUIPanel.SetActive(true);
        LockUIState(true);
    }

    public void HideQuiz()
    {
        quizUIPanel.SetActive(false);
        LockUIState(false);
    }

    void LockUIState(bool isLocked)
    {
        //// 플레이어 움직임 제한
        //if (PlayerMove.Instance != null)
        //    PlayerMove.Instance.enabled = !isLocked;

        // 레이캐스트 상호작용 제한
        if (RaycastInteractor.Instance != null)
            RaycastInteractor.Instance.enabled = !isLocked;

        // 커서 설정
        Cursor.lockState = isLocked ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isLocked;
    }
}
