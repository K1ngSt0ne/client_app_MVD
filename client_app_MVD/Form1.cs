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

namespace client_app_MVD
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public class MySQLData
        {
            public class MySqlExecute
            {

                /// <summary>
                /// Возвращаемый набор данных.
                /// </summary>
                public class MyResult
                {
                    /// <summary>
                    /// Возвращает результат запроса.
                    /// </summary>
                    public string ResultText;
                    /// <summary>
                    /// Возвращает True - если произошла ошибка.
                    /// </summary>
                    public string ErrorText;
                    /// <summary>
                    /// Возвращает текст ошибки.
                    /// </summary>
                    public bool HasError;
                }

                /// <summary>
                /// Для выполнения запросов к MySQL с возвращением 1 параметра.
                /// </summary>
                /// <param name="sql">Текст запроса к базе данных</param>
                /// <param name="connection">Строка подключения к базе данных</param>
                /// <returns>Возвращает значение при успешном выполнении запроса, текст ошибки - при ошибке.</returns>
                public static MyResult SqlScalar(string sql, string connection)
                {
                    MyResult result = new MyResult();
                    try
                    {
                        MySql.Data.MySqlClient.MySqlConnection connRC = new MySql.Data.MySqlClient.MySqlConnection(connection);
                        MySql.Data.MySqlClient.MySqlCommand commRC = new MySql.Data.MySqlClient.MySqlCommand(sql, connRC);
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


                /// <summary>
                /// Для выполнения запросов к MySQL без возвращения параметров.
                /// </summary>
                /// <param name="sql">Текст запроса к базе данных</param>
                /// <param name="connection">Строка подключения к базе данных</param>
                /// <returns>Возвращает True - ошибка или False - выполнено успешно.</returns>
                public static MyResult SqlNoneQuery(string sql, string connection)
                {
                    MyResult result = new MyResult();
                    try
                    {
                        MySql.Data.MySqlClient.MySqlConnection connRC = new MySql.Data.MySqlClient.MySqlConnection(connection);
                        MySql.Data.MySqlClient.MySqlCommand commRC = new MySql.Data.MySqlClient.MySqlCommand(sql, connRC);
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

            /// <summary>
            /// Методы реализующие выполнение запросов с возвращением набора данных.
            /// </summary>
            public class MySqlExecuteData
            {
                /// <summary>
                /// Возвращаемый набор данных.
                /// </summary>
                public class MyResultData
                {
                    /// <summary>
                    /// Возвращает результат запроса.
                    /// </summary>
                    public DataTable ResultData;
                    /// <summary>
                    /// Возвращает True - если произошла ошибка.
                    /// </summary>
                    public string ErrorText;
                    /// <summary>
                    /// Возвращает текст ошибки.
                    /// </summary>
                    public bool HasError;
                }


                /// <summary>
                /// Выполняет запрос выборки набора строк.
                /// </summary>
                /// <param name="sql">Текст запроса к базе данных</param>
                /// <param name="connection">Строка подключения к базе данных</param>
                /// <returns>Возвращает набор строк в DataSet.</returns>
                public static MyResultData SqlReturnDataset(string sql, string connection)
                {
                    MyResultData result = new MyResultData();
                    try
                    {
                        MySql.Data.MySqlClient.MySqlConnection connRC = new MySql.Data.MySqlClient.MySqlConnection(connection);
                        MySql.Data.MySqlClient.MySqlCommand commRC = new MySql.Data.MySqlClient.MySqlCommand(sql, connRC);
                        connRC.Open();

                        try
                        {
                            MySql.Data.MySqlClient.MySqlDataAdapter AdapterP = new MySql.Data.MySqlClient.MySqlDataAdapter();
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
            panel1.Visible = false;
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
    }
}
