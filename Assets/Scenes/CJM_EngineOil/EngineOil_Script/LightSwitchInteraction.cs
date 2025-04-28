using UnityEngine;

[RequireComponent(typeof(Collider))]
public class LightSwitchInteraction : MonoBehaviour
{
    [Tooltip("이 스위치가 제어할 라이트들")]
    public Light[] targetLights;

    void Awake()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = false;  // OnMouseDown 인식을 위해
    }

    void OnMouseDown()
    {
        if (targetLights == null || targetLights.Length == 0)
        {
            Debug.LogWarning("LightSwitchInteraction: 제어할 라이트가 할당되지 않았습니다.", this);
            return;
        }

        foreach (var lt in targetLights)
        {
            if (lt != null)
                lt.enabled = !lt.enabled;
        }

        Debug.Log($"Toggled {targetLights.Length} lights. Now {(targetLights[0]?.enabled == true ? "ON" : "OFF")}.");
    }
}
