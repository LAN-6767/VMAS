using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMAutoStart
{
    internal class VMC
    {
        public delegate void Loadvminfo();
        public static ConfigBase[] GetConfig()
        {
            List<ConfigBase> config = new List<ConfigBase>();
            String vmrunlistresult = Runcmdresult.Runcmd("vmrun list");
            String[] strs = vmrunlistresult.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
            for (int i = 4; i < strs.Length; i++)
            {
                config.Add(new ConfigBase()
                {
                    Path = strs[i],
                    IsAutorun = false,
                    Isrun = true
                });
            }
            return config.ToArray();
        }
        public static String VmrunType3(String operation, String vmpath, Loadvminfo loadvminfo)
        {
            String cmdtip = "vmrun " + operation + " \"" + vmpath + "\" nogui";
            new Thread(() =>
            {
                Runcmdresult.Runcmd(cmdtip);
                loadvminfo();

            }).Start();
            return cmdtip;
        }
        public static void Startvm(String vmpath)
        {
            Runcmdresult.Runcmd("vmrun start " + vmpath + " nogui");
        }
        public static void Stopvm(String vmpath)
        {
            Runcmdresult.Runcmd("vmrun stop " + vmpath + " nogui");
        }
        public static void Resetvm(String vmpath)
        {
            Runcmdresult.Runcmd("vmrun reset " + vmpath + " nogui");
        }
        public static bool CheckVmrun()
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.Arguments = "/c where vmrun"; // 使用 "where" 命令来检查 vmrun 命令是否存在
            p.Start();
            p.WaitForExit();
            return p.ExitCode == 0;
        }
    }
}
