using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene_Load : MonoBehaviour
{
    // 로드할 씬 이름을 인스펙터에서 설정할 수 있게 함
    public string sceneName;

    // 버튼에 연결할 함수
    public void LoadScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
