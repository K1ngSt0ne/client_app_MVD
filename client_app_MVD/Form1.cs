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
using System.Collections;//для ArrayList
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
        //глобальные переменные
        string conncetion_string = "server=localhost;user=root;password=12345;database=mvd_database_course_work;port=3306";
        ArrayList dataGrid_columns_name = new ArrayList();
        DataGridView current_dataGrid = new DataGridView();
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
               //будем использовать этот метолд для авторизации
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
            panel1.Visible =panel4.Visible=  panel15.Visible=panel18.Visible=panel20.Visible=false;
            
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
                    MessageBox.Show(access_to_database.ErrorText, "Уведомление");
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
                        авторизацияToolStripMenuItem.Visible = false;
                        регистрацияToolStripMenuItem.Visible = false;
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
            panel13.Visible = true;
            panel13.BringToFront();
            var applicants_table = MySQLData.MySqlExecuteData.SqlReturnDataset("select last_name, first_name, patronymic, Date_of_birth from applicants_table;", conncetion_string);
            dataGridView3.DataSource = applicants_table.ResultData;
            current_dataGrid = dataGridView3;
        }

        private void see_ranks_table(object sender, EventArgs e)
        {
            panel5.Visible = true;
            panel5.BringToFront();
            var rank_table = MySQLData.MySqlExecuteData.SqlReturnDataset("Select id_rank, Rank_name from rank_table;", conncetion_string);
            datatable_see(rank_table, 20,43, 20, panel5);            
        }
        void datatable_see(MySQLData.MySqlExecuteData.MyResultData myResultData, int location_X, int location_Y, int size, Panel panel_add)
        {
            int default_size = size;
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
                    panel_add.Controls.Add(label);
                    size += 400;
                    location_X += 190;
                }
                size = default_size;
                location_X = 20;
                location_Y += 30;
            }
        }

        private void type_incident_view(object sender, EventArgs e)
        {
            panel6.Visible = true;
            panel6.BringToFront();
            var type_incident_table = MySQLData.MySqlExecuteData.SqlReturnDataset("Select id_type_incident, incident_name from type_incident_table;", conncetion_string);
            datatable_see(type_incident_table, 20, 58, 90, panel6);
        }

        private void status_application_view(object sender, EventArgs e)
        {
            panel7.Visible = true;
            panel7.BringToFront();
            var status_application_table = MySQLData.MySqlExecuteData.SqlReturnDataset("Select id_status_application, status_application_name from status_application;", conncetion_string);
            datatable_see(status_application_table, 20, 58,150, panel7);
        }

        private void time_rewiev_application_view(object sender, EventArgs e)
        {
            panel8.Visible = true;
            panel8.BringToFront();
            var time_review_table = MySQLData.MySqlExecuteData.SqlReturnDataset("select id_time_review,how_long_days from time_review_table;", conncetion_string);
            datatable_see(time_review_table, 20, 58, 140, panel8);
        }

        private void status_table_view(object sender, EventArgs e)
        {
            panel9.Visible = true;
            panel9.BringToFront();
            var status_table = MySQLData.MySqlExecuteData.SqlReturnDataset("select status_name, Duties from status_table; ", conncetion_string);
            datatable_see(status_table, 20, 58, 140, panel9);
        }

        private void personal_table_view(object sender, EventArgs e)
        {
            panel11.Visible = true;
            panel11.BringToFront();
            var personal_table = MySQLData.MySqlExecuteData.SqlReturnDataset("select last_name, first_name, patronymic, Date_of_birth from personal_table;", conncetion_string);
            dataGridView1.DataSource = personal_table.ResultData;
            current_dataGrid = dataGridView1;
            //select last_name, first_name, patronymic, Date_of_birth from 

        }

        private void forms_entrance_view(object sender, EventArgs e)
        {
            panel10.Visible = true;
            panel10.BringToFront();
            var forms_entrance = MySQLData.MySqlExecuteData.SqlReturnDataset("select id_form_entrance, form_name from forms_entrance_table;", conncetion_string);
            datatable_see(forms_entrance, 20, 58, 100, panel10);
        }

        private void members_ivent_view(object sender, EventArgs e)
        {
            panel12.Visible = true;
            panel12.BringToFront();
            var member_table = MySQLData.MySqlExecuteData.SqlReturnDataset("select last_name, first_name, patronymic, Date_of_birth from members_ivent;", conncetion_string);
            dataGridView2.DataSource = member_table.ResultData;
            current_dataGrid = dataGridView2;
        }

        private void application_view(object sender, EventArgs e)
        {
            panel14.Visible = true;
            panel14.BringToFront();
            var application_table = MySQLData.MySqlExecuteData.SqlReturnDataset("select id_application,type_incident_table.incident_name, applications_table.entrance_date,status_application.status_application_name,applicants_table.last_name from applications_table inner join type_incident_table on applications_table.id_type_incident = type_incident_table.id_type_incident inner join status_application on applications_table.id_status_application = status_application.id_status_application inner join applicants_table on applications_table.id_applicant = applicants_table.id_applicant;", conncetion_string);
            dataGridView4.DataSource = application_table.ResultData;
            current_dataGrid = dataGridView4;
        }

        private void added_applicant(object sender, EventArgs e)
        {
            panel15.Visible = true;
        }
        //Добавление участников события
        private void added_member_ivent(object sender, EventArgs e)
        {
            panel16.Visible = true;
        }
        void combobox_items_added(MySQLData.MySqlExecuteData.MyResultData myResultData, ComboBox comboBox, string displaymember, string valuemember)
        {
            comboBox.DataSource = myResultData.ResultData;
            comboBox.DisplayMember = displaymember;
            comboBox.ValueMember = valuemember;
        }
        //ДОбавление события
        private void added_application(object sender, EventArgs e)
        {
            //добавление нового заявления
            panel17.Visible = true;
            var type_incident_table = MySQLData.MySqlExecuteData.SqlReturnDataset("SELECT id_type_incident, incident_name FROM mvd_database_course_work.type_incident_table;", conncetion_string);
            var time_rewiew_table = MySQLData.MySqlExecuteData.SqlReturnDataset("SELECT id_time_review,how_long_days FROM mvd_database_course_work.time_review_table;", conncetion_string);
            var applicant_table = MySQLData.MySqlExecuteData.SqlReturnDataset("SELECT id_applicant, last_name FROM mvd_database_course_work.applicants_table;", conncetion_string);
            var status_application_table = MySQLData.MySqlExecuteData.SqlReturnDataset("SELECT id_status_application,status_application_name FROM mvd_database_course_work.status_application;", conncetion_string);
            var personal_table = MySQLData.MySqlExecuteData.SqlReturnDataset("SELECT id_personal, last_name FROM mvd_database_course_work.personal_table;", conncetion_string);
            var form_entrance_table = MySQLData.MySqlExecuteData.SqlReturnDataset("SELECT id_form_entrance,form_name FROM mvd_database_course_work.forms_entrance_table;", conncetion_string);
            var form_reaction_table = MySQLData.MySqlExecuteData.SqlReturnDataset("SELECT id_reaction_form,reacion_name FROM mvd_database_course_work.reaction_form_table;", conncetion_string);
            combobox_items_added(type_incident_table, comboBox1, "incident_name", "id_type_incident");
            combobox_items_added(time_rewiew_table,comboBox6, "how_long_days", "id_time_review");
            combobox_items_added(applicant_table,comboBox3,"last_name","id_applicant");
            combobox_items_added(status_application_table, comboBox2, "status_application_name", "id_status_application");
            combobox_items_added(personal_table,comboBox4,"last_name","id_personal");
            combobox_items_added(form_entrance_table,comboBox5, "form_name", "id_form_entrance");
            combobox_items_added(form_reaction_table,comboBox7, "reacion_name", "id_reaction_form");
            // string value_of_combobox = Convert.ToString(comboBox1.SelectedItem);
        }
        //Добавление заявителя, нужен триггер на таблицу
        private void added_applicant_table(object sender, EventArgs e)
        {
            var added_data = dateTimePicker1.Value.Year.ToString() + "-"+dateTimePicker1.Value.Month.ToString() +"-"+ dateTimePicker1.Value.Day.ToString();
            var added_aplicant = MySQLData.MySqlExecute.SqlNoneQuery("call mvd_database_course_work.added_applicant('"+ textBox16.Text + "', '"+ textBox15.Text+"', '"+ textBox14.Text +"', '"+ textBox10.Text+"', '"+ textBox11.Text +"', '" + added_data + "');", conncetion_string);
            if (added_aplicant.HasError)
                MessageBox.Show(added_aplicant.ErrorText);
            else
            {
                MessageBox.Show("Успешно!", "Уведомление");
                panel15.Visible = false;
            }
        }

        private void added_member_ivent_table(object sender, EventArgs e)
        {
            var added_data = dateTimePicker2.Value.Year.ToString() + "-" + dateTimePicker2.Value.Month.ToString() + "-" + dateTimePicker2.Value.Day.ToString();
            var added_aplicant = MySQLData.MySqlExecute.SqlNoneQuery("call mvd_database_course_work.added_member_ivent('" + textBox16.Text + "', '" + textBox15.Text + "', '" + textBox14.Text + "', '" + textBox10.Text + "', '" + textBox11.Text + "', '" + added_data + "');", conncetion_string);
            if (added_aplicant.HasError)
                MessageBox.Show(added_aplicant.ErrorText);
            else
            {
                MessageBox.Show("Успешно!", "Уведомление");
                panel16.Visible = false;
            }
        }
        private void added_application_table(object sender, EventArgs e)
        {
            var added_data = dateTimePicker3.Value.Year.ToString() + "-" + dateTimePicker3.Value.Month.ToString() + "-" + dateTimePicker3.Value.Day.ToString();
            var added_application = MySQLData.MySqlExecute.SqlNoneQuery("call mvd_database_course_work.entrance_KUSP("+ comboBox1.SelectedIndex+1 + ",  " + comboBox2.SelectedIndex+1 + ", '"+ added_data + "', " + comboBox3.SelectedIndex+1 + ", " + comboBox4.SelectedIndex+1 + ", " + comboBox5.SelectedIndex+1 + ", " + comboBox6.SelectedIndex+1 + ", " + comboBox7.SelectedIndex+1 + ", '"+ textBox13.Text + "');", conncetion_string);
            if (added_application.HasError)
                MessageBox.Show(added_application.ErrorText);
            else
            {
                MessageBox.Show("Успешно!", "Уведомление");
                panel17.Visible = false;
            }
        }

        private void selected_age_of_member_ivent(object sender, EventArgs e)
        {
            analitycs_view("Возраст");
        }

        private void selected_applications(object sender, EventArgs e)
        {
            analitycs_view("Дата1");
        }

        private void incident_information(object sender, EventArgs e)
        {
            analitycs_view("Дата2");
        }
        void analitycs_view(string name)
        {
            panel18.Visible = true;
            switch (name)
            {
                case "Дата1":
                    break;
                case "Дата2":
                    label54.Text = "Дата  с";
                    break;
                case "Возраст":
                    label54.Text = "Возраст с";
                    label55.Text = "Возраст по";
                    maskedTextBox1.Mask = maskedTextBox2.Mask = "000";                   
                    break;
            }
        }
        private void analytics_click(object sender, EventArgs e)
        {
            MySQLData.MySqlExecuteData.MyResultData resultData=null;
            panel18.Visible = false;
            string date1 = "";
            string date2 = "";
            switch (label54.Text)
            {
                case "Дата с":
                    date1 = parse_str_to_datetime(maskedTextBox1.Text);
                    date2 = parse_str_to_datetime(maskedTextBox2.Text);
                    resultData = MySQLData.MySqlExecuteData.SqlReturnDataset("call mvd_database_course_work.incident_information_from_to('" + date1 + "', '" + date2 + "');", conncetion_string);
                    break;
                case "Дата  с":
                    date1 = parse_str_to_datetime(maskedTextBox1.Text);
                    date2 = parse_str_to_datetime(maskedTextBox2.Text);
                    resultData = MySQLData.MySqlExecuteData.SqlReturnDataset("call mvd_database_course_work.selected_applications('" + date1 + "', '" + date2 + "');", conncetion_string);
                    break;
                case "Возраст с":
                    date1 = maskedTextBox1.Text;
                    date2 = maskedTextBox2.Text;
                    resultData = MySQLData.MySqlExecuteData.SqlReturnDataset("call mvd_database_course_work.selected_age_of_members_ivent(" + date1 + ", " + date2+ ");", conncetion_string);
                    break;
            }
            if (resultData.HasError)
                MessageBox.Show(resultData.ErrorText);
            else
            {
                dataGridView5.DataSource = resultData.ResultData;
                current_dataGrid = dataGridView5;
                panel19.Visible = true;
                panel19.BringToFront();
            }
        }
        string parse_str_to_datetime(string date)
        {
            DateTime dateTime = DateTime.Parse(date);
            string new_datetime = dateTime.Year + "-" + dateTime.Month + "-" + dateTime.Day;
            return new_datetime;
        }

        private void find_rows(object sender, EventArgs e)
        {
            panel20.Visible = true;
            for (int i = 0; (i <= (current_dataGrid.Columns.Count - 1)); i++)
            {
                dataGrid_columns_name.Add(current_dataGrid.Columns[i].HeaderCell.Value);
            }
            comboBox8.DataSource = dataGrid_columns_name;
        }
        private void find_rows_in_datagrid(object sender, EventArgs e)
        {
            var column_index=dataGrid_columns_name.IndexOf(comboBox8.SelectedItem);
            if (textBox20.Text != "")
            {
                for (int i = 0; i < current_dataGrid.RowCount; i++)
                {
                    current_dataGrid.Rows[i].Selected = false;
                    if (current_dataGrid.Rows[i].Cells[column_index].Value != null)
                    {
                        if (current_dataGrid.Rows[i].Cells[column_index].Value.ToString().ToLower().Contains(textBox20.Text.ToLower()))
                        {
                            current_dataGrid.Rows[i].Cells[column_index].Style.BackColor = Color.Red;
                        }
                        else
                        {
                            current_dataGrid.Rows[i].Cells[column_index].Style.BackColor = Color.White;
                        }
                    }
                }
            }
        
            else
            {
                for (int i = 0; i < current_dataGrid.RowCount; i++)
                    for (int j = 0; j < current_dataGrid.ColumnCount; j++)
                        current_dataGrid.Rows[i].Cells[j].Style.BackColor = Color.White;
            }
        }
    }
}
