using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace client_app_MVD
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void service_click(object sender, EventArgs e)
        {

        }

        private void enter_to_database(object sender, EventArgs e)
        {
            panel1.Visible = true;
        }

        private void about_program(object sender, EventArgs e)
        {
            MessageBox.Show("здесь наверняка будет что нибудь интресное...", "Информация к размышлению");
        }

        private void close_app(object sender, EventArgs e)
        {
            DialogResult result=MessageBox.Show("Вы уверены что хотите выйти?", "Уведомление",MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
                Application.Exit();
        }

        private void cancel_operation(object sender, EventArgs e)
        {
            info_message();
            //нужно взять все активные панели и проверять какая из них сейчас в фокусе, а потом ее закрывать
        }

        private void access_to_database(object sender, EventArgs e)
        {
            

        }

        private void help(object sender, EventArgs e)
        {
            panel2.Visible = true;
            panel2.BringToFront();
            StreamReader sr = new StreamReader("help1.txt");
            string line="";
            while (!sr.EndOfStream)//пока не конец
            {
                line = sr.ReadLine();//читаем построчно    
                label4.Text += line + Environment.NewLine;
            }
            sr.Close();

        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Escape:
                    if (panel2.Visible == true)
                        info_message();
                    else if (panel1.Visible == true)
                    {
                        info_message();
                    }
                       
                    break;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
        void info_message()
        {
            foreach (Panel panel in this.Controls.OfType<Panel>())
            {
                if (panel.Focused)
                {
                    MessageBox.Show("Операция отменена", "Уведомление");
                    panel.Visible = false;
                }
            }

        }

    }
}
