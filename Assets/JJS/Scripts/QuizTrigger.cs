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
    public Button closeButton;
    public Text feedbackText; // 정답/오답 피드백 텍스트

    [Header("문제 데이터")]
    public List<QuizQuestion> questionPool = new List<QuizQuestion>();

    [Header("참조 설정")]
    public TireScene_PlayerMove playerController;
    public TireScene_RayCast raycastInteractor;

    [Header("피드백 설정")]
    public float feedbackDuration = 1.0f; // 피드백 표시 시간

    private List<QuizQuestion> selectedQuestions = new List<QuizQuestion>();
    private int currentQuestionIndex = 0;
    private int score = 0;
    private bool isQuizActive = false;
    private Coroutine feedbackCoroutine;

    void Start()
    {
        quizPanel.SetActive(false);
        resultText.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false);

        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);

        if (playerController == null)
            playerController = FindObjectOfType<TireScene_PlayerMove>();

        if (raycastInteractor == null)
            raycastInteractor = FindObjectOfType<TireScene_RayCast>();

        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i;
            answerButtons[i].onClick.AddListener(() => CheckAnswer(index));
        }

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
        closeButton.gameObject.SetActive(false);
        SetPlayerControl(false);

        foreach (var btn in answerButtons)
            btn.interactable = true;

        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);

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
            answerButtons[i].interactable = true;
        }

        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);

        currentQuestionIndex++;
    }

    void CheckAnswer(int selectedIndex)
    {
        QuizQuestion currentQuestion = selectedQuestions[currentQuestionIndex - 1];

        bool isCorrect = (selectedIndex == currentQuestion.correctIndex);
        if (isCorrect)
        {
            score++;
            ShowFeedback("정답!", Color.green);
        }
        else
        {
            ShowFeedback("오답!", Color.red);
        }

        foreach (var btn in answerButtons)
            btn.interactable = false;

        StartCoroutine(NextQuestionAfterDelay(feedbackDuration));
    }

    void ShowFeedback(string message, Color color)
    {
        if (feedbackText == null) return;

        if (feedbackCoroutine != null)
            StopCoroutine(feedbackCoroutine);

        feedbackCoroutine = StartCoroutine(ShowFeedbackCoroutine(message, color));
    }

    IEnumerator ShowFeedbackCoroutine(string message, Color color)
    {
        feedbackText.text = message;
        feedbackText.color = color;
        feedbackText.gameObject.SetActive(true);

        yield return new WaitForSeconds(feedbackDuration);

        feedbackText.gameObject.SetActive(false);
    }

    IEnumerator NextQuestionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowNextQuestion();
    }

    void EndQuiz()
    {
        resultText.text = $"점수: {score}/{selectedQuestions.Count}";
        resultText.gameObject.SetActive(true);
        closeButton.gameObject.SetActive(true);

        foreach (var btn in answerButtons)
            btn.interactable = false;

        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);
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

        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);
    }

    void SetPlayerControl(bool enable)
    {
        if (playerController != null)
            playerController.enabled = enable;

        if (raycastInteractor != null)
            raycastInteractor.enabled = enable;
    }
}
