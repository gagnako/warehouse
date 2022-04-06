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
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using System.Data;

namespace WareHouseWPF
{
	/// <summary>
	/// Логика взаимодействия для Catalogs.xaml
	/// </summary>
	public partial class Catalogs : Window
	{
		MySqlConnection con = new MySqlConnection("server=localhost;user=root;port=3306;password=1234;database=warehouse");
		bool isWaybill = false;
		public Catalogs()
		{
			InitializeComponent();
		}

		private void CatalogsWndw_Loaded(object sender, RoutedEventArgs e)
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
			GetSuppliers("select * from suppliers");
			GetClients("select * from clients");
			GetGoods("select * from goods");
		}
		private void GetSuppliers(string zapros)
		{
			NameSupplier_textBox.Clear();
			AddressSupplier_textBox.Clear();
			PhoneSupplier_maskedBox.Clear();
			INN_maskedBox.Clear();
			Invoice_maskedBox.Clear();
			Director_textBox.Clear();
			MySqlDataAdapter adapter = new MySqlDataAdapter(zapros, con);
			DataTable dataTable = new DataTable();
			con.Open();
			adapter.Fill(dataTable);
			con.Close();
			Suppliers_dataGrid.ItemsSource = dataTable.DefaultView;
		}

		private void GetClients(string zapros)
		{
			SecondName_textBox.Clear();
			FirstName_textBox.Clear();
			Patronymic_textBox.Clear();
			AddressClient_textBox.Clear();
			PhoneClient_maskedBox.Clear();
			MySqlDataAdapter adapter = new MySqlDataAdapter(zapros, con);
			DataTable dataTable = new DataTable();
			con.Open();
			adapter.Fill(dataTable);
			con.Close();
			Clients_dataGrid.ItemsSource = dataTable.DefaultView;
		}

		private void GetGoods(string zapros)
		{
			NameGoods_textBox.Clear();
			Cost_numeric.Value = 0;
			MySqlDataAdapter adapter = new MySqlDataAdapter(zapros, con);
			DataTable dataTable = new DataTable();
			con.Open();
			adapter.Fill(dataTable);
			con.Close();
			Goods_dataGrid.ItemsSource = dataTable.DefaultView;
		}

		private void Suppliers_dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			DataRowView currentRow = Suppliers_dataGrid.SelectedItem as DataRowView;
			if (currentRow != null)
			{
				NameSupplier_textBox.Text = currentRow[1].ToString();
				AddressSupplier_textBox.Text = currentRow[2].ToString();
				PhoneSupplier_maskedBox.Text = currentRow[3].ToString();
				INN_maskedBox.Text = currentRow[4].ToString();
				Invoice_maskedBox.Text = currentRow[5].ToString();
				Director_textBox.Text = currentRow[6].ToString();
			}
		}

		private void SearchSuppliers_textBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			GetSuppliers($"select * from suppliers where `Name` like '{SearchSuppliers_textBox.Text}%'");
		}

		private void AddSupplier_button_Click(object sender, RoutedEventArgs e)
		{
			if (NameSupplier_textBox.Text == "" || AddressSupplier_textBox.Text == "" || PhoneSupplier_maskedBox.Text.Length < 14
				|| INN_maskedBox.Text == "" || Invoice_maskedBox.Text == "" || Director_textBox.Text == "")
			{
				MessageBox.Show("Вы не заполнили обязательные поля");
			}
			else
			{
				MySqlCommand command = new MySqlCommand("insert into suppliers (Name, Address, Phone, INN, Invoice, Director) " +
					$"values ('{NameSupplier_textBox.Text}', '{AddressSupplier_textBox.Text}', '{PhoneSupplier_maskedBox.Text}', '{INN_maskedBox.Text}', " +
					$"'{Invoice_maskedBox.Text}', '{Director_textBox.Text}')", con);
				con.Open();
				command.ExecuteNonQuery();
				con.Close();
				GetSuppliers("select * from suppliers");
				SearchSuppliers_textBox.Text = "";
				MessageBox.Show("Вы добавили поставщика");
			}
		}

		private void ChangeSupplier_button_Click(object sender, RoutedEventArgs e)
		{
			DataRowView currentRow = Suppliers_dataGrid.SelectedItem as DataRowView;
			if (currentRow == null)
			{
				MessageBox.Show("Вы не выбрали поставщика");
			}
			else if (NameSupplier_textBox.Text == "" || AddressSupplier_textBox.Text == "" || PhoneSupplier_maskedBox.Text.Length < 14
				|| INN_maskedBox.Text == "" || Invoice_maskedBox.Text == "" || Director_textBox.Text == "")
			{
				MessageBox.Show("Вы не заполнили обязательные поля");
			}
			else
			{
				MySqlCommand command = new MySqlCommand($"update suppliers set Name = '{NameSupplier_textBox.Text}', " +
					$"Address = '{AddressSupplier_textBox.Text}', Phone = '{PhoneSupplier_maskedBox.Text}', " +
					$"INN = '{INN_maskedBox.Text}', Invoice = '{Invoice_maskedBox.Text}', Director = " +
					$"'{Director_textBox.Text}' where Id_supplier = '{currentRow[0]}'", con);
				con.Open();
				command.ExecuteNonQuery();
				con.Close();
				GetSuppliers("select * from suppliers");
				SearchSuppliers_textBox.Text = "";
				MessageBox.Show("Вы обновили поставщика");
			}
		}

		private void DeleteSupplier_button_Click(object sender, RoutedEventArgs e)
		{
			DataRowView currentRow = Suppliers_dataGrid.SelectedItem as DataRowView;
			if (currentRow == null)
			{
				MessageBox.Show("Вы не выбрали поставщика");
			}
			else
			{
				MySqlCommand command = new MySqlCommand($"delete from suppliers " +
					$"where Id_supplier = '{currentRow[0]}'", con);
				con.Open();
				command.ExecuteNonQuery();
				con.Close();
				GetSuppliers("select * from suppliers");
				SearchSuppliers_textBox.Text = "";
				MessageBox.Show("Вы удалили поставщика");
			}
		}

		private void SearchClients_textBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			string[] fio = new string[3];
			try
			{
				fio[0] = SearchClients_textBox.Text.Split()[0];
				fio[1] = SearchClients_textBox.Text.Split()[1];
				fio[2] = SearchClients_textBox.Text.Split()[2];
			}
			catch { }
			GetClients($"select * from clients where (SecondName like '{fio[0]}%') and (FirstName like '{fio[1]}%') and (Patronymic like '{fio[2]}%')");
		}

		private void Clients_dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			DataRowView currentRow = Clients_dataGrid.SelectedItem as DataRowView;
			if (currentRow != null)
			{
				SecondName_textBox.Text = currentRow[1].ToString();
				FirstName_textBox.Text = currentRow[2].ToString();
				Patronymic_textBox.Text = currentRow[3].ToString();
				AddressClient_textBox.Text = currentRow[4].ToString();
				PhoneClient_maskedBox.Text = currentRow[5].ToString();
			}
		}

		private void AddClient_button_Click(object sender, RoutedEventArgs e)
		{
			if (SecondName_textBox.Text == "" || FirstName_textBox.Text == "" || Patronymic_textBox.Text == ""
				|| AddressClient_textBox.Text == "" || PhoneClient_maskedBox.Text.Length < 14)
			{
				MessageBox.Show("Вы не заполнили обязательные поля");
			}
			else
			{
				MySqlCommand command = new MySqlCommand("insert into clients (SecondName, FirstName, Patronymic, Address, Phone) " +
					$"values ('{SecondName_textBox.Text}', '{FirstName_textBox.Text}', '{Patronymic_textBox.Text}', '{AddressClient_textBox.Text}', '{PhoneClient_maskedBox.Text}')", con);
				con.Open();
				command.ExecuteNonQuery();
				con.Close();
				GetClients("select * from clients");
				SearchClients_textBox.Text = "";
				MessageBox.Show("Вы добавили клиента");
			}
		}

		private void ChangeClient_button_Click(object sender, RoutedEventArgs e)
		{
			DataRowView currentRow = Clients_dataGrid.SelectedItem as DataRowView;
			if (currentRow == null)
			{
				MessageBox.Show("Вы не выбрали клиента");
			}
			else if (SecondName_textBox.Text == "" || FirstName_textBox.Text == "" || Patronymic_textBox.Text == "" || AddressClient_textBox.Text == "" || PhoneClient_maskedBox.Text.Length < 14)
			{
				MessageBox.Show("Вы не заполнили обязательные поля");
			}
			else
			{
				MySqlCommand command = new MySqlCommand($"update clients set SecondName = '{SecondName_textBox.Text}', " +
					$"FirstName = '{FirstName_textBox.Text}', Patronymic = '{Patronymic_textBox.Text}', " +
					$"Address = '{AddressClient_textBox.Text}', Phone = '{PhoneClient_maskedBox.Text}' " +
					$"where Id_client = '{currentRow[0]}'", con);
				con.Open();
				command.ExecuteNonQuery();
				con.Close();
				GetClients("select * from clients");
				SearchClients_textBox.Text = "";
				MessageBox.Show("Вы обновили клиента");
			}
		}

		private void DeleteClient_button_Click(object sender, RoutedEventArgs e)
		{
			DataRowView currentRow = Clients_dataGrid.SelectedItem as DataRowView;
			if (currentRow == null)
			{
				MessageBox.Show("Вы не выбрали клиента");
			}
			else
			{
				MySqlCommand command = new MySqlCommand($"delete from clients " +
					$"where Id_client = '{currentRow[0]}'", con);
				con.Open();
				command.ExecuteNonQuery();
				con.Close();
				GetClients("select * from clients");
				SearchClients_textBox.Text = "";
				MessageBox.Show("Вы удалили клиента");
			}
		}

		private void SearchGoods_textBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			GetGoods($"select * from goods where `Name` like '{SearchGoods_textBox.Text}%'");
		}

		private void Goods_dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			DataRowView currentRow = Goods_dataGrid.SelectedItem as DataRowView;
			if (currentRow != null)
			{
				NameGoods_textBox.Text = currentRow[1].ToString();
				Manafacturer_textBox.Text = currentRow[2].ToString();
				Cost_numeric.Value = Convert.ToUInt32(currentRow[3]);
			}
		}

		private void AddGoods_button_Click(object sender, RoutedEventArgs e)
		{
			if (NameGoods_textBox.Text == "" || Manafacturer_textBox.Text == "")
			{
				MessageBox.Show("Вы не заполнили обязательные поля");
			}
			else
			{
				MySqlCommand command = new MySqlCommand("insert into goods (Name, Manufacturer, Cost) " +
					$"values ('{NameGoods_textBox.Text}', '{Manafacturer_textBox.Text}', '{Cost_numeric.Value}')", con);
				con.Open();
				command.ExecuteNonQuery();
				con.Close();
				GetGoods("select * from goods");
				SearchGoods_textBox.Text = "";
				MessageBox.Show("Вы добавили товар");
			}
		}

		private void ChangeGoods_button_Click(object sender, RoutedEventArgs e)
		{
			DataRowView currentRow = Goods_dataGrid.SelectedItem as DataRowView;
			if (currentRow == null)
			{
				MessageBox.Show("Вы не выбрали товар");
			}
			else if (NameGoods_textBox.Text == "")
			{
				MessageBox.Show("Вы не заполнили обязательные поля");
			}
			else
			{
				MySqlCommand command = new MySqlCommand($"update goods set Name = '{NameGoods_textBox.Text}', " +
					$"Manufacturer = '{Manafacturer_textBox.Text}', Cost = '{Cost_numeric.Value}' where Id_goods = '{currentRow[0]}'", con);
				con.Open();
				command.ExecuteNonQuery();
				con.Close();
				GetGoods("select * from goods");
				SearchGoods_textBox.Text = "";
				MessageBox.Show("Вы обновили товар");
			}
		}

		private void DeleteGoods_button_Click(object sender, RoutedEventArgs e)
		{
			DataRowView currentRow = Goods_dataGrid.SelectedItem as DataRowView;
			if (currentRow == null)
			{
				MessageBox.Show("Вы не выбрали товар");
			}
			else
			{
				MySqlCommand command = new MySqlCommand($"delete from goods " +
					$"where Id_goods = '{currentRow[0]}'", con);
				con.Open();
				command.ExecuteNonQuery();
				con.Close();
				GetGoods("select * from goods");
				SearchGoods_textBox.Text = "";
				MessageBox.Show("Вы удалили товар");
			}
		}

		private void Waybills_button_Click(object sender, RoutedEventArgs e)
		{
			isWaybill = true;
			Waybills waybills = new Waybills();
			waybills.Show();
			Close();
		}
		private void Suppliers_dataGrid_AutoGeneratedColumns(object sender, EventArgs e)
		{
			Suppliers_dataGrid.Columns[0].Visibility = Visibility.Hidden;
			Suppliers_dataGrid.Columns[1].Header = "Наименование";
			Suppliers_dataGrid.Columns[2].Header = "Адрес";
			Suppliers_dataGrid.Columns[3].Header = "Телефон";
			Suppliers_dataGrid.Columns[4].Header = "ИНН";
			Suppliers_dataGrid.Columns[5].Header = "Расчетный счет";
			Suppliers_dataGrid.Columns[6].Header = "Директор";
			Suppliers_dataGrid.Columns[7].Header = "Статус";
		}

		private void Clients_dataGrid_AutoGeneratedColumns(object sender, EventArgs e)
		{
			Clients_dataGrid.Columns[0].Visibility = Visibility.Hidden;
			Clients_dataGrid.Columns[1].Header = "Фамилия";
			Clients_dataGrid.Columns[2].Header = "Имя";
			Clients_dataGrid.Columns[3].Header = "Отчество";
			Clients_dataGrid.Columns[4].Header = "Адрес";
			Clients_dataGrid.Columns[5].Header = "Телефон";
			Clients_dataGrid.Columns[6].Header = "Статус";
		}

		private void Goods_dataGrid_AutoGeneratedColumns(object sender, EventArgs e)
		{
			Goods_dataGrid.Columns[0].Visibility = Visibility.Hidden;
			Goods_dataGrid.Columns[1].Header = "Наименование";
			Goods_dataGrid.Columns[2].Header = "Производитель";
			Goods_dataGrid.Columns[3].Header = "Цена";
			Goods_dataGrid.Columns[4].Header = "Статус";
		}

		private void Exit_button_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void CatalogsWndw_Closed(object sender, EventArgs e)
		{
			if (!isWaybill)
			{
				Application.Current.Windows[0].Show();
			}
		}

		private void SFP_textBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			if (int.TryParse(e.Text, out int i))
			{
				e.Handled = true;
			}
		}
	}
}


