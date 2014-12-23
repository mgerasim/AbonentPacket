using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.Specialized;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Data;
using System.Text.RegularExpressions;
using iTextSharp.text.pdf;
using iTextSharp.text.xml;

namespace AbonentPacket
{
    class Document :  Object
    {
        public string inn;
        public string account;
        public string phone;
        public string korobka;
        public string name;
        public int typedoc;
        protected string Path;
        public string pagecount;

        public Document(string _account, string _korobka, string Path)
        {
            this.account = _account;
            this.inn = "";
            this.phone = "";
            this.korobka = _korobka;
            this.name = "";
            this.typedoc = 3; // Договор - по умолчанию
            this.pagecount = "0";
            this.Path = Path;
        }

        public Document(AMS.Profile.Ini Init, string Path)
        {
            this.Path = Path;
            try
            {
                this.account    = Init.GetValue("ROSTELECOM", "account",    this.account);
                this.phone      = Init.GetValue("ROSTELECOM", "phone",      this.phone);
                this.inn        = Init.GetValue("ROSTELECOM", "inn",        this.inn);
                this.korobka    = Init.GetValue("ROSTELECOM", "korobka",    this.korobka);
                this.name       = Init.GetValue("ROSTELECOM", "name",       this.name);
                this.typedoc    = Init.GetValue("ROSTELECOM", "typedoc",    this.typedoc);
                this.pagecount  = Init.GetValue("ROSTELECOM", "pagecount", this.pagecount);
            }
            catch
            {
            }
        }

        public override string ToString()
        {
            return this.account;
        }

        public void IniWrite()
        {
            string file = Path + "\\"+account+".ini";

            FileStream tmp = File.Create(file.Replace(".ini", ".tmp"));

            FileStream ini;
            if (!File.Exists(file))
            {
                ini = File.Create(file);
                ini.Dispose();
            }

            AMS.Profile.Ini Init = new AMS.Profile.Ini(file);
            try
            {
                Init.SetValue("ROSTELECOM", "account", this.account);
                Init.SetValue("ROSTELECOM", "phone", this.phone);
                Init.SetValue("ROSTELECOM", "inn", this.inn);
                Init.SetValue("ROSTELECOM", "korobka", this.korobka);
                Init.SetValue("ROSTELECOM", "name", this.name);
                Init.SetValue("ROSTELECOM", "typedoc", this.typedoc);
                Init.SetValue("ROSTELECOM", "pagecount", this.pagecount);
            }
            catch
            {
            }

            tmp.Dispose();
            File.Delete(file.Replace(".ini", ".tmp"));
        }

        public void IniRead()
        {
            string file = Path + "\\" + account + ".ini";

            if (File.Exists(file))
            {
                AMS.Profile.Ini Init = new AMS.Profile.Ini(file);
                try
                {
                    this.account = Init.GetValue("ROSTELECOM", "account", this.account);
                    this.phone = Init.GetValue("ROSTELECOM", "phone", this.phone);
                    this.inn = Init.GetValue("ROSTELECOM", "inn", this.inn);
                    this.korobka = Init.GetValue("ROSTELECOM", "korobka", this.korobka);
                    this.name = Init.GetValue("ROSTELECOM", "name", this.name);
                    this.typedoc = Init.GetValue("ROSTELECOM", "typedoc", this.typedoc);
                    this.pagecount = Init.GetValue("ROSTELECOM", "pagecount", this.pagecount);
                }
                catch
                {
                }
            }                
        }

        public int GetPageCountPDF()
        {
            int res = 0;
            try
            {
                string ppath = Path + "\\" + this.account + ".pdf";

                PdfReader pdfReader = new PdfReader(ppath);
                this.pagecount = pdfReader.NumberOfPages.ToString();
                pdfReader.Dispose();
                pdfReader.Close();
                pdfReader = null;
            }
            catch (Exception ex)
            {
                res = 1;
                AbonentPacket.Program.logger_main.Error("Document: GetPageCountPDF: " + ex.Message);
                AbonentPacket.Program.logger_main.Error("Document: GetPageCountPDF: " + ex.StackTrace);
                throw ex;
            }
             
               // this.pagecount = this.getNumberOfPdfPages(ppath).ToString();
            return res;
        }

        public int getNumberOfPdfPages(string fileName)
        {
            using (StreamReader sr = new StreamReader(File.OpenRead(fileName)))
            {
                Regex regex = new Regex(@"/Type\s*/Page[^s]");
                MatchCollection matches = regex.Matches(sr.ReadToEnd());

                return matches.Count;
            }
        }


        public int GetInfoFromSTART_inn()
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;

            AbonentPacket.Program.logger_main.Error("Document: GetInfoFromSTART: Begin");
            int res = 0;
            try
            {

                string sURL = "";
                if (AbonentPacket.Program.theForm._DepartID == 2)
                {
                    sURL = "http://10.200.2.85/info/inn?account=" + this.account;

                }
                AbonentPacket.Program.logger_main.Debug("Document: GetInfoFromSTART: ConnectionString: " + sURL);
                string sBody;

                request = (HttpWebRequest)WebRequest.Create(sURL);
                AbonentPacket.Program.Log("Document: GetInfoFromSTART: Create");

                request.UseDefaultCredentials = true;
                request.PreAuthenticate = true;
                request.Credentials = CredentialCache.DefaultCredentials;

                response = (HttpWebResponse)request.GetResponse();
                AbonentPacket.Program.Log("Document: GetInfoFromSTART:: HttpWebResponse: Get");

                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    sBody = reader.ReadToEnd();
                    sBody = sBody.Replace("     ", "");
                    sBody = sBody.Replace("    ", "");
                    sBody = sBody.Replace("   ", "");
                    sBody = sBody.Replace("\n", "");
                    sBody = sBody.Trim();
                }

                AbonentPacket.Program.Log("Document: GetInfoFromSTART:: HttpWebResponse: Body: " + sBody);

                this.inn = sBody;
                
            }
            catch (Exception ex)
            {
                res = 1;
                AbonentPacket.Program.logger_main.Error("Document: GetInfoFromXML: " + ex.Message);
                AbonentPacket.Program.logger_main.Error("Document: GetInfoFromXML: " + ex.StackTrace);
                throw ex;
            }
            finally
            {
            }
            AbonentPacket.Program.logger_main.Error("Document: GetInfoFromSTART: End");    
            return res;
        }

        public int GetInfoFromSTART_phone()
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;

            AbonentPacket.Program.logger_main.Error("Document: GetInfoFromSTART_phone: Begin");
            int res = 0;
            try
            {

                string sURL = "";
                if (AbonentPacket.Program.theForm._DepartID == 2)
                {
                    sURL = "http://10.200.2.85/info/phone?account=" + this.account;

                }
                AbonentPacket.Program.logger_main.Debug("Document: GetInfoFromSTART: ConnectionString: " + sURL);
                string sBody;

                request = (HttpWebRequest)WebRequest.Create(sURL);
                AbonentPacket.Program.Log("Document: GetInfoFromSTART: Create");

                request.UseDefaultCredentials = true;
                request.PreAuthenticate = true;
                request.Credentials = CredentialCache.DefaultCredentials;

                response = (HttpWebResponse)request.GetResponse();
                AbonentPacket.Program.Log("Document: GetInfoFromSTART:: HttpWebResponse: Get");

                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    sBody = reader.ReadToEnd();
                    sBody = sBody.Replace("     ", " ");
                    sBody = sBody.Replace("    ", " ");
                    sBody = sBody.Replace("   ", " ");                   
                    
                    sBody = sBody.Replace("\n", "");
                    sBody = sBody.Trim();
                    if (sBody.Length > 1496)
                    {
                        sBody = sBody.Substring(0, 1495);
                    }
                }

                AbonentPacket.Program.Log("Document: GetInfoFromSTART_phone:: HttpWebResponse: Body: " + sBody);

                this.phone = sBody;

            }
            catch (Exception ex)
            {
                res = 1;
                AbonentPacket.Program.logger_main.Error("Document: GetInfoFromXML: " + ex.Message);
                AbonentPacket.Program.logger_main.Error("Document: GetInfoFromXML: " + ex.StackTrace);
                throw ex;
            }
            finally
            {
            }
            AbonentPacket.Program.logger_main.Error("Document: GetInfoFromSTART: End");
            return res;
        }

        public int GetInfoFromSTART_name()
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;

            AbonentPacket.Program.logger_main.Error("Document: GetInfoFromSTART_name: Begin");
            int res = 0;
            try
            {

                string sURL = "";
                if (AbonentPacket.Program.theForm._DepartID == 2)
                {
                    sURL = "http://10.200.2.85/info/name?account=" + this.account;

                }
                AbonentPacket.Program.logger_main.Debug("Document: GetInfoFromSTART: ConnectionString: " + sURL);
                string sBody;

                request = (HttpWebRequest)WebRequest.Create(sURL);
                AbonentPacket.Program.Log("Document: GetInfoFromSTART: Create");

                request.UseDefaultCredentials = true;
                request.PreAuthenticate = true;
                request.Credentials = CredentialCache.DefaultCredentials;

                response = (HttpWebResponse)request.GetResponse();
                AbonentPacket.Program.Log("Document: GetInfoFromSTART:: HttpWebResponse: Get");

                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    sBody = reader.ReadToEnd();
                    sBody = sBody.Replace("&quot;", "\"");
                    sBody = sBody.Replace("     ", " ");
                    sBody = sBody.Replace("    ", " ");
                    sBody = sBody.Replace("   ", " ");
                    sBody = sBody.Replace("\n", " ");
                    sBody = sBody.Trim();
                }

                AbonentPacket.Program.Log("Document: GetInfoFromSTART_phone:: HttpWebResponse: Body: " + sBody);

                this.name = sBody;

            }
            catch (Exception ex)
            {
                res = 1;
                AbonentPacket.Program.logger_main.Error("Document: GetInfoFromXML: " + ex.Message);
                AbonentPacket.Program.logger_main.Error("Document: GetInfoFromXML: " + ex.StackTrace);
                throw ex;
            }
            finally
            {
            }
            AbonentPacket.Program.logger_main.Error("Document: GetInfoFromSTART: End");
            return res;
        }

        public int SendToArchive()
        {
            int res = 0;
            try
            {
                
                this.GetInfoFromSTART_inn();
                this.GetInfoFromSTART_phone();
                this.GetInfoFromSTART_name();
                this.GetPageCountPDF();

                NameValueCollection nvc = new NameValueCollection();
                nvc.Add("app", "1");
                nvc.Add("document[inn]", this.inn);
                nvc.Add("document[page_count]", this.pagecount);
                nvc.Add("document[korobka2]", this.korobka);
                nvc.Add("document[comments]", "");
                nvc.Add("document[name_auth_user]", AbonentPacket.Program.theForm._User);
                nvc.Add("document[depart_id]", AbonentPacket.Program.theForm._DepartID.ToString());
                nvc.Add("document[scaned_at]", "1");
                nvc.Add("document[prsnl_data_permission]", "1");
                nvc.Add("document[user_id]", AbonentPacket.Program.theForm._UserID.ToString());
                nvc.Add("document[doc_type_id]", this.typedoc.ToString());
                nvc.Add("document[phone]", this.phone);
                nvc.Add("document[abonent_name]", this.name);
                nvc.Add("document[account]", this.account);

                res = HttpUploadFile("http://10.200.2.85/documents/create",
                     this.Path + "\\" + this.account + ".pdf", "document[pdf_file]", "application/pdf", nvc);
            }
            catch (Exception ex)
            {
                AbonentPacket.Program.logger_main.Error("Document: GetInfoFromXML: " + ex.Message);
                AbonentPacket.Program.logger_main.Error("Document: GetInfoFromXML: " + ex.StackTrace);
            }
            finally {

            }

            return res;
        }

        public static int HttpUploadFile(string url, string file, string paramName, string contentType, NameValueCollection nvc)
        {
            AbonentPacket.Program.logger_main.Error("HttpUploadFile: Begin");
            int res = -1;
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.Method = "POST";
            wr.KeepAlive = true;
            wr.Credentials = System.Net.CredentialCache.DefaultCredentials;

            Stream rs = wr.GetRequestStream();

            string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
            foreach (string key in nvc.Keys)
            {
                rs.Write(boundarybytes, 0, boundarybytes.Length);
                string formitem = string.Format(formdataTemplate, key, nvc[key]);
                byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                rs.Write(formitembytes, 0, formitembytes.Length);
            }
            rs.Write(boundarybytes, 0, boundarybytes.Length);

            string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
            string header = string.Format(headerTemplate, paramName, file, contentType);
            byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
            rs.Write(headerbytes, 0, headerbytes.Length);

            FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[4096];
            int bytesRead = 0;
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                rs.Write(buffer, 0, bytesRead);
            }
            fileStream.Close();


            byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            rs.Write(trailer, 0, trailer.Length);
            rs.Close();

            HttpWebResponse wresp = null;
            try
            {
                wresp = (HttpWebResponse)wr.GetResponse();
                Stream stream2 = wresp.GetResponseStream();
                StreamReader reader2 = new StreamReader(stream2);
                res = Convert.ToInt32(wresp.StatusCode);
            }
            catch (Exception ex)
            {
                res = 1;
                AbonentPacket.Program.logger_main.Error("Document: HttpUploadFile: " + ex.Message);
                AbonentPacket.Program.logger_main.Error("Document: HttpUploadFile: " + ex.StackTrace);
                throw ex;

                if (wresp != null)
                {
                    wresp.Close();
                    wresp = null;
                }
            }
            finally
            {
                wr = null;
            }
            AbonentPacket.Program.logger_main.Error("HttpUploadFile: End");
            return res;
        }

        public static void Send(Socket socket, byte[] buffer, int offset, int size, int timeout)
        {
            AbonentPacket.Program.logger_main.Error("HttpUploadFile: End");
            int startTickCount = Environment.TickCount;
            int sent = 0;  // how many bytes is already sent
            do
            {
                if (Environment.TickCount > startTickCount + timeout)
                    throw new Exception("Timeout.");
                try
                {
                    sent += socket.Send(buffer, offset + sent, size - sent, SocketFlags.None);
                }
                catch (SocketException ex)
                {
                    if (ex.SocketErrorCode == SocketError.WouldBlock ||
                        ex.SocketErrorCode == SocketError.IOPending ||
                        ex.SocketErrorCode == SocketError.NoBufferSpaceAvailable)
                    {
                        // socket buffer is probably full, wait and try again
                        Thread.Sleep(30);
                    }
                    else
                        throw ex;  // any serious error occurr
                }
            } while (sent < size);
        }

        public static void Receive(Socket socket, byte[] buffer, int offset, int size, int timeout)
        {
            int startTickCount = Environment.TickCount;
            int received = 0;  // how many bytes is already received
            do
            {
                if (Environment.TickCount > startTickCount + timeout)
                    throw new Exception("Timeout.");
                try
                {
                    received += socket.Receive(buffer, offset + received, size - received, SocketFlags.None);
                }
                catch (SocketException ex)
                {
                    if (ex.SocketErrorCode == SocketError.WouldBlock ||
                        ex.SocketErrorCode == SocketError.IOPending ||
                        ex.SocketErrorCode == SocketError.NoBufferSpaceAvailable)
                    {
                        // socket buffer is probably empty, wait and try again
                        Thread.Sleep(30);
                    }
                    else
                        throw ex;  // any serious error occurr
                }
            } while (received < size);
        }


        public string Status
        {
            get
            {
                string file = Path + "\\" + account + ".200";
                if (File.Exists(file))
                {
                    return "Успешно отправлен";
                }
                    
                file = Path + "\\" + account + ".err";
                if (File.Exists(file))
                {
                    return "Ошибка при отправке...";
                }
                    
                file = Path + "\\" + account + ".run";
                if (File.Exists(file))
                {
                    return "Отправляется на сервер...";
                }

                file = Path + "\\" + account + ".ini";
                if (File.Exists(file))
                {
                    return "В очереди...";
                }
                return "Создан";
            }
        }


        

    }
}
