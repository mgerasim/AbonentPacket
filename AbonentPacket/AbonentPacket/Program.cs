using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using NLog;

namespace AbonentPacket
{
    static class Program
    {
        public static Logger logger_main = LogManager.GetLogger("main");
        static public void Log(string log)
        {
            logger_main.Debug(log);
        }

        public static void ThreadSendFiles()
        {
            while (true) {
                try
                {
                    if (System.IO.Directory.Exists(theForm._FolderJpegFiles))
                    {
                        foreach (var file in Directory.GetFiles(theForm._FolderJpegFiles, "*.ini", SearchOption.TopDirectoryOnly))
                        {
                            if (File.Exists(file.Replace(".ini", ".run")))
                            {
                                continue;
                            }
                            if (File.Exists(file.Replace(".ini", ".tmp")))
                            {
                                continue;
                            }
                            AMS.Profile.Ini Init = new AMS.Profile.Ini(file);
                            Document doc = new Document(Init, theForm._FolderJpegFiles);
                            FileStream fs1 = File.Create(file.Replace(".ini", ".run"));
                            fs1.Dispose();
                            if (doc.SendToArchive() == 200)
                            {
                                FileStream fs = File.Create(file.Replace(".ini", ".200"));
                                fs.Dispose();
                                File.Move(theForm._FolderJpegFiles + "\\" + doc.account + ".pdf", theForm._FolderWorkFiles + "\\" + doc.account + "-" + DateTime.Now.ToString("yyyyMMddHHmm") + ".pdf");
                                foreach (var del in Directory.GetFiles(theForm._FolderJpegFiles, doc.account + ".*", SearchOption.TopDirectoryOnly))
                                {
                                    File.Delete(del);
                                }
                            }
                            else
                            {
                                FileStream fs = File.Create(file.Replace(".ini", ".err"));
                                fs.Dispose();
                            }
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    AbonentPacket.Program.logger_main.Debug(ex.Message);
                    AbonentPacket.Program.logger_main.Debug(ex.StackTrace);
                }

                Thread.Sleep(1000); // 10 sec
            }
        
        }

        public static FormAbonentPacket theForm;
        public static Thread theThreadSender;
        
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            theForm = new FormAbonentPacket();
            theThreadSender = new Thread(new ThreadStart(ThreadSendFiles));
            theThreadSender.Start();                        
            Application.Run(theForm);
            theThreadSender.Abort();
        }
    }
}
