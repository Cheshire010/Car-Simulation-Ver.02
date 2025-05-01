using UnityEngine;
using System.Collections;

public class JYJ_BlinkOnInteract : MonoBehaviour, JYJ_RaycastInteractor.IInteractable
{
    [Header("깜빡임 설정")]
    public Color blinkColor = Color.red;
    public float blinkInterval = 0.5f;

    private Renderer objectRenderer;
    private Material materialInstance;
    private Color originalColor;
    private Coroutine blinkCoroutine;
    private bool isBlinking = false;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer != null)
        {
            // 머티리얼 인스턴스화 (원본 보존)
            materialInstance = new Material(objectRenderer.material);
            objectRenderer.material = materialInstance;
            originalColor = materialInstance.color;
        }
        else
        {
            Debug.LogWarning("Renderer 컴포넌트가 없습니다.", this);
        }
    }

    public void Interact()
    {
        if (!isBlinking)
        {
            blinkCoroutine = StartCoroutine(BlinkEffect());

            isBlinking = true;
        
        }
    }

    private IEnumerator BlinkEffect()
    {
        while (true)
        {
            materialInstance.color = blinkColor;
            yield return new WaitForSeconds(blinkInterval);
            materialInstance.color = originalColor;
            yield return new WaitForSeconds(blinkInterval);
        }
    }

    private void OnDisable()
    {
        if (materialInstance != null)
        {
            materialInstance.color = originalColor;
        }
    }

    private void OnDestroy()
    {
        if (materialInstance != null)
        {
            Destroy(materialInstance);
        }
    }
}
