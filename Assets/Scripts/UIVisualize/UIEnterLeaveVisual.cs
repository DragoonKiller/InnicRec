using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Utils;

public sealed class UIEnterLeaveVisual : UIVisual, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        t = 0;
        activating = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        t = t / activeTime * idleTime;
        activating = false;
    }
}
