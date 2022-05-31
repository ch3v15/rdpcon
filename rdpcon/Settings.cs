using System;
using System.IO;
using System.Collections.Generic;
namespace rdpcon
{
    public static class Settings
    {
        public static Dictionary<string, Tuple<string, string>> Config = new Dictionary<string, Tuple<string, string>>();
        public static void Load()
        {
            string[] Params = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "/rdpapp.config");
            foreach (string Param in Params)
            {
                if (!string.IsNullOrEmpty(Param))
                {
                    string CommandParam = Param.Split(new char[] { '=' }, 2)[0].Trim();
                    string ArgumentParam = Param.Split(new char[] { '=' }, 2)[1].Split(new char[] { ';' }, 2)[0].Trim();
                    string Comment = Param.Split(new char[] { '=' }, 2)[1].Split(new char[] { ';' }, 2)[1].Trim();
                    Config.Add(CommandParam, new Tuple<string, string>(ArgumentParam, Comment));
                }
            }
        }

        public static int ToInt(this string STR) =>
            int.Parse(STR);
        public static bool ToBool(this string STR) =>
            bool.Parse(STR);
    }
}