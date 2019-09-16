using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Utils;

public class TimelineTimeStampControl : MonoBehaviour
{
    [Tooltip("该时间线的起始时间. 单位为毫秒.")]
    public float begin;

    [Tooltip("该时间线的终止时间. 单位为毫秒.")]
    public float end;

    [Tooltip("标准时间间隔. 单位为毫秒.")]
    public List<double> standardTimeSpan;

    [Header("Debug")]

    [Tooltip("正在被该时间线组件使用的时间线时间戳.")]
    [SerializeField] List<TimeStampControl> activeTimelineStamps = new List<TimeStampControl>();

    public float timeSpan => end - begin;
    public ObjectPool timelineTimeStampPool => Env.inst.recordTimelineTimeStampPool;
    RectTransform rect => this.GetComponent<RectTransform>();
    RectTransform stampRect => timelineTimeStampPool.source.GetComponent<RectTransform>();

    void Update()
    {
        UpdateTimelineTimeStamps();

    }


    void UpdateTimelineTimeStamps()
    {
        try
        {
            if(timeSpan <= Maths.eps) throw new InvalidOperationException();
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
            var span = (float)standardTimeSpan[bsRes];
            // 确定时间戳个数.
            var cnt = (int)(timeSpan / span) + 1;
            // 同步时间戳对象.
            while(activeTimelineStamps.Count < cnt) activeTimelineStamps.Add(AcquireNew());
            while(activeTimelineStamps.Count > cnt)
            {
                Retire(activeTimelineStamps[activeTimelineStamps.Count - 1]);
                activeTimelineStamps.RemoveAt(activeTimelineStamps.Count - 1);
            }

            // 算出从左开始, 第一个间隔为 span 的标记的位置.
            float beginStampTime = (begin / span).CeilToInt() * span;

            float step = rect.rect.width / timeSpan;
            for(int i = 0; i < cnt; i++)
            {
                float curTime = i * span + beginStampTime;
                // 安排时间标记的位置.
                var targetRect = activeTimelineStamps[i].GetComponent<RectTransform>();
                targetRect.offsetMin = stampRect.offsetMin + Vector2.right * step * (curTime - begin);
                targetRect.offsetMax = stampRect.offsetMax + Vector2.right * step * (curTime - begin);
                // 安排时间标记的时间显示.
                activeTimelineStamps[i].displayTime = TimeSpan.FromMilliseconds(curTime);
            }
        }
        catch(InvalidOperationException)
        {
            foreach(var i in activeTimelineStamps) Retire(i);
            activeTimelineStamps.Clear();
        }
    }

    TimeStampControl AcquireNew()
    {
        var x = timelineTimeStampPool.Aquire().GetComponent<TimeStampControl>();
        x.transform.SetParent(this.transform);
        return x;
    }

    void Retire(TimeStampControl cc)
    {
        timelineTimeStampPool.Retire(cc.gameObject);
    }
}
