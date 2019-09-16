using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeStampControl : PooledBehaviour
{
    /// <summary>
    /// 显示类型.
    /// </summary>
    public enum DisplayStyle
    {
        HourMinuteSecond,
        MinuteSecondMillisecond
    }

    [Tooltip("显示时间的格式.")]
    public DisplayStyle style;

    [Tooltip("显示的时间.")]
    public TimeSpan displayTime;

    [Tooltip("用于显示时间的文本框.")]
    public Text text;

    void Update()
    {
        if(style == DisplayStyle.MinuteSecondMillisecond)
        {
            text.text = $"{displayTime.Minutes.ToString("00")}:{displayTime.Seconds.ToString("00")}.{(displayTime.Milliseconds / 100).ToString("00")}";
        }
        else if(style == DisplayStyle.HourMinuteSecond)
        {
            text.text = $"{displayTime.Hours.ToString("00")}:{displayTime.Minutes.ToString("00")}:{displayTime.Seconds.ToString("00")}";
        }
        else
        {
            text.text = "";
        }
    }

    public override void SetAquire()
    {
        // 什么都不做.
        // 具体的父子关系绑定由 ViewportTimelineControl 完成.
    }

    public override void SetRetire()
    {
        this.transform.position = Vector2.one * 99999;
    }
}
