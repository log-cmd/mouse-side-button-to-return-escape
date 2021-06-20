using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace mouse_side_button_to_return_escape
{
    class MouseHook
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetWindowsHookEx(int hookType, HookProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        delegate IntPtr HookProc(int code, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public System.IntPtr dwExtraInfo;
        }

        HookProc proc;
        IntPtr hookId = IntPtr.Zero;

        public void Hook()
        {
            if (hookId != IntPtr.Zero)
            {
                return;
            }

            proc = HookProcedure;
            hookId = SetWindowsHookEx(14 /*WH_MOUSE_LL*/, proc, IntPtr.Zero, 0);
        }

        public void UnHook()
        {
            UnhookWindowsHookEx(hookId);
            hookId = IntPtr.Zero;
        }

        public event Action OnX1Down;
        public event Action OnX2Down;

        IntPtr HookProcedure(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (wParam == (IntPtr)0x020B/*WM_XBUTTONDOWN*/))
            {
                MSLLHOOKSTRUCT m = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));

                if (m.mouseData == 0x00010000)
                {
                    OnX1Down?.Invoke();
                }
                else if (m.mouseData == 0x00020000)
                {
                    OnX2Down?.Invoke();
                }
            }
            return CallNextHookEx(hookId, nCode, wParam, lParam);
        }
    }
}
