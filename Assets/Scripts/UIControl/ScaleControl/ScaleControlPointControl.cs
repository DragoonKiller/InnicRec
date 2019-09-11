using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 缩放条的控制点的脚本.
/// </summary>
[RequireComponent(typeof(Image))]
public class ScaleControlPointControl : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Action OnMouseDown;
    public Action OnMouseUp;
    public void OnPointerDown(PointerEventData e) => OnMouseDown?.Invoke();
    public void OnPointerUp(PointerEventData e) => OnMouseUp?.Invoke();
}
