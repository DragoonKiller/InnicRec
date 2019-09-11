using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RecordStickButtonControl : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] UIPassiveVisual activeImage = null;

    public Action OnClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick?.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        activeImage.activate = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        activeImage.activate = false;
    }
}
