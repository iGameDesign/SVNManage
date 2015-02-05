namespace CServer
{
    using SvnRightsManager;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Data;
    using Mono.Data.Sqlite;

    internal class SvnRightsMgr
    {
        private ArrayList m_aLines = new ArrayList();
        private MyConfig m_conf = new MyConfig();
        private int m_nAfterLockVer = -1;
        private int m_nAfterUnlockVer = -1;
        private int m_nVer = 0;
        private string m_strAuthFile = "";
        private string m_strDest;
        private string m_strHookDir;
        private string m_strLogDir;
		private string m_strLink;
        private string m_strReposPath;
        private string m_strRet = "";

		private string m_baseurl;
		public string m_svndb;
		private string m_svnpath;
        public string m_strToken;
		private string m_autobranch;

		private string m_strUrlDest;

        private string m_strAuthDB = "";
	
        public SvnRightsMgr()
        {
            string[] commandLineArgs = Environment.GetCommandLineArgs();
            if (commandLineArgs.Length != 2)
            {
                throw new Exception("启动参数非法，需指定一个配置文件，包含内容：\r\n库根路径，\r\n网址(https://127.0.0.1/)\r\n参考权限路径(baobaotang:/bbt)\r\n目标权限根路径(baobaotang:/branches/autobranch/)\r\n日志查看目录");
            }
            this.ReadConfig(commandLineArgs[1]);
        }

        private ArrayList AddLines(ArrayList aFullLines, string strSrcPath, string strDestPath)
        {
            ArrayList list = new ArrayList();
            for (int i = 0; i < aFullLines.Count; i++)
            {
                if ((aFullLines[i] != null) && ((aFullLines[i] as string[])[0].IndexOf(strSrcPath) == 0))
                {
                    int num2 = list.Add((aFullLines[i] as string[]).Clone());
                    (list[num2] as string[])[0] = (list[num2] as string[])[0].Replace(strSrcPath, strDestPath);
                }
            }
            return list;
        }

        private void AddTreeNode(string[] strSectionLines, ref ArrayList aLines)
        {
            if (strSectionLines.Length != 0)
            {
                aLines.Add(strSectionLines);
            }
        }

        private bool CopyRights(string strRightsFile, string srcRights, string strDestRights)
        {
			CommandDo.Execute("svn", string.Format("up {0}", strRightsFile));
            this.InitRights(this.GetFileText(strRightsFile));
            string strSrcPath = srcRights;
            string strDestPath = strDestRights;
            ArrayList aLines = this.AddLines(this.m_aLines, strSrcPath, strDestPath);
            if (strRightsFile.Length > 0)
            {
                this.DelRights(this.m_aLines, strDestPath);
                string path = strRightsFile;
                string destFileName = string.Concat(new object[] { Path.GetDirectoryName(path), Path.DirectorySeparatorChar, Path.GetFileNameWithoutExtension(path), "_", DateTime.Now.Ticks, ".bak" });
                File.Copy(path, destFileName);
                this.WriteFileText(strRightsFile, this.m_aLines, true);
                this.WriteFileText(strRightsFile, aLines, false);
				CommandDo.Execute("svn", string.Format("up {0}", this.m_strAuthFile));
				CommandDo.Execute("svn", string.Format("ci -m 'CServer auto' {0}", this.m_strAuthFile));
                return true;
            }
            return false;
        }

        // 新版的权限是用sqlit管理，需要复制数据库里面的记录
        public bool CopyRights_sql(string strproject, string srcsrcbranch, string strdstbranch)
        {
            // log(string.format("project:{0}; srcbranch:{1}; dstbranch:{2}", strproject, srcsrcbranch, strdstbranch), consolecolor.black);
            bool bResult = false;

            try
            {
                string dbpath = "data source=" + this.m_strAuthDB;
                //string dbpath = "data source=./submin.db";
                List<AuthDB> authlist = new List<AuthDB>();
                SqliteConnection conn = null;
                conn = new SqliteConnection(dbpath);
                conn.Open();

                string sql = "select * from permissions where repository = '" + strproject + "'";
                SqliteCommand cmd = new SqliteCommand(sql, conn);
                SqliteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (reader.GetString(2).StartsWith(srcsrcbranch))
                    {
                        AuthDB auth = new AuthDB();
                        auth.repository = reader.GetString(0);
                        auth.repositorytype = reader.GetString(1);
                        auth.path = reader.GetString(2).Replace(srcsrcbranch, strdstbranch); // modify
                        auth.subjecttype = reader.GetString(3);
                        try
                        {
                            auth.subjectid = reader.GetInt32(4);
                        }
                        catch(Exception e)
                        {
                            log(string.Format("[CopyRights_sql]Exception: {0}{1}", e.Message, e.StackTrace), ConsoleColor.Black);
                            log(string.Format("[CopyRights_sql]Info: {0}", "此条数据特殊，权限为空，不插入"), ConsoleColor.Black);
                            continue;
                        }
                        
                        auth.type = reader.GetString(5);
                        log(string.Format("auth.path:{0}", auth.path), ConsoleColor.Black);

                        authlist.Add(auth);
                    }
                }

                SqliteTransaction trans = conn.BeginTransaction();
                cmd = new SqliteCommand(conn);
                cmd.Transaction = trans;
                cmd.CommandText = "insert into permissions values(@repository, @repositorytype, @path, @subjecttype, @subjectid, @type)";
                foreach (AuthDB item in authlist)
                {
                    cmd.Parameters.AddRange(new SqliteParameter[] {
                        new SqliteParameter("@repository", item.repository),
                        new SqliteParameter("@repositorytype", item.repositorytype),
                        new SqliteParameter("@path", item.path),
                        new SqliteParameter("@subjecttype", item.subjecttype),
                        new SqliteParameter("@subjectid", item.subjectid),
                        new SqliteParameter("@type", item.type)
                    });
                    cmd.ExecuteNonQuery();
                }
                trans.Commit();

                conn.Close();
                bResult = true;
            }
            catch (Exception e)
            {
                log(string.Format("[CopyRights_sql]Exception: {0}{1}", e.Message, e.StackTrace), ConsoleColor.Black);
            }

            return bResult;
        }

//         public bool CopyRights_sql(string strProject, string srcSrcBranch, string strDstBranch)
//         {
//             log(string.Format("Project:{0}; SrcBranch:{1}; DstBranch:{2}", strProject, srcSrcBranch, strDstBranch), ConsoleColor.Black);
//             bool bResult = false;
// 
//             try
//             {
//                 string dbPath = "Data Source=" + this.m_strAuthDB;
// 
//                 log(string.Format("dbPath: {0}", dbPath), ConsoleColor.Black);
//                 //string dbPath = @"Data Source=submin.db";
//                 List<AuthDB> authList = new List<AuthDB>();
//                 SQLiteConnection conn = null;
//                 
//                 SQLiteConnectionStringBuilder connstr = new SQLiteConnectionStringBuilder();
//                 connstr.DataSource = this.m_strAuthDB;
//                 conn.ConnectionString = connstr.ToString();
//                 conn.Open();
//                 
//                 // read
//                 string sql = "select * from permissions where repository = '" + strProject + "'";
//                 SQLiteCommand cmd = new SQLiteCommand(sql, conn);
//                 SQLiteDataReader reader = cmd.ExecuteReader();
//                 while (reader.Read())
//                 {
//                     if (reader.GetString(2).StartsWith(srcSrcBranch))
//                     {
//                         AuthDB auth = new AuthDB();
//                         auth.repository = reader.GetString(0);
//                         auth.repositorytype = reader.GetString(1);
//                         auth.path = reader.GetString(2).Replace(srcSrcBranch, strDstBranch); // modify
//                         auth.subjecttype = reader.GetString(3);
//                         auth.subjectid = reader.GetInt32(4);
//                         auth.type = reader.GetString(5);
//                         log(string.Format("auth.path:{0}", auth.path), ConsoleColor.Black);
// 
//                         authList.Add(auth);
//                         Console.WriteLine(reader.GetString(0) + " " + reader.GetString(2));
//                         Console.WriteLine(auth.repository + " " + auth.path);
//                     }
//                 }
// 
//                 // write
//                 SQLiteTransaction trans = conn.BeginTransaction();
// 
//                 foreach (AuthDB item in authList)
//                 {
//                     cmd = new SQLiteCommand(conn);
//                     cmd.Transaction = trans;
//                     cmd.CommandText = "insert into permissions values(@repository, @repositorytype, @path, @subjecttype, @subjectid, @type)";
// 
//                     cmd.Parameters.AddRange(new SQLiteParameter[] {
//                     new SQLiteParameter("@repository", item.repository),
//                     new SQLiteParameter("@repositorytype", item.repositorytype),
//                     new SQLiteParameter("@path", item.path),
//                     new SQLiteParameter("@subjecttype", item.subjecttype),
//                     new SQLiteParameter("@subjectid", item.subjectid),
//                     new SQLiteParameter("@type", item.type)
//                 });
//                     cmd.ExecuteNonQuery();
//                 }
//                 trans.Commit();
// 
//                 conn.Close();
//                 bResult = true;
//             }
//             catch (Exception e)
//             {
//                 log(string.Format("Exception: {0}, {1}", e.Message, e.StackTrace), ConsoleColor.Black);
//             }
// 
//             return bResult;
//         }

        public string CreateRoot(string strIP, string strUserName, string strArg, string strVer, string szProjectName)
		{
			this.m_strRet = "WARNING:no result!";
			string strfrom = string.Format("IP:{0}, User:{1}", strIP, strUserName);
			string strparams = string.Format("cp -m \"create root\" -m \"checker:auto  {0}\" {1} {2}", 
							new object[] { 
								strfrom, 
								this.getStrUrlSrc(strVer),
								this.getStrUrlSrc(strArg)});
			Program.log(strfrom, ConsoleColor.Blue);
			Program.log("svn " + strparams, ConsoleColor.Blue);
            
            //if (svnresult.IndexOf("commit", StringComparison.OrdinalIgnoreCase) > -1)
			{
				// 复制权限文件
                
                //string strSrc = this.getStrSrc(strVer);
                //string strDestRights = this.getStrSrc(strArg);
                //this.CopyRights(this.m_strAuthFile, strSrc, strDestRights);

                // 创建主分支不能copy权限数据，会出问题
                if (!("/" + strArg).StartsWith(strVer))
                {
                    bool bRet = false;
                    bRet = this.CopyRights_sql(szProjectName, strVer, "/" + strArg);
                    if (bRet)
                    {
                        string svnresult = CommandDo.Execute("svn", strparams); //"commit test";// 
                        if (svnresult.IndexOf("commit", StringComparison.OrdinalIgnoreCase) > -1)
                        {
                            //
                            // 添加主分支到列表
                            this.addroot(strArg, szProjectName);
                            this.m_strRet = string.Format("创建分支[{0}]成功！具体地址:\r\n{1}\r\n", strArg, this.getStrUrlSrc(strArg));
                        }
                        else
                        {
                            this.m_strRet = string.Format("创建分支[{0}]失败！原因：\r\n{1}\r\nsvn参数：{2}", this.getStrUrlSrc(strArg), svnresult, strparams);
                        }
                    }
                    else
                    {
                        this.m_strRet = string.Format("WARNING:复制权限失败，有可能是权限管理数据库里面源库不存在或者有重置的目标库。项目：{0}，源库：{1}，目标库：{2}",
                            szProjectName, strVer, "/" + strArg);
                    }
                }
                else
                {
                    this.m_strRet = string.Format("WARNING:创建分支失败，目标库名跟源库名有重复的部分。项目：{0}，源库：{1}，目标库：{2}",
                        szProjectName, strVer, "/" + strArg);
                }
			}
            
			return this.m_strRet;
		}

        private bool CreateSvnBranch(string strIP, string strUserName, string strBranchNameA, string strVer, string szProjectName)
        {
            this.m_strUrlDest = string.Format(m_baseurl + "/{0}/{1}/",
                m_svndb.Trim(new Char[] { '/' }),
                m_autobranch.Trim(new Char[] { '/' }));
            this.m_nVer = this.getVer(this.m_svndb);
            string strprefix = string.Format("{0}a{1:0000}_", this.m_strUrlDest, this.m_nVer);
            string pureBranchName = this.GetPureBranchName(strBranchNameA);
            string strfrom = string.Format("IP:{0}, User:{1}", strIP, strUserName);
            string strparam = string.Format("cp -m \"create branch\" -m \"checker:auto  {0}\" {1} {2}{3}", 
				new object[] { 
					strfrom, 
					this.getStrUrlSrc(strVer), 
					strprefix, 
					pureBranchName });
			Program.log(strfrom, ConsoleColor.Blue);
            Program.log(this.getStrUrlSrc(strVer), ConsoleColor.Blue);
			Program.log(strprefix, ConsoleColor.Blue);
            Program.log(pureBranchName, ConsoleColor.Blue);
			Program.log(strparam, ConsoleColor.Blue);
            string svnresult = CommandDo.Execute("svn", strparam); //"commit test";// 
			this.m_strRet = (svnresult.IndexOf("commit", StringComparison.OrdinalIgnoreCase) > -1)
				? string.Format("创建分支[{0}]成功！具体地址:\r\n{1}\r\n", pureBranchName, strprefix + pureBranchName)
				: string.Format("创建分支[{0}]失败！原因：\r\n{1}\r\nsvn参数：{2}", pureBranchName, svnresult, strparam);
			return (svnresult.IndexOf("commit", StringComparison.OrdinalIgnoreCase) > -1);
        }

        private void DelRights(ArrayList aLines, string strDelDir)
        {
            for (int i = 0; i < aLines.Count; i++)
            {
                string[] strArray = aLines[i] as string[];
                if (((strArray != null) && (strArray.Length > 0)) && (strArray[0].IndexOf(strDelDir) == 0))
                {
                    aLines[i] = null;
                }
            }
        }

        public string getAuthor(int nVersion)
        {
            this.m_strReposPath = string.Format("{0}/{1}", m_svnpath, m_svndb);
            return CommandDo.Execute("svnlook", string.Format("author {0} -r {1}", this.m_strReposPath, nVersion));
        }

        public string getChanged(int nVersion)
        {
            this.m_strReposPath = string.Format("{0}/{1}", m_svnpath, m_svndb);
            return CommandDo.Execute("svnlook", string.Format("changed {0} -r {1}", this.m_strReposPath, nVersion));
        }

        public string getRoots(string szProjectName)
		{
            if (szProjectName == "")
                return "";
            string rootsfile = szProjectName + "/trunkroots.conf";
			string trunks = GetRootsText(rootsfile);
			return trunks;
		}

		public void addroot(string rootname, string szProjectName)
		{
            if (szProjectName == "")
                return ;

            string rootFilePath = Directory.GetCurrentDirectory() + "/" + szProjectName + "/trunkroots.conf";
            int nMainBranchMaxID = getMainBranchMaxID(rootFilePath);
            using (StreamWriter writer = new StreamWriter(rootFilePath, true))
			{
				writer.WriteLine(Convert.ToString(nMainBranchMaxID + 1) + "\t" + "/" + rootname);
			}
		}

        /// <summary>
        /// 获取最大的主分支ID
        /// </summary>
        /// <param name="strFile"></param>
        /// <returns></returns>
        private int getMainBranchMaxID(string strFile)
        {
            int nRresult = 0;
            string str;
            StreamReader reader = File.OpenText(strFile);
            while ((str = reader.ReadLine()) != null)
            {
                nRresult = Convert.ToInt32(str.Split('\t')[0]);
            }
            reader.Close();

            return nRresult;
        }

		private string GetRootsText(string strFile)
		{
			string str;
			StreamReader reader = File.OpenText(strFile);
			StringBuilder builder = new StringBuilder();
			while ((str = reader.ReadLine()) != null)
            //if ((str = reader.ReadToEnd()) != "")
			{
				//builder.Append(str + "\t");
                builder.Append(str + "\r\n");
			}
			reader.Close();
			return builder.ToString();
		}

        private string GetFileText(string strFile)
        {
            string str;
            StreamReader reader = File.OpenText(strFile);
            StringBuilder builder = new StringBuilder();
            while ((str = reader.ReadLine()) != null)
            {
                builder.Append(str + "\r\n");
            }
            reader.Close();
            return builder.ToString();
        }

        public string getLastVersion()
        {
            this.m_strReposPath = string.Format("{0}/{1}", m_svnpath, m_svndb);
            return CommandDo.Execute("svnlook", string.Format("youngest {0}", this.m_strReposPath));
        }

        public bool getLockStatu(string szProjectName)
        {
            bool flag = false;
            if (szProjectName == "")
                return flag;

            try
            {
                using (StreamReader reader = new StreamReader(Directory.GetCurrentDirectory() + "/" + szProjectName + "/lockstatu.txt"))
                {
                    flag = int.Parse(reader.ReadLine()) == 1;
                }
            }
            catch (Exception exception)
            {
                Program.log(exception.Message, ConsoleColor.Red);
                Program.log("The lockstatu file could not be read, set lockstatu to 0", ConsoleColor.Red);
                flag = false;
            }
            return flag;
        }

        public string getLockStatus(string szProjectName)
        {
            string flags = "";
            if (szProjectName == "")
                return flags;

            try
            {
                using (StreamReader reader = new StreamReader(Directory.GetCurrentDirectory() + "/" + szProjectName + "/lockstatu.txt"))
                {
                    flags = reader.ReadLine();
                }
            }
            catch (Exception exception)
            {
                Program.log(exception.Message, ConsoleColor.Red);
                Program.log("The lockstatu file could not be read, set lockstatus to null string", ConsoleColor.Red);
                flags = "";
            }
            return flags;
        }

        public void setLockStatus(string flags, string szProjectName)
        {
            if (szProjectName == "")
                return;

            try
            {
                if (flags != "")
                {
                    using (StreamWriter writer = new StreamWriter(Directory.GetCurrentDirectory() + "/" + szProjectName + "/lockstatu.txt"))
                    {
                        writer.WriteLine(flags);
                    }
                }
            }
            catch (Exception exception)
            {
                Program.log(exception.Message, ConsoleColor.Red);
                Program.log("The lockstatu file could not be read, set lockstatus to null string", ConsoleColor.Red);
            }
            return;
        }

        private string GetPureBranchName(string strBranchNameA)
        {
            string str = strBranchNameA;
            return str.Replace(" ", "").Replace("/", "").Replace("&", "").Replace("|", "").Replace("<", "").Replace(">", "");
        }

        private string GetPurePath(string strDir)
        {
            if ((strDir.Length > 0) && (strDir.LastIndexOf('/') == (strDir.Length - 1)))
            {
                return strDir.Substring(0, strDir.Length - 1);
            }
            if ((strDir.Length > 0) && (strDir.LastIndexOf('\\') == (strDir.Length - 1)))
            {
                return strDir.Substring(0, strDir.Length - 1);
            }
            return strDir;
        }

        private string GetPurePathEx(string strDirArg)
        {
            string str = strDirArg.Replace("\"", "");
            if ((str.Length > 0) && (str.LastIndexOf('/') == (str.Length - 1)))
            {
                return str.Substring(0, str.Length - 1);
            }
            if ((str.Length > 0) && (str.LastIndexOf('\\') == (str.Length - 1)))
            {
                return str.Substring(0, str.Length - 1);
            }
            return str;
        }

        public string getRightsBranches(string szProjectName)
        {
			CommandDo.Execute("svn", string.Format("up {0}", this.m_strAuthFile));
			this.InitRights(this.GetFileText(this.m_strAuthFile));
            //string str = "gunsoul:/branches/autobranch/";
            string str = szProjectName + ":/branches/autobranch/";
            ArrayList list = new ArrayList();
            Hashtable hashtable = new Hashtable();
            string key = "";
            string str3 = "";
            for (int i = 0; i < this.m_aLines.Count; i++)
            {
                string[] strArray = this.m_aLines[i] as string[];
                string str4 = strArray[0];
                if (str4.StartsWith(str))
                {
                    key = str4.Substring(str.Length);
                    if (key.IndexOf("/") > 0)
                    {
                        key = key.Substring(0, key.IndexOf("/"));
                    }
                    if (!(hashtable.ContainsKey(key) || key.EndsWith("_isbc", StringComparison.CurrentCultureIgnoreCase)))
                    {
                        hashtable[key] = 1;
                        list.Add(key);
                        str3 = str3 + key + "\r\n";
                    }
                }
            }
            return str3;
        }

        private string[] GetSectionLines(string strSection)
        {
            string[] strArray = strSection.Split(new string[] { "]\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            ArrayList list = new ArrayList();
            foreach (string str in strArray)
            {
                if (str != "\r\n")
                {
                    list.Add(str);
                }
            }
            string[] strArray2 = new string[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                strArray2[i] = list[i].ToString();
            }
            return strArray2;
        }

        private void GetTree(string s, ref ArrayList aLines)
        {
            string str = "";
            string[] strArray = s.Split(new string[] { "[" }, StringSplitOptions.RemoveEmptyEntries);
            int num = 1;
            foreach (string str2 in strArray)
            {
                if (str2.Length > 0)
                {
                    str = str + string.Format("{0}{1}\r\n", num, str2);
                    num++;
                    string[] sectionLines = this.GetSectionLines(str2);
                    this.AddTreeNode(sectionLines, ref aLines);
                }
            }
        }

        public int getunlockVer(string szProjectName)
        {
            int num = 0;
            if (szProjectName == "")
                return num;

            try
            {
                using (StreamReader reader = new StreamReader(Directory.GetCurrentDirectory() + "/" + szProjectName + "/unlockver.txt"))
                {
                    num = int.Parse(reader.ReadLine());
                }
            }
            catch (Exception exception)
            {
                Program.log(exception.Message, ConsoleColor.Red);
                Program.log("The unlock version file could not be read, set unlock version to -1", ConsoleColor.Red);
                num = -1;
            }
            return num;
        }

        private int getVer(string szProjectName)
        {
            int num = 0;
            if (szProjectName == "")
                return num;

            try
            {
                using (StreamReader reader = new StreamReader(Directory.GetCurrentDirectory() + "/" + szProjectName + "/ver.txt"))
                {
                    num = int.Parse(reader.ReadLine());
                }
            }
            catch (Exception exception)
            {
                Program.log(exception.Message, ConsoleColor.Red);
                Program.log("The version file could not be read, set version to 0", ConsoleColor.Red);
                num = 0;
            }
            return num;
        }

        private void InitRights(string strRights)
        {
            this.m_aLines = new ArrayList();
            this.GetTree(strRights, ref this.m_aLines);
        }

        public bool Lock(bool bLock)
        {
            this.m_strHookDir = string.Format("{0}/{1}/hooks", m_svnpath, m_svndb);
            string purePathEx = this.GetPurePathEx(this.m_strHookDir);
            if (bLock)
            {
            }
            this.SetLockStatu(bLock, this.m_svndb);
            return true;
        }

        public string OperSvn(string strIP, string strUserName, string strArg, string strVer, string szProjectName)
        {
            this.m_strRet = "WARNING:no result!";
            if (this.CreateSvnBranch(strIP, strUserName, strArg, strVer, szProjectName))
            {
                bool bRet = true;
                this.m_strDest = string.Format("{0}:{1}", m_svndb, m_autobranch);
                //this.m_nVer = this.getVer(this.m_svndb) + 1;
                string strSrc = this.getStrSrc(strVer);
                string strDestRights = string.Format(this.m_strDest + "/a{0:0000}_{1}", this.m_nVer, this.GetPureBranchName(strArg));
                //this.CopyRights(this.m_strAuthFile, strSrc, strDestRights);
                bRet = this.CopyRights_sql(szProjectName, strVer, strDestRights.Replace(szProjectName + ":", ""));
                if (bRet == false)
                {
                    this.m_strRet = string.Format("WARNING:创建分支成功，但是复制权限失败，有可能是权限管理数据库里面有重置的字段。项目：{0}，源库：{1}，目标库：{2}",
                        szProjectName, strVer, strDestRights.Replace(szProjectName + ":", ""));
                }
                else
                {
                    this.setVer(++this.m_nVer, szProjectName);
                }
            }
            return this.m_strRet;
        }

		private string getStrSrc(string strVer)
        {
			string result = string.Format("{0}:/{1}", m_svndb, strVer.Trim(new Char[] { '/' }));
			//Program.log(result, ConsoleColor.DarkRed);
			return result;
        }

        private string getStrUrlSrc(string strVer)
        {
			string result = string.Format(m_baseurl + "/{0}/{1}/", 
				m_svndb.Trim(new Char[] { '/' }), 
				strVer.Trim(new Char[] { '/' }));
			//Program.log(result, ConsoleColor.DarkRed);
			return result;
		}

        public void ProcessLockBranch(string strRemoteIP, string strUser, bool bLock, ref string strret)
        {
            Hashtable hashtable = new Hashtable();
            Program.log(string.Format("来自[{0}]的用户[{1}], 请求{2}主分支", strRemoteIP, strUser, bLock ? "锁定" : "解锁"), ConsoleColor.Magenta);
            bool flag = this.Lock(bLock);
            strret = bLock ? "锁定主分支成功，在解锁前将不能再提交主分支代码" : "主分支解锁成功，现在可以提交代码了。";
            Program.log(string.Format(strret, new object[0]), ConsoleColor.Cyan);
            string str = this.getLastVersion();
            Program.log(string.Format("当前主分支的最新版本号为:{0}", str), ConsoleColor.DarkRed);
            if (bLock)
            {
                this.m_nAfterLockVer = int.Parse(str);
            }
            else
            {
                this.m_nAfterUnlockVer = int.Parse(str);
                this.setunlockVer(this.m_nAfterUnlockVer, this.m_svndb);
            }
            if ((((this.m_nAfterLockVer != this.m_nAfterUnlockVer) && bLock) && (this.m_nAfterLockVer > 0)) && (this.m_nAfterUnlockVer > 0))
            {
                hashtable.Clear();
                for (int i = this.m_nAfterUnlockVer + 1; i <= this.m_nAfterLockVer; i++)
                {
                    string key = this.getAuthor(i);
                    string str3 = this.getChanged(i);
                    if (!hashtable.Contains(key))
                    {
                        hashtable.Add(key, str3);
                    }
                    else
                    {
                        hashtable[key] = hashtable[key] + "\r\n" + str3;
                    }
                }
                StringBuilder builder = new StringBuilder(strret);
                try
                {
                    foreach (DictionaryEntry entry in hashtable)
                    {
						string strFileName;
						string strCommiter = entry.Value.ToString();
						string strFileContent = this.sortandpullback(ref strCommiter);
                        if ((this.m_nAfterUnlockVer + 1) == this.m_nAfterLockVer)
                        {
                            strFileName = string.Format("[{0}][{1}].txt", this.m_nAfterLockVer, entry.Key);
                        }
                        else
                        {
                            strFileName = string.Format("[{0}-{1}][{2}].txt", this.m_nAfterUnlockVer, this.m_nAfterLockVer, entry.Key);
                        }
						strFileName = strFileName.Replace("\r\n", "").Trim();
						strFileName = strFileName.Replace("\n", "").Trim();
                        using (StreamWriter writer = new StreamWriter(strFileName))
                        {
							writer.Write(strFileContent);
                        }
                        string destFileName = this.m_strLogDir + "/" + strFileName;
                        File.Copy(strFileName, destFileName, true);
						builder.AppendFormat("\r\n===========================\r\n{0}\r\n{1}{2}\r\n", entry.Key, m_strLink, strFileName);
                    }
                }
                catch (Exception exception)
                {
                    Program.log(exception.Message, ConsoleColor.Red);
                    builder.AppendFormat("\r\n===========================\r\n{0}\r\n", exception.Message);
                }
                strret = builder.ToString();
            }
        }

        public string getUnblockUser(string szProjectName)
        {
            if (szProjectName == "")
                return "";
            string rootsfile = szProjectName + "/unblockuser.txt";
            string str;

            StreamReader reader = File.OpenText(rootsfile);
            StringBuilder builder = new StringBuilder();
            while ((str = reader.ReadLine()) != null)
            //if ((str = reader.ReadToEnd()) != "")
            {
                //builder.Append(str + "\t");
                builder.Append(str + "\r\n");
            }
            reader.Close();
            return builder.ToString();
        }

        public bool setUnblockUser(string szProjectName, string users)
        {
            bool bResult = false;
            if (szProjectName == "")
                return bResult;
            string rootsfile = szProjectName + "/unblockuser.txt";

            using (StreamWriter writer = new StreamWriter(Directory.GetCurrentDirectory() + "/" + szProjectName + "/unblockuser.txt"))
            {
                writer.WriteLine(users);
            }

            bResult = true;
            return bResult;
        }

        // this function is useless, waiting to delete!
        public void ProcessLockBranches(string strRemoteIP, string strUser, bool bLock, ref string strret)
        {
            Hashtable hashtable = new Hashtable();
            Program.log(string.Format("来自[{0}]的用户[{1}], 请求{2}主分支", strRemoteIP, strUser, bLock ? "锁定" : "解锁"), ConsoleColor.Magenta);
            bool flag = this.Lock(bLock);
            strret = bLock ? "锁定主分支成功，在解锁前将不能再提交主分支代码" : "主分支解锁成功，现在可以提交代码了。";
            Program.log(string.Format(strret, new object[0]), ConsoleColor.Cyan);
            string str = this.getLastVersion();
            Program.log(string.Format("当前主分支的最新版本号为:{0}", str), ConsoleColor.DarkRed);
            if (bLock)
            {
                this.m_nAfterLockVer = int.Parse(str);
            }
            else
            {
                this.m_nAfterUnlockVer = int.Parse(str);
                this.setunlockVer(this.m_nAfterUnlockVer, this.m_svndb);
            }
            if ((((this.m_nAfterLockVer != this.m_nAfterUnlockVer) && bLock) && (this.m_nAfterLockVer > 0)) && (this.m_nAfterUnlockVer > 0))
            {
                hashtable.Clear();
                for (int i = this.m_nAfterUnlockVer + 1; i <= this.m_nAfterLockVer; i++)
                {
                    string key = this.getAuthor(i);
                    string str3 = this.getChanged(i);
                    if (!hashtable.Contains(key))
                    {
                        hashtable.Add(key, str3);
                    }
                    else
                    {
                        hashtable[key] = hashtable[key] + "\r\n" + str3;
                    }
                }
                StringBuilder builder = new StringBuilder(strret);
                try
                {
                    foreach (DictionaryEntry entry in hashtable)
                    {
                        string strFileName;
                        string strCommiter = entry.Value.ToString();
                        string strFileContent = this.sortandpullback(ref strCommiter);
                        if ((this.m_nAfterUnlockVer + 1) == this.m_nAfterLockVer)
                        {
                            strFileName = string.Format("[{0}]:[{1}][{2}].txt", this.m_svndb, this.m_nAfterLockVer, entry.Key);
                        }
                        else
                        {
                            strFileName = string.Format("[{0}]:[{1}-{2}][{3}].txt", this.m_svndb, this.m_nAfterUnlockVer, this.m_nAfterLockVer, entry.Key);
                        }
                        strFileName = strFileName.Replace("\r\n", "").Trim();
                        strFileName = strFileName.Replace("\n", "").Trim();
                        using (StreamWriter writer = new StreamWriter(strFileName))
                        {
                            writer.Write(strFileContent);
                        }
                        string destFileName = this.m_strLogDir + "/" + strFileName;
                        File.Copy(strFileName, destFileName, true);
                        builder.AppendFormat("\r\n===========================\r\n{0}\r\n{1}{2}\r\n", entry.Key, m_strLink, strFileName);
                    }
                }
                catch (Exception exception)
                {
                    Program.log(exception.Message, ConsoleColor.Red);
                    builder.AppendFormat("\r\n===========================\r\n{0}\r\n", exception.Message);
                }
                strret = builder.ToString();
            }
        }

        private void ReadConfig(string confName)
        {
            this.m_conf.ReadFile(confName);

			m_baseurl = this.m_conf.GetValue("baseurl");
            m_svnpath = this.m_conf.GetValue("svnpath").ToString();
            //m_svndb = this.m_conf.GetValue("svndb").ToString();
            m_strToken = this.m_conf.GetValue("token");
            m_autobranch = this.m_conf.GetValue("autobranch").ToString();

            this.m_strLogDir = this.m_conf.GetValue("logs").ToString();
			this.m_strLink = this.m_conf.GetValue("linkpre").ToString();
            this.m_strAuthDB = this.m_conf.GetValue("authbd").ToString();
			this.m_strAuthFile = this.m_conf.GetValue("authzfile").ToString();

            //this.m_strReposPath = string.Format("{0}/{1}", m_svnpath, m_svndb);
            //this.m_strDest = string.Format("{0}:{1}", m_svndb, m_autobranch);
            //this.m_strHookDir = string.Format("{0}/{1}/hooks", m_svnpath, m_svndb);

            //this.m_strUrlDest = string.Format(m_baseurl + "/{0}/{1}/",
            //    m_svndb.Trim(new Char[] { '/' }),
            //    m_autobranch.Trim(new Char[] { '/' }));
            //this.m_nVer = this.getVer(this.m_svndb) + 1;
            //this.m_nAfterUnlockVer = this.getunlockVer(this.m_svndb);
        }

        public void SetLockStatu(bool bLock, string szProjectName)
        {
            if (szProjectName == "")
                return ;

            using (StreamWriter writer = new StreamWriter(Directory.GetCurrentDirectory() + "/" + szProjectName + "/lockstatu.txt"))
            {
                writer.WriteLine(bLock ? 1 : 0);
            }
        }

        public void setunlockVer(int nVer, string szProjectName)
        {
            if (szProjectName == "")
                return;

            using (StreamWriter writer = new StreamWriter(Directory.GetCurrentDirectory() + "/" + szProjectName + "/unlockver.txt"))
            {
                writer.WriteLine(nVer);
            }
        }

        private void setVer(int nVer, string szProjectName)
        {
            if (szProjectName == "")
                return;

            using (StreamWriter writer = new StreamWriter(Directory.GetCurrentDirectory() + "/" + szProjectName + "/ver.txt"))
            {
                writer.WriteLine(nVer);
            }
        }

        public string sortandpullback(ref string str)
        {
            int num;
            Hashtable hashtable = new Hashtable();
            string[] items = str.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            string[] keys = new string[items.Length];
            for (num = 0; num < items.Length; num++)
            {
                keys[num] = items[num].Substring(1).Trim();
                if (hashtable.ContainsKey(keys[num]))
                {
                    hashtable[keys[num]] = hashtable[keys[num]] + items[num].Substring(0, 1);
                }
                else
                {
                    hashtable.Add(keys[num], items[num].Substring(0, 1));
                }
            }
            string str2 = "";
            StringBuilder builder = new StringBuilder();
            Array.Sort<string, string>(keys, items);
            for (num = 0; num < keys.Length; num++)
            {
                if (keys[num] != str2)
                {
                    builder.AppendLine(string.Format("{0}\t{1}", hashtable[keys[num]], keys[num]));
                    str2 = keys[num];
                }
            }
            return builder.ToString();
        }

        private void WriteFileText(string strFile, ArrayList aLines, bool bAdd)
        {
            if (bAdd)
            {
                File.WriteAllText(strFile, "");
            }
            StreamWriter writer = File.AppendText(strFile);
            for (int i = 0; i < aLines.Count; i++)
            {
                if (aLines[i] != null)
                {
                    string[] strArray = aLines[i] as string[];
                    for (int j = 0; j < strArray.Length; j++)
                    {
                        if (j == 0)
                        {
                            writer.Write('[');
                            writer.Write(strArray[0]);
                            writer.WriteLine(']');
                        }
                        else
                        {
                            writer.Write(strArray[j]);
                        }
                    }
                }
            }
            writer.Flush();
            writer.Close();
        }

        public static void log(string strlog, ConsoleColor newcolr)
        {
            strlog = string.Format("[{0}] ", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")) + strlog;
            ConsoleColor foregroundColor = Console.ForegroundColor;
            Console.ForegroundColor = newcolr;
            Console.WriteLine(strlog);
            Console.ForegroundColor = foregroundColor;
            StreamWriter writer = new StreamWriter(Directory.GetCurrentDirectory() + "/log/log.txt", true);
            writer.WriteLine(strlog);
            writer.Close();
        }

    }

    class AuthDB
    {
        public AuthDB()
        { }

        public string repository;
        public string repositorytype;
        public string path;
        public string subjecttype;
        public int subjectid;
        public string type;
    }

}
