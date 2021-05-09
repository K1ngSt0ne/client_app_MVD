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
using MySql.Data.MySqlClient;
using System.Security.Cryptography;

namespace client_app_MVD
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            
        }
        string conncetion_string = "server=localhost;user=root;password=12345;database=mvd_database_course_work;port=3306";
        public class MySQLData
        {
            //класс для выполнения запросов
            public class MySqlExecute
            {
                public class MyResult
                {
                    
                    public string ResultText;                    
                    public string ErrorText;                    
                    public bool HasError;
                } 
               //удем использовать этот метолд для авторизации
                public static MyResult SqlScalar(string sql, string connection)
                {
                    MyResult result = new MyResult();
                    try
                    {
                        MySqlConnection connRC = new MySqlConnection(connection);
                        MySqlCommand commRC = new MySqlCommand(sql, connRC);
                        connRC.Open();
                        try
                        {
                            result.ResultText = commRC.ExecuteScalar().ToString();
                            result.HasError = false;
                        }
                        catch (Exception ex)
                        {
                            result.ErrorText = ex.Message;
                            result.HasError = true;
                        }
                        connRC.Close();
                    }
                    catch (Exception ex)//Этот эксепшн на случай отсутствия соединения с сервером.
                    {
                        result.ErrorText = ex.Message;
                        result.HasError = true;
                    }
                    return result;
                }


                //для чего то пригодится, однозначно
                public static MyResult SqlNoneQuery(string sql, string connection)
                {
                    MyResult result = new MyResult();
                    try
                    {
                        MySqlConnection connRC = new MySqlConnection(connection);
                        MySqlCommand commRC = new MySqlCommand(sql, connRC);
                        connRC.Open();
                        try
                        {
                            commRC.ExecuteNonQuery();
                            result.HasError = false;
                        }
                        catch (Exception ex)
                        {
                            result.ErrorText = ex.Message;
                            result.HasError = true;
                        }
                        connRC.Close();
                    }
                    catch (Exception ex)//Этот эксепшн на случай отсутствия соединения с сервером.
                    {
                        result.ErrorText = ex.Message;
                        result.HasError = true;
                    }
                    return result;
                }

            }
            //класс для возвращаемых данных
            public class MySqlExecuteData
            {

                public class MyResultData
                {
                    public DataTable ResultData; 
                    public string ErrorText;
                    public bool HasError;
                }
                //заполним дата грип (а может нам и не понадобиться)
                public static MyResultData SqlReturnDataset(string sql, string connection)
                {
                    MyResultData result = new MyResultData();
                    try
                    {
                        MySqlConnection connRC = new MySqlConnection(connection);
                        MySqlCommand commRC = new MySqlCommand(sql, connRC);
                        connRC.Open();

                        try
                        {
                            MySqlDataAdapter AdapterP = new MySqlDataAdapter();
                            AdapterP.SelectCommand = commRC;
                            DataSet ds1 = new DataSet();
                            AdapterP.Fill(ds1);
                            result.ResultData = ds1.Tables[0];
                        }
                        catch (Exception ex)
                        {
                            result.HasError = true;
                            result.ErrorText = ex.Message;
                        }
                        connRC.Close();
                    }
                    catch (Exception ex)//Этот эксепшн на случай отсутствия соединения с сервером.
                    {
                        result.ErrorText = ex.Message;
                        result.HasError = true;
                    }
                    return result;

                }

            }        
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
            panel3.Visible = true;
        }

        private void close_app(object sender, EventArgs e)
        {
            DialogResult result=MessageBox.Show("Вы уверены что хотите выйти?", "Уведомление",MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
                Application.Exit();
        }

        private void cancel_operation(object sender, EventArgs e)
        {
            MessageBox.Show("Операция отменена", "Уведомление");
            panel1.Visible =panel4.Visible= false;
            
        }
        //работающая авторизация
        private void access_to_database(object sender, EventArgs e)
        {
            var prepare_hash = new SHA1Managed().ComputeHash(Encoding.UTF8.GetBytes(textBox2.Text));
            var hash = string.Concat(prepare_hash.Select(b => b.ToString("x2")));
            if ((textBox1.Text == "") || (textBox2.Text == ""))
                MessageBox.Show("Какое-то из полей пустое. Ошибка!", "Предупреждение");
            else
            {
                var access_to_database = MySQLData.MySqlExecute.SqlScalar("select exists(select login, password from users where login='" + textBox1.Text + "' and password='" + hash + "')", conncetion_string);
                if (access_to_database.HasError)
                    MessageBox.Show(access_to_database.ErrorText);
                else
                {
                    if (access_to_database.ResultText == "1")
                    {
                        var user_role = MySQLData.MySqlExecute.SqlScalar("select user_role from users where login='"+textBox1.Text+"'", conncetion_string);
                        switch (user_role.ResultText)
                        {
                            case "Сотрудник":
                                MessageBox.Show("Добро пожаловать, Сотрудник!", "Приветствие");
                                служебнаяИнформацияToolStripMenuItem1.Visible = true;
                                аналитикаToolStripMenuItem1.Visible = true;
                                картотеткиToolStripMenuItem.Visible = true;
                                справочникиToolStripMenuItem2.Visible = true;
                                break;
                            case "Заявитель":
                                MessageBox.Show("Добро пожаловать, Заявитель!", "Приветствие");
                                справочникиToolStripMenuItem2.Visible = true;
                                узнатьОСостоянииЗаявленияToolStripMenuItem.Visible = true;
                                break;
                            case "Участник события":
                                MessageBox.Show("Добро пожаловать, Участник события!", "Приветствие");
                                справочникиToolStripMenuItem2.Visible = true;
                                break;
                            case "Администратор":
                                MessageBox.Show("Добро пожаловать, Администратор!", "Приветствие");
                                служебнаяИнформацияToolStripMenuItem1.Visible = true;
                                аналитикаToolStripMenuItem1.Visible = true;
                                картотеткиToolStripMenuItem.Visible = true;
                                справочникиToolStripMenuItem2.Visible = true;
                                break;
                        }
                        panel1.Visible = false;

                    }    
                    else
                    {
                        MessageBox.Show("Неверный логин или пароль", "Предупреждение");
                        textBox1.Text = textBox2.Text = "";
                    }
                }
            }           
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
                        panel2.Visible = false;
                    else if (panel1.Visible == true)
                    {
                        MessageBox.Show("Операция отменена", "Уведомление");
                        panel1.Visible = false;
                    }                       
                    break;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void close_about_programm(object sender, EventArgs e)
        {
            panel3.Visible = false;
        }

        private void contacts_view(object sender, EventArgs e)
        {
            MessageBox.Show("Пластовец Сергей Григореьвич\nE-mail: acdc2018@ro.ru\nТелефон: 89811965249", "Контакты");
        }
        //рабочая регистрация пользователй (на авторизацию в базе)
        private void user_registration(object sender, EventArgs e)
        {
            if ((textBox3.Text == "") || (textBox4.Text == "") || (textBox5.Text == "") || (textBox6.Text == "") || (textBox7.Text == "") || (textBox8.Text == "") || (textBox9.Text == ""))
                MessageBox.Show("Какое-то из полей пустое. Ошибка!", "Предупреждение");
            else
            {
                if (textBox7.Text != textBox8.Text)
                    MessageBox.Show("Пароли не совпадают. Ошибка!");
                else
                {
                    var user_registration_string = MySQLData.MySqlExecute.SqlNoneQuery("call users_registration('"+ textBox3.Text+"', '"+textBox4.Text+"', '"+textBox5.Text+"', '"+textBox6.Text+"', '"+textBox7.Text+"', '"+textBox9.Text+"');", conncetion_string);
                    if (user_registration_string.HasError)
                        MessageBox.Show(user_registration_string.ErrorText);
                    else
                    {
                        MessageBox.Show("Успешно!", "Уведомление");
                        panel4.Visible = false;
                    }
                }
            }
        }

        private void registration_panel_open(object sender, EventArgs e)
        {
            panel4.Visible = true;
        }

        private void see_applicants_table(object sender, EventArgs e)
        {

        }

        private void see_ranks_table(object sender, EventArgs e)
        {
            int location_x = 20;
            int location_y = 43;
            int size = 20;
            panel5.Visible = true;
            var rank_table = MySQLData.MySqlExecuteData.SqlReturnDataset("Select id_rank, Rank_name from rank_table;", conncetion_string);
            datatable_see(rank_table, location_x,location_y, size);
            //MessageBox.Show(rank_table.ResultData.Rows[0][1].ToString());// 0 столбец - идшка, 1 - название звания
            
        }
        void datatable_see(MySQLData.MySqlExecuteData.MyResultData myResultData, int location_X, int location_Y, int size)
        {
            for (int i = 0; i < myResultData.ResultData.Rows.Count; i++)
            {
                for (int j = 0; j < myResultData.ResultData.Columns.Count; j++)
                {
                    var label = new Label();
                    label.Size = new Size(size, 20);
                    label.Location = new Point(location_X, location_Y);
                    label.Font = new Font("Arial", 14);
                    label.ForeColor = Color.White;
                    label.Text = myResultData.ResultData.Rows[i][j].ToString();
                    panel5.Controls.Add(label);
                    size += 200;
                    location_X += 120;
                }
                size = 20;
                location_X = 20;
                location_Y += 30;
            }
        }
    }
}
