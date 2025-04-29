using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator), typeof(Collider))]
public class EngineOilCapInteraction : MonoBehaviour
{
    [Header("움직일 오일 객체")]
    public Transform engineOil;

    [Header("뚜껑 기준 이동 오프셋 (열렸을 때)")]
    public Vector3 moveOffset = new Vector3(0f, 0f, -0.2f);
    public float moveDuration = 0.8f;

    Animator _anim;
    Collider _col;

    Vector3 _origPos;
    Quaternion _origRot;
    bool isOpen = false;

    void Awake()
    {
        _anim = GetComponent<Animator>();
        _col = GetComponent<Collider>();
        _col.isTrigger = true;

        // 엔진오일의 원래 월드 위치/회전 저장
        _origPos = engineOil.position;
        _origRot = engineOil.rotation;
    }

    void OnMouseDown()
    {
        if (isOpen)
        {
            // 1) 캡 닫기 애니메이션 트리거
            _anim.SetTrigger("Close_Cap");    // Animator에 등록된 이름으로 바꿔주세요
            // 2) 오일을 원위치로 되돌린다
            StartCoroutine(MoveOilCoroutine(false));
        }
        else
        {
            // 1) 캡 열기 애니메이션 트리거
            _anim.SetTrigger("Open_Cap");     // Animator에 등록된 이름으로 바꿔주세요
            // 2) 오일을 옆으로 이동
            StartCoroutine(MoveOilCoroutine(true));
        }

        isOpen = !isOpen;
    }

    IEnumerator MoveOilCoroutine(bool opening)
    {
        // (1) 뚜껑 애니메이션 타이밍 대기
        yield return new WaitForSeconds(0.5f);

        // (2) 시작/종료 위치·회전 결정
        Vector3 startPos = engineOil.position;
        Quaternion startRot = engineOil.rotation;

        Vector3 endPos;
        Quaternion endRot;
        if (opening)
        {
            // 뚜껑(transform) 기준으로 moveOffset 만큼 월드 좌표 계산
            endPos = transform.TransformPoint(moveOffset);
            endRot = transform.rotation;
        }
        else
        {
            // 원래 자리로 복귀
            endPos = _origPos;
            endRot = _origRot;
        }

        // (3) 부드럽게 보간
        float t = 0f;
        while (t < moveDuration)
        {
            t += Time.deltaTime;
            float f = Mathf.SmoothStep(0f, 1f, t / moveDuration);
            engineOil.position = Vector3.Lerp(startPos, endPos, f);
            engineOil.rotation = Quaternion.Slerp(startRot, endRot, f);
            yield return null;
        }

        // (4) 최종 고정
        engineOil.position = endPos;
        engineOil.rotation = endRot;
    }
}
