using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class KeyLineCardControl : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Action OnEnter;
    public Action OnLeave;

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnEnter?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnLeave?.Invoke();
    }
}
