using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BonnetInteraction : MonoBehaviour
{
    Animator animator;
    bool isOpen = false;  // IdleClosed 상태로 시작

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, 100f))
            {
                if (hit.collider.transform.IsChildOf(transform))
                {
                    if (isOpen)
                        animator.SetTrigger("Close_Bonnet");
                    else
                        animator.SetTrigger("Open_Bonnet");

                    isOpen = !isOpen;
                    Debug.Log(isOpen ? "Opened bonnet" : "Closed bonnet");
                }
            }
        }
    }
}
