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
    public partial class FormUser : Form
    {
        public int DepartID = -1;
        public int UserID = -1;
        public string UserName;
        public FormUser()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void User_Load(object sender, EventArgs e)
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            try
            {
                if (this.DepartID == -1)
                {
                    FormDepart formDepart = new FormDepart();
                    if (formDepart.ShowDialog() == DialogResult.Cancel)
                    {
                        MessageBox.Show("Необходимо указать филиал!");
                        this.DialogResult = DialogResult.Cancel;
                        this.Close();
                    }
                    else
                    {
                        this.DepartID = formDepart.ID;
                    }
                }
                
                string sURL;
                sURL = "http://10.200.2.85/info/workers.xml?depart_id=" + this.DepartID.ToString();
                string sBody;

                request = (HttpWebRequest)WebRequest.Create(sURL);

                request.UseDefaultCredentials = true;
                request.PreAuthenticate = true;
                request.Credentials = CredentialCache.DefaultCredentials;

                response = (HttpWebResponse)request.GetResponse();

                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    sBody = reader.ReadToEnd();
                }

                XmlDataDocument xmldoc = new XmlDataDocument();
                XmlNodeList xmlnode;
                int i = 0;
                xmldoc.Load(XmlReader.Create(new StringReader(sBody)));
                xmlnode = xmldoc.GetElementsByTagName("row");

                for (i = 0; i < xmlnode.Count; i++)
                {
                    string s = xmlnode[i].Attributes[0].Value;
                    User theUser = new User();
                    theUser.ID = Convert.ToInt32(xmlnode[i].Attributes[0].Value);
                    theUser.Name = xmlnode[i].Attributes[1].Value;
                    theUser.DepartID = this.DepartID;
                    this.comboBox1.Items.Add(theUser);
                }

                if (xmlnode.Count > 0)
                {
                    this.comboBox1.SelectedIndex = 0;
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
                User user = (User)this.comboBox1.SelectedItem;
                this.UserID = user.ID;
                this.DepartID = user.DepartID;
                this.UserName = user.Name;
                buttonSave.Enabled = true;
            }
            else
            {
                this.UserID = -1;
                buttonSave.Enabled = false;
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            AbonentPacket.Program.theForm._UserID = this.UserID;
            AbonentPacket.Program.theForm._User = this.UserName;
            AbonentPacket.Program.theForm.WriteIni();
        }
    }
}
