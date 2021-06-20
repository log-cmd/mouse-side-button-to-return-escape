using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace mouse_side_button_to_return_escape
{
    class SI
    {
        struct INPUT
        {
            public int type;
            public InputUnion u;
        }

        [StructLayout(LayoutKind.Explicit)]
        struct InputUnion
        {
            [FieldOffset(0)]
            public MOUSEINPUT mi;
            [FieldOffset(0)]
            public KEYBDINPUT ki;
            [FieldOffset(0)]
            public HARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct HARDWAREINPUT
        {
            public uint uMsg;
            public ushort wParamL;
            public ushort wParamH;
        }

        static class NativeMethods
        {

            [DllImport("user32.dll", SetLastError = true)]
            public static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

            [DllImport("user32.dll", EntryPoint = "MapVirtualKeyA")]
            public static extern int MapVirtualKey(int wCode, int wMapType);

            [DllImport("user32.dll")]
            public static extern IntPtr GetMessageExtraInfo();

            [DllImport("user32.dll")]
            public static extern bool SetCursorPos(int x, int y);
        }

        const int INPUT_MOUSE = 0;
        const int INPUT_KEYBOARD = 1;
        const int INPUT_HARDWARE = 2;
        const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
        const uint KEYEVENTF_KEYUP = 0x0002;
        const uint KEYEVENTF_UNICODE = 0x0004;
        const uint KEYEVENTF_SCANCODE = 0x0008;

        public static void Send(System.Windows.Forms.Keys key, bool down)
        {
            INPUT[] inp = new INPUT[1];

            inp[0].type = INPUT_KEYBOARD;
            inp[0].u.ki.wVk = (ushort)key;
            inp[0].u.ki.wScan = (ushort)NativeMethods.MapVirtualKey(inp[0].u.ki.wVk, 0);
            inp[0].u.ki.dwFlags = down ? 0 : KEYEVENTF_KEYUP;
            inp[0].u.ki.dwExtraInfo = NativeMethods.GetMessageExtraInfo();
            inp[0].u.ki.time = 0;

            NativeMethods.SendInput((uint)inp.Length, inp, Marshal.SizeOf(typeof(INPUT)));
        }
    }
}
