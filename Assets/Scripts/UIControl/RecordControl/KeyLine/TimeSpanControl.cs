using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 记录一个单独的时间区间.
/// </summary>
public class TimeSpanControl : PooledBehaviour
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
