using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils;

/// <summary>
/// 控制一个单独的 key 的时间线显示.
/// </summary>
public class KeyLineControl : MonoBehaviour
{
    [Tooltip("完整的按键时间段. 单位是视野比率(0.0 到 1.0).")]
    public List<Vector2> intervals = new List<Vector2>();

    [Tooltip("不完整的(有起始没结尾, 有结尾没起始)的按键端点. 单位是视野比率(0.0 到 1.0).")]
    public List<float> points = new List<float>();

    [Tooltip("管辖所有区间的GameObject.")]
    [SerializeField] RectTransform intervalsRoot = null;

    [Tooltip("管辖所有单点的GameObject.")]
    [SerializeField] RectTransform pointsRoot = null;

    [Tooltip("在图标的上方和下方留出空隙.")]
    public float preserveHeight;

    [Tooltip("左侧背景.")]
    [SerializeField] Image panelImage = null;

    [Tooltip("中心线的图片.")]
    [SerializeField] Image lineImage = null;

    [Tooltip("左侧文本.")]
    [SerializeField] Text text = null;

    [Tooltip("接收点击和鼠标事件的背景图.")]
    [SerializeField] KeyLineCardControl cardControl = null;

    [Tooltip("鼠标悬浮在 key 面板上时, 面板视觉效果触发器.")]
    [SerializeField] UIPassiveVisual cardHoverImage = null;

    [Tooltip("鼠标悬浮在 key 面板上时, 时间线视觉效果触发器.")]
    [SerializeField] UIPassiveVisual lineHoverImage = null;

    [Header("Debug")]

    [Tooltip("所有正在显示的 key 时间段面板.")]
    [SerializeField] List<TimeSpanControl> intervalControls = new List<TimeSpanControl>();

    [Tooltip("所有正在显示的 key 时间点面板.")]
    [SerializeField] List<TimePointControl> pointControls = new List<TimePointControl>();

    [Tooltip("光标是否悬浮在该条目上.")]
    public bool selected;


    // 这个 KeyLine 左侧背景图标, 以及中心线的颜色.
    Color panelColor => ColorManager.inst.GetPanelColor(keyName, keyCategory);

    // 鼠标悬浮在 Key 面板上面时区间的颜色. 会直接按照 alpha 值叠在 panelColor 上.
    Color panelHoverColor => ColorManager.inst.GetPanelColor(keyName, keyCategory, true);

    Color lineColor => ColorManager.inst.GetLineColor(keyName, keyCategory);

    // 鼠标悬浮在 Key 面板上面时线条的颜色. 会直接按照 alpha 值叠在 panelColor 上.
    Color lineHoverColor => ColorManager.inst.GetLineColor(keyName, keyCategory, true);

    // 这个 KeyLine 的所有区间的颜色.
    Color intervalColor => ColorManager.inst.GetIntervalColor(keyName, keyCategory);

    // 这个 KeyLine 的所有区间的颜色.
    Color intervalHoverColor => ColorManager.inst.GetIntervalColor(keyName, keyCategory);

    // 这个 KeyLine 的所有单点的颜色.
    Color pointColor => ColorManager.inst.GetPointColor(keyName, keyCategory, true);

    // 这个 KeyLine 的所有单点的颜色.
    Color pointHoverColor => ColorManager.inst.GetPointColor(keyName, keyCategory, true);


    public string keyName
    {
        get => text.text;
        set => text.text = value;
    }

    public string keyCategory = "Unknown";

    ObjectPool intervalPool => Env.inst?.intervalPool;
    ObjectPool pointPool => Env.inst?.pointPool;
    RectTransform rect => this.GetComponent<RectTransform>();

    public void Clear()
    {
        foreach(var i in intervalControls) intervalPool.Retire(i.gameObject);
        foreach(var i in pointControls) pointPool.Retire(i.gameObject);
    }

    void Start()
    {
        cardControl.OnEnter = OnCardPanelEnter;
        cardControl.OnLeave = OnCardPanelExit;
    }

    void Update()
    {
        UpdateIntervals();
        UpdateSingles();
        UpdateColors();
    }

    void OnDestroy()
    {
        if(cardControl != null)
        {
            cardControl.OnEnter -= OnCardPanelEnter;
            cardControl.OnLeave -= OnCardPanelExit;
        }
    }

    void OnCardPanelEnter()
    {
        selected = true;
        cardHoverImage.activate = true;
        lineHoverImage.activate = true;
    }
    void OnCardPanelExit()
    {
        cardHoverImage.activate = false;
        lineHoverImage.activate = false;
        selected = false;
    }

    void UpdateColors()
    {
        panelImage.color = panelColor;
        cardHoverImage.active.color = panelHoverColor;
        lineImage.color = lineColor;
        lineHoverImage.active.color = lineHoverColor;
        foreach(var i in intervalControls) i.image.color = selected ? intervalHoverColor : intervalColor;
        foreach(var i in pointControls) i.image.color = selected ? pointHoverColor : pointColor;
    }

    void UpdateIntervals()
    {
        // 同步池子对象.
        intervalPool.Sync(intervalControls, intervals.Count);

        // 设置区间位置.
        for(int i = 0; i < intervals.Count; i++)
        {
            var (l, r) = intervals[i];
            var c = intervalControls[i];
            var t = c.GetComponent<RectTransform>();
            t.SetParent(intervalsRoot);
            t.offsetMin = new Vector2(rect.rect.width * l, preserveHeight);
            t.offsetMax = new Vector2(rect.rect.width * r, -preserveHeight);
        }
    }

    void UpdateSingles()
    {
        // 同步池子对象.
        pointPool.Sync(pointControls, points.Count);

        // 设置位置.
        for(int i = 0; i < points.Count; i++)
        {
            var t = pointControls[i].GetComponent<RectTransform>();
            var mix = pointPool.source.GetComponent<RectTransform>().rect.xMin;
            var mxx = pointPool.source.GetComponent<RectTransform>().rect.xMax;
            t.SetParent(pointsRoot);
            t.offsetMin = new Vector2(rect.rect.width * points[i] - mix, preserveHeight);
            t.offsetMax = new Vector2(rect.rect.width * points[i] + mxx, -preserveHeight);
        }
    }

}
