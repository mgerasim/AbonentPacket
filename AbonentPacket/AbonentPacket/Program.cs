using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using NLog;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Drawing;

namespace AbonentPacket
{
    static class Program
    {
        public static Logger logger_main = LogManager.GetLogger("main");
        static public void Log(string log)
        {
            logger_main.Debug(log);
        }

        public static void ThreadConvertJpegToPdf()
        {
            while (true)
            {
                try
                {
                    if (System.IO.Directory.Exists(theForm._FolderJpegFiles))
                    {
                        foreach (var dir in Directory.GetDirectories(theForm._FolderJpegFiles, "*", SearchOption.AllDirectories))
                        {
                            var doc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 25, 25, 25, 25);
                            PdfWriter.GetInstance(doc, new FileStream(dir + @".pdf.tmp", FileMode.Create));
                            doc.Open();
                            foreach(var jpeg in Directory.GetFiles(dir,"*", SearchOption.TopDirectoryOnly))
                            {
                                iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(jpeg);
                                if (image.Height > iTextSharp.text.PageSize.A4.Height - 25)
                                {
                                    image.ScaleToFit(iTextSharp.text.PageSize.A4.Width - 25, iTextSharp.text.PageSize.A4.Height - 25);
                                }
                                else if (image.Width > iTextSharp.text.PageSize.A4.Width - 25)
                                {
                                    image.ScaleToFit(iTextSharp.text.PageSize.A4.Width - 25, iTextSharp.text.PageSize.A4.Height - 25);
                                }
                                image.Alignment = iTextSharp.text.Image.ALIGN_MIDDLE; 
                                doc.Add(image);
                            }
                            doc.Close();
                            File.Move(dir + @".pdf.tmp", dir + @".pdf");
                            File.Delete(dir + @".pdf.tmp");
                            Directory.Move(dir, theForm._FolderWorkFiles+@"\\"+Path.GetFileName(dir));
                        }
                    }
                }
                catch (Exception ex)
                {
                    AbonentPacket.Program.logger_main.Debug(ex.Message);
                    AbonentPacket.Program.logger_main.Debug(ex.StackTrace);
                }
                Thread.Sleep(100); // 10 sec
            }        
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
        public static Thread theThreadConvert;
        
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
            theThreadConvert = new Thread(new ThreadStart(ThreadConvertJpegToPdf));
            Application.Run(theForm);
            theThreadSender.Abort();
            theThreadConvert.Abort();
        }
    }
}
