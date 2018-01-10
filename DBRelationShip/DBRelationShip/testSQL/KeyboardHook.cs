using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace testSQL
{
    [StructLayout(LayoutKind.Sequential)]
    public struct KeyBoardHookStruct
    {
        /// <summary>
        /// Specifies a virtual-key code. The code must be a value in the range 1 to 254. 
        /// </summary>
        public uint VKCode;

        /// <summary>
        /// Specifies a hardware scan code for the key.
        /// </summary>
        public uint ScanCode;

        /// <summary>
        /// Specifies the extended-key flag, event-injected flag, context code, 
        /// and transition-state flag. This member is specified as follows. 
        /// An application can use the following values to test the keystroke flags. 
        /// </summary>
        public uint Flags;

        /// <summary>
        /// Specifies the time stamp for this message. 
        /// </summary>
        public uint Time;

        /// <summary>
        /// Specifies extra information associated with the message. 
        /// </summary>
        public uint ExtraInfo;

    }

    public class KeyboardHook
    {
        #region (invokestuff)
        [DllImport("kernel32.dll")]
        static extern uint GetCurrentThreadId();

        [DllImport("user32.dll")]
        static extern IntPtr SetWindowsHookEx(int code, HookProcKeyboard func, IntPtr hInstance, uint threadID);

        [DllImport("user32.dll")]
        static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        static extern int CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
        #endregion

        #region constans
        private const int WH_KEYBOARD = 2;
        private const int HC_ACTION = 0;
        #endregion

        delegate int HookProcKeyboard(int code, IntPtr wParam, IntPtr lParam);
        private HookProcKeyboard KeyboardProcDelegate = null;
        private IntPtr khook;

        /// <summary>
        /// 钩子函数初始化，在插件启动时调用
        /// </summary>
        public void InitHook()
        {
            uint id = GetCurrentThreadId();
            this.KeyboardProcDelegate = new HookProcKeyboard(this.KeyboardProc);
            khook = SetWindowsHookEx(WH_KEYBOARD, this.KeyboardProcDelegate, IntPtr.Zero, id);
        }

        /// <summary>
        /// 插件卸载时，关闭钩子函数
        /// </summary>
        public void UnHook()
        {
            if (khook != IntPtr.Zero)
            {
                UnhookWindowsHookEx(khook);
            }
        }

        /// <summary>
        /// 具体的钩子逻辑处理。
        /// </summary>
        /// <param name="code"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        private int KeyboardProc(int code, IntPtr wParam, IntPtr lParam)
        {
            bool processedAll = false;
            try
            {
                if (code >= 0)
                {
                    KeyBoardHookStruct kbh = (KeyBoardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyBoardHookStruct));
                    if (kbh.VKCode == (int)Keys.S && (int)Control.ModifierKeys == (int)Keys.Control)  // 截获F8
                    {
                        MessageBox.Show("快捷键已拦截!不能保存!");
                        return 1;
                    }
                    if ((int)Control.ModifierKeys == (int)Keys.Alt && (int)Control.ModifierKeys == (int)Keys.Enter)
                    {
                        MessageBox.Show("捕捉到Alt+Enter!");
                        return 1;
                    }

                }

                //if (code != HC_ACTION)
                //{
                //    return CallNextHookEx(khook, code, wParam, lParam);
                //}

                //if ((int)wParam == (int)Keys.Enter)
                //{
                //    processedAll = ProcessEnter(wParam, lParam);
                //}
                ////if ((int)wParam == (int)Keys.F2 && ((int)lParam & (int)Keys.LControlKey) != 0 )//
                ////{
                ////    docService.NewRequirement(ref processedAll);
                ////}
            }
            catch
            {
            }
            if (!processedAll)
            {
                return CallNextHookEx(khook, code, wParam, lParam);
            }
            else
            {
                return 1;
            }
        }
        /// <summary>
        /// 处理回车键事件
        /// </summary>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        private bool ProcessEnter(IntPtr wParam, IntPtr lParam)
        {
            bool processedAll = false;

            if (((int)lParam & (int)Keys.Alt) != 0)//同时按下左Alt+enter。
            {
                ////docService.NewRequirement(ref processedAll);
            }
            else if (((int)lParam & (int)Keys.Alt) != 0 && ((int)lParam & (int)Keys.LControlKey) != 0)//同时按下alt，ctrl+enter
            {
                ////docService.ContinueRequirement(ref processedAll);
            }
            else
            {
                //docService.NewParagraph(ref processedAll);
            }

            return processedAll;
        }
    }
}
