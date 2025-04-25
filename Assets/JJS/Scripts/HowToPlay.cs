using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HowToPlay : MonoBehaviour
{
    public GameObject targetImage; // 보여줄 이미지 오브젝트

    public void ShowImage()
    {
        if (targetImage != null)
        {
            targetImage.SetActive(true);
        }
    }
}
