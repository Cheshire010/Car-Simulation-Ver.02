using UnityEngine;

public class JYJ_Stop : MonoBehaviour
{
    void Update()
    {
        // ESC키 입력을 감지하되, 아무것도 하지 않음 (입력 '소비')
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 아무 동작도 하지 않음
            return;
        }
    }
}
