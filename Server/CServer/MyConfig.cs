namespace CServer
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    internal class MyConfig
    {
        private Dictionary<string, string> m_keyvalues = new Dictionary<string, string>();

        public string GetValue(string key)
        {
            return this.m_keyvalues[key];
        }

        public void ReadFile(string confFile)
        {
            string str;
            StreamReader reader = File.OpenText(confFile);
            while ((str = reader.ReadLine()) != null)
            {
                str = str.Trim();
                if (str[0] != '#')
                {
                    string[] strArray = str.Split(new char[] { '=' });
                    if (strArray.Length == 2)
                    {
                        this.m_keyvalues.Add(strArray[0].Trim(), strArray[1].Trim());
                    }
                }
            }
        }
    }
}

