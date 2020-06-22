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
        int userId;
        string sql;
        public MainWindow()
        {
            InitializeComponent();
            cmd.Connection = conn;
            cmd.Parameters.Add("@coefAnalys", MySqlDbType.Double);
            cmd.Parameters.Add("@coefSetup", MySqlDbType.Double);
            cmd.Parameters.Add("@coefService", MySqlDbType.Double);
            cmd.Parameters.Add("@coefTime", MySqlDbType.Double);
            cmd.Parameters.Add("@coefDiff", MySqlDbType.Double);
            cmd.Parameters.Add("@coefMoney", MySqlDbType.Double);
        }

        private void mainWindow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                conn.Open();
                sql = $"select * from users where login = '{loginSignIn.Text}' and password = '{passwordSignIn.Password.ToString()}'";
                cmd.Connection = conn;
                cmd.CommandText = sql;
                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        signInForm.Visibility = Visibility.Hidden;
                        mainWindowMenegerForm.Visibility = Visibility.Visible;
                        while (reader.Read())
                        {
                            userId = reader.GetInt32(0);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Логин или пароль неверные");
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
                    else
                    {
                        MessageBox.Show("нет данных");
                    }
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
    }
}
