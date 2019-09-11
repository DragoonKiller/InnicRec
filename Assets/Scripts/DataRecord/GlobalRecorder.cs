using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Collections.Concurrent;
using UnityEngine;
using System.IO;
using System.Text;
using System.Threading;
using System.Diagnostics;
using Newtonsoft.Json;
using DateTime = System.DateTime;
using TimeSpan = System.TimeSpan;
using Debug = UnityEngine.Debug;

public class GlobalRecorder : MonoBehaviour
{
    Record _record;
    public Record record
    {
        get => _record;
        set
        {
            _record = value;
            if(_record != null)
            {
                offsetTime = record.totalTime;
                beginTime = DateTime.Now;
            }
        }
    }

    struct Input
    {
#pragma warning disable CS0649
        public string key;
        public Record.RecordEvent.Type type;
        public long time;
        public float x;
        public float y;
        public float z;
        public float w;
#pragma warning restore CS0649
    }

    // 工作线程将数据传递给主线程的队列.
    readonly ConcurrentQueue<Input> recordQueue = new ConcurrentQueue<Input>();

    // 按键捕获程序开始执行的时间.
    DateTime startTime;

    // 开始录制的时间.
    DateTime beginTime;

    // 记录末尾的偏移量.
    TimeSpan offsetTime;

    Process proc;

    Thread reader;

    void Start()
    {
        // 寻找并开启记录器.
        var procStart = new ProcessStartInfo();
        var fileName = Path.Combine(Directory.GetCurrentDirectory(), "Build", "InnicBaseRecorder.exe");
        if(File.Exists(fileName)) procStart.FileName = fileName;
        fileName = Path.Combine(Directory.GetCurrentDirectory(), "InnicBaseRecorder.exe");
        if(File.Exists(fileName)) procStart.FileName = fileName;
        procStart.Arguments = "";
        procStart.UseShellExecute = false;
        procStart.RedirectStandardOutput = true;
        proc = new Process() { StartInfo = procStart };
        proc.Start();

        // 记录起始时间. 假设这个时间点和记录器内部所记录的时间点是一致的.
        startTime = DateTime.Now;

        // 开一个线程不断读取 stdout, 将内容放到队列中.
        // 注意: 必须不停地执行以消耗 InnicBaseRecorder 的输出.
        var data = proc.StandardOutput;
        reader = new Thread(() =>
        {
            var sb = new StringBuilder();
            while(true)
            {
                var ch = data.Read();
                if(ch == '$') sb.Clear();
                else if(ch == '#')
                {
                    var x = JsonConvert.DeserializeObject<Input>(sb.ToString());
                    recordQueue.Enqueue(x);
                    sb.Clear();
                }
                else sb.Append((char)ch);
            }
        });
        reader.Priority = System.Threading.ThreadPriority.BelowNormal;
        reader.Start();
    }

    void Update()
    {
        PullQueue();
        SetRecordTime();
    }

    void PullQueue()
    {
        // 这个函数在主线程执行.
        // 注意: 不论是否正在记录, 主线程必须不停地执行以消耗 InnicBaseRecorder 的输出, 使其不爆缓冲.
        while(recordQueue.TryDequeue(out var input))
        {
            var time = startTime + new TimeSpan(input.time) - beginTime + offsetTime;
            if(record != null)
            {
                record.Add(input.key, input.type, time, input.x, input.y, input.z, input.w);
            }
        }
    }


    void SetRecordTime()
    {
        if(record == null) return;
        var curTime = DateTime.Now;
        record.totalTime = offsetTime + (curTime - beginTime);
    }

    void OnDestroy()
    {
        if(reader.IsAlive) reader.Abort();
        reader = null;
        if(!proc.HasExited) proc.Kill();
        proc = null;
    }
}
