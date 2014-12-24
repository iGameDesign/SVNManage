using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SVNConf
{
	class Program
	{
		static void Main(string[] args)
		{
			StreamReader objReader = new StreamReader("authz", UnicodeEncoding. GetEncoding("UTF-8"));
			Console.WriteLine(objReader.ReadToEnd());
			return;
			string sLine = "";
			while (sLine != null)
			{
				sLine = objReader.ReadLine();
				if (sLine != null)
				{
					Console.WriteLine(sLine);
				}
			}
		}
	}
}
