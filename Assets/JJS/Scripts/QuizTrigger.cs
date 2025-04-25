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

        if (playerController == null)
        {
            playerController = FindObjectOfType<TireScene_PlayerMove>();
            if (playerController == null)
                Debug.LogWarning("PlayerMove 컴포넌트를 찾을 수 없습니다!");
        }

        if (raycastInteractor == null)
        {
            raycastInteractor = FindObjectOfType<TireScene_RayCast>();
            if (raycastInteractor == null)
                Debug.LogWarning("RaycastInteractor 컴포넌트를 찾을 수 없습니다!");
        }

        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i;
            answerButtons[i].onClick.AddListener(() => CheckAnswer(index));
        }
    }

    public void Interact() // IInteractable 구현
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
        SetPlayerControl(false);

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

        QuizQuestion currentQuestion = selectedQuestions[currentQuestionIndex];
        questionText.text = currentQuestion.question;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].GetComponentInChildren<Text>().text = currentQuestion.answers[i];
        }

        currentQuestionIndex++;
    }

    void CheckAnswer(int selectedIndex) // 오류 수정 확인
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
        StartCoroutine(CloseQuizAfterDelay(3f));
    }

    IEnumerator CloseQuizAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        quizPanel.SetActive(false);
        resultText.gameObject.SetActive(false);
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
