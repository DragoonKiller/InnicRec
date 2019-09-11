using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class ColorManager : MonoBehaviour
{
    public static ColorManager inst;

    public ColorManager() => inst = this;


    [Tooltip("随机种子.")]
    public int seed;

    [Header("Panel Controls")]

    [Tooltip("视野条背景颜色.")]
    public Color viewportBarBackColor;

    [Tooltip("视野条基本颜色.")]
    public Color viewportBarColor;

    [Tooltip("鼠标移动到视野条上时的颜色.")]
    public Color viewportBarSelectColor;

    [Tooltip("视野条控制点的基本颜色.")]
    public Color viewportEndpointColor;

    [Tooltip("鼠标移动到视野条控制点上时的颜色.")]
    public Color viewportEndpointSelectColor;

    [Header("Record Panels")]

    [Tooltip("记录面板的常规颜色.")]
    public Color panelColor;

    [Tooltip("记录面板中, 标志 keyline 部分的背景颜色.")]
    public Color panelKeyBackColor;

    [Header("Key Line")]

    [Tooltip("键盘上的按键对应的面板的标准颜色.")]
    public Color baseKeyPanelColor;

    [Tooltip("键盘上的按键对应的时间线的标准颜色.")]
    public Color baseKeyLineColor;

    [Tooltip("键盘上的按键对应的区间的标准颜色.")]
    public Color baseKeyIntervalColor;

    [Tooltip("键盘上的按键对应的点的标准颜色.")]
    public Color baseKeyPointColor;


    [Tooltip("键盘上的按键对应的面板的标准颜色.")]
    public Color baseMousePanelColor;

    [Tooltip("键盘上的按键对应的时间线的标准颜色.")]
    public Color baseMouseLineColor;

    [Tooltip("键盘上的按键对应的区间的标准颜色.")]
    public Color baseMouseIntervalColor;

    [Tooltip("键盘上的按键对应的点的标准颜色.")]
    public Color baseMousePointColor;


    [Tooltip("未知按键的标准颜色.")]
    public Color unknownKeyColor;

    [Tooltip("选中/不选中 keyline 面板时, 饱和度的比值.")]
    public float saturationRate;

    [Tooltip("选中/不选中 keyline 面板时, 明度的比值.")]
    public float brightnessRate;

    [Tooltip("按键色相的随机比率.")]
    [Range(0, 1)] public float hueRate;

    [Header("Debug")]

    [SerializeField] float recordSeed;
    System.Random rand;

    readonly Dictionary<string, Vector3> colorBias = new Dictionary<string, Vector3>();

    void Start()
    {
        recordSeed = seed;
        Setup();
    }

    void Update()
    {
        if(seed != recordSeed) Setup();
    }

    void Setup()
    {
        colorBias.Clear();
        rand = new System.Random(seed);
    }

    public Color GetPanelColor(string keyName, string category, bool highlighted = false)
        => GetColor(keyName, category, baseMousePanelColor, baseKeyPanelColor, unknownKeyColor, highlighted);

    public Color GetLineColor(string keyName, string category, bool highlighted = false)
        => GetColor(keyName, category, baseMouseLineColor, baseKeyLineColor, unknownKeyColor, highlighted);

    public Color GetIntervalColor(string keyName, string category, bool highlighted = false)
        => GetColor(keyName, category, baseMouseIntervalColor, baseKeyIntervalColor, unknownKeyColor, highlighted);

    public Color GetPointColor(string keyName, string category, bool highlighted = false)
        => GetColor(keyName, category, baseMousePointColor, baseKeyPointColor, unknownKeyColor, highlighted);

    Color GetColor(string keyName, string category, Color mouseColor, Color keyColor, Color unknownColor, bool highlighted = false)
    {
        if(highlighted) return GetHighlightedColor(GetColor(keyName, category, mouseColor, keyColor, unknownColor, false));
        if(category.ToLower() == "mouse") return ColorWithBias(keyName, mouseColor);
        if(category.ToLower() == "key") return ColorWithBias(keyName, keyColor);
        return unknownColor;
    }


    Color GetHighlightedColor(Color x)
    {
        Color.RGBToHSV(x, out var h, out var s, out var v);
        return Color.HSVToRGB(
            h,
            s * saturationRate,
            v * brightnessRate
        );
    }

    float NextRd() => ((float)(rand.NextDouble() % 1.0)).Xmap(0, 1, -1, 1);
    Vector3 NextVec() => new Vector3(NextRd(), NextRd(), NextRd());

    Color ColorWithBias(string keyName, Color x)
    {
        var key = keyName.ToLower();
        Color.RGBToHSV(x, out var h, out var s, out var v);
        Vector3 sel;
        if(!colorBias.TryGetValue(key, out sel)) sel = NextVec();
        colorBias[key] = sel;
        return Color.HSVToRGB(h * (1f + sel.x * hueRate), s, v);
    }

}
