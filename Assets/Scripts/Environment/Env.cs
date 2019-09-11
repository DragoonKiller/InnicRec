using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Env : MonoBehaviour
{
    public static Env inst;

    Env() => inst = this;

    [Tooltip("按键记录器.")]
    public GlobalRecorder inputRecorder;

    [Tooltip("用于每条记录中, 视野条上方的时间戳的对象池.")]
    public ObjectPool recordViewportTimeStampPool;

    [Tooltip("区间事件记录的对象池.")]
    public ObjectPool intervalPool;

    [Tooltip("单点事件记录的对象池.")]
    public ObjectPool pointPool;

    public bool cursorVisible;

    void Start()
    {
        foreach(var i in FindObjectsOfType<RecordPanelControl>()) i.record = Record.GenerateTestData();
    }

    void Update()
    {
        Cursor.visible = cursorVisible;
        StateMachine.Run();
    }
}
