#include <windows.h>

#include "keytrans.h"

#include <iostream>
#include <queue>

using namespace std;

// 事件类型.
static enum EventType
{
    EventNone = 0,
    EventDown = 1,
    EventUp = 2,
};

static enum MouseWhellEventType
{
    MouseScrollUp = 7864320,
    MouseScrollDown = -7864320
};

// 记录一个按键事件, 例如键盘按键或鼠标点击等.
// 注意这是和绘制程序直接通信的, 因此内容和序列化信息有所不同.
static struct EventData
{
    string key;
    EventType type;
    unsigned long long time;
    float x;
    float y;
    float z;
    float w;
};

static HHOOK hookHandle;
static long long beginTime;
static HANDLE mutex;
static HANDLE thread;
static DWORD thrid;
static queue<EventData> dataQueue;
static FILE* f = stdout;

// 注意这个时间精确到 1ms = 1e-3s.
// 但是 C# 中的 tick 是 100ns = 1e-7s.
// 所以要乘一个常数.
static unsigned long long GetDeltaTime() { return (GetTickCount64() - beginTime) * 10000llu; }

static void Out(EventData const& x)
{
    // 使用符号 $ 开头. 之前的所有信息应该被直接忽略.
    fprintf(f, "$");

    // json 格式传输一个对象.
    fprintf(f, "{");
    fprintf(f, "\"key\":\"%s\",", x.key.c_str());
    fprintf(f, "\"time\":%llu,", x.time);
    fprintf(f, "\"type\":%d,", x.type);
    fprintf(f, "\"x\":%f,", x.x);
    fprintf(f, "\"y\":%f,", x.y);
    fprintf(f, "\"z\":%f,", x.z);
    fprintf(f, "\"w\":%f", x.w);
    fprintf(f, "}");

    // 符号 # 结束当前事件传送, 并开始传送下一个事件.
    fprintf(f, "#");

    // 及时更新.
    fflush(f);
}

static void Store(EventData const& x)
{
    auto res = WaitForSingleObject(mutex, 1000);
    if (res != WAIT_OBJECT_0) return;
    dataQueue.push(x);
    ReleaseMutex(mutex);
}

static void Store(string const& key, EventType type, float x = 0, float y = 0, float z = 0, float w = 0)
{
    Store(EventData{ key, type, GetDeltaTime(), x, y, z, w });
}

static void Fetch()
{
    auto res = WaitForSingleObject(mutex, 1000);
    if (res != WAIT_OBJECT_0) return;
    if (dataQueue.empty())
    {
        ReleaseMutex(mutex);
        return;
    }
    auto x = dataQueue.front();
    dataQueue.pop();
    ReleaseMutex(mutex);
    Out(x);
}

static void FetchLoop()
{
    while (true) Fetch();
}


static LRESULT OnMouseEvent(int nCode, WPARAM wParam, MOUSEHOOKSTRUCT* lParam)
{
    if (nCode >= 0)
    {
        if (wParam == WM_MOUSEMOVE)
        {
            auto x = lParam->pt.x;
            auto y = lParam->pt.y;
            Store("MouseMove", EventNone, x, y, 0, 0);
        }
        else if (wParam == WM_MOUSEWHEEL) Store("MouseWhell", EventNone, lParam->pt.x == MouseScrollUp ? 1 : -1);
        else if (wParam == WM_LBUTTONDOWN) Store("MouseLeft", EventDown);
        else if (wParam == WM_LBUTTONUP) Store("MouseLeft", EventUp);
        else if (wParam == WM_RBUTTONDOWN) Store("MouseRight", EventDown);
        else if (wParam == WM_RBUTTONUP) Store("MouseRight", EventUp);
        else if (wParam == WM_MBUTTONDOWN) Store("MouseMiddle", EventDown);
        else if (wParam == WM_MBUTTONUP) Store("MouseMiddle", EventUp);
    }
    return CallNextHookEx(hookHandle, nCode, (WPARAM)wParam, (LPARAM)lParam);
}

static struct KeyboardHookStruct
{
    unsigned int vkCode;
    unsigned int scanCode;
    unsigned int flags;
    unsigned int time;
    unsigned int* dwExtraInfo;
};

static LRESULT OnKeyEvent(int nCode, WPARAM wParam, KeyboardHookStruct* lParam)
{
    if (nCode >= 0)
    {
        Store(VkToString(lParam->vkCode), wParam == WM_KEYDOWN ? EventDown : EventUp);
    }
    return CallNextHookEx(hookHandle, nCode, (WPARAM)wParam, (LPARAM)lParam);
}

int main()
{
    FreeConsole();

    auto x = GetCurrentThreadId();

    mutex = CreateMutex(nullptr, false, nullptr);

    // 另开一个线程可以保证在 stdout 卡住(缓冲区满)时, 钩子仍然正常运行.
    thread = CreateThread(
        nullptr,
        1024 * 1024 * 8,
        (LPTHREAD_START_ROUTINE)FetchLoop,
        nullptr,
        0,
        &thrid
    );
    if (thread == nullptr) return 0;


    beginTime = GetTickCount64();

    auto libUser = LoadLibrary(TEXT("user32.dll"));

    // 鼠标点击事件钩子.
    HHOOK mouseHook = SetWindowsHookEx(
        WH_MOUSE_LL,
        (HOOKPROC)OnMouseEvent,
        libUser,
        0
    );

    // 键盘事件钩子.
    HHOOK keyHook = SetWindowsHookEx(
        WH_KEYBOARD_LL,
        (HOOKPROC)OnKeyEvent,
        libUser,
        0
    );

    // 事件循环, 用于接收事件.
    MSG msg;
    while (GetMessage(&msg, NULL, 0, 0) > 0)
    {
        TranslateMessage(&msg);
        DispatchMessage(&msg);
    }

    UnhookWindowsHookEx(mouseHook);

    return WaitForSingleObject(thread, 10000);
}