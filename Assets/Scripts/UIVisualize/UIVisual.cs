using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils;


public class UIVisual : MonoBehaviour
{
    [Tooltip("鼠标移动到 UI 组件上方时, 显示的图片.")]
    public Image active;

    [Tooltip("显示过程的 alpha 值曲线.")]
    public AnimationCurve activeCurve;

    [Tooltip("消退过程的 alpha 值曲线.")]
    public AnimationCurve idleCurve;

    [Tooltip("从 idle 到 active 的耗时.")]
    public float activeTime;

    [Tooltip("从 active 到 idle 的耗时.")]
    public float idleTime;

    [Header("Debug")]
    [SerializeField] protected bool activating;
    [SerializeField] protected float t;

    public void Activate() => activating = true;
    public void Deactivate() => activating = false;

    protected virtual void Update()
    {
        if(activating)
        {
            t += Time.deltaTime;
            active.color = active.color.A(activeCurve.Evaluate(t));
        }
        else
        {
            t -= Time.deltaTime;
            t = t.Clamp(0, idleTime);
            active.color = active.color.A(idleCurve.Evaluate(t));
        }
    }
}