using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Net;
using System.Collections.Specialized;
using System.Net.Sockets;

namespace AbonentPacket
{
    public partial class FormAbonentPacket : Form
    {

        public string _FolderJpegFiles;
        public string _FolderWorkFiles;
        public int _DepartID;
        public int _UserID;
        public string _User;
        public string _Korobka = "";

        public SortedSet<string> setAccount;        

        public void ReadIni()
        {
            string file = Application.StartupPath + "\\" + "AbonentPacket.ini";
            FileStream ini;
            if (!File.Exists(file))
            {
                ini = File.Create(file);
                ini.Dispose();
            }

            AMS.Profile.Ini Init = new AMS.Profile.Ini(file);
            _FolderJpegFiles = Init.GetValue("FOLDERS", "JpegFiles", "");
            _FolderWorkFiles = Init.GetValue("FOLDERS", "WorkFiles", "");
            _UserID = Init.GetValue("EARCHIVE", "UserID", this._UserID);
            _DepartID = Init.GetValue("EARCHIVE", "DepartID", this._DepartID);
            _User = Init.GetValue("EARCHIVE", "FormUser", this._User);
            _Korobka = Init.GetValue("EARCHIVE", "Korobka", this._Korobka);

        }

        public void WriteIni()
        {
            AMS.Profile.Ini Init = new AMS.Profile.Ini(Application.StartupPath + "\\" + "AbonentPacket.ini");
            Init.SetValue("FOLDERS", "JpegFiles", _FolderJpegFiles);
            Init.SetValue("FOLDERS", "WorkFiles", _FolderWorkFiles);
            Init.SetValue("EARCHIVE", "UserID", this._UserID);
            Init.SetValue("EARCHIVE", "DepartID", this._DepartID);
            Init.SetValue("EARCHIVE", "FormUser", this._User);
            Init.SetValue("EARCHIVE", "Korobka", this._Korobka);
        }

        public FormAbonentPacket()
        {
            setAccount = new SortedSet<string>();
            this._DepartID = -1;
            this._UserID = -1;
            InitializeComponent();            
        }      
        

        private void UpdateForm()
        {
            MessageBox.Show(this.Size.Width.ToString());
             
        }

        private void domainUpDown1_SelectedItemChanged(object sender, EventArgs e)
        {
            
        }

        private void FormAbonentPacket_SizeChanged(object sender, EventArgs e)
        {
        }

        private void splitContainer1_Panel1_SizeChanged(object sender, EventArgs e)
        {
            //MessageBox.Show(this.splitContainer1.Panel1.ClientSize.Width.ToString());
        }

        private void weToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormSettings theSetting = new FormSettings();
            theSetting.textBox1.Text = this._FolderJpegFiles;
            theSetting.textBox2.Text = this._FolderWorkFiles;
            theSetting.Show();
        }

        private void FormAbonentPacket_FormClosed(object sender, FormClosedEventArgs e)
        {   
        }

        public void UpdateList()
        {
            timer1.Stop();
            if (System.IO.Directory.Exists(this._FolderJpegFiles))
            {
                SortedSet<string> setTemp = new SortedSet<string>();
                SortedSet<string> setFiles = new SortedSet<string>();

                foreach (var file in Directory.GetFiles(this._FolderJpegFiles, "*.pdf", SearchOption.TopDirectoryOnly))
                {
                    FileInfo fi = new FileInfo(file);
                    string account = fi.Name.Replace(".pdf", "");                    
                    setTemp.Add(account);
                    setFiles.Add(account);
                }

                // new
                setTemp.ExceptWith(setAccount);
                foreach (var account in setTemp)
                {
                    setAccount.Add(account);
                    Document doc = new Document(account, this._Korobka, this._FolderJpegFiles);
                    listBoxAccount.Items.Add(doc);
                    doc.IniWrite();
                }
                // delete
                foreach (var account in setAccount)
                {
                    if (setFiles.Count(t => t == account) == 0)
                    {
                        Document document;
                        foreach (var doc in listBoxAccount.Items)
                        {
                            if (doc.ToString() == account)
                            {
                                listBoxAccount.Items.Remove(doc);
                                document = (Document)doc;
                                document = null;
                                break;
                            }
                        }
                        setAccount.Remove(account);
                        break;
                    }
                } 
                if (listBoxAccount.SelectedIndex == -1) 
                {
                    if (listBoxAccount.Items.Count > 0)
                    {
                        var item = listBoxAccount.Items[0];
                        listBoxAccount.SelectedItem = item;
                    }
                } 
            }
            timer1.Start();

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateList();
            if (listBoxAccount.SelectedIndex > -1)
            {
                Document doc = (Document)listBoxAccount.SelectedItem;
                this.textBoxStatus.Text = doc.Status;
                if (doc.Status == "Создан")
                {
                    doc.IniWrite();
                }
            }
            
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void listBoxAccount_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxAccount.SelectedIndex > -1)
            {
                this.EnableDocumentForm(true);
                Document doc = (Document)listBoxAccount.SelectedItem;
                doc.IniRead();
                this.textBoxAccount.Text = doc.account;
                this.textBoxINN.Text = doc.inn;
                this.textBoxName.Text = doc.name;
                this.textBoxKorobka.Text = doc.korobka;
                this.textBoxStatus.Text = doc.Status;
            }
            else
            {
                this.EnableDocumentForm(false);
            }
        }

        private void textBoxAccount_TextChanged(object sender, EventArgs e)
        {
            Document doc = (Document)listBoxAccount.SelectedItem;
            if (doc != null)
            {
                doc.account = this.textBoxAccount.Text;
            }
        }

        private void textBoxINN_TextChanged(object sender, EventArgs e)
        {
            Document doc = (Document)listBoxAccount.SelectedItem;
            if (doc != null)
            {
                doc.inn = this.textBoxINN.Text;
            }
        }

        private void textBoxName_TextChanged(object sender, EventArgs e)
        {

            Document doc = (Document)listBoxAccount.SelectedItem;
            if (doc != null)
            {
                doc.name = this.textBoxName.Text;
            }
        }

        private void textBoxKorobka_TextChanged(object sender, EventArgs e)
        {

            Document doc = (Document)listBoxAccount.SelectedItem;
            if (doc != null)
            {
                doc.korobka = this.textBoxKorobka.Text;
            }
        }

        private void textBoxPageCount_TextChanged(object sender, EventArgs e)
        {
            Document doc = (Document)listBoxAccount.SelectedItem;
            if (doc != null)
            {
                doc.pagecount = this.textBoxPageCount.Text;
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            Document doc = (Document)listBoxAccount.SelectedItem;
            doc.IniWrite();
        }

        private void EnableDocumentForm(bool bStatus)
        {
            this.textBoxAccount.Enabled = false;
            this.textBoxINN.Enabled = bStatus;
            this.textBoxName.Enabled = bStatus;
            this.textBoxKorobka.Enabled = false;
            this.buttonSave.Enabled = bStatus;
            this.textBoxPageCount.Enabled = bStatus;
            if (bStatus == false)
            {
                this.textBoxAccount.Text = "";
                this.textBoxINN.Text = "";
                this.textBoxName.Text = "";
                this.textBoxKorobka.Text = "";             
                this.textBoxPageCount.Text = "";
                this.textBoxStatus.Text = "";
            }
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            Document doc = (Document)listBoxAccount.SelectedItem;
            doc.SendToArchive();
        }

        private void listBoxAccount_EnabledChanged(object sender, EventArgs e)
        {
        }

        private void FormAbonentPacket_Load(object sender, EventArgs e)
        {
            ReadIni();

            FormSettings theSettings = new FormSettings();
            theSettings.textBox1.Text = this._FolderJpegFiles;
            theSettings.textBox2.Text = this._FolderWorkFiles;
            DialogResult res = theSettings.ShowDialog();
            if (res == DialogResult.Cancel)
            {
                MessageBox.Show("Необходимо указать соответствующие директории!");
                this.Close();
            }
            
            if (this._DepartID == -1 || this._UserID == -1 || this._User.Length == 0)
            {
                FormUser formUser = new FormUser();
                formUser.DepartID = this._DepartID;
                if (formUser.ShowDialog() == DialogResult.Cancel)
                {
                    MessageBox.Show("Необходимо указать пользователя");
                    this.Close();
                }
                else
                {

                }
            }

            FormKorobka theFormKorobka = new FormKorobka();
            theFormKorobka.textBoxKorobka.Text = this._Korobka;
            if (theFormKorobka.ShowDialog() == DialogResult.OK)
            {
                this._Korobka = theFormKorobka.textBoxKorobka.Text;
                this.WriteIni();
            }
            else
            {
                MessageBox.Show("Необходимо указать номер коробки!");
                this.Close();
            }
            theFormKorobka = null;

            timer1.Start();            
            this.EnableDocumentForm(false);
            Program.theThreadSender.Start();
            Program.theThreadConvert.Start();  
        }

        private void listBoxAccount_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index > -1)
            {
                e.DrawBackground();

                Graphics g = e.Graphics;
                
                ListBox lb = (ListBox)sender;
                Document doc = (Document)lb.Items[e.Index];
                if (doc.Status == "Ошибка при отправке...")
                {
                    g.FillRectangle(new SolidBrush(Color.Red), e.Bounds);
                    
                }
                g.DrawString(lb.Items[e.Index].ToString(), e.Font, new SolidBrush(Color.Black), new PointF(e.Bounds.X, e.Bounds.Y));
                e.DrawFocusRectangle();                
            }
        }


    }
}
