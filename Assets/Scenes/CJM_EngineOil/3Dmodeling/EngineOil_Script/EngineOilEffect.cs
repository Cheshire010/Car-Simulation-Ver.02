using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EngineOilEffect : MonoBehaviour
{
    [Header("엔진오일 워터 파티클 프리팹")]
    public ParticleSystem fillEffectPrefab;

    [Header("파티클이 나올 지점 (뚜껑 스포트)")]
    public Transform spoutPoint;

    [Header("재생 시간 (초)")]
    public float playDuration = 10f;

    void Awake()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = false; // 물리 충돌 활성화
    }

    void Update()
    {
        // 1. 좌클릭 감지
        if (Input.GetMouseButtonDown(0))
        {
            // 2. 마우스 레이캐스트로 오브젝트 선택 확인
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == this.gameObject)
            {
                StartParticleEffect();
            }
        }
    }

    void StartParticleEffect()
    {
        if (fillEffectPrefab == null || spoutPoint == null)
        {
            Debug.LogWarning("fillEffectPrefab 또는 spoutPoint를 인스펙터에 할당하세요!");
            return;
        }

        // 3. 파티클 생성 및 재생
        ParticleSystem ps = Instantiate(
            fillEffectPrefab,
            spoutPoint.position,
            spoutPoint.rotation
        );
        ps.Play();

        // 4. 자동 정지 및 제거
        StartCoroutine(StopAndDestroy(ps));
    }

    IEnumerator StopAndDestroy(ParticleSystem ps)
    {
        yield return new WaitForSeconds(playDuration);
        ps.Stop();

        // 파티클 시스템이 완전히 소멸될 때까지 대기
        yield return new WaitForSeconds(ps.main.startLifetime.constantMax);
        Destroy(ps.gameObject);
    }
}
