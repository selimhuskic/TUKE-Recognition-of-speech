using NAudio.Wave;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TUKE_Recognition_of_speech.Helper;
using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;


namespace TUKE_Recognition_of_speech
{
    public partial class TUKEasr : Form { 
          
        public TUKEasr()
        {
            InitializeComponent();
            Rectangle workingArea = Screen.GetWorkingArea(this);
            Location = new Point(workingArea.Right - Size.Width,
                                      workingArea.Bottom - Size.Height - 35);
            InitializeTimer();
            Show();
            
            ContextMenu contextMenu = new ContextMenu();
            ContextMenu contextMenuPictureBox = new ContextMenu();
            
            
            TransparencyKey = Color.Khaki;
            FormBorderStyle = FormBorderStyle.None;
            
            contextMenu.MenuItems.Add("Restore button",
                (s, ee) => 
                {
                    Show();
                    WindowState = FormWindowState.Normal;
                    notifyIcon1.Visible = false;
                    
                });

            contextMenu.MenuItems.Add("Exit", (s, ee) => Application.Exit());

            contextMenuPictureBox.MenuItems.Add("Minimize", (s,ee) => WindowState = FormWindowState.Minimized);
            contextMenuPictureBox.MenuItems.Add("Exit", (s, ee) => Application.Exit());


            notifyIcon1.ContextMenu = contextMenu;
            Record.ContextMenu = contextMenuPictureBox;
            WindowState = FormWindowState.Normal;

            KeyPreview = true;
            
            MaximizeBox = false;
            TopMost = true;

            dele = new WinEventDelegate(WinEventProc);
            IntPtr m_hhook = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, dele, 0, 0, WINEVENT_OUTOFCONTEXT);

            hook.KeyPressed +=
            new EventHandler<KeyPressedEventArgs>(hook_KeyPressed);
            hook.RegisterHotKey(ModKeys.Control, Keys.D);

            Icon = Properties.Resources.on_1__p4m_icon;

            notifyIcon1.Icon = Properties.Resources.on_1__p4m_icon;
            tt.SetToolTip(Record,"Left click to record, right for context menu");
        }

        delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        private bool recording { get; set; } = false;
        ToolTip tt = new ToolTip();
        KeyboardHook hook = new KeyboardHook();
        MemoryStream memoryStream = null;
        WaveInEvent waveIn = null;
        WaveFileWriter writer = null;
        Timer timer;       
        IntPtr prev;
        WinEventDelegate dele = null;
        private const uint WINEVENT_OUTOFCONTEXT = 0;
        private const uint EVENT_SYSTEM_FOREGROUND = 3;
        private const int KEYUP = 0x2;
        private const int EXTENDEDKEY = 0x1;
        private const int ALT = 0xA4;
        private const int SHOW_MAXIMIZED = 3;
        private const int MYACTION_HOTKEY_ID = 1;
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        const int SM_CXDRAG = 68;
        const int SM_CYDRAG = 69;

       
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll", SetLastError = true)]        
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
        
        //Keyboard hook-Alt+A
        private void hook_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            recording = !recording;
           
            SetForegroundWindow(prev);

            if (recording)
            {

                waveIn = new WaveInEvent();
                waveIn.WaveFormat = new WaveFormat(16000, 16, 1);
                memoryStream = new MemoryStream();
                writer = new WaveFileWriter(memoryStream, waveIn.WaveFormat);
                timer.Enabled = true;

                try
                {
                    waveIn.StartRecording();
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                }

                waveIn.DataAvailable += (s, a) =>
                {
                   
                    Record.BackgroundImage = Properties.Resources.off_2_;
                   
                    notifyIcon1.Icon = Properties.Resources.off_2__ZwD_icon;
                    writer.Write(a.Buffer, 0, a.BytesRecorded);
                };


                waveIn.RecordingStopped += /*async*/ (s, a) =>
                {

                    Record.BackgroundImage = Properties.Resources.on_1_;
                    notifyIcon1.Icon = Properties.Resources.on_1__p4m_icon;


                    /*await*/
                    ServerConnect.SendAudioGetResponse(memoryStream.ToArray(), this);
                    if (writer != null)
                        writer.Dispose();

                    writer = null;

                    if (waveIn != null)
                        waveIn.Dispose();

                    waveIn = null;

                };
            }
            else
            {
                if (waveIn != null)
                {
                    waveIn.StopRecording();
                    timer.Enabled = false;
                }
            }
            FormClosing += (s, a) =>
            {
                if (waveIn != null)
                    waveIn.StopRecording();

                Application.ExitThread();
            };
        }      
        private void ActiveWindowTitle()
        {
           
            IntPtr handle = IntPtr.Zero;
           
            
            handle = GetForegroundWindow();
            string name = getClassName(handle);
          
          
            handle = GetForegroundWindow();
            try
            {
                if (handle != Handle && name != "Shell_TrayWnd" && name != "NotifyIconOverflowWindow")
                {
                    prev = handle;
                }
            }
            catch (Exception ex)
            {
               // MessageBox.Show();
            }
        }
        public void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            ActiveWindowTitle();
        }
        public string getClassName(IntPtr handle)
        {
            StringBuilder className = new StringBuilder(100);
            int nret = GetClassName(handle, className, className.Capacity);
            return className.ToString();
        }
        //Every recording can last up to 7.5 seconds
        private void InitializeTimer()
        {
            timer = new Timer();
            timer.Interval = 7500;          
            timer.Tick += new EventHandler(Timer_Tick);
            
        }        
        //Start recording on a mouse click
        private void Record_Click(object sender, MouseEventArgs me)
         {
            
                if (me.Button == MouseButtons.Left)
                {
                    recording = !recording;


                    SetForegroundWindow(prev);
                    if (recording)
                    {

                        waveIn = new WaveInEvent();
                        waveIn.WaveFormat = new WaveFormat(16000, 16, 1);
                        memoryStream = new MemoryStream();
                        writer = new WaveFileWriter(memoryStream, waveIn.WaveFormat);
                        timer.Enabled = true;

                        try
                        {
                            waveIn.StartRecording();
                        }
                        catch (Exception err)
                        {
                            MessageBox.Show(err.Message);
                        }

                        waveIn.DataAvailable += (s, a) =>
                            {
                                Record.BackgroundImage = Properties.Resources.off_2_;
                                notifyIcon1.Icon = Properties.Resources.off_2__ZwD_icon;

                                writer.Write(a.Buffer, 0, a.BytesRecorded);
                            };

                        waveIn.RecordingStopped += /*async*/ (s, a) =>
                        {
                            Record.BackgroundImage = Properties.Resources.on_1_;
                            notifyIcon1.Icon = Properties.Resources.on_1__p4m_icon;
                            //Record.BackgroundImage = Image.FromFile("C:\\Users\\selim\\OneDrive\\Desktop\\lg.circle-slack-loading-icon.gif");
                            /*async*/
                            ServerConnect.SendAudioGetResponse(memoryStream.ToArray(), this);

                            if (writer != null)
                                writer.Dispose();

                            writer = null;

                            if (waveIn != null)
                                waveIn.Dispose();

                            waveIn = null;

                        };
                    }
                    else
                    {
                        if (waveIn != null)
                        {
                            waveIn.StopRecording();
                            timer.Enabled = false;
                        }
                    }
                }

               
            

            FormClosing += (s, a) =>
            {
                if (waveIn != null)
                    waveIn.StopRecording();

                Application.ExitThread();
            };
        
        }
        //Triggered when the timer interval gets to 0
        private void Timer_Tick(object Sender, EventArgs e)
        {        
            waveIn.StopRecording();
            timer.Enabled = false;
            notifyIcon1.Icon = Properties.Resources.on_1__p4m_icon;
        }     
        private void TUKEasr_Resize(object sender, EventArgs e)
        {           
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon1.ShowBalloonTip(3000);
                notifyIcon1.Visible = true;
            }
        }
        //Button is restored on desktop on a double click 
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
            
        }     
        //Provides the server response
        public void WriteOutResponse(string messageServer)
        {


            dynamic output = JObject.Parse(messageServer);
            Regex r = new Regex(@"\[\w*\]");           
            SendKeys.SendWait(r.Replace(output.data[0].text.ToString(),""));           
        }
        private void NotifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) {
                recording = !recording;
                

                SetForegroundWindow(prev);

                if (recording)
                {
                    
                    waveIn = new WaveInEvent();
                    waveIn.WaveFormat = new WaveFormat(16000, 16, 1);
                    memoryStream = new MemoryStream();
                    writer = new WaveFileWriter(memoryStream, waveIn.WaveFormat);
                    timer.Enabled = true;
                    
                 
                    
                    try
                    {
                        waveIn.StartRecording();
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show(err.Message);
                    }

                    waveIn.DataAvailable += (s, a) =>
                    {
                        Record.BackgroundImage = Properties.Resources.off_2_;
                        notifyIcon1.Icon = Properties.Resources.off_2__ZwD_icon;

                        writer.Write(a.Buffer, 0, a.BytesRecorded);
                    };


                    waveIn.RecordingStopped += /*async*/ (s, a) =>
                    {

                        Record.BackgroundImage = Properties.Resources.on_1_;
                        notifyIcon1.Icon = Properties.Resources.on_1__p4m_icon;
                        //Record.BackgroundImage = Image.FromFile("C:\\Users\\selim\\OneDrive\\Desktop\\giphy.webp");
                        
                        /*await*/
                        ServerConnect.SendAudioGetResponse(memoryStream.ToArray(), this);
                        if (writer != null)
                            writer.Dispose();

                        writer = null;

                        if(waveIn!=null)
                            waveIn.Dispose();

                        waveIn = null;

                    };

                    
                }
                else
                {
                    if (waveIn != null)
                    {
                        waveIn.StopRecording();
                        timer.Enabled = false;
                    }

                }
                FormClosing += (s, a) =>
                {
                    if (waveIn != null)
                        waveIn.StopRecording();

                    Application.ExitThread();
                };
            }            
        }
        private new void MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }      
    }
}
