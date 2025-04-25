using UnityEngine;
using UnityEngine.SceneManagement;

public class TireScene_MenuManager : MonoBehaviour
{
    public TireScene_MenuManager Instance { get; private set; }
    public GameObject pauseMenuUI;
    public bool GameIsPaused { get; private set; } = false;

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // 씬 로드 시 이벤트 제거
    }

    void Start()
    {
        // 씬 로드 시 자동으로 호출되는 이벤트를 추가합니다.
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        GameIsPaused = true;
    }

    public void ResumeGame()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        GameIsPaused = false;
    }

    public void OnContinueButton()
    {
        ResumeGame();
    }

    public void OnExitButton()
    {
        SceneManager.LoadScene("Lobby_Scene");
    }

    // 씬이 로드될 때 자동으로 pauseMenuUI를 비활성화
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 새로운 씬이 로드되면, pauseMenuUI를 비활성화하고 게임 상태를 다시 진행 상태로 설정
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        GameIsPaused = false; // 일시 정지 상태 초기화
    }

    public void RestartScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void LoadTitleScene()
    {
        SceneManager.LoadScene("Title_Scene"); // "TitleScene"을 타이틀 씬 이름으로 바꿔줘
    }
}