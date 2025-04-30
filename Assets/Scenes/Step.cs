using TMPro;
using UnityEngine;
using System.Collections;

public class Step : MonoBehaviour
{
    public Common common;
    public TextMeshProUGUI guideText;

    public AudioSource audioSource;
    public AudioClip c_door_open;
    public AudioClip c_door_close;
    public AudioClip g_box_open;
    public AudioClip g_box_close;
    public AudioClip af_input;
    public AudioClip intro1;
    public AudioClip intro2;
    public AudioClip intro3;
    public AudioClip intro4;
    public AudioClip txt1;
    public AudioClip txt2;
    public AudioClip txt3;
    public AudioClip txt4;
    public AudioClip txt5;
    public AudioClip txt6;
    public AudioClip txt7;
    public AudioClip txt8;

    private bool[] stepActive = new bool[11];
    private bool[] stepLocked = new bool[11];
    private int currentStep = 0;

    private bool c_door = false;
    private bool g_box = false;
    private bool af = false;
    private bool quiz = false;

    private bool isPlayingAudio = false;
    private bool isAnimating = false;

    void Start()
    {
        stepActive[0] = true;
        InitializeSteps();
        ExecuteStep(0);
    }

    void InitializeSteps()
    {
        for (int i = 1; i < stepActive.Length; i++)
        {
            stepActive[i] = false;
        }
    }

    public void ExecuteStep(int step)
    {
        currentStep = step;

        if (!stepActive[step] || stepLocked[step]) return;

        switch (step)
        {
            case 0:
                StartCoroutine(PlayIntroSequence());
                break;
            case 1:
                PlayAudioWithGuide(txt1, "차량 보조석 문을 열어주세요.");
                break;
            case 2:
                PlayAudioWithGuide(txt2, "글로브 박스를 열어주세요.");
                break;
            case 3:
                PlayAudioWithGuide(txt3, "에어컨 필터를 제거해 주세요.");
                break;
            case 4:
                PlayAudioWithGuide(txt4, "새 에어컨 필터를 결합하실때는 방향을 잘 살피셔야 합니다.");
                break;
            case 5:
                PlayAudioWithGuide(txt5, "에어컨 필터를 결합해 주세요.");
                break;
            case 6:
                PlayAudioWithGuide(txt6, "글로브 박스를 닫아 주세요.");
                break;
            case 7:
                PlayAudioWithGuide(txt7, "차문을 닫아 주세요.");
                break;
            case 8:
                PlayAudioWithGuide(txt8, "잘 숙지 하였는지 책상으로 가서 퀴즈를 풀어 확인해보세요.");
                break;
        }
    }

    void PlayAudioWithGuide(AudioClip clip, string guide)
    {
        if (isPlayingAudio) return;
        SetGuideText(guide);
        StartCoroutine(PlayAudioAndUnlock(clip));
    }

    IEnumerator PlayAudioAndUnlock(AudioClip clip)
    {
        isPlayingAudio = true;
        audioSource.PlayOneShot(clip);
        yield return new WaitForSeconds(clip.length);
        isPlayingAudio = false;
    }

    void SetGuideText(string text)
    {
        if (guideText != null)
        {
            guideText.text = text;
        }
    }

    public void CheckAction(Collider collider)
    {
        if (collider == null || isPlayingAudio || isAnimating) return;

        if (collider.name == "e180_mirror_R.002")
        {
            if (currentStep == 1 && !c_door)
            {
                common.PerformRotateOnly(common.carDoorTransform, -60f, "Y");
                c_door = true;
                audioSource.PlayOneShot(c_door_open);
                Next(2);
            }
            else if (currentStep == 7 && c_door)
            {
                common.PerformRotateOnly(common.carDoorTransform, -12.9f, "Y");
                c_door = false;
                audioSource.PlayOneShot(c_door_close);
                Next(8);
            }
        }

        if (collider.name == "e180_dash.009")
        {
            if (currentStep == 2 && !g_box)
            {
                common.PerformRotateOnly(common.gloveBoxTransform, -49.359f, "Z");
                g_box = true;
                audioSource.PlayOneShot(g_box_open);
                Next(3);
            }
            else if (currentStep == 6 && g_box)
            {
                common.PerformRotateOnly(common.gloveBoxTransform, -90.848f, "Z");
                g_box = false;
                audioSource.PlayOneShot(g_box_close);
                Next(7);
            }
        }

        if (collider.name == "e180_dash.002")
        {
            if (currentStep == 3 && !af)
            {
                StartCoroutine(RemoveFilter());
                audioSource.PlayOneShot(af_input);
            }
            else if (currentStep == 4 && af)
            {
                StartCoroutine(RotateFilter());
                af = false;
            }
            else if (currentStep == 5 && !af)
            {
                StartCoroutine(MoveFilterIntoPlace());
                audioSource.PlayOneShot(af_input);
            }
        }

        if (collider.name == "퀴즈")
        {
            if (currentStep == 8 && !quiz)
            {
                quiz = true;
                // 퀴즈 완료 처리
            }
        }
    }

    IEnumerator PlayIntroSequence()
    {
        string[] introTexts = {
            "차량 에어컨 필터는 차량 내 공기 오염을 방지 하는 중요한 요소입니다.",
            "6개월에 한번 혹은 5천 ~8천km마다 갈아주며 본인 차량에 맞는 에어컨 필터를 ",
            "구매하여 직접 교체한다면 비용을 절감 할 수 있습니다.",
            "자, 그럼 지금부터 에어컨 필터를 교체해 보겠습니다."
        };

        AudioClip[] clips = { intro1, intro2, intro3, intro4 };

        isPlayingAudio = true;

        for (int i = 0; i < clips.Length; i++)
        {
            SetGuideText(introTexts[i]);
            audioSource.PlayOneShot(clips[i]);
            float dn = (i == 1) ? 0f : 0.5f;
            yield return new WaitForSeconds(clips[i].length + dn);
        }

        isPlayingAudio = false;
        Next(1);
    }

    IEnumerator RemoveFilter()
    {
        if (isAnimating) yield break;
        isAnimating = true;

        float duration = 1.0f;
        Vector3 startPos = common.airconFilterTransform.localPosition;
        Vector3 endPos = new Vector3(0.41f, 0.6571809f, 0.550599f);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            common.airconFilterTransform.localPosition = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        common.airconFilterTransform.localPosition = endPos;
        common.airconFilterTransform.gameObject.SetActive(false);
        StartCoroutine(AppearNewFilter());

        isAnimating = false;
    }

    IEnumerator AppearNewFilter()
    {
        yield return new WaitForSeconds(1f);

        common.airconFilterTransform.gameObject.SetActive(true);
        Vector3 startPos = new Vector3(0.41f, 0.5571809f, 0.550599f);
        Vector3 targetPos = new Vector3(0.41f, 0.6571809f, 0.550599f);
        common.airconFilterTransform.localPosition = startPos;

        Quaternion rotated = common.airconFilterTransform.localRotation * Quaternion.Euler(0f, 0f, 180f);
        common.airconFilterTransform.localRotation = rotated;

        float elapsed = 0f;
        float duration = 1.0f;
        while (elapsed < duration)
        {
            common.airconFilterTransform.localPosition = Vector3.Lerp(startPos, targetPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        common.airconFilterTransform.localPosition = targetPos;
        common.airconFilterTransform.GetComponent<Collider>().enabled = true;

        af = true;
        Next(4);
    }

    IEnumerator RotateFilter()
    {
        if (isAnimating) yield break;
        isAnimating = true;

        float duration = 1.0f;
        Quaternion startRot = common.airconFilterTransform.localRotation;
        Vector3 endEuler = startRot.eulerAngles + new Vector3(0f, 0f, 180f);
        if (endEuler.z > 360f) endEuler.z -= 360f;
        Quaternion endRot = Quaternion.Euler(endEuler);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            common.airconFilterTransform.localRotation = Quaternion.Slerp(startRot, endRot, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        common.airconFilterTransform.localRotation = endRot;

        isAnimating = false;
        Next(5);
    }

    IEnumerator MoveFilterIntoPlace()
    {
        if (isAnimating) yield break;
        isAnimating = true;

        float duration = 1.0f;
        Vector3 startPos = common.airconFilterTransform.localPosition;
        Vector3 targetPos = new Vector3(0.41f, 0.6571809f, 0.850599f);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            common.airconFilterTransform.localPosition = Vector3.Lerp(startPos, targetPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        common.airconFilterTransform.localPosition = targetPos;

        isAnimating = false;
        Next(6);
    }

    private void Next(int nextStep)
    {
        if (stepLocked[currentStep]) return;
        stepLocked[currentStep] = true;
        stepActive[nextStep] = true;
        ExecuteStep(nextStep);
    }
}
