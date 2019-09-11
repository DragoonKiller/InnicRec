using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class ScalbleFrameMaterialMaintaince : MonoBehaviour
{
    RectTransform rect => this.GetComponent<RectTransform>();
    Image image => this.GetComponent<Image>();

    void Start()
    {
        
    }

    void Update()
    {
        image.material.SetTexture("_MainTex", image.mainTexture);
        image.material.SetVector("_TexSize", image.sprite.rect.size);
        image.material.SetVector("_RectSize", rect.rect.size);
    }

}
