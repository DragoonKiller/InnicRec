#include "keytrans.h"
using namespace std;


#define stringify(x) #x
#define caseStringify(x,y,t) case VK_##x: return {string(stringify(t)), string(stringify(y))}

#undef DELETE
#undef XBUTTON1
#undef XBUTTON2
#undef DECIMAL
#define Key Key
#define Mouse Mouse
#define Unknown Unknown

pair<string, string> VkToString(int vk)
{
    char c[2] = { 0 };
    if (vk >= '0' && vk <= '9') { c[0] = (char)vk; return { string("Key"), string(c) }; }
    if (vk >= 'A' && vk <= 'Z') { c[0] = (char)vk; return { string("Key"), string(c) }; }
    switch (vk)
    {
        caseStringify(LBUTTON, MouseLeft, Mouse);
        caseStringify(RBUTTON, MouseRight, Mouse);
        caseStringify(CANCEL, Cancel, Unknown);
        caseStringify(MBUTTON, MouseMiddle, Mouse);
        caseStringify(XBUTTON1, XButton1, Unknown);
        caseStringify(XBUTTON2, XButton2, Unknown);
        caseStringify(BACK, Backspace, Key);
        caseStringify(TAB, Tab, Key);
        caseStringify(CLEAR, Clear, Unknown);
        caseStringify(RETURN, Enter, Key);
        caseStringify(SHIFT, Shift, Key);
        caseStringify(CONTROL, Control, Key);
        caseStringify(MENU, Menu, Key);
        caseStringify(PAUSE, Pause, Key);
        caseStringify(CAPITAL, CapsLock, Key);
        caseStringify(KANA, KanaMode, Unknown);
        caseStringify(JUNJA, JunjaMode, Unknown);
        caseStringify(FINAL, Final, Unknown);
        caseStringify(KANJI, Kanji, Unknown);
        caseStringify(ESCAPE, Escape, Key);
        caseStringify(CONVERT, Convert, Unknown);
        caseStringify(NONCONVERT, NonConvert, Unknown);
        caseStringify(ACCEPT, Accept, Unknown);
        caseStringify(MODECHANGE, ModeChange, Unknown);
        caseStringify(SPACE, Space, Key);
        caseStringify(PRIOR, Prior, Key);
        caseStringify(NEXT, Next, Key);
        caseStringify(END, End, Key);
        caseStringify(HOME, Home, Key);
        caseStringify(LEFT, Left, Key);
        caseStringify(UP, Up, Key);
        caseStringify(RIGHT, Right, Key);
        caseStringify(DOWN, Down, Key);
        caseStringify(SELECT, Select, Unknown);
        caseStringify(PRINT, Print, Key);
        caseStringify(EXECUTE, Execute, Unknown);
        caseStringify(SNAPSHOT, Snapshot, Key);
        caseStringify(INSERT, Insert, Key);
        caseStringify(DELETE, Delete, Key);
        caseStringify(HELP, Help, Unknown);
        caseStringify(LWIN, LeftWin, Key);
        caseStringify(RWIN, RightWin, Key);
        caseStringify(APPS, Apps, Unknown);
        caseStringify(SLEEP, Sleep, Unknown);
        caseStringify(NUMPAD0, NumPad0, Key);
        caseStringify(NUMPAD1, NumPad1, Key);
        caseStringify(NUMPAD2, NumPad2, Key);
        caseStringify(NUMPAD3, NumPad3, Key);
        caseStringify(NUMPAD4, NumPad4, Key);
        caseStringify(NUMPAD5, NumPad5, Key);
        caseStringify(NUMPAD6, NumPad6, Key);
        caseStringify(NUMPAD7, NumPad7, Key);
        caseStringify(NUMPAD8, NumPad8, Key);
        caseStringify(NUMPAD9, NumPad9, Key);
        caseStringify(MULTIPLY, NumPadMult, Key);
        caseStringify(ADD, NumPadAdd, Key);
        caseStringify(SEPARATOR, NumPadSep, Key);
        caseStringify(SUBTRACT, NumpadSub, Key);
        caseStringify(DECIMAL, NumPadDecimal, Unknown);
        caseStringify(DIVIDE, NumPadDivide, Key);
        caseStringify(F1, F1, Key);
        caseStringify(F2, F2, Key);
        caseStringify(F3, F3, Key);
        caseStringify(F4, F4, Key);
        caseStringify(F5, F5, Key);
        caseStringify(F6, F6, Key);
        caseStringify(F7, F7, Key);
        caseStringify(F8, F8, Key);
        caseStringify(F9, F9, Key);
        caseStringify(F10, F10, Key);
        caseStringify(F11, F11, Key);
        caseStringify(F12, F12, Key);
        caseStringify(F13, F13, Key);
        caseStringify(F14, F14, Key);
        caseStringify(F15, F15, Key);
        caseStringify(F16, F16, Key);
        caseStringify(F17, F17, Key);
        caseStringify(F18, F18, Key);
        caseStringify(F19, F19, Key);
        caseStringify(F20, F20, Key);
        caseStringify(F21, F21, Key);
        caseStringify(F22, F22, Key);
        caseStringify(F23, F23, Key);
        caseStringify(F24, F24, Key);
        caseStringify(NUMLOCK, NumLock, Key);
        caseStringify(SCROLL, ScrollLock, Key);
        caseStringify(OEM_NEC_EQUAL, OEM_NEC_EQUAL, Unknown);
        caseStringify(OEM_FJ_MASSHOU, OEM_FJ_MASSHOU, Unknown);
        caseStringify(OEM_FJ_TOUROKU, OEM_FJ_TOUROKU, Unknown);
        caseStringify(OEM_FJ_LOYA, OEM_FJ_LOYA, Unknown);
        caseStringify(OEM_FJ_ROYA, OEM_FJ_ROYA, Unknown);
        caseStringify(LSHIFT, LeftShift, Key);
        caseStringify(RSHIFT, RightShift, Key);
        caseStringify(LCONTROL, LeftControl, key);
        caseStringify(RCONTROL, RightControl, key);
        caseStringify(LMENU, LeftMenu, Key);
        caseStringify(RMENU, RightMenu, Key);
        caseStringify(BROWSER_BACK, BROWSER_BACK, Unknown);
        caseStringify(BROWSER_FORWARD, BROWSER_FORWARD, Unknown);
        caseStringify(BROWSER_REFRESH, BROWSER_REFRESH, Unknown);
        caseStringify(BROWSER_STOP, BROWSER_STOP, Unknown);
        caseStringify(BROWSER_SEARCH, BROWSER_SEARCH, Unknown);
        caseStringify(BROWSER_FAVORITES, BROWSER_FAVORITES, Unknown);
        caseStringify(BROWSER_HOME, BROWSER_HOME, Unknown);
        caseStringify(VOLUME_MUTE, VOLUME_MUTE, Unknown);
        caseStringify(VOLUME_DOWN, VOLUME_DOWN, Unknown);
        caseStringify(VOLUME_UP, VOLUME_UP, Unknown);
        caseStringify(MEDIA_NEXT_TRACK, MEDIA_NEXT_TRACK, Unknown);
        caseStringify(MEDIA_PREV_TRACK, MEDIA_PREV_TRACK, Unknown);
        caseStringify(MEDIA_STOP, MEDIA_STOP, Unknown);
        caseStringify(MEDIA_PLAY_PAUSE, MEDIA_PLAY_PAUSE, Unknown);
        caseStringify(LAUNCH_MAIL, LAUNCH_MAIL, Unknown);
        caseStringify(LAUNCH_MEDIA_SELECT, LAUNCH_MEDIA_SELECT, Unknown);
        caseStringify(LAUNCH_APP1, LAUNCH_APP1, Unknown);
        caseStringify(LAUNCH_APP2, LAUNCH_APP2, Unknown);
        caseStringify(OEM_1, Semicolon, Key);
        caseStringify(OEM_PLUS, Plus, Key);
        caseStringify(OEM_COMMA, Comma, Key);
        caseStringify(OEM_MINUS, Minus, Key);
        caseStringify(OEM_PERIOD, Period, Key);
        caseStringify(OEM_2, QuestionMark, Key);
        caseStringify(OEM_3, Tilde, Key);
        caseStringify(OEM_4, OpenBracket, Key);
        caseStringify(OEM_5, Pipe, Key);
        caseStringify(OEM_6, CloseBracket, Key);
        caseStringify(OEM_7, Quote, Key);
        caseStringify(OEM_8, Backslash, Key);
        caseStringify(OEM_AX, ProcessKey, Unknown);
        caseStringify(OEM_102, Oem102, Unknown);
        caseStringify(ICO_HELP, ICOHelp, Unknown);
        caseStringify(ICO_00, ICO00, Unknown);
        caseStringify(PROCESSKEY, ProcessKey, Unknown);
        caseStringify(ICO_CLEAR, ICOClear, Unknown);
        caseStringify(PACKET, Packet, Unknown);
    }
    c[0] = (char)vk;
    return { "Unknown", string(c) };
}
