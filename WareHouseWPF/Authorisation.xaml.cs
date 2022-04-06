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
using MySql.Data.MySqlClient;

namespace WareHouseWPF
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class Authorisation : Window
	{
		MySqlConnection con = new MySqlConnection("server=localhost;user=root;port=3306;password=1234;database=warehouse");
		public static int idEmployee;
		public static string fullnameEmployee;
		public Authorisation()
		{
			InitializeComponent();
		}

		private void Login_button_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				con.Open();
				con.Close();
			}
			catch
			{
				con.Close();
				MessageBox.Show("Не удалось подключиться к базе данных");
				Application.Current.Shutdown();
				return;
			}
			MySqlCommand cmd = new MySqlCommand("select Id_employee, concat(SecondName, ' ', FirstName, ' ', " +
				"Patronymic), Login, Password from employees", con);
			con.Open();
			MySqlDataReader reader = cmd.ExecuteReader();
			while (reader.Read())
			{
				if (reader.GetString(2) == Login_textBox.Text && reader.GetString(3) == Password_textBox.Password)
				{
					idEmployee = reader.GetInt32(0);
					fullnameEmployee = reader.GetString(1);
					Catalogs catalogs = new Catalogs();
					catalogs.Show();
					Hide();
					Login_textBox.Clear();
					Password_textBox.Clear();
					reader.Close();
					con.Close();
					MessageBox.Show("Вы успешно вошли, " + fullnameEmployee);
					return;
				}
			}
			reader.Close();
			con.Close();
			MessageBox.Show("Неверный логин или пароль");
		}

		private void Close_button_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
