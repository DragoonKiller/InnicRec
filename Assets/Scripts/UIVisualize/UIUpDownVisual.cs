using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Utils;

public sealed class UIUpDownVisual : UIVisual, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        t = 0;
        activating = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        t = t / activeTime * idleTime;
        activating = false;
    }
}
