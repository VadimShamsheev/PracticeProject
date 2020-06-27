using MySql.Data.MySqlClient;
using Practika.SqlConn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.Common;

namespace Practika
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MySqlConnection conn = DBUtils.GetDBConnection();
        MySqlCommand cmd = new MySqlCommand();
        public static int userId;
        string sql;
        bool priv;
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                conn.Open();
                cmd.Connection = conn;
                cmd.Parameters.Add("@coefAnalys", MySqlDbType.Double);
                cmd.Parameters.Add("@coefSetup", MySqlDbType.Double);
                cmd.Parameters.Add("@coefService", MySqlDbType.Double);
                cmd.Parameters.Add("@coefTime", MySqlDbType.Double);
                cmd.Parameters.Add("@coefDiff", MySqlDbType.Double);
                cmd.Parameters.Add("@coefMoney", MySqlDbType.Double);
                cmd.Parameters.Add("@id", MySqlDbType.Int32);
                filterUser.Items.Add("Без фильтра");
                filterUser.Text = "Без фильтра";
            }
            catch
            {
                MessageBox.Show("Невозможно подключиться к базе данных");
            }
        }

        private void mainWindow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                sql = $"select * from users where login = '{loginSignIn.Text}' and password = '{passwordSignIn.Password.ToString()}'";
                cmd.Connection = conn;
                cmd.CommandText = sql;
                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        signInForm.Visibility = Visibility.Hidden;
                        while (reader.Read())
                        {
                            userId = reader.GetInt32(0);
                            priv = reader.GetBoolean(3);
                            if (priv)
                            {
                                mainWindowMenegerForm.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                mainWindowWorkerForm.Visibility = Visibility.Visible;
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Логин или пароль неверные");
                    }
                }
                if (priv)
                {
                    sql = $"SELECT name from users where id in (SELECT executor_id from subordinates where manager_id = {userId})";
                    cmd.CommandText = sql;
                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    filterUser.Items.Add(reader[i].ToString());
                                }
                            }
                        }
                    }
                    sql = $"select * from coefficient where id = {userId}";
                    cmd.CommandText = sql;
                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                coefAnalys.Text = reader[1].ToString();
                                coefSetup.Text = reader[2].ToString();
                                coefService.Text = reader[3].ToString();
                                coefTime.Text = reader[4].ToString();
                                coefDiff.Text = reader[5].ToString();
                                coefMoney.Text = reader[6].ToString();
                            }
                        }
                    }
                    UpdateDataGridManager($"SELECT taskfor.id, users.name, taskfor.name, difficulty, status, natureWork, time_left FROM taskfor, users WHERE worker_id in (SELECT executor_id from subordinates where manager_id = {userId}) and taskfor.worker_id = users.id");
                }
                else
                {
                    UpdateDataGridWorker($"select taskfor.id, users.name, taskfor.name, difficulty, status, natureWork, time_left from taskfor, users where worker_id = {userId} and users.id = worker_id");
                }
            }
            catch (Exception exc)
            {
                loginSignIn.Text = exc.Message;
            }

        }

        private void register_Click(object sender, RoutedEventArgs e)
        {
            signInForm.Visibility = Visibility.Hidden;
            registerForm.Visibility = Visibility.Visible;
        }

        private void coefChange_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                cmd.Parameters["@coefAnalys"].Value = coefAnalys.Text;
                cmd.Parameters["@coefSetup"].Value = coefSetup.Text;
                cmd.Parameters["@coefService"].Value = coefService.Text;
                cmd.Parameters["@coefTime"].Value = coefTime.Text;
                cmd.Parameters["@coefDiff"].Value = coefDiff.Text;
                cmd.Parameters["@coefMoney"].Value = coefMoney.Text;
                sql = $"update coefficient set coef_analys = @coefAnalys," +
                    $"coef_setup = @coefSetup," +
                    $"coef_service = @coefService," +
                    $"coef_time = @coefTime," +
                    $"coef_diff = @coefDiff," +
                    $"coef_money = @coefMoney " +
                    $"where id = {userId}";
                cmd.CommandText = sql;
                int writeCmd = cmd.ExecuteNonQuery();
                MessageBox.Show("Коэффициенты обновлены");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void UpdateDataGridManager(string sql)
        {
            todoList.ItemsSource = null;
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = sql;

            using (DbDataReader reader = cmd.ExecuteReader())
            {
                List<TasksList> tl = new List<TasksList>();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i += 7)
                        {
                            tl.Add(new TasksList() { id = reader.GetInt32(i), worker = reader[i+1].ToString(), name = reader[i + 2].ToString(), difficulty = reader.GetInt32(i + 3), status = reader[i + 4].ToString(), natureWork = reader[i + 5].ToString(), timeLeft = reader.GetInt32(i + 6) });
                        }
                        
                    }
                    todoList.ItemsSource = tl;
                }

            }
        }
        public void UpdateDataGridWorker(string sql)
        {
            todoListWorker.ItemsSource = null;
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = sql;

            using (DbDataReader reader = cmd.ExecuteReader())
            {
                List<TasksList> tl = new List<TasksList>();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i += 7)
                        {
                            tl.Add(new TasksList() { id = reader.GetInt32(i), worker = reader[i + 1].ToString(), name = reader[i + 2].ToString(), difficulty = reader.GetInt32(i + 3), status = reader[i + 4].ToString(), natureWork = reader[i + 5].ToString(), timeLeft = reader.GetInt32(i + 6) });
                        }

                    }
                    todoListWorker.ItemsSource = tl;
                }

            }
        }

        private void addTask_Click(object sender, RoutedEventArgs e)
        {
            AddWindow addTask = new AddWindow();
            addTask.Show();
        }

        private void updateTable_Click(object sender, RoutedEventArgs e)
        {
            if (filterUser.Text == "Без фильтра")
            {
                UpdateDataGridManager($"SELECT taskfor.id, users.name, taskfor.name, difficulty, status, natureWork, time_left FROM taskfor, users WHERE worker_id in (SELECT executor_id from subordinates where manager_id = {userId}) and taskfor.worker_id = users.id");
            }
            else
            {
                UpdateDataGridManager($"SELECT taskfor.id, users.name, taskfor.name, difficulty, status, natureWork, time_left FROM taskfor, users WHERE worker_id in (SELECT executor_id from subordinates where manager_id = {userId}) and taskfor.worker_id = users.id and users.name = '{filterUser.Text}'");
            }
        }
        private void updateTableWorker_Click(object sender, RoutedEventArgs e)
        {
            UpdateDataGridWorker($"select taskfor.id, users.name, taskfor.name, difficulty, status, natureWork, time_left from taskfor, users where worker_id = {userId} and users.id = worker_id");
        }

        private void updateTask_Click(object sender, RoutedEventArgs e)
        {
            TasksList path = todoList.SelectedItem as TasksList;
            UpdateWindow uw = new UpdateWindow(path.id, priv, path.worker, path.name, path.difficulty, path.status, path.natureWork, path.timeLeft);
            uw.Show();
        }
        private void deleteTask_Click(object sender, RoutedEventArgs e)
        {
            TasksList tasks = todoList.SelectedItem as TasksList;
            cmd.Parameters["@id"].Value = tasks.id;
            sql = "delete from taskfor where id = @id";
            cmd.CommandText = sql;
            int doThings = cmd.ExecuteNonQuery();
            MessageBox.Show("Данные успешно удалены");
        }

        private void workerList_Click(object sender, RoutedEventArgs e)
        {

        }

        private void todoList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TasksList tl = todoList.SelectedItem as TasksList;
            if (tl != null && (tl.status == "Выполнена" || tl.status == "Отменена"))
            {
                updateTask_Button.IsEnabled = false;
            }
            else
            {
                updateTask_Button.IsEnabled = true;
            }
        }

        private void updateTaskWorker_Click(object sender, RoutedEventArgs e)
        {
            TasksList path = todoListWorker.SelectedItem as TasksList;
            UpdateWindow uw = new UpdateWindow(path.id, priv, path.worker, path.name, path.difficulty, path.status, path.natureWork, path.timeLeft);
            uw.Show();
        }
    }

    public class TasksList
    {
        public int id { get; set; }
        public string worker { get; set; }
        public string name { get; set; }
        public int difficulty { get; set; }
        public string status { get; set; }
        public string natureWork { get; set; }
        public int timeLeft { get; set; }
    }
}
