namespace CServer
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;

    internal class Program
    {
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

        private static void Main(string[] args)
        {
            try
            {
                if (args.Length != 5)
                {
                }
                log("version 1.3", ConsoleColor.White);
                IPAddress localaddr = Dns.Resolve(Environment.MachineName).AddressList[0];
                TcpListener listener = new TcpListener(localaddr, 0x4658);
                listener.Start();
                log("开启端口服务....", ConsoleColor.Green);
                log("本地节点：" + listener.LocalEndpoint, ConsoleColor.Green);
                log("等待连接.....", ConsoleColor.Green);
                SvnRightsMgr mgr = new SvnRightsMgr();
                bool flag = true;
                Socket socket = null;
                ASCIIEncoding encoding = new ASCIIEncoding();
                Encoding unicode = Encoding.Unicode;
                while (flag)
                {
                    try
                    {
                        socket = listener.AcceptSocket();
                        byte[] buffer = new byte[0x400];
                        int num = socket.Receive(buffer);
                        string[] strArray = unicode.GetString(buffer).Trim("\0".ToCharArray()).Split(new string[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                        string format = "Success";

                        if (strArray[0] == "getRightsBranches")
                        {
                            log(string.Format("来自[{0}]的用户[{1}], 请求建立查询未关闭分支", socket.RemoteEndPoint, strArray[1]), ConsoleColor.Magenta);
                            mgr.m_svndb = strArray[2];
                            format = mgr.getRightsBranches(strArray[2]);
                        }
                        // 获取所有处于控制下的主分支
                        else if (strArray[0] == "getroots")
                        {
                            mgr.m_svndb = strArray[1];
                            format = mgr.getRoots(strArray[1]);
                        }
                        else if (strArray[0] == "getlockstatu")
                        {
                            log(string.Format("来自[{0}]的用户[{1}], 请求查询分支锁定状态", socket.RemoteEndPoint, strArray[1]), ConsoleColor.Magenta);
                            mgr.m_svndb = strArray[3];
                            format = mgr.getLockStatu(strArray[3]) ? "1" : "0";
                        }
                        // 获取被锁定的分支ID [Add by pp: 2014-09-18]
                        else if (strArray[0] == "getlockstatus")
                        {
                            log(string.Format("来自[{0}]的用户[{1}], 请求查询分支锁定状态", socket.RemoteEndPoint, strArray[1]), ConsoleColor.Magenta);
                            mgr.m_svndb = strArray[3];
                            format = mgr.getLockStatus(strArray[3]);
                        }
                        else if (strArray[0] == "login")
                        {
                            //bool flag2 = strArray[2] == "5cf0c953776a862fc7b992625fc8c9d";
                            bool flag2 = strArray[2] == mgr.m_strToken;
                            mgr.m_svndb = strArray[3];
                            log(string.Format("来自[{0}]的用户[{1}], 请求登录,　请求密钥为:{2}, 操作的库为{3}", socket.RemoteEndPoint, strArray[1], strArray[2], strArray[3]), ConsoleColor.Magenta);
                            format = flag2 ? "登录成功" : " 登录失败";
                            log(string.Format(format, new object[0]), ConsoleColor.Cyan);
                        }
                        else if (strArray[0] == "lockmasterbranch")
                        {
                            //mgr.m_svndb = strArray[3];
                            mgr.ProcessLockBranch(socket.RemoteEndPoint.ToString(), strArray[2], int.Parse(strArray[2]) == 1, ref format);
                        }
                        // 设置分支的锁定状态 [Add by pp: 2014-09-18]
                        else if (strArray[0] == "lockbranches")
                        {
                            //mgr.ProcessLockBranches(socket.RemoteEndPoint.ToString(), strArray[2], int.Parse(strArray[2]) == 1, ref format);
                            mgr.m_svndb = strArray[3];
                            mgr.setLockStatus(strArray[2], strArray[3]);
                        }
                        else if (strArray[0] == "createroot")
                        {
                            mgr.m_svndb = strArray[5];
                            log(string.Format("来自[{0}]的用户[{1}], 请求建立主分支:{2}, 选择的版本是:{3}", strArray[1], strArray[2], strArray[3], strArray[4]), ConsoleColor.Cyan);
                            format = mgr.CreateRoot(strArray[1], strArray[2], strArray[3], strArray[4], strArray[5]);
                            log(string.Format("建立分支结果为:{0}", format), ConsoleColor.Cyan);
                        }
                        else if (strArray[0] == "createbranch")
                        {
                            mgr.m_svndb = strArray[5];
                            log(string.Format("来自[{0}]的用户[{1}], 请求建立分支:{2}, 选择的版本是:{3}", strArray[1], strArray[2], strArray[3], strArray[4]), ConsoleColor.Cyan);
                            format = mgr.OperSvn(strArray[1], strArray[2], strArray[3], strArray[4], strArray[5]);
                            log(string.Format("建立分支结果为:{0}", format), ConsoleColor.Cyan);
                        }
                        else if (strArray[0] == "getUnblockUser")
                        {
                            log(string.Format("来自[{0}]的用户[{1}], 请求获取锁库白名单", socket.RemoteEndPoint, strArray[1]), ConsoleColor.Magenta);
                            mgr.m_svndb = strArray[2];
                            format = mgr.getUnblockUser(strArray[2]);
                        }
                        else if (strArray[0] == "setUnblockUser")
                        {
                            bool bRet = false;
                            log(string.Format("来自[{0}]的用户[{1}], 请求设置锁库白名单", socket.RemoteEndPoint, strArray[1]), ConsoleColor.Magenta);
                            mgr.m_svndb = strArray[2];
                            if(strArray.Length == 3)
                                bRet = mgr.setUnblockUser(strArray[2], "");
                            else if(strArray.Length == 4)
                                bRet = mgr.setUnblockUser(strArray[2], strArray[3]);
                             
                            format = bRet ? " 设置成功" : " 设置失败";
                        }
                        else
                        {
                            format = "收到未定义的指令，拒绝执行。";
                        }
                        if (format == "")
                            format = "操作成功。";
                        socket.Send(unicode.GetBytes(format));
                    }
                    catch (Exception exception)
                    {
                        log(exception.ToString(), ConsoleColor.Red);
                    }
                }
                socket.Close();
                listener.Stop();
                Console.ReadLine();
            }
            catch (Exception exception2)
            {
                log(string.Format("Error:{0}", exception2.Message), ConsoleColor.Red);
                Console.ReadLine();
            }
        }
    }
}
