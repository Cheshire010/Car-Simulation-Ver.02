using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class PortalEntranceManager : MonoBehaviour
{
    [Tooltip("감지할 박스 객체의 태그")]
    public string boxTag = "Box3";

    [Tooltip("채워질 때 사용할 파티클 Prefab")]
    public ParticleSystem fillEffectPrefab;

    [Header("딜레이 설정")]
    [Tooltip("박스 감지 후 파티클 생성 전 대기 시간(초)")]
    public float spawnDelay = 3f;
    [Tooltip("파티클 재생 지속 시간(초)")]
    public float playDuration = 10f;

    private Collider _col;

    void Awake()
    {
        // Trigger Collider로 설정
        _col = GetComponent<Collider>();
        _col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        // 지정된 태그(Box3) 충돌 시 이펙트 시퀀스 실행
        if (other.CompareTag(boxTag))
            StartCoroutine(SpawnAndPlay());
    }

    IEnumerator SpawnAndPlay()
    {
        // 1) spawnDelay만큼 대기
        yield return new WaitForSeconds(spawnDelay);

        // 2) Entrance 위치 위로 파티클 생성
        Vector3 spawnPos = transform.position + Vector3.up * 1.5f;
        var ps = Instantiate(fillEffectPrefab, spawnPos, Quaternion.identity);

        // 3) 파티클 재생
        ps.Play();

        // 4) playDuration만큼 대기
        yield return new WaitForSeconds(playDuration);

        // 5) 파티클 정지 및 제거
        ps.Stop();
        Destroy(ps.gameObject);
    }
}
