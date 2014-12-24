namespace SvnRightsManager
{
    using System;
    using System.Diagnostics;

    public class CommandDo
    {
        public static string Execute(string command, string cmdargs)
        {
            return Execute(command, cmdargs, 0x1770);
        }

        public static string Execute(string command, string cmdargs, int milliseconds)
        {
            string message = "";
            if ((command != null) && (command != ""))
            {
                Process process = new Process();
                ProcessStartInfo info = new ProcessStartInfo();
                info.FileName = command;
                info.Arguments = cmdargs;
                info.UseShellExecute = false;
                info.RedirectStandardInput = false;
                info.RedirectStandardOutput = true;
                info.CreateNoWindow = true;
                info.RedirectStandardError = true;
                process.StartInfo = info;
                try
                {
                    if (!process.Start())
                    {
                        return message;
                    }
                    bool flag = true;
                    if (milliseconds == 0)
                    {
                        process.WaitForExit();
                    }
                    else
                    {
                        flag = process.WaitForExit(milliseconds);
                    }
                    if (flag)
                    {
                        message = process.StandardOutput.ReadToEnd();
                        if (message.Length == 0)
                        {
                            message = process.StandardError.ReadToEnd();
                        }
                        return message;
                    }
                    process.CloseMainWindow();
                    process.Kill();
                    process.Close();
                    process.Dispose();
                    return "Process Dead Lock, wait time out.";
                }
                catch (Exception exception)
                {
                    message = exception.Message;
                }
                finally
                {
                    if (process != null)
                    {
                        process.Close();
                    }
                }
            }
            return message;
        }
    }
}

