using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace rdpcon
{
    public partial class RDPAPP : Form
    {
        private List<Tuple<string, string, string>> Hosts = new List<Tuple<string, string, string>>();
        private RdpClient RDP;
        public string Filename;
        public string[] HashList;
        public int TimeOut, LogWait;
        public int Logined = 0;
        public RDPAPP(string Filename, string[] HashList, int TimeOut, int LogWait)
        {
            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
            this.Filename = Filename;
            this.HashList = HashList;
            this.TimeOut = TimeOut;
            this.LogWait = LogWait;
            RDP = new RdpClient();
            Controls.Add(RDP);
            RDP.Size = new Size(1, 1);
            RDP.OnResponse += Rdp_OnResponse;
            RDP.OnReady += Rdp_OnReady;

            LoadHosts();

            Shown += delegate
            {
                RDP.ItimeOutTotal = TimeOut * 1000;
                RDP.ItimeOutLog = LogWait * 1000;
                ConnectNext();
            };
        }

        private void Rdp_OnReady(RdpClient sender)
        {
            if (Hosts.Count == 0) Message(sender, ResponseType.Finished);
            else
                ConnectNext();
        }

        private void ConnectNext()
        {
            if (Hosts.Count != 0)
            {
                RDP.Connect(Hosts[0].Item1, Hosts[0].Item2, Hosts[0].Item3);
                Hosts.RemoveAt(0);
            }
        }

        private void UpdateInfo()
        {
            Text = string.Format("RDPAPP (Goods - {0}, Left - {1})", Logined, Hosts.Count);
        }
        private string lastSaved;
        private void Rdp_OnResponse(RdpClient sender, ResponseType response)
        {
            Message(sender, response);
            SaveHost(sender, response);
        }

        private void SaveHost(RdpClient sender, ResponseType response)
        {
            if (lastSaved == sender.Server) return;
            if (response == ResponseType.LoggedIn)
            {
                lastSaved = sender.Server;
                using (StreamWriter Writer = File.AppendText(string.Format("Output/{0}_$goodresult.txt", Path.GetFileNameWithoutExtension(Filename))))
                {
                    Writer.WriteAsync(string.Format("\n{0}:3389@{1};{2}", sender.Server, sender.UserName, sender.Password));
                    Writer.Flush();
                }
            }
        }

        private void LoadHosts()
        {
            foreach (string Hash in HashList)
            {
                string IP = Hash.Split(new char[] { ':' })[0].Trim();
                string Log = Hash.Split(new char[] { '@' })[1].Split(new char[] { ';' })[0].Trim();
                string Pass = Hash.Split(new char[] { '@' })[1].Split(new char[] { ';' })[1].Trim();
                Hosts.Add(new Tuple<string, string, string>(IP, Log, Pass));
            }
        }

        private void RDPAPP_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to stop this process?", "RDPAPP", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                e.Cancel = true;
        }

        private void ClearConsole_Click(object sender, EventArgs e) =>
            RDPConsole.Clear();

        private void Message(RdpClient client, ResponseType type)
        {
            Color color = Color.WhiteSmoke;
            string text = string.Format("[{0}] [{1}, {3} & {4}] {2}" + Environment.NewLine,
                DateTime.Now.ToString("h:mm:ss"), client.Server, type, client.UserName, client.Password);

            if (Settings.Config["rdpapp_color"].Item1.ToBool())
            {
                switch (type)
                {
                    case ResponseType.Connecting:
                        color = Color.DodgerBlue;
                        break;
                    case ResponseType.Connected:
                        color = Color.Gold;
                        break;
                    case ResponseType.LoggedIn:
                        Logined++;
                        color = Color.LawnGreen;
                        break;
                    case ResponseType.Disconnected:
                        color = Color.Red;
                        break;
                    case ResponseType.Error:
                        color = Color.Red;
                        break;
                    case ResponseType.LoginFailed:
                        color = Color.Orange;
                        break;
                    case ResponseType.Finished:
                        color = Color.WhiteSmoke;
                        break;
                    case ResponseType.TimedOut:
                        color = Color.Red;
                        break;
                }
            }

            UpdateInfo();
            RDPConsole.Focus();
            int length = RDPConsole.TextLength;

            RDPConsole.SelectionStart = length;
            RDPConsole.SelectionColor = color;
            RDPConsole.AppendText(text);

            RDPConsole.SelectionStart = RDPConsole.Text.Length;
            RDPConsole.ScrollToCaret();

            if (Settings.Config["rdpapp_auto_clear"].Item1.ToBool() && RDPConsole.Lines.Length >= Settings.Config["rdpapp_max_lines"].Item1.ToInt())
                RDPConsole.Clear();
        }
    }
}
