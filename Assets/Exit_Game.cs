using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit_Game : MonoBehaviour
{
    // 이 함수는 버튼에 연결하면 됩니다.
    public void QuitGame()
    {
        // 에디터에서는 작동하지 않지만, 빌드된 게임에서는 작동합니다.
        Application.Quit();

#if UNITY_EDITOR
        // Unity 에디터에서 테스트할 때 에디터 종료
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
