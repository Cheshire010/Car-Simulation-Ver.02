using UnityEngine;
using static RaycastInteractor;
using System.Collections;

public class InteractWash : MonoBehaviour, IInteractable
{
    [Header("애니메이터 설정")]
    public Animator washerAnimator;
    public string animationName = "Washer";

    [Header("파티클 설정")]
    public ParticleSystem particleEffect;

    [Header("채팅 설정")]
    public ChatManager chatManager;
    public string[] completionMessage = { "테이블에 문제가 생성되었습니다. 문제를 풀어주십시오." };

    [Header("활성화 오브젝트")]
    public GameObject tableObject; // 활성화시킬 테이블 오브젝트

    public void Interact()
    {
        // 애니메이션 실행
        if (washerAnimator == null)
            washerAnimator = GetComponent<Animator>();

        washerAnimator?.Play(animationName, -1, 0f);

        // 파티클 코루틴 시작
        StartCoroutine(HandleParticle());
    }

    private IEnumerator HandleParticle()
    {
        // 1초 대기
        yield return new WaitForSeconds(1f);

        if (particleEffect != null)
        {
            // 파티클 오브젝트 활성화
            particleEffect.gameObject.SetActive(true);
            particleEffect.Play();
            Debug.Log("파티클 활성화 및 실행");

            // 5초 후 파티클 정지 및 비활성화
            yield return new WaitForSeconds(5f);
            particleEffect.Stop();
            particleEffect.gameObject.SetActive(false);
            Debug.Log("파티클 비활성화");
        }

        // 채팅 메시지 트리거
        if (chatManager != null)
        {
            chatManager.StartChat("WashComplete", completionMessage);
        }
        else
        {
            Debug.LogWarning("ChatManager가 할당되지 않았습니다!");
        }

        // 테이블 오브젝트 활성화
        if (tableObject != null)
        {
            tableObject.SetActive(true);
            Debug.Log("테이블 오브젝트 활성화");
        }

        // 자신도 비활성화
        gameObject.SetActive(false);
        Debug.Log("자신 비활성화");
    }
}
