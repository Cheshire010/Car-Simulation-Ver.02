using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class QuizTrigger : MonoBehaviour, TireScene_RayCast.IInteractable
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
    public TireScene_PlayerMove playerController;
    public TireScene_RayCast raycastInteractor;

    private List<QuizQuestion> selectedQuestions = new List<QuizQuestion>();
    private int currentQuestionIndex = 0;
    private int score = 0;
    private bool isQuizActive = false;

    void Start()
    {
        quizPanel.SetActive(false);
        resultText.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false); // 종료 버튼 초기 비활성화

        if (playerController == null)
            playerController = FindObjectOfType<TireScene_PlayerMove>();

        if (raycastInteractor == null)
            raycastInteractor = FindObjectOfType<TireScene_RayCast>();

        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i;
            answerButtons[i].onClick.AddListener(() => CheckAnswer(index));
        }

        closeButton.onClick.AddListener(CloseQuiz); // 종료 버튼 리스너 등록
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
        closeButton.gameObject.SetActive(false); // 종료 버튼 숨김
        SetPlayerControl(false);

        // 버튼 재활성화
        foreach (var btn in answerButtons)
            btn.interactable = true;

        ShowNextQuestion();
    }

    List<QuizQuestion> GetRandomQuestions(int count)
    {
        List<QuizQuestion> selected = new List<QuizQuestion>();
        List<QuizQuestion> tempList = new List<QuizQuestion>(questionPool);

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
        closeButton.gameObject.SetActive(true); // 종료 버튼 활성화

        // 버튼 비활성화
        foreach (var btn in answerButtons)
            btn.interactable = false;
    }

    void CloseQuiz()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        quizPanel.SetActive(false);
        resultText.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false);
        SetPlayerControl(true);
        isQuizActive = false;
    }

    void SetPlayerControl(bool enable)
    {
        if (playerController != null)
            playerController.enabled = enable;

        if (raycastInteractor != null)
            raycastInteractor.enabled = enable;
    }
}
