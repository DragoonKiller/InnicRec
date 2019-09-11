using System;
using System.Linq;
using System.Collections.Generic;
using Utils;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using System.Threading;

public sealed class Record
{

    /// <summary>
    /// 代表一个输入事件, 包括按下和放开.
    /// </summary>
    public class RecordEvent : IComparable<RecordEvent>
    {
        public enum Type
        {
            None,
            Begin,
            End,
        }

        public string key;
        public TimeSpan time;
        public Type type;
        public float x;
        public float y;
        public float z;
        public float w;
        public RecordEvent pairing;
        public bool isLeft => type == Type.Begin;
        public bool isRight => type == Type.End;
        public bool isPairedLeft => pairing != null && time < pairing.time;
        public bool isPairedRight => pairing != null && pairing.time < time;

        public int CompareTo(RecordEvent v)
        {
            if(time != v.time) return time.CompareTo(v.time);
            return key.CompareTo(key);
        }
    }

    /// <summary>
    /// 输入事件.
    /// </summary>
    [NonSerialized] public readonly Dictionary<string, List<RecordEvent>> events = new Dictionary<string, List<RecordEvent>>();

    /// <summary>
    /// 正在等待右端点输入的左端点.
    /// </summary>
    [NonSerialized] public readonly Dictionary<string, RecordEvent> waitForClose = new Dictionary<string, RecordEvent>();
    
    /// <summary>
    /// 这条记录的总时间.
    /// </summary>
    [NonSerialized] public TimeSpan totalTime;

    public void Add(string key, RecordEvent.Type type, TimeSpan time, float x, float y, float z, float w)
    {
        var e = new RecordEvent() {
            key = key,
            time = time,
            type = type,
            x = x, y = y, z = z, w = w
        };

        // 当 e.type == Begin 且 wait for close 时,
        // 说明这个按键很可能是长按某个键的结果, 目前不对此进行记录.
        // 对于没有配对的节点, 当做单独的点来算.
        if(e.type == RecordEvent.Type.Begin && !waitForClose.ContainsKey(e.key))
        {
            events.GetOrDefault(key).Add(e);
            waitForClose.Add(e.key, e);
        }
        else if(e.type == RecordEvent.Type.End && waitForClose.ContainsKey(e.key))
        {
            events.GetOrDefault(key).Add(e);
            var g = waitForClose[e.key];
            g.pairing = e;
            e.pairing = g;
            waitForClose.Remove(e.key);
        }
    }

    // ================================================================================================================
    // Tests 
    // ================================================================================================================

    public static Record GenerateTestData()
    {
        var x = new Record();
        x.totalTime = TimeSpan.FromSeconds(10);

        void Add(string key, TimeSpan deltaTimeLeft, TimeSpan deltaTimeRight)
        {
            var a = new RecordEvent() {
                key = key,
                time = deltaTimeLeft,
                type = RecordEvent.Type.Begin,
            };
            var b = new RecordEvent() {
                key = key,
                time = deltaTimeRight,
                type = RecordEvent.Type.End,
            };
            a.pairing = b;
            b.pairing = a;
            x.events.GetOrDefault(a.key).Add(a);
            x.events.GetOrDefault(b.key).Add(b);
        }

        Add("MouseLeft", TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(2.0));
        Add("G", TimeSpan.FromSeconds(7.0), TimeSpan.FromSeconds(9.0));
        Add("T", TimeSpan.FromSeconds(5.0), TimeSpan.FromSeconds(6.0));
        Add("G", TimeSpan.FromSeconds(2.5), TimeSpan.FromSeconds(3.0));
        Add("H", TimeSpan.FromSeconds(6.0), TimeSpan.FromSeconds(8.0));
        Add("E", TimeSpan.FromSeconds(4.0), TimeSpan.FromSeconds(8.0));
        Add("F", TimeSpan.FromSeconds(3.0), TimeSpan.FromSeconds(4.0));
        Add("OemOpenBrackets", TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(6.0));
        Add("LShiftKey", TimeSpan.FromSeconds(6.0), TimeSpan.FromSeconds(10.0));

        return x;
    }

    // ================================================================================================================
    // Serialization
    // ================================================================================================================

    struct SerializedRecordEvent
    {
        public int id;
        public string key;
        public long time;
        public RecordEvent.Type type;
        public float x;
        public float y;
        public float z;
        public float w;
        public int pairing;
    }

    class SerializedRecord
    {
        public long totalTime;
        public List<SerializedRecordEvent> events = new List<SerializedRecordEvent>();
    }

    public string Serialize()
    {
        var serialized = new SerializedRecord {
            totalTime = totalTime.Ticks
        };

        var e2id = new Dictionary<RecordEvent, int>();
        int id = 0;
        foreach(var i in events.Values) foreach(var e in i) e2id[e] = id++;
        foreach(var x in e2id)
        {
            var e = x.Key;
            var i = x.Value;
            serialized.events.Add(new SerializedRecordEvent() {
                id = e2id[e],
                key = e.key,
                time = e.time.Ticks,
                type = e.type,
                x = e.x,
                y = e.y,
                z = e.z,
                w = e.w,
                pairing = e2id.GetOrDefault(e.pairing, -1)
            });
        }

        return JsonConvert.SerializeObject(serialized, Formatting.Indented);
    }

    public static Record Deserialize(string content)
    {
        var res = new Record();
        var serialized = JsonConvert.DeserializeObject<SerializedRecord>(content);
        var id2e = new Dictionary<int, RecordEvent>();
        var id2pairing = new Dictionary<int, int>();

        foreach(var i in serialized.events)
        {
            var x = new RecordEvent() {
                key = i.key,
                time = new TimeSpan(i.time),
                type = i.type,
                x = i.x, y = i.y, z = i.z, w = i.w,
                pairing = null
            };
            res.events.GetOrDefault(i.key).Add(x);
            id2e[i.id] = x;
            if(i.pairing >= 0)
            {
                id2pairing[i.pairing] = i.id;
                id2pairing[i.id] = i.pairing;
            }
        }

        // 配对.
        foreach(var a in id2pairing)
        {
            id2e[a.Key].pairing = id2e[a.Value];
            id2e[a.Value].pairing = id2e[a.Key];
        }

        // 注意, 因为记录开启和关闭而导致的不完成区间记录不会再一次进入等待队列.
        // foreach(var i in res.events.Values) foreach(var e in i) if(e.pairing == null && e.isLeft) res.waitForClose[e.key] = e;

        res.totalTime = new TimeSpan(serialized.totalTime);

        return res;
    }

    public void SaveToFile(string path) => File.WriteAllText(path, Serialize());
}
