using System;
using System.Windows.Forms;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using UnityEngine.EventSystems;

public class RecordPanelControl : MonoBehaviour
{
    [Tooltip("视野卷动条.")]
    public ScaleControl viewport;

    [Tooltip("这个面板的高度应该是多少.")]
    public float targetHeight;

    [Tooltip("这个面板的顶部应该处于什么位置.")]
    public float targetTop;

    [Tooltip("是否被选择.")]
    public bool selected;

    [Tooltip("存放所有 KeyLine 的对象.")]
    public Transform keylinesRoot;

    [Tooltip("指示该面板是否被选择的图片框.")]
    [SerializeField] UIPassiveVisual selectionVisual = null;

    [Tooltip("用于创建 keyline 对象的模板.")]
    [SerializeField] KeyLineControl source = null;

    [Tooltip("视野条的时间标尺.")]
    [SerializeField] ViewportTimeStampControl viewportTimeStamps = null;

    [Tooltip("时间线的时间标尺.")]
    [SerializeField] TimelineTimeStampControl timelineTimeStamps = null;

    [Tooltip("关闭页面的按钮.")]
    [SerializeField] RecordCancelButtonControl cancelButton = null;

    [Tooltip("开始/结束录制的按钮.")]
    [SerializeField] RecordRecordButtonControl recordButton = null;

    [Tooltip("保存记录的按钮.")]
    [SerializeField] RecordSaveButtonControl saveButton = null;

    [Tooltip("锁定视野条长度的按钮.")]
    [SerializeField] RecordStickButtonControl stickButton = null;

    [Tooltip("是否正在记录的图标.")]
    [SerializeField] UIPassiveVisual recordStateImage = null;

    [Tooltip("是否锁定视野条长度的图标.")]
    [SerializeField] UIPassiveVisual stickStateImage = null;

    [Tooltip("是否应该删除.")]
    public bool shouldRemove;

    [Tooltip("是否需要实时更新视野条.")]
    public bool updateRecordView;

    [Header("Debug")]

    [Tooltip("所有 key 的显示面板.")]
    public List<KeyLineControl> keyControls = new List<KeyLineControl>();

    [Tooltip("这个面板对应的记录.")]
    public Record record = new Record();

    [Tooltip("各个记录的按键次数.")]
    public List<int> timeCounts = new List<int>();

    public RectTransform rect => this.GetComponent<RectTransform>();

    TimeSpan begin => new TimeSpan((long)(record.totalTime.Ticks * (double)viewport.beginPoint));

    TimeSpan end => new TimeSpan((long)(record.totalTime.Ticks * (double)viewport.endPoint));

    /// <summary>
    /// 锁定视野的时间长度.
    /// </summary>
    TimeSpan timeLen = TimeSpan.Zero;

    void Start()
    {
        cancelButton.OnClick = OnCancel;
        saveButton.OnClick = OnSave;
        recordButton.OnClick = OnRecord;
        stickButton.OnClick = OnStick;
    }

    void Update()
    {
        UpdateActivated();
        UpdateSelf();
        UpdateKeyLinesObject();
        UpdateLinesPosition();
        UpdateKeyLinesInfo();
        UpdateRecordingViewport();
        UpdateViewportTimeline();
        UpdateStateView();
        UpdateTimeline();
        UpdateTimeCounts();
    }

    void OnDestory()
    {
        if(cancelButton != null) cancelButton.OnClick -= OnCancel;
        if(saveButton != null) saveButton.OnClick -= OnSave;
        if(recordButton != null) recordButton.OnClick -= OnRecord;
        if(stickButton != null) stickButton.OnClick -= OnStick;
    }

    void OnRecord()
    {
        if(Env.inst.inputRecorder.record == record)
        {
            Env.inst.inputRecorder.record = null;
            timeLen = TimeSpan.Zero;
        }
        else
        {
            Env.inst.inputRecorder.record = record;
            timeLen = end - begin;
        }
    }

    void OnSave()
    {
        string fileName = null;
        using(var f = new SaveFileDialog())
        {
            f.ShowDialog();
            fileName = f.FileName;
        }
        if(string.IsNullOrEmpty(fileName)) return;
        record.SaveToFile(fileName);
    }

    void OnStick()
    {
        updateRecordView = !updateRecordView;
        timeLen = end - begin;
    }

    void OnCancel() => shouldRemove = true;

    void UpdateActivated()
    {
        if(selectionVisual.activate != selected) selectionVisual.activate = selected;
    }

    void UpdateSelf()
    {
        rect.localPosition = rect.localPosition.Y(targetTop);
        rect.sizeDelta = rect.sizeDelta.Y(targetHeight);
    }

    void UpdateKeyLinesObject()
    {
        // 同步对象.
        // 不采用对象池子.
        while(keyControls.Count < record.events.Count)
        {
            var x = Instantiate(source.gameObject, keylinesRoot);
            var c = x.GetComponent<KeyLineControl>();
            keyControls.Add(c);
        }
        while(keyControls.Count > record.events.Count)
        {
            var c = keyControls[keyControls.Count - 1];
            c.Clear();
            DestroyImmediate(c.gameObject);
            keyControls.RemoveAt(keyControls.Count - 1);
        }
    }

    void UpdateLinesPosition()
    {
        int k = 0;
        for(int i = 0; i < keyControls.Count; i++)
        {
            if(keyControls[i].selected) keyControls[i].transform.SetSiblingIndex(keyControls.Count - 1);
            else keyControls[i].transform.SetSiblingIndex(k++);
        }
    }

    void UpdateKeyLinesInfo()
    {
        // 设置绘制信息.

        // 如果什么都没有记录..
        // keyControls.Count == 0 等价于 events.Count == 0.
        if(record.events.Count == 0 || record.totalTime == TimeSpan.Zero) return;

        // 这个函数根据输入的时间算出时间条上的位置.
        // 超出范围的 clamp 处理.
        float GetPos(TimeSpan x)
        {
            var fullSeq = (end - begin).TotalMilliseconds;
            var leftSeq = (x - begin).TotalMilliseconds;
            var rate = leftSeq / fullSeq;
            return ((float)rate).Clamp(0, 1);
        }

        // 枚举每个 key, 二分查找并扫描属于该区间的所有事件.
        int kc = 0;
        var used = new bool[record.events.Count];

        foreach(var (keyName, lst) in record.events.Select(x => (x.Key, x.Value)))
        {
            var c = keyControls[kc++];
            c.points.Clear();
            c.intervals.Clear();

            // 设置面板属性.
            c.keyName = keyName;
            c.keyCategory = record.NameToCategory(keyName);
            c.gameObject.name = c.GetInstanceID() + " KeyLine " + keyName;
            var ct = c.GetComponent<RectTransform>();
            var size = ct.rect.size;
            var panelRect = keylinesRoot.GetComponent<RectTransform>();
            var step = record.events.Count <= 1 ? size.y : (panelRect.rect.height - size.y) / (record.events.Count - 1);
            step = step.Min(size.y);
            ct.offsetMax = new Vector2(panelRect.anchorMax.x, -(kc - 1) * step);
            ct.offsetMin = new Vector2(panelRect.anchorMin.x, -(kc - 1) * step - size.y);

            // BinarySearch 的返回值: 最后一个小于等于它的元素的后面那个位置.
            // 左端界要取第一个大于等于它的元素.
            int l = lst.BinarySearch(new Record.RecordEvent() { time = begin, key = "" });
            if(l < 0) l = ~l;
            while(l != 0 && begin <= lst[l - 1].time) l--;
            int r = lst.BinarySearch(new Record.RecordEvent() { time = end, key = "" });
            if(r < 0) r = ~r;

            // 取合法区间的左右部分.
            // 这么做是为了保证包含了 (begin, end) 的区间也会被正确绘制.
            if(l > 0) l--;
            if(r < lst.Count - 1) r++;

            // 扫描并设置绘画区间.
            for(int i = l; i < r; i++)
            {
                var pt = lst[i];

                // 和本区间没有交集, 跳过.
                if(pt.isPairedRight && pt.time < begin) continue;
                if(pt.isPairedLeft && pt.time > end) continue;

                // 该端点是左端点.
                if(pt.isPairedLeft)
                {
                    // 全段可见.
                    if(pt.pairing.time <= end) c.intervals.Add(new Vector2(GetPos(pt.time), GetPos(pt.pairing.time)));
                    // 部分可见.
                    else c.intervals.Add(new Vector2(GetPos(pt.time), 1));
                }
                // 该端点是右端点.
                else if(pt.isPairedRight)
                {
                    // 全段可见.
                    if(begin <= pt.pairing.time)
                    {
                        // 什么都不做.
                        // 全段可见的区间, 遍历到左端点时应当已经添加完毕.
                    }
                    // 部分可见.
                    else c.intervals.Add(new Vector2(0, GetPos(pt.time)));
                }
                // 该端点没有匹配任何左右端点, 是孤立点.
                else if(pt.pairing == null)
                {
                    c.points.Add(GetPos(pt.time));
                }
                else
                {
                    Debug.LogError($"bad point {pt.GetHashCode()} {pt.type} {pt.key} {pt.time} {pt.pairing}");
                }
            }
        }

    }

    // 在记录时间是更新视野条, 保持其时间长度不变.
    void UpdateRecordingViewport()
    {
        if(Env.inst.inputRecorder.record != record) return;
        if(!updateRecordView) return;
        // 以当前的右端点为界, 缩放视野条.
        var span = (float)(timeLen.TotalSeconds / record.totalTime.TotalSeconds);
        viewport.beginPoint = viewport.endPoint - span;
    }

    // 设置视野条上方的时间标尺.
    void UpdateViewportTimeline()
    {
        if(record.totalTime == TimeSpan.Zero) return;
        // 根据记录的起止时间, 设置时间标尺.
        viewportTimeStamps.timeSpan = (float)record.totalTime.TotalMilliseconds;
        timelineTimeStamps.begin = (float)record.totalTime.TotalMilliseconds * viewport.beginPoint;
        timelineTimeStamps.end = (float)record.totalTime.TotalMilliseconds * viewport.endPoint;
    }

    // 更新按钮图标的状态.
    void UpdateStateView()
    {
        recordStateImage.activate = Env.inst.inputRecorder.record == record;
        stickStateImage.activate = updateRecordView;
    }

    // 设置 key 时间轴上的时间标尺.
    void UpdateTimeline()
    {
        // TODO.
    }

    void UpdateTimeCounts()
    {
        while(timeCounts.Count < record.events.Count) timeCounts.Add(0);
        while(timeCounts.Count > record.events.Count) timeCounts.RemoveAt(timeCounts.Count - 1);
        int i = 0;
        foreach(var lst in record.events.Values) timeCounts[i++] = lst.Count;
    }
}
