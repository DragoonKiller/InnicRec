using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class PooledObjectTest : PooledBehaviour
{
    public Image image => this.GetComponent<Image>();
    
    public override void SetAquire()
    {
        image.color = new Color(1, 1, 1, 1);
    }

    public override void SetRetire()
    {
        image.color = new Color(0, 0, 0, image.color.a);
        this.transform.position = Vector2.one * 1e6f;
    }
}
