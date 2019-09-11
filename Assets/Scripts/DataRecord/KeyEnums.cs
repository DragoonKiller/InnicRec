using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class KeyEnumsExt
{
    public static string MakeReadable(string keyName)
    {
        if(Enum.TryParse<VirtualKeys>(keyName, out var vk)) return vk.MakeReadable();
        if(Enum.TryParse<MouseButton>(keyName, out var ms)) return ms.MakeReadable();
        return keyName;
    }

    public static string MakeReadable(this MouseButton x)
    {
        switch(x)
        {
        case MouseButton.MouseLeft: return "Mouse Left";
        case MouseButton.MouseRight: return "Mouse Right";
        case MouseButton.MouseMiddle: return "Mouse Middle";
        }
        return $"Unknown {((int)x).ToString("X")}";
    }

    public static string MakeReadable(this VirtualKeys x)
    {
        switch(x)
        {
        case VirtualKeys.Modifiers: return "Modifiers";
        case VirtualKeys.None: return "None";
        case VirtualKeys.LButton: return "Mouse Left";
        case VirtualKeys.RButton: return "Mouse Right";
        case VirtualKeys.Cancel: return "Cancel";
        case VirtualKeys.MButton: return "Mouse Middle";
        case VirtualKeys.XButton1: return "XButton 1";
        case VirtualKeys.XButton2: return "XButton 2";
        case VirtualKeys.Back: return "Backspace";
        case VirtualKeys.Tab: return "Tab";
        case VirtualKeys.LineFeed: return "Line Feed";
        case VirtualKeys.Clear: return "Clear";
        case VirtualKeys.Enter: return "Enter";
        case VirtualKeys.ShiftKey: return "Shift";
        case VirtualKeys.ControlKey: return "Control";
        case VirtualKeys.Menu: return "Menu";
        case VirtualKeys.Pause: return "Pause";
        case VirtualKeys.Capital: return "Capital";
        case VirtualKeys.KanaMode: return "Kana Mode";
        case VirtualKeys.JunjaMode: return "Junja Mode";
        case VirtualKeys.FinalMode: return "Final Mode";
        case VirtualKeys.HanjaMode: return "Hanja Mode";
        case VirtualKeys.Escape: return "Escape";
        case VirtualKeys.IMEConvert: return "IME Convert";
        case VirtualKeys.IMENonconvert: return "IME Nonconvert";
        case VirtualKeys.IMEAccept: return "IME Accept";
        case VirtualKeys.IMEModeChange: return "IME Mode Change";
        case VirtualKeys.Space: return "Space";
        case VirtualKeys.Prior: return "Prior";
        case VirtualKeys.Next: return "Next";
        case VirtualKeys.End: return "End";
        case VirtualKeys.Home: return "Home";
        case VirtualKeys.Left: return "Left";
        case VirtualKeys.Up: return "Up";
        case VirtualKeys.Right: return "Right";
        case VirtualKeys.Down: return "Down";
        case VirtualKeys.Select: return "Select";
        case VirtualKeys.Print: return "Print";
        case VirtualKeys.Execute: return "Execute";
        case VirtualKeys.PrintScreen: return "PrintScreen";
        case VirtualKeys.Insert: return "Insert";
        case VirtualKeys.Delete: return "Delete";
        case VirtualKeys.Help: return "Help";
        case VirtualKeys.D0: return "D0";
        case VirtualKeys.D1: return "D1";
        case VirtualKeys.D2: return "D2";
        case VirtualKeys.D3: return "D3";
        case VirtualKeys.D4: return "D4";
        case VirtualKeys.D5: return "D5";
        case VirtualKeys.D6: return "D6";
        case VirtualKeys.D7: return "D7";
        case VirtualKeys.D8: return "D8";
        case VirtualKeys.D9: return "D9";
        case VirtualKeys.A: return "A";
        case VirtualKeys.B: return "B";
        case VirtualKeys.C: return "C";
        case VirtualKeys.D: return "D";
        case VirtualKeys.E: return "E";
        case VirtualKeys.F: return "F";
        case VirtualKeys.G: return "G";
        case VirtualKeys.H: return "H";
        case VirtualKeys.I: return "I";
        case VirtualKeys.J: return "J";
        case VirtualKeys.K: return "K";
        case VirtualKeys.L: return "L";
        case VirtualKeys.M: return "M";
        case VirtualKeys.N: return "N";
        case VirtualKeys.O: return "O";
        case VirtualKeys.P: return "P";
        case VirtualKeys.Q: return "Q";
        case VirtualKeys.R: return "R";
        case VirtualKeys.S: return "S";
        case VirtualKeys.T: return "T";
        case VirtualKeys.U: return "U";
        case VirtualKeys.V: return "V";
        case VirtualKeys.W: return "W";
        case VirtualKeys.X: return "X";
        case VirtualKeys.Y: return "Y";
        case VirtualKeys.Z: return "Z";
        case VirtualKeys.LWin: return "LWin";
        case VirtualKeys.RWin: return "RWin";
        case VirtualKeys.Apps: return "Apps";
        case VirtualKeys.Sleep: return "Sleep";
        case VirtualKeys.NumPad0: return "NumPad 0";
        case VirtualKeys.NumPad1: return "NumPad 1";
        case VirtualKeys.NumPad2: return "NumPad 2";
        case VirtualKeys.NumPad3: return "NumPad 3";
        case VirtualKeys.NumPad4: return "NumPad 4";
        case VirtualKeys.NumPad5: return "NumPad 5";
        case VirtualKeys.NumPad6: return "NumPad 6";
        case VirtualKeys.NumPad7: return "NumPad 7";
        case VirtualKeys.NumPad8: return "NumPad 8";
        case VirtualKeys.NumPad9: return "NumPad 9";
        case VirtualKeys.Multiply: return "NumPad Multiply";
        case VirtualKeys.Add: return "NumPad Add";
        case VirtualKeys.Separator: return "NumPad Separator";
        case VirtualKeys.Subtract: return "NumPad Subtract";
        case VirtualKeys.Decimal: return "Decimal";
        case VirtualKeys.Divide: return "Divide";
        case VirtualKeys.F1: return "F1";
        case VirtualKeys.F2: return "F2";
        case VirtualKeys.F3: return "F3";
        case VirtualKeys.F4: return "F4";
        case VirtualKeys.F5: return "F5";
        case VirtualKeys.F6: return "F6";
        case VirtualKeys.F7: return "F7";
        case VirtualKeys.F8: return "F8";
        case VirtualKeys.F9: return "F9";
        case VirtualKeys.F10: return "F10";
        case VirtualKeys.F11: return "F11";
        case VirtualKeys.F12: return "F12";
        case VirtualKeys.F13: return "F13";
        case VirtualKeys.F14: return "F14";
        case VirtualKeys.F15: return "F15";
        case VirtualKeys.F16: return "F16";
        case VirtualKeys.F17: return "F17";
        case VirtualKeys.F18: return "F18";
        case VirtualKeys.F19: return "F19";
        case VirtualKeys.F20: return "F20";
        case VirtualKeys.F21: return "F21";
        case VirtualKeys.F22: return "F22";
        case VirtualKeys.F23: return "F23";
        case VirtualKeys.F24: return "F24";
        case VirtualKeys.NumLock: return "Num Lock";
        case VirtualKeys.Scroll: return "Scroll";
        case VirtualKeys.LShiftKey: return "Left Shift";
        case VirtualKeys.RShiftKey: return "Right Shift";
        case VirtualKeys.LControlKey: return "Left Control";
        case VirtualKeys.RControlKey: return "Right Control";
        case VirtualKeys.LMenu: return "Left Menu";
        case VirtualKeys.RMenu: return "Right Menu";
        case VirtualKeys.BrowserBack: return "Browser Back";
        case VirtualKeys.BrowserForward: return "Browser Forward";
        case VirtualKeys.BrowserRefresh: return "Browser Refresh";
        case VirtualKeys.BrowserStop: return "Browser Stop";
        case VirtualKeys.BrowserSearch: return "Browser Search";
        case VirtualKeys.BrowserFavorites: return "Browser Favorites";
        case VirtualKeys.BrowserHome: return "Browser Home";
        case VirtualKeys.VolumeMute: return "Volume Mute";
        case VirtualKeys.VolumeDown: return "Volume Down";
        case VirtualKeys.VolumeUp: return "Volume Up";
        case VirtualKeys.MediaNextTrack: return "Media Next Track";
        case VirtualKeys.MediaPreviousTrack: return "Media Previous Track";
        case VirtualKeys.MediaStop: return "Media Stop";
        case VirtualKeys.MediaPlayPause: return "Media Play Pause";
        case VirtualKeys.LaunchMail: return "Launch Mail";
        case VirtualKeys.SelectMedia: return "Select Media";
        case VirtualKeys.LaunchApplication1: return "Launch Application1";
        case VirtualKeys.LaunchApplication2: return "Launch Application2";
        case VirtualKeys.OemSemicolon: return "Semicolon";
        case VirtualKeys.Oemplus: return "Plus";
        case VirtualKeys.Oemcomma: return "Comma";
        case VirtualKeys.OemMinus: return "Minus";
        case VirtualKeys.OemPeriod: return "Period";
        case VirtualKeys.OemQuestion: return "Question Mark";
        case VirtualKeys.Oemtilde: return "Oemtilde";
        case VirtualKeys.OemOpenBrackets: return "Open Brackets";
        case VirtualKeys.OemPipe: return "Pipe";
        case VirtualKeys.OemCloseBrackets: return "Close Brackets";
        case VirtualKeys.OemQuotes: return "Quotes";
        case VirtualKeys.Oem8: return "Oem8";
        case VirtualKeys.OemBackslash: return "Backslash";
        case VirtualKeys.ProcessKey: return "ProcessKey";
        case VirtualKeys.Packet: return "Packet";
        case VirtualKeys.Attn: return "Attn";
        case VirtualKeys.Crsel: return "Crsel";
        case VirtualKeys.Exsel: return "Exsel";
        case VirtualKeys.EraseEof: return "EraseEof";
        case VirtualKeys.Play: return "Play";
        case VirtualKeys.Zoom: return "Zoom";
        case VirtualKeys.NoName: return "NoName";
        case VirtualKeys.Pa1: return "Pa1";
        case VirtualKeys.OemClear: return "Clear";
        case VirtualKeys.KeyCode: return "KeyCode?";
        case VirtualKeys.Shift: return "ShiftModifier";
        case VirtualKeys.Control: return "ControlModifier";
        }
        return $"Unknown({((int)x).ToString("X")})";
    }
}

public enum MouseButton : int
{
    None = 0,
    MouseLeft = 1,
    MouseRight = 2,
    MouseMiddle = 3,
}

// System.Windows.Forms.Keys [Assembly System.Windows.Forms, mono version]
public enum VirtualKeys : int
{
    Modifiers = -65536,
    None = 0,
    LButton = 1,
    RButton = 2,
    Cancel = 3,
    MButton = 4,
    XButton1 = 5,
    XButton2 = 6,
    Back = 8,
    Tab = 9,
    LineFeed = 10,
    Clear = 12,
    Return = 13,
    Enter = 13,
    ShiftKey = 16,
    ControlKey = 17,
    Menu = 18,
    Pause = 19,
    Capital = 20,
    CapsLock = 20,
    KanaMode = 21,
    HanguelMode = 21,
    HangulMode = 21,
    JunjaMode = 23,
    FinalMode = 24,
    HanjaMode = 25,
    KanjiMode = 25,
    Escape = 27,
    IMEConvert = 28,
    IMENonconvert = 29,
    IMEAccept = 30,
    IMEAceept = 30,
    IMEModeChange = 31,
    Space = 32,
    Prior = 33,
    PageUp = 33,
    Next = 34,
    PageDown = 34,
    End = 35,
    Home = 36,
    Left = 37,
    Up = 38,
    Right = 39,
    Down = 40,
    Select = 41,
    Print = 42,
    Execute = 43,
    Snapshot = 44,
    PrintScreen = 44,
    Insert = 45,
    Delete = 46,
    Help = 47,
    D0 = 48,
    D1 = 49,
    D2 = 50,
    D3 = 51,
    D4 = 52,
    D5 = 53,
    D6 = 54,
    D7 = 55,
    D8 = 56,
    D9 = 57,
    A = 65,
    B = 66,
    C = 67,
    D = 68,
    E = 69,
    F = 70,
    G = 71,
    H = 72,
    I = 73,
    J = 74,
    K = 75,
    L = 76,
    M = 77,
    N = 78,
    O = 79,
    P = 80,
    Q = 81,
    R = 82,
    S = 83,
    T = 84,
    U = 85,
    V = 86,
    W = 87,
    X = 88,
    Y = 89,
    Z = 90,
    LWin = 91,
    RWin = 92,
    Apps = 93,
    Sleep = 95,
    NumPad0 = 96,
    NumPad1 = 97,
    NumPad2 = 98,
    NumPad3 = 99,
    NumPad4 = 100,
    NumPad5 = 101,
    NumPad6 = 102,
    NumPad7 = 103,
    NumPad8 = 104,
    NumPad9 = 105,
    Multiply = 106,
    Add = 107,
    Separator = 108,
    Subtract = 109,
    Decimal = 110,
    Divide = 111,
    F1 = 112,
    F2 = 113,
    F3 = 114,
    F4 = 115,
    F5 = 116,
    F6 = 117,
    F7 = 118,
    F8 = 119,
    F9 = 120,
    F10 = 121,
    F11 = 122,
    F12 = 123,
    F13 = 124,
    F14 = 125,
    F15 = 126,
    F16 = 127,
    F17 = 128,
    F18 = 129,
    F19 = 130,
    F20 = 131,
    F21 = 132,
    F22 = 133,
    F23 = 134,
    F24 = 135,
    NumLock = 144,
    Scroll = 145,
    LShiftKey = 160,
    RShiftKey = 161,
    LControlKey = 162,
    RControlKey = 163,
    LMenu = 164,
    RMenu = 165,
    BrowserBack = 166,
    BrowserForward = 167,
    BrowserRefresh = 168,
    BrowserStop = 169,
    BrowserSearch = 170,
    BrowserFavorites = 171,
    BrowserHome = 172,
    VolumeMute = 173,
    VolumeDown = 174,
    VolumeUp = 175,
    MediaNextTrack = 176,
    MediaPreviousTrack = 177,
    MediaStop = 178,
    MediaPlayPause = 179,
    LaunchMail = 180,
    SelectMedia = 181,
    LaunchApplication1 = 182,
    LaunchApplication2 = 183,
    OemSemicolon = 186,
    Oem1 = 186,
    Oemplus = 187,
    Oemcomma = 188,
    OemMinus = 189,
    OemPeriod = 190,
    OemQuestion = 191,
    Oem2 = 191,
    Oemtilde = 192,
    Oem3 = 192,
    OemOpenBrackets = 219,
    Oem4 = 219,
    OemPipe = 220,
    Oem5 = 220,
    OemCloseBrackets = 221,
    Oem6 = 221,
    OemQuotes = 222,
    Oem7 = 222,
    Oem8 = 223,
    OemBackslash = 226,
    Oem102 = 226,
    ProcessKey = 229,
    Packet = 231,
    Attn = 246,
    Crsel = 247,
    Exsel = 248,
    EraseEof = 249,
    Play = 250,
    Zoom = 251,
    NoName = 252,
    Pa1 = 253,
    OemClear = 254,
    KeyCode = 65535,
    Shift = 65536,
    Control = 131072,
    Alt = 262144
}
