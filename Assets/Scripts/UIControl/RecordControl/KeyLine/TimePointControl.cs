using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 记录绘制一个时间点.
/// </summary>
public class TimePointControl : PooledBehaviour
{
    public Image image => this.GetComponent<Image>();

    public override void SetAquire()
    {

    }

    public override void SetRetire()
    {
        this.transform.position = Vector2.one * 99999;
    }
}
