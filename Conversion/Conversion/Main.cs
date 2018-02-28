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
        private MyHook hook_Main = new MyHook();
        public static string filePath = "table.txt";
        public static bool workMode = false;
        public static string inputStr;
        ArrayList convIndex = new ArrayList();
        ArrayList convChar = new ArrayList();
        string[] strTemp = new string[2];

        // 以下三行API用于模拟Ctrl+V
        [DllImport("user32.dll", EntryPoint = "keybd_event", SetLastError = true)]
        public static extern void keybd_event(Keys bVk, byte bScan, uint dwFlags, uint dwExtraInfo);
        public const int KEYEVENTF_KEYUP = 2;

        public Main()
        {
            InitializeComponent();
            this.hook_Main.OnKeyDown += new KeyEventHandler(HookKeyDown);
            this.FormClosed += Main_FormClosed;
        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.hook_Main.UnInstallHook();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            ReadTable();
            hook_Main.InstallHook("1");
            notifyIcon1.Text = "Conversion By Fang.Jingxian";
            notifyIcon1.ShowBalloonTip(2, "Running", "You can double-click this icon to hide or show the window...",ToolTipIcon.Info);
        }

        private void HookMain_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape && Control.ModifierKeys == Keys.Shift)
            {
                this.Close();
            }
        }

        private void HookKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData.ToString())
            {
                case "Oem5":
                    workMode = true;
                    break;
                case "Space":
                    workMode = false;
                    if (inputStr != null)
                    {
                        Judgment(inputStr);
                        inputStr = null;
                    }
                    break;
                default:
                    break;
            }

            LogWrite(e.KeyData.ToString());
        }

        private void LogWrite(string txt)
        {
            if (this.resultinfo.Lines.Length > 100)
            {
                this.resultinfo.Text = null;
            }

            if (workMode == true && txt != "Space")
            {
                this.resultinfo.AppendText("Key: "+ txt + Environment.NewLine);
                this.resultinfo.SelectionStart = this.resultinfo.Text.Length;
                inputStr += txt;
            }
        }

        private void Judgment(string txt)
        {
            txt = txt.ToLower();
            txt = txt.Replace(@"\", "");
            txt = txt.Replace("oem5", "");
            txt = txt.Replace("space", "");
            txt = txt.Replace("、", "");
            txt = txt.Replace("return", "");
            
            this.resultinfo.AppendText("Judgment:" + txt + Environment.NewLine);
            this.resultinfo.SelectionStart = this.resultinfo.Text.Length;

            foreach (var item in convIndex)
            {
                if (item.ToString() == txt)
                {
                    for (int i = 0; i < txt.Length + 1; i++)
                    {
                        SendKeys.Send("{BACKSPACE}");
                    }

                    Clipboard.SetText(convChar[convIndex.IndexOf(txt)].ToString());
                    this.resultinfo.AppendText("Output:" + convChar[convIndex.IndexOf(txt)] + Environment.NewLine);
                    this.resultinfo.SelectionStart = this.resultinfo.Text.Length;

                    keybd_event(Keys.ControlKey, 0, 0, 0);
                    keybd_event(Keys.V, 0, 0, 0);
                    keybd_event(Keys.ControlKey, 0, KEYEVENTF_KEYUP, 0);

                    inputStr = null;
                }
            }
        }

        private void ReadTable()
        {
            try
            {
                StreamReader fileReader = new StreamReader(filePath, Encoding.UTF8);
                string nextLine;
                while ((nextLine = fileReader.ReadLine()) != null)
                {
                    strTemp = nextLine.Split(',');
                    convIndex.Add(strTemp[0]);
                    convChar.Add(strTemp[1]);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Can not read table.txt,please check it.", "ERROR");
                throw;
            }
        }

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

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void hideShowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HideAndShow(sender,e);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
