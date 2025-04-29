using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class JYJ_Quad : MonoBehaviour, JYJ_RaycastInteractor.IInteractable
{
    [System.Serializable]
    public class QuizQuestion
    {
        public string question;
        public string[] answers = new string[4];
        public int correctIndex;
    }

    [Header("UI 설정")]
    public GameObject quizPanel;
    public Text questionText;
    public Button[] answerButtons;
    public Text resultText;
    public Button closeButton; // 종료 버튼 추가

    [Header("문제 데이터")]
    public List<QuizQuestion> questionPool = new List<QuizQuestion>();

    [Header("참조 설정")]
    public JYJ_Player_Move playerController;
    public JYJ_RaycastInteractor raycastInteractor;

    private List<QuizQuestion> selectedQuestions = new List<QuizQuestion>();
    private int currentQuestionIndex = 0;
    private int score = 0;
    private bool isQuizActive = false;

    void Start()
    {
        quizPanel.SetActive(false);
        resultText.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false); // 종료 버튼 비활성화

        if (playerController == null)
        {
            playerController = FindObjectOfType<JYJ_Player_Move>();
            if (playerController == null)
                Debug.LogWarning("PlayerMove 컴포넌트를 찾을 수 없습니다!");
        }

        if (raycastInteractor == null)
        {
            raycastInteractor = FindObjectOfType<JYJ_RaycastInteractor>();
            if (raycastInteractor == null)
                Debug.LogWarning("RaycastInteractor 컴포넌트를 찾을 수 없습니다!");
        }

        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i;
            answerButtons[i].onClick.AddListener(() => CheckAnswer(index));
        }

        // 종료 버튼 리스너 등록
        closeButton.onClick.AddListener(CloseQuiz);
    }

    public void Interact()
    {
        if (!isQuizActive)
        {
            StartQuiz();
        }
    }

    void StartQuiz()
    {
        isQuizActive = true;
        score = 0;
        currentQuestionIndex = 0;
        selectedQuestions = GetRandomQuestions(5);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        quizPanel.SetActive(true);
        resultText.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false); // 시작 시 종료 버튼 숨김
        SetPlayerControl(false);

        // 정답 버튼 모두 다시 활성화
        foreach (var btn in answerButtons)
        {
            btn.interactable = true;
            // btn.gameObject.SetActive(true); // 버튼을 아예 숨겼었다면 주석 해제
        }

        ShowNextQuestion();
    }


    List<QuizQuestion> GetRandomQuestions(int count)
    {
        if (questionPool.Count < count)
        {
            Debug.LogError($"최소 {count}개 이상의 문제가 필요합니다!");
            return new List<QuizQuestion>();
        }

        List<QuizQuestion> tempList = new List<QuizQuestion>(questionPool);
        List<QuizQuestion> selected = new List<QuizQuestion>();

        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, tempList.Count);
            selected.Add(tempList[randomIndex]);
            tempList.RemoveAt(randomIndex);
        }
        return selected;
    }

    void ShowNextQuestion()
    {
        if (currentQuestionIndex >= selectedQuestions.Count)
        {
            EndQuiz();
            return;
        }

        // 문제 풀이 중에는 항상 종료 버튼 숨김
        if (closeButton != null)
            closeButton.gameObject.SetActive(false);

        QuizQuestion currentQuestion = selectedQuestions[currentQuestionIndex];
        questionText.text = currentQuestion.question;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].GetComponentInChildren<Text>().text = currentQuestion.answers[i];
        }

        currentQuestionIndex++;
    }


    void CheckAnswer(int selectedIndex)
    {
        QuizQuestion currentQuestion = selectedQuestions[currentQuestionIndex - 1];

        if (selectedIndex == currentQuestion.correctIndex)
        {
            score++;
            Debug.Log("정답!");
        }
        else
        {
            Debug.Log("오답!");
        }

        ShowNextQuestion();
    }

    void EndQuiz()
    {
        resultText.text = $"점수: {score}/{selectedQuestions.Count}";
        resultText.gameObject.SetActive(true);

        if (closeButton != null)
        {
            closeButton.gameObject.SetActive(true);
            closeButton.interactable = true;
        }

        // 정답 버튼 모두 비활성화
        foreach (var btn in answerButtons)
        {
            btn.interactable = false;
            // btn.gameObject.SetActive(false); // 버튼을 아예 숨기고 싶다면 주석 해제
        }
    }


    void CloseQuiz()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        quizPanel.SetActive(false);
        resultText.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false); // 종료 버튼 비활성화
        SetPlayerControl(true);
        isQuizActive = false;
    }

    void SetPlayerControl(bool enable)
    {
        if (playerController != null)
            playerController.enabled = enable;
        else
            Debug.LogWarning("PlayerMove 참조 없음");

        if (raycastInteractor != null)
            raycastInteractor.enabled = enable;
        else
            Debug.LogWarning("RaycastInteractor 참조 없음");
    }
}
