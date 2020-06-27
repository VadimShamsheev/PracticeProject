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
    public partial class AddWindow : Window
    {
        MySqlConnection conn = DBUtils.GetDBConnection();
        MySqlCommand cmd = new MySqlCommand();
        string sql;
        public AddWindow() 
        {
            InitializeComponent();
            conn.Open();
            cmd.Connection = conn;
            cmd.Parameters.Add("@worker", MySqlDbType.Int32);
            cmd.Parameters.Add("@name", MySqlDbType.String);
            cmd.Parameters.Add("@diff", MySqlDbType.Int32);
            cmd.Parameters.Add("@status", MySqlDbType.String);
            cmd.Parameters.Add("@nature", MySqlDbType.String);
            cmd.Parameters.Add("@time", MySqlDbType.Int32);

            sql = $"SELECT name from users where id in (SELECT executor_id from subordinates where manager_id = {MainWindow.userId})";
            cmd.CommandText = sql;
            using (DbDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        for (int i = 0; i<reader.FieldCount; i++)
                        {
                            worker.Items.Add(reader[i].ToString());
                        }
                    }
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                sql = $"select id from users where name = '{worker.Text}'";
                cmd.CommandText = sql;
                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            cmd.Parameters["@worker"].Value = reader[0].ToString();
                        }
                    }
                }
                cmd.Parameters["@name"].Value = name.Text;
                cmd.Parameters["@diff"].Value = diff.Text;
                cmd.Parameters["@status"].Value = status.Text;
                cmd.Parameters["@nature"].Value = nature.Text;
                cmd.Parameters["@time"].Value = time.Text;
                sql = "insert into taskfor " +
                    "(worker_id, name, difficulty, status, natureWork, time_left) " +
                    "values (@worker, @name, @diff, @status, @nature, @time)";
                cmd.CommandText = sql;
                int doCom = cmd.ExecuteNonQuery();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
