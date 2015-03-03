using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Xml;

namespace AbonentPacket
{
    public partial class FormDepart : Form
    {
        public int ID;
        public string Name;

        public FormDepart()
        {
            this.ID = -1;
            InitializeComponent();
        }

        private void Depart_Load(object sender, EventArgs e)
        {
            HttpWebRequest request = null; 
            HttpWebResponse response = null;

            try
            {
                AbonentPacket.Program.Log("FormDepart_Load: Enter");
                string sURL;
                sURL = "http://10.200.2.85/info/departs.xml";
                AbonentPacket.Program.Log("FormDepart_Load: URL: " + sURL);
                string sBody;

                request = (HttpWebRequest)WebRequest.Create(sURL);
                AbonentPacket.Program.Log("FormDepart_Load: HttpWebRequest: Create");
                
                request.UseDefaultCredentials = true;
                request.PreAuthenticate = true;
                request.Credentials = CredentialCache.DefaultCredentials;
                
                response = (HttpWebResponse)request.GetResponse();
                AbonentPacket.Program.Log("FormDepart_Load: HttpWebResponse: Get");

                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    sBody = reader.ReadToEnd();
                }
                AbonentPacket.Program.Log("FormDepart_Load: HttpWebResponse: Body" + sBody);

                XmlDataDocument xmldoc = new XmlDataDocument();
                XmlNodeList xmlnode;
                int i = 0;
                string str = null;
                xmldoc.Load(XmlReader.Create(new StringReader(sBody)));
                xmlnode = xmldoc.GetElementsByTagName("row");

                for (i = 0; i < xmlnode.Count; i++)
                {
                    string s = xmlnode[i].Attributes[0].Value;
                    Depart theDepart = new Depart();
                    theDepart.ID = Convert.ToInt32(xmlnode[i].Attributes[0].Value);
                    theDepart.Name = xmlnode[i].Attributes[1].Value;
                    this.comboBox1.Items.Add(theDepart);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.DialogResult = DialogResult.Cancel;
                AbonentPacket.Program.logger_main.Error("FormDepart_Load: Catch " + ex.Message);
                AbonentPacket.Program.logger_main.Error("FormDepart_Load: Catch " + ex.Source);
                AbonentPacket.Program.logger_main.Error("FormDepart_Load: Catch " + ex.StackTrace);
            }
            finally
            {
                request = null;
                response = null;
            }

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBox1.SelectedIndex > -1)
            {
                this.buttonSave.Enabled = true;
                Depart depart = (Depart)this.comboBox1.SelectedItem;
                this.ID = depart.ID;
            }
            else
            {
                this.ID = -1;
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            AbonentPacket.Program.theForm._DepartID = this.ID;
            AbonentPacket.Program.theForm.WriteIni();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
