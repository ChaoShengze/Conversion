using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Conversion
{
    public partial class Main : Form
    {
        private MyHook hook_Main = new MyHook(); // 实例化系统键盘钩子类 MyHook
        public static string filePath = "table.txt"; // 表文件地址，相对路径
        public static bool workMode = false; // 是否进行按键捕获
        public static string inputStr; // 存储有效输入的String的变量
        ArrayList convIndex = new ArrayList(); // 存储需要转换的String的动态数组
        ArrayList convChar = new ArrayList(); // 存储转换的目标String的动态数组
        string[] strTemp = new string[2]; // 用以存储临时String的公共变量
        public static bool Upper = false; // 公共变量，用以判断捕获的字符是否应该为大写
        public static bool CapsLock = false; // 大写锁定在运行时用API实时检测键盘会有bug，所以在挂载钩子时读取一次值以后用程序自行判断

        /// <summary>
        /// 调用Windows API 模拟Ctrl+V输入
        /// </summary>
        [DllImport("user32.dll", EntryPoint = "keybd_event", SetLastError = true)]
        public static extern void keybd_event(Keys bVk, byte bScan, uint dwFlags, uint dwExtraInfo);
        public const int KEYEVENTF_KEYUP = 2;

        /// <summary>
        /// 主进程，绑定按键捕获和窗口关闭自动卸载钩子的事件
        /// </summary>
        public Main()
        {
            InitializeComponent();
            this.hook_Main.OnKeyDown += new KeyEventHandler(HookKeyDown);
            this.FormClosed += Main_FormClosed;
        }

        /// <summary>
        /// 窗口关闭自动卸载钩子
        /// </summary>
        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.hook_Main.UnInstallHook();
        }

        /// <summary>
        /// 程序加载时进行读表、挂载键盘钩子、读取大写锁定状态、显示任务栏图标的行为
        /// </summary>
        private void Main_Load(object sender, EventArgs e)
        {
            ReadTable();
            hook_Main.InstallHook("1");
            CapsLock = CapsLockStatus;
            notifyIcon1.Text = "Conversion By Chao.Shengze && Fang.Jingxian";
            notifyIcon1.ShowBalloonTip(2, "Running", "You can double-click this icon to hide or show the window...", ToolTipIcon.Info);
        }

        /// <summary>
        /// 判断输入的字符的大小写,根据CapsLock和Shift按键的状况共计四种情况
        /// 重要：因为实际是捕获键盘的按键输入而不是捕获输入了的文本，所以需要自行判断按键输入的是大写还是小写字母
        /// </summary>
        private static void isUpper()
        {
            if (CapsLock == true && ShiftStatus == true)
            {
                Upper = false;
            }
            else if (CapsLock == false && ShiftStatus == true)
            {
                Upper = true;
            }
            else if (CapsLock == true && ShiftStatus == false)
            {
                Upper = true;
            }
            else if (CapsLock == false && ShiftStatus == false)
            {
                Upper = false;
            }
        }

        /// <summary>
        /// CapsLockStatus
        /// </summary>
        public static bool CapsLockStatus
        {
            get
            {
                byte[] bs = new byte[256];
                Win32API.GetKeyboardState(bs);
                return (bs[0x14] == 1);
            }
        }

        /// <summary>
        /// ShiftStatus
        /// </summary>
        public static bool ShiftStatus
        {
            get
            {
                int keyNum = 16;
                return (Win32API.GetKeyState(keyNum) < 0);
            }
        }

        /// <summary>
        /// 按键捕获响应函数
        /// </summary>
        private void HookKeyDown(object sender, KeyEventArgs e)
        {
            bool needWrite = false; // 判断是否要将捕获的这次按键输出至log并写入inputStr

            // 判断大小写以让检测支持大小写区分
            isUpper();

            // 对按键进行判断和截断（不响应某些按键）
            switch (e.KeyData.ToString())
            {
                case "Oem5": // "Oem5"即是反斜杠，修改workMode开始处理捕获的信息
                    workMode = true;
                    needWrite = false;
                    break;
                case "Space": // 空格键修改workMode结束处理
                    workMode = false;
                    if (inputStr != null) // 判断在输入反斜杠和输入空格之间有没有输入内容，若有则调用判断函数处理并清空inputStr以准备响应下次流程
                    {
                        Judgment(inputStr);
                        inputStr = null;
                    }
                    needWrite = false;
                    break;
                case "Back": // 退格时按顺序删除上一个字符，此体系只对顺序输入有效
                    BackSpace();
                    needWrite = false;
                    break;
                case "LShiftKey":
                case "RShiftKey":
                case "Return":
                case "Tab":
                case "Up":
                case "Down":
                case "Left":
                case "Right":
                    needWrite = false;
                    break;
                case "Capital": // 当按下CapsLock按键时截获并修改状态
                    if (CapsLock == true)
                    {
                        CapsLock = false;
                    }
                    else
                    {
                        CapsLock = true;
                    }
                    needWrite = false;
                    break;
                default:
                    needWrite = true;
                    break;
            }

            if (needWrite)
            {
                if (Upper == true) // 根据大小写判断向inputStr中输入大小写格式的字符
                {
                    LogAndKeyStringWrite(e.KeyData.ToString().ToUpper());
                }
                else
                {
                    LogAndKeyStringWrite(e.KeyData.ToString().ToLower());
                }
            }
        }

        /// <summary>
        /// 输出Log和写入inputStr
        /// </summary>
        private void LogAndKeyStringWrite(string txt)
        {
            if (this.resultinfo.Lines.Length > 100)
            {
                this.resultinfo.Text = null;
            }

            if (workMode == true && txt != "Space") // 空格这个按键不写入inputStr
            {
                this.resultinfo.AppendText("Key: " + txt + Environment.NewLine);
                this.resultinfo.SelectionStart = this.resultinfo.Text.Length;
                inputStr += txt;
            }
        }

        /// <summary>
        /// 按下backspace时响应
        /// </summary>
        private void BackSpace()
        {
            if (inputStr != null && inputStr.Length > 1)
            {
                inputStr = inputStr.Remove(inputStr.Length - 1, 1);
            }
        }

        /// <summary>
        /// 判断函数
        /// </summary>
        private void Judgment(string txt)
        {
            bool canConv = false;

            resultinfo.AppendText("Judgment:" + txt + Environment.NewLine);
            resultinfo.SelectionStart = resultinfo.Text.Length;

            for (int i = 0; i < txt.Length + 1; i++) // 根据输入的inputStr的长度来删除需要转义的字符串以在原位置输入要转义的目标字符，+1是要算上不在inputStr中的反斜杠；如果未能匹配成功此处也当做自动删除
            {
                SendKeys.Send("{BACKSPACE}");
            }

            // 遍历convIndex检查是否需要进行转义
            foreach (var item in convIndex)
            {
                if (item.ToString() == txt)
                {
                    if (convChar[convIndex.IndexOf(txt)].ToString().IndexOf(@"\run ") == 0) // 用以启动程序，智能识别参数
                    {
                        string runPath = "";
                        runPath = convChar[convIndex.IndexOf(txt)].ToString().Replace(@"\run ", "");
                        System.Diagnostics.Debug.WriteLine(runPath);
                        try
                        {
                              if (runPath.IndexOf(@"| ") != -1)
                            {
                                string startPath = runPath.Substring(0, runPath.IndexOf(@"|"));
                                string startParameter = runPath.Substring(runPath.IndexOf(@"|"), runPath.Length - runPath.IndexOf(@"|")).Replace("|","");
                                System.Diagnostics.Debug.WriteLine(startPath);
                                System.Diagnostics.Process.Start(startPath, startParameter);
                            }
                            else
                            {
                                System.Diagnostics.Process.Start(runPath);
                            }
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("Executable file path is illegal!","PLEASE CHECK THE TABLE");
                            //throw;
                        }
                    }
                    else if (convChar[convIndex.IndexOf(txt)].ToString().IndexOf(@"\code ") == 0) // 用以复制和粘贴代码段，从单独的文件中读取（原格式是文本即可，后缀是js、cpp、cs等无所谓）
                    {
                        string codePath = convChar[convIndex.IndexOf(txt)].ToString().Replace(@"\code ", "");
                        try
                        {
                            StreamReader fileReader = new StreamReader(codePath, Encoding.UTF8); // 虽然指定了是UTF8格式，但是实际上Unicode格式也能读取
                            string nextLine; // 用以存储读取的每一行的变量
                            string allLine = ""; // 用以存储所有行

                            // 循环读取至文件末
                            while ((nextLine = fileReader.ReadLine()) != null)
                            {
                                allLine += "\n" + nextLine;
                            }

                            Clipboard.SetDataObject(allLine);
                            resultinfo.AppendText("Output code fragment!" + Environment.NewLine);
                            resultinfo.SelectionStart = resultinfo.Text.Length;

                            // 模拟键盘Ctrl+V操作，C#的SendKey和SendKeyWait方法对于Edge、Adobe系列等软件不兼容
                            keybd_event(Keys.ControlKey, 0, 0, 0);
                            keybd_event(Keys.V, 0, 0, 0);
                            keybd_event(Keys.ControlKey, 0, KEYEVENTF_KEYUP, 0);
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("Can not read \"table.txt\",please check it.", "ERROR"); // 读取异常时提示并自动退出
                            Close();
                        }
                    }
                    else // 是正常的符号转义
                    {
                        Clipboard.SetDataObject(convChar[convIndex.IndexOf(txt)]); // 将要输出的目标字符以text形式存储至系统的剪贴板中
                        resultinfo.AppendText("Output:" + convChar[convIndex.IndexOf(txt)] + Environment.NewLine);
                        resultinfo.SelectionStart = resultinfo.Text.Length;

                        // 模拟键盘Ctrl+V操作，C#的SendKey和SendKeyWait方法对于Edge、Adobe系列等软件不兼容
                        keybd_event(Keys.ControlKey, 0, 0, 0);
                        keybd_event(Keys.V, 0, 0, 0);
                        keybd_event(Keys.ControlKey, 0, KEYEVENTF_KEYUP, 0);
                    }

                    canConv = true;
                    inputStr = null; // 复位inputStr
                }
            }

            if (canConv == false)
            {
                notifyIcon1.ShowBalloonTip(1, "Error", "Can not convert \"" + inputStr + "\" .", ToolTipIcon.Error);
            }
        }

        /// <summary>
        /// 读表函数
        /// </summary>
        private void ReadTable()
        {
            try
            {
                StreamReader fileReader = new StreamReader(filePath, Encoding.UTF8); // 虽然指定了是UTF8格式，但是实际上Unicode格式也能读取
                string nextLine; // 用以存储读取的每一行的变量

                // 循环读取至文件末
                while ((nextLine = fileReader.ReadLine()) != null)
                {
                    strTemp = nextLine.Split(','); // 以半角字符","来分割每一行的字符串，strTemp[0]是需要转义的字符串，strTemp[1]是目标字符串
                    convIndex.Add(strTemp[0]);
                    convChar.Add(strTemp[1]);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Can not read \"table.txt\",please check it.", "ERROR"); // 读取异常时提示并自动退出
                Close();
            }
        }

        /// <summary>
        /// 显示或隐藏窗体
        /// </summary>
        private void HideAndShow(object sender, EventArgs e)
        {
            if (this.Visible == false)
            {
                this.Show();
            }
            else
            {
                this.Hide();
            }
        }

        /// <summary>
        /// 任务栏图标 Hide/Show 菜单响应
        /// </summary>
        private void hideShowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HideAndShow(sender, e);
        }

        /// <summary>
        /// 任务栏图标 Exit 菜单响应
        /// </summary>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Pause 按钮响应
        /// </summary>
        private void pause(object sender, EventArgs e)
        {
            Controller();
        }

        /// <summary>
        /// Pause/Continue 处理函数，供多处调用
        /// 进行的操作有：改变按钮的文字→卸载/挂载键盘钩子→任务栏提示→写入Log
        /// </summary>
        private void Controller()
        {
            switch (btn_pause.Text)
            {
                case "Pause":
                    btn_pause.Text = "Continue";
                    hook_Main.UnInstallHook();
                    notifyIcon1.ShowBalloonTip(1, "Pause", "Program is now stopped.", ToolTipIcon.Warning);
                    this.resultinfo.AppendText("Pause :Program is now stopped." + Environment.NewLine);
                    this.resultinfo.SelectionStart = this.resultinfo.Text.Length;
                    break;
                case "Continue":
                    btn_pause.Text = "Pause";
                    hook_Main.InstallHook("1");
                    CapsLock = CapsLockStatus;
                    notifyIcon1.ShowBalloonTip(1, "Continue", "Program is now running.", ToolTipIcon.Warning);
                    this.resultinfo.AppendText("Continue :Program is now running." + Environment.NewLine);
                    this.resultinfo.SelectionStart = this.resultinfo.Text.Length;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 任务栏图标 Pause 菜单响应
        /// </summary>
        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Controller();
        }

        /// <summary>
        /// Hide 按钮响应
        /// </summary>
        private void btn_hide_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        /// <summary>
        /// Close 按钮响应
        /// </summary>
        private void btn_exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}