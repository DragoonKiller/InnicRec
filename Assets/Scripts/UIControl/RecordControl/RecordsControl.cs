﻿using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Utils;

// 控制所有记录的显示.
public class RecordsControl : MonoBehaviour
{
    [Tooltip("记录面板的模板.")]
    public RecordPanelControl source;

    [Tooltip("用于控制缩放的卷动条.")]
    public ScaleControl viewport;

    [Tooltip("滚轮每次移动多少像素.")]
    public float moveSpeed;

    [Tooltip("滚轮每次移动的放缩比.")]
    public float scaleSpeed;

    [Tooltip("被该脚本管辖的记录面板.")]
    public List<RecordPanelControl> records;

    [Tooltip("创建新的记录时会复制这个对象.")]
    public RecordPanelControl recordSource;

    [Tooltip("新建的记录对象应当被放置到这个GameObject下.")]
    public Transform recordsRoot;

    public IEnumerable<RecordPanelControl> selected => records.Where(x => x.selected);

    RectTransform rect => this.GetComponent<RectTransform>();

    float panelHeight => records.Count == 0
        ? rect.rect.height
        : rect.rect.height / (viewport.endPoint - viewport.beginPoint) / records.Count;

    void Update()
    {
        StandarizeViewport();
        UpdateViewport();
        UpdateRecordPanelHeight();
        UpdateRecordPanelPosition();
        UpdateRemoval();
    }

    void StandarizeViewport()
    {
        // 如果有小于等于一条记录, 把整个 viewport 设置到全局.
        if(records == null || records.Count == 0)
        {
            viewport.beginPoint = 0;
            viewport.endPoint = 1;
        }
    }

    void UpdateViewport()
    {
        // 向前: 1, 向后: -1.
        var dz = Input.mouseScrollDelta.y;
        if(dz == 0) return;

        // 当前未选择记录. 处理全部记录的视野.
        if(selected == null || selected.Count() == 0)
        {
            // 常规滚动: 移动视野条.
            if(!Input.anyKey) ChangeViewportPos(viewport, dz);
            // 按住左 control: 放大或缩小.
            else if(Input.GetKey(KeyCode.LeftControl)) ChangeViewportScale(viewport, dz);
        }
        // 当前选择了一些记录, 处理这些记录的视野.
        else
        {
            // 常规滚动: 移动视野条.
            if(!Input.anyKey) foreach(var control in selected) ChangeViewportPos(control.viewport, dz);
            // 按住左 control: 放大或缩小.
            else foreach(var control in selected) ChangeViewportScale(control.viewport, dz);
        }
    }


    void ChangeViewportPos(ScaleControl control, float dz)
    {
        var delta = -dz * moveSpeed / rect.rect.height * (control.endPoint - control.beginPoint);
        if(control.beginPoint + delta < 0) delta = delta.ClampAbs(control.beginPoint);
        if(control.endPoint + delta > 1) delta = delta.ClampAbs(1 - control.endPoint);
        control.beginPoint += delta;
        control.endPoint += delta;
    }

    void ChangeViewportScale(ScaleControl control, float dz)
    {
        // 以鼠标的竖直位置为缩放中心.
        var centerScreen = (rect.worldToLocalMatrix * Input.mousePosition).y;
        // 换坐标系, 从上往下.
        var centerScreenLocal = rect.rect.height - centerScreen;
        // 算出比例.
        var rate = centerScreenLocal / rect.rect.height;
        // 算出轴上对应点.
        var center = rate * (control.endPoint - control.beginPoint) + control.beginPoint;

        float nextBegin = 0, nextEnd = 0;
        if(dz < 0)
        {
            nextBegin = (control.beginPoint - center) * scaleSpeed + center;
            nextEnd = (control.endPoint - center) * scaleSpeed + center;
        }
        else if(dz > 0)
        {
            nextBegin = (control.beginPoint - center) / scaleSpeed + center;
            nextEnd = (control.endPoint - center) / scaleSpeed + center;
        }

        control.beginPoint = nextBegin;
        control.endPoint = nextEnd;
    }

    void UpdateRecordPanelHeight()
    {
        foreach(var rec in records) rec.targetHeight = panelHeight;
    }

    void UpdateRecordPanelPosition()
    {
        var beginPixel = rect.rect.height / (viewport.endPoint - viewport.beginPoint) * viewport.beginPoint;
        for(int i = 0; i < records.Count; i++)
        {
            float top = i * panelHeight;
            records[i].targetTop = -top + beginPixel;
        }
    }

    void UpdateRemoval()
    {
        for(int i = 0; i < records.Count; i++)
        {
            if(!records[i].shouldRemove) continue;
            DestroyImmediate(records[i].gameObject);
        }
        records.RemoveAll(x => x == null);
    }

    public void OpenFile(string path)
    {
        var x = CreateNew();
        x.record = Record.Deserialize(System.IO.File.ReadAllText(path));
    }

    public void SaveRecord(RecordPanelControl control)
    {

    }

    public RecordPanelControl CreateNew()
    {
        var x = Instantiate(recordSource, recordsRoot);
        records.Add(x);
        return x;
    }
}