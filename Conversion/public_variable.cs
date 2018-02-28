using System;
using System.Runtime.InteropServices;

namespace Conversion
{
    #region 委托定义

    /// <summary>
    /// 钩子委托声明
    /// </summary>
    /// <param name="nCode"></param>
    /// <param name="wParam"></param>
    /// <param name="lParam"></param>
    /// <returns></returns>
    public delegate int HookProc(int nCode, Int32 wParam, IntPtr lParam);    

    /// <summary>
    /// 无返回委托声明
    /// </summary>
    public delegate void VoidCallback();

    #endregion 委托定义

    #region 枚举定义

    public enum WH_Codes : int
    {
        /// <summary>
        /// 底层键盘钩子
        /// </summary>
        WH_KEYBOARD_LL = 13,        
    }   

    public enum WM_KEYBOARD : int
    {
        /// <summary>
        /// 非系统按键按下
        /// </summary>
        WM_KEYDOWN = 0x100,

        /// <summary>
        /// 非系统按键释放
        /// </summary>
        WM_KEYUP = 0x101,

        /// <summary>
        /// 系统按键按下
        /// </summary>
        WM_SYSKEYDOWN = 0x104,

        /// <summary>
        /// 系统按键释放
        /// </summary>
        WM_SYSKEYUP = 0x105
    }

    /// <summary>
    /// SetWindowPos标志位枚举
    /// </summary>
    /// <remarks>详细说明,请参见MSDN中关于SetWindowPos函数的描述</remarks>
    public enum SetWindowPosFlags : int
    {
        /// <summary>
        /// 
        /// </summary>
        SWP_NOSIZE = 0x0001,

        /// <summary>
        /// 
        /// </summary>
        SWP_NOMOVE = 0x0002,

        /// <summary>
        /// 
        /// </summary>
        SWP_NOZORDER = 0x0004,

        /// <summary>
        /// 
        /// </summary>
        SWP_NOREDRAW = 0x0008,

        /// <summary>
        /// 
        /// </summary>
        SWP_NOACTIVATE = 0x0010,

        /// <summary>
        /// 
        /// </summary>
        SWP_FRAMECHANGED = 0x0020,

        /// <summary>
        /// 
        /// </summary>
        SWP_SHOWWINDOW = 0x0040,

        /// <summary>
        /// 
        /// </summary>
        SWP_HIDEWINDOW = 0x0080,

        /// <summary>
        /// 
        /// </summary>
        SWP_NOCOPYBITS = 0x0100,

        /// <summary>
        /// 
        /// </summary>
        SWP_NOOWNERZORDER = 0x0200,

        /// <summary>
        /// 
        /// </summary>
        SWP_NOSENDCHANGING = 0x0400,

        /// <summary>
        /// 
        /// </summary>
        SWP_DRAWFRAME = 0x0020,

        /// <summary>
        /// 
        /// </summary>
        SWP_NOREPOSITION = 0x0200,

        /// <summary>
        /// 
        /// </summary>
        SWP_DEFERERASE = 0x2000,

        /// <summary>
        /// 
        /// </summary>
        SWP_ASYNCWINDOWPOS = 0x4000

    }

    #endregion 枚举定义

    #region 结构定义    

    /// <summary>
    /// 键盘钩子事件结构定义
    /// </summary>
    /// <remarks>详细说明请参考MSDN中关于 KBDLLHOOKSTRUCT 的说明</remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct KeyboardHookStruct
    {
        /// <summary>
        /// Specifies a virtual-key code. The code must be a value in the range 1 to 254. 
        /// </summary>
        public UInt32 VKCode;

        /// <summary>
        /// Specifies a hardware scan code for the key.
        /// </summary>
        public UInt32 ScanCode;

        /// <summary>
        /// Specifies the extended-key flag, event-injected flag, context code, 
        /// and transition-state flag. This member is specified as follows. 
        /// An application can use the following values to test the keystroke flags. 
        /// </summary>
        public UInt32 Flags;

        /// <summary>
        /// Specifies the time stamp for this message. 
        /// </summary>
        public UInt32 Time;

        /// <summary>
        /// Specifies extra information associated with the message. 
        /// </summary>
        public UInt32 ExtraInfo;
    }

    #endregion 结构定义
}
