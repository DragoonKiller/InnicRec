using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using Utils;

// 控制缩放的滚动条.
[RequireComponent(typeof(Image))]
public class ScaleControl : MonoBehaviour
{
    public enum Layout
    {
        Horizontal,
        Vertical,
    }

    [Tooltip("起始位置控制点.")]
    public ScaleControlPointControl beginControl;

    [Tooltip("终止位置控制点.")]
    public ScaleControlPointControl endControl;

    [Tooltip("选择区间的控制点.")]
    public ScaleControlPointControl selectedControl;

    [Tooltip("起始位置.")]
    [Range(0, 1)] [SerializeField] float begin;

    [Tooltip("终止位置.")]
    [Range(0, 1)] [SerializeField] float end;

    [Tooltip("轴上两个端点的最小比例.")]
    public float minimalDistance;

    [Tooltip("竖直还是水平.")]
    public Layout layout;

    [Header("Debug")]

    // 记录鼠标按下的位置, 用于判定鼠标拖动.
    [SerializeField] float mouseAnchor;
    // 记下控制端点的位置, 用于响应鼠标拖动.
    [SerializeField] float anchor;
    // 记下我正在控制哪个端点.
    [SerializeField] RectTransform controlTrans;

    RectTransform rect => this.GetComponent<RectTransform>();

    // 带有值域限制的范围.
    public float beginPoint
    {
        get => begin;
        set => begin = value.Clamp(0, end - minimalDistance);
    }
    public float endPoint
    {
        get => end;
        set => end = value.Clamp(begin + minimalDistance, 1);
    }


    Image selectedImage => selectedControl.GetComponent<Image>();
    Image beginImage => beginControl.GetComponent<Image>();
    Image endImage => endControl.GetComponent<Image>();

    RectTransform selectedTrans => selectedControl.GetComponent<RectTransform>();
    RectTransform beginTrans => beginControl.GetComponent<RectTransform>();
    RectTransform endTrans => endControl.GetComponent<RectTransform>();

    // 整个轴的起始位置, 局部坐标系, 像素单位.
    Vector2 beginBoundaryLocal => layout == Layout.Horizontal
        ? new Vector2(rect.rect.xMin, (rect.rect.yMin + rect.rect.yMax) * 0.5f)
        : new Vector2((rect.rect.xMin + rect.rect.xMax) * 0.5f, rect.rect.yMax);
    Vector2 endBoundaryLocal => layout == Layout.Horizontal
        ? new Vector2(rect.rect.xMax, (rect.rect.yMin + rect.rect.yMax) * 0.5f)
        : new Vector2((rect.rect.xMin + rect.rect.xMax) * 0.5f, rect.rect.yMin);

    // 整个轴的起始位置, UI 坐标系, 像素单位.
    Vector2 beginBoundary => rect.TransformPoint(beginBoundaryLocal);
    Vector2 endBoundary => rect.TransformPoint(endBoundaryLocal);

    // 鼠标指向的位置映射到轴上的数值.
    float mousePointing => beginBoundary.To(endBoundary).Dot(beginBoundary.To(Input.mousePosition)) / beginBoundary.To(endBoundary).magnitude.Sqr();

    void Start()
    {
        beginControl.OnMouseDown += OnBeginMouseDown;
        beginControl.OnMouseUp += OnBeginMouseUp;
        endControl.OnMouseDown += OnEndMouseDown;
        endControl.OnMouseUp += OnEndMouseUp;
        selectedControl.OnMouseDown += OnSelectedMouseDown;
        selectedControl.OnMouseUp += OnSelectedMouseUp;
    }

    void OnDestroy()
    {
        if(beginControl) beginControl.OnMouseDown -= OnBeginMouseDown;
        if(beginControl) beginControl.OnMouseUp -= OnBeginMouseUp;
        if(endControl) endControl.OnMouseDown -= OnEndMouseDown;
        if(endControl) endControl.OnMouseUp -= OnEndMouseUp;
        if(selectedControl) selectedControl.OnMouseDown -= OnSelectedMouseDown;
        if(selectedControl) selectedControl.OnMouseUp -= OnSelectedMouseUp;
    }

    void Update()
    {
        ProcessMouseAction();
        UpdateControlPointPosition();
    }

    void ProcessMouseAction()
    {
        var delta = mousePointing - mouseAnchor;
        var cur = delta + anchor;
        if(controlTrans == beginTrans) beginPoint = cur;
        else if(controlTrans == endTrans) endPoint = cur;
        else if(controlTrans == selectedTrans)
        {
            var len = endPoint - beginPoint;
            cur = cur.Clamp(0, 1 - len);
            beginPoint = cur;
            endPoint = cur + len;
        }
    }

    void UpdateControlPointPosition()
    {
        SetControlPosition(beginTrans, begin);
        SetControlPosition(endTrans, end);
        if(layout == Layout.Horizontal)
        {
            selectedTrans.localPosition = selectedTrans.localPosition.X(beginTrans.anchoredPosition.x);
            selectedTrans.sizeDelta = selectedTrans.sizeDelta.X(endTrans.anchoredPosition.x - beginTrans.anchoredPosition.x);
        }
        else
        {
            selectedTrans.localPosition = selectedTrans.localPosition.Y(endTrans.anchoredPosition.y);
            selectedTrans.sizeDelta = selectedTrans.sizeDelta.Y(beginTrans.anchoredPosition.y - endTrans.anchoredPosition.y);
        }
    }

    void OnBeginMouseDown()
    {
        mouseAnchor = mousePointing;
        anchor = begin;
        controlTrans = beginTrans;
    }

    void OnBeginMouseUp()
    {
        controlTrans = null;
    }

    void OnEndMouseDown()
    {
        mouseAnchor = mousePointing;
        anchor = end;
        controlTrans = endTrans;
    }

    void OnEndMouseUp()
    {
        controlTrans = null;
    }

    void OnSelectedMouseDown()
    {
        mouseAnchor = mousePointing;
        anchor = begin;
        controlTrans = selectedTrans;
    }

    void OnSelectedMouseUp()
    {
        controlTrans = null;
    }

    void SetControlPosition(RectTransform trans, float pos)
    {
        if(layout == Layout.Horizontal)
        {
            trans.anchoredPosition = trans.anchoredPosition.X(pos * beginBoundary.To(endBoundary).magnitude);
        }
        else
        {
            trans.anchoredPosition = trans.anchoredPosition.Y(-pos * beginBoundary.To(endBoundary).magnitude);
        }
    }

}
