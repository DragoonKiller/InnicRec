using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageColorBinder : MonoBehaviour
{
    public string[] colorNames;
    public Image[] images;
    void Update()
    {
        for(int i = 0; i < System.Math.Min(colorNames.Length, images.Length); i++)
        {
            var c = (Color)typeof(ColorManager).GetField(colorNames[i], BindingFlags.Public | BindingFlags.Instance).GetValue(ColorManager.inst);
            images[i].color = new Color(c.r, c.g, c.b, images[i].color.a);
        }
    }
}
