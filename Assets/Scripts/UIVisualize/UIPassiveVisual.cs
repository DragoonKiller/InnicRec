using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class UIPassiveVisual : UIVisual
{
    public bool activate
    {
        get => activating;
        set
        {
            if(value == activating) return;

            if(value)
            {
                t = 0;
                activating = true;
            }
            else
            {
                t = t / activeTime * idleTime;
                activating = false;
            }
        }
    }
}
