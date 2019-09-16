using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Utils;

public class ViewportTimeStampControl : MonoBehaviour
{
    [Tooltip("该时间线所表示的时间长度. 单位为毫秒.")]
    public float timeSpan; 

    [Tooltip("标准时间间隔. 单位为毫秒.")]
    public List<double> standardTimeSpan;

    [Header("Debug")]

    [Tooltip("正在被该时间线组件使用的视野条时间戳.")]
    [SerializeField] List<TimeStampControl> activeViewportStamps = new List<TimeStampControl>();

    public ObjectPool viewportTimeStampPool => Env.inst.recordViewportTimeStampPool;
    RectTransform rect => this.GetComponent<RectTransform>();
    RectTransform stampRect => viewportTimeStampPool.source.GetComponent<RectTransform>();

    void Update()
    {
        UpdateViewTimeStamps();
        
    }

    void UpdateViewTimeStamps()
    {
        try
        {
            // 算出这个 timeline 最多能容纳多少个时间戳.
            int n = (int)(rect.rect.width / stampRect.rect.width);
            // 只能放一个时间戳, 甚至一个都放不下, 此时时间戳是没有意义的.
            if(n <= 1) throw new InvalidOperationException();
            // 算出每个时间戳的最小间隔. 单位 ms.
            float maxTimeBetweenStamps = timeSpan / (n - 1);
            // 选取比这个间隔更大的一个标准间隔.
            var bsRes = standardTimeSpan.BinarySearch(maxTimeBetweenStamps);
            if(bsRes < 0) bsRes = ~bsRes;
            // 二分查找找到了更小的一个. 要选更大的那个.
            if(bsRes < standardTimeSpan.Count && standardTimeSpan[bsRes] < maxTimeBetweenStamps) bsRes += 1;
            // 没有标准时间戳可以放置. 放弃.
            if(bsRes >= standardTimeSpan.Count) throw new InvalidOperationException();
            // 确定时间间隔. 毫秒单位.
            var span = standardTimeSpan[bsRes];
            // 确定时间戳个数.
            var cnt = (int)(timeSpan / span);
            // 同步时间戳对象.
            while(activeViewportStamps.Count < cnt) activeViewportStamps.Add(AcquireNew());
            while(activeViewportStamps.Count > cnt)
            {
                Retire(activeViewportStamps[activeViewportStamps.Count - 1]);
                activeViewportStamps.RemoveAt(activeViewportStamps.Count - 1);
            }
            // 安排每个时间戳的位置.
            float step = (float)(span / timeSpan * rect.rect.width);
            for(int i = 0; i < cnt; i++)
            {
                var targetRect = activeViewportStamps[i].GetComponent<RectTransform>();
                // 最左边肯定是 00:00:00, 不需要打时间戳.
                targetRect.offsetMin = stampRect.offsetMin + Vector2.right * step * (i + 1) - stampRect.rect.size * 0.5f;
                targetRect.offsetMax = stampRect.offsetMax + Vector2.right * step * (i + 1) - stampRect.rect.size * 0.5f;
            }
            // 安排每个时间戳的时间标记.
            for(int i = 0; i < cnt; i++)
            {
                activeViewportStamps[i].style = TimeSpan.FromMilliseconds(span).TotalHours < 1.0 
                    ? TimeStampControl.DisplayStyle.MinuteSecondMillisecond
                    : TimeStampControl.DisplayStyle.HourMinuteSecond;
                activeViewportStamps[i].displayTime = TimeSpan.FromMilliseconds((i + 1) * span);
            }
        }
        catch(InvalidOperationException)
        {
            foreach(var i in activeViewportStamps) Retire(i);
            activeViewportStamps.Clear();
        }
    }

    TimeStampControl AcquireNew()
    {
        var x = viewportTimeStampPool.Aquire().GetComponent<TimeStampControl>();
        x.transform.SetParent(this.transform);
        return x;
    }

    void Retire(TimeStampControl cc)
    {
        viewportTimeStampPool.Retire(cc.gameObject);
    }
}
