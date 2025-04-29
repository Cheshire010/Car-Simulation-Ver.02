using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EngineOilEffect : MonoBehaviour
{
    [Header("엔진오일 워터 파티클 프리팹")]
    public ParticleSystem fillEffectPrefab;

    [Header("파티클이 나올 지점 (뚜껑 스포트)")]
    public Transform spoutPoint;

    // 재생 시간 (초)
    public float playDuration = 10f;

    void Awake()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = false;
    }

    void OnMouseDown()
    {
        if (fillEffectPrefab == null || spoutPoint == null)
        {
            Debug.LogWarning("fillEffectPrefab 또는 spoutPoint 를 할당하세요.");
            return;
        }

        // 1) 파티클 인스턴스화
        var ps = Instantiate(
            fillEffectPrefab,
            spoutPoint.position,
            spoutPoint.rotation);

        // 2) 재생
        ps.Play();

        // 3) 일정 시간 후 정지 & 제거
        StartCoroutine(StopAndDestroy(ps));
    }

    IEnumerator StopAndDestroy(ParticleSystem ps)
    {
        // playDuration 만큼 대기
        yield return new WaitForSeconds(playDuration);

        // 정지
        ps.Stop();

        // 파티클이 완전히 사라질 때까지 잠시 대기 (옵션)
        yield return new WaitForSeconds(ps.main.startLifetime.constantMax);

        // 게임 오브젝트 제거
        Destroy(ps.gameObject);
    }
}
