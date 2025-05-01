using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Start_Game : MonoBehaviour
{
    public void LoadScene()
    {
        SceneManager.LoadScene("Lobby_Scene");
    }
}
