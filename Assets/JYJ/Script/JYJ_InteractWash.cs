using UnityEngine;
using static JYJ_RaycastInteractor;
using System.Collections;

public class JYJ_InteractWash : MonoBehaviour, IInteractable
{
    [Header("애니메이터 설정")]
    public Animator washerAnimator;
    public string animationName = "Washer";

    [Header("파티클 설정")]
    public ParticleSystem particleEffect;

    [Header("채팅 설정")]
    public JYJ_ChatManager chatManager;
    public string[] completionMessage = { "테이블에 문제가 생성되었습니다. 문제를 풀어주십시오." };

    [Header("활성화 오브젝트")]
    public GameObject tableObject;

    [Header("사운드 설정")] // 추가된 부분
    public AudioSource audioSource;
    public AudioClip[] soundClips;

    public void Interact()
    {
        if (washerAnimator == null)
            washerAnimator = GetComponent<Animator>();

        washerAnimator?.Play(animationName, -1, 0f);
        StartCoroutine(HandleParticle());
    }

    private IEnumerator HandleParticle()
    {
        yield return new WaitForSeconds(1f);

        if (particleEffect != null)
        {
            particleEffect.gameObject.SetActive(true);
            particleEffect.Play();
            yield return new WaitForSeconds(5f);
            particleEffect.Stop();
            particleEffect.gameObject.SetActive(false);
        }

        if (chatManager != null)
        {
            // 수정된 부분: this 매개변수 추가
            chatManager.StartChat(completionMessage);
        }

        if (tableObject != null) tableObject.SetActive(true);
        gameObject.SetActive(false);
    }

    // 추가된 메서드들
    public void PlayChatSound(int index)
    {
        if (audioSource.isPlaying)
            audioSource.Stop();

        if (index < soundClips.Length)
            audioSource.PlayOneShot(soundClips[index]);
    }

    public void StopAllSounds()
    {
        if (audioSource.isPlaying)
            audioSource.Stop();
    }
}
