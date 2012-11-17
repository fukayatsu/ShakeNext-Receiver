/*
    timerでs1のチャクタリング防止
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO.Ports;
using System.Runtime.InteropServices;

namespace SN_Receiver1
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        extern static uint SendInput(
            uint nInputs,   // INPUT 構造体の数(イベント数)
            INPUT[] pInputs,   // INPUT 構造体
            int cbSize     // INPUT 構造体のサイズ
            );

        [StructLayout(LayoutKind.Sequential)]  // アンマネージ DLL 対応用 struct 記述宣言
        struct INPUT
        {
            public int type;  // 0 = INPUT_MOUSE(デフォルト), 1 = INPUT_KEYBOARD
            public MOUSEINPUT mi;
            // Note: struct の場合、デフォルト(パラメータなしの)コンストラクタは、
            //       言語側で定義済みで、フィールドを 0 に初期化する。
        }

        [StructLayout(LayoutKind.Sequential)]  // アンマネージ DLL 対応用 struct 記述宣言
        struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public int mouseData;  // amount of wheel movement
            public int dwFlags;
            public int time;  // time stamp for the event
            public IntPtr dwExtraInfo;
            // Note: struct の場合、デフォルト(パラメータなしの)コンストラクタは、
            //       言語側で定義済みで、フィールドを 0 に初期化する。
        }

        // dwFlags
        const int MOUSEEVENTF_MOVED = 0x0001;
        const int MOUSEEVENTF_LEFTDOWN = 0x0002;  // 左ボタン Down
        const int MOUSEEVENTF_LEFTUP = 0x0004;  // 左ボタン Up
        const int MOUSEEVENTF_RIGHTDOWN = 0x0008;  // 右ボタン Down
        const int MOUSEEVENTF_RIGHTUP = 0x0010;  // 右ボタン Up
        const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;  // 中ボタン Down
        const int MOUSEEVENTF_MIDDLEUP = 0x0040;  // 中ボタン Up
        const int MOUSEEVENTF_WHEEL = 0x0080;
        const int MOUSEEVENTF_XDOWN = 0x0100;
        const int MOUSEEVENTF_XUP = 0x0200;
        const int MOUSEEVENTF_ABSOLUTE = 0x8000;

        const int screen_length = 0x10000;  // for MOUSEEVENTF_ABSOLUTE (この値は固定)
        
        
        double aw=0, ah=0, w=0, h=0;
        Point p=new Point(0,0);
        bool shakeEnable = true;
        bool clickMode = false;

        public Form1()
        {
            InitializeComponent();
            pictureBox1.BackColor = Color.Transparent;

            Form2 form2 = new Form2(this);
            form2.ShowDialog();
            
        }

        public SerialPort getPort() {
            return serialPort1;
        }

        private void 設定ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2(this);
            form2.ShowDialog();
        }

        private void 終了ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        delegate void myDelegate();
        void received() {
            String s=serialPort1.ReadExisting();
            this.Text = s;
            if (s[0] == 'w') {
                string[] t = s.Split(',');
                aw = Int32.Parse(t[1]);
                ah = Int32.Parse(t[2]);

            }
            Properties.Settings ms = Properties.Settings.Default;
            if (s == "b1") {
                SendKeys.SendWait(ms.b1);//home
            }
            else if (s == "b2")
            {
                SendKeys.SendWait(ms.b2);//end
            }
            else if (s == "b3" )
            {
                SendKeys.SendWait(ms.b3);//back
            }
            else if (s == "l2")
            {
                if (clickMode)  clickMode = false;
                else SendKeys.SendWait(ms.l2);//down
            }
            else if (s == "b4" )
            {
                SendKeys.SendWait(ms.b4);//right 
                //この辺で画像送信
            }
            else if(s == "l1"){
                if (clickMode) clickMode = false;
                else SendKeys.SendWait(ms.l1);//up
            }
            else if (s == "m1")
            {
               SendKeys.SendWait(ms.m1);//+F5
            }
            else if (s == "m2")
            {
                SendKeys.SendWait(ms.m2);//ESC
            }

            else if (s == "s1" && shakeEnable)
            {
                SendKeys.SendWait(ms.l2);
                shakeEnable = false;
                timer1.Enabled = true;
            }
            else if (s[0] == 'v')
            {
                if (!clickMode)
                {

                    
                    //Cursor.Hide();
                    //ポインタを動かす
                    string[] t = s.Split(',');
                    //t[0]:v1, t[1]:a, t[2]:x, t[3]:y ,
                    int a = Int32.Parse(t[1]);
                    double ax = double.Parse(t[2]);
                    double ay = double.Parse(t[3]);


                    if (a == 0 || a == 2)
                    {
                        
                        pictureBox1.Visible = true;
                        //this.Visible = true;
                    }
                    else if (a == 1)
                    {
                        pictureBox1.Visible = false;
                  
                        //this.Visible = false;
                    }
                    p = new Point((int)(ax * w / aw), (int)(ay * h / ah));
                    //pictureBox1.Location = p;
                    this.Location = new Point(p.X - 25, p.Y - 25);
                    this.Text = p.ToString();
                    //this.TopMost = true;
                    Cursor.Position = new Point((int)w, (int)h / 2);
                }
                else
                {
                    //ポインタを動かす
                    string[] t = s.Split(',');
                    //t[0]:v1, t[1]:a, t[2]:x, t[3]:y ,
                    int a = Int32.Parse(t[1]);
                    double ax = double.Parse(t[2]);
                    double ay = double.Parse(t[3]);
                  
                    p = new Point((int)(ax * w / aw), (int)(ay * h / ah));
                    Cursor.Position = p;
                }

            }
            else if (s[0] == 'c')
            {
                if (clickMode)
                {
                    //crick
                    INPUT[] input = new INPUT[2];

                    if (s == "c2")
                    {
                        input[0].mi.dwFlags = MOUSEEVENTF_LEFTDOWN;
                        input[1].mi.dwFlags = MOUSEEVENTF_LEFTUP;
                    }
                    else if (s == "c1")
                    {
                        input[0].mi.dwFlags = MOUSEEVENTF_RIGHTDOWN;
                        input[1].mi.dwFlags = MOUSEEVENTF_RIGHTUP;
                    }

                    SendInput(2, input, Marshal.SizeOf(input[0]));//効かないときがある

                    //clickMode = false;
                }
                else
                {
                    clickMode = true;
                    this.Location = new Point(-100,-100);
                }
            }
        }
        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Invoke(new myDelegate(received));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            w = Screen.PrimaryScreen.Bounds.Width;
            h = Screen.PrimaryScreen.Bounds.Height;
            this.TopMost = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            shakeEnable = true;
            timer1.Enabled = false;
        }

        //その名の通り
        protected override bool ShowWithoutActivation
        {
            get
            {
                return true;// base.ShowWithoutActivation;
            }
        }
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                // 自身をアクティブフォーカスさせない
                case 0x21:  // WM_MOUSEACTIVATE
                 m.Result = new IntPtr(3);   // MA_NOACTIVATE
                 return;

            }
            base.WndProc(ref m);
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            Cursor.Show();
        }
    }
}
