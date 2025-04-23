using UnityEngine;
using static RaycastInteractor;

public class ChangeYellow : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        MeshRenderer mr = GetComponent<MeshRenderer>();
        if (mr != null)
        {
            mr.material.color = Color.yellow;
        }
    }
}
