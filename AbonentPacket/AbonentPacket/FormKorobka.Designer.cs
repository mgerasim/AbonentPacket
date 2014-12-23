namespace AbonentPacket
{
    partial class FormKorobka
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxKorobka = new System.Windows.Forms.TextBox();
            this.buttonBoxKorobka = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(135, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Укажите номер коробки:";
            // 
            // textBoxKorobka
            // 
            this.textBoxKorobka.Location = new System.Drawing.Point(15, 26);
            this.textBoxKorobka.Name = "textBoxKorobka";
            this.textBoxKorobka.Size = new System.Drawing.Size(156, 20);
            this.textBoxKorobka.TabIndex = 1;
            // 
            // buttonBoxKorobka
            // 
            this.buttonBoxKorobka.Location = new System.Drawing.Point(15, 52);
            this.buttonBoxKorobka.Name = "buttonBoxKorobka";
            this.buttonBoxKorobka.Size = new System.Drawing.Size(75, 23);
            this.buttonBoxKorobka.TabIndex = 2;
            this.buttonBoxKorobka.Text = "Сохранить";
            this.buttonBoxKorobka.UseVisualStyleBackColor = true;
            this.buttonBoxKorobka.Click += new System.EventHandler(this.buttonBoxKorobka_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(96, 52);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "Отмена";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // FormKorobka
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(186, 91);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonBoxKorobka);
            this.Controls.Add(this.textBoxKorobka);
            this.Controls.Add(this.label1);
            this.Name = "FormKorobka";
            this.Text = "Коробка";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonBoxKorobka;
        private System.Windows.Forms.Button buttonCancel;
        public System.Windows.Forms.TextBox textBoxKorobka;
    }
}