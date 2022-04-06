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
using Word = Microsoft.Office.Interop.Word;
using System.Data;

namespace WareHouseWPF
{
	/// <summary>
	/// Логика взаимодействия для Waybills.xaml
	/// </summary>
	public partial class Waybills : Window
	{
		MySqlConnection con = new MySqlConnection("server=localhost;user=root;port=3306;password=1234;database=warehouse");
		List<string[]> goods = new List<string[]>();
		List<Goods> goodsGrid = new List<Goods>();
		List<string[]> goodsData = new List<string[]>();
		List<int> idSuppliers = new List<int>();
		List<string[]> clients = new List<string[]>();
		decimal fullCostNew = 0;
		bool isCatalogs = false;
		public Waybills()
		{
			InitializeComponent();
		}


		private void GetOtherData()
		{
			Goods_comboBox.Items.Clear();
			Supplier_comboBox.Items.Clear();
			SupplierNew_comboBox.Items.Clear();
			Client_comboBox.Items.Clear();
			ClientNew_comboBox.Items.Clear();
			Goods_dataGrid.Items.Clear();
			goods.Clear();
			goodsGrid.Clear();
			goodsData.Clear();
			MySqlCommand command = new MySqlCommand("select * from goods where GoodsStatus = 1", con);
			con.Open();
			MySqlDataReader reader = command.ExecuteReader();
			while (reader.Read())
			{
				string[] row = new string[] { reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3) };
				Goods_comboBox.Items.Add(row[1]);
				goods.Add(row);
			}
			reader.Close();
			con.Close();
			command = new MySqlCommand("select Id_supplier, Name from suppliers where SuppliersStatus = 1", con);
			con.Open();
			reader = command.ExecuteReader();
			while (reader.Read())
			{
				idSuppliers.Add(reader.GetInt32(0));
				SupplierNew_comboBox.Items.Add(reader.GetString(1));
				Supplier_comboBox.Items.Add(reader.GetString(1));
			}
			reader.Close();
			con.Close();
			command = new MySqlCommand("select Id_client, concat(SecondName, ' ', FirstName, ' ', " +
				"Patronymic), Address from clients where ClientStatus = 1", con);
			con.Open();
			reader = command.ExecuteReader();
			while (reader.Read())
			{
				string[] row = new string[] { reader.GetString(0), reader.GetString(1), reader.GetString(2) };
				ClientNew_comboBox.Items.Add(row[1]);
				Client_comboBox.Items.Add(row[1]);
				clients.Add(row);
			}
			reader.Close();
			con.Close();
		}

		private void GetWaybills(string zapros)
		{
			GetOtherData();
			DateTime_dateTimePicker.SelectedDate = DateTime.Now;
			Type_comboBox.SelectedIndex = -1;
			Supplier_comboBox.SelectedIndex = -1;
			Sdal_textBox.Clear();
			Client_comboBox.SelectedIndex = -1;
			Address_textBox.Clear();
			FullCost_textBox.Clear();
			MySqlDataAdapter adapter = new MySqlDataAdapter(zapros, con);
			DataTable dataTable = new DataTable();
			con.Open();
			adapter.Fill(dataTable);
			con.Close();
			Waybills_dataGrid.ItemsSource = dataTable.DefaultView;
			Waybills_dataGrid.Columns[0].Header = "Код накладной";
			Waybills_dataGrid.Columns[1].Header = "Дата и время";
			Waybills_dataGrid.Columns[2].Header = "Тип накладная";
			Waybills_dataGrid.Columns[3].Header = "Поставщик";
			Waybills_dataGrid.Columns[4].Header = "Сдал";
			Waybills_dataGrid.Columns[5].Header = "Принял";
			Waybills_dataGrid.Columns[6].Header = "Адрес";
			Waybills_dataGrid.Columns[7].Header = "Полная стоимость";
		}

		private void GetToWaybills()
		{
			TypeNew_comboBox.SelectedIndex = -1;
			Goods_comboBox.SelectedIndex = -1;
			Quantity_numeric.Value = 1;
			DateTimeNew_dateTimePicker.SelectedDate = DateTime.Now;
			TypeNew_comboBox.SelectedIndex = -1;
			SupplierNew_comboBox.SelectedIndex = -1;
			SdalNew_textBox.Clear();
			ClientNew_comboBox.SelectedIndex = -1;
			AddressNew_textBox.Clear();
			GoodsPrice_label.Content = "Товар*";
			FullCostNew_textBox.Clear();
		}

		private void WaybillsWndw_Loaded(object sender, RoutedEventArgs e)
		{
			GetWaybills("select Id_waybill, DateTime, Type, suppliers.Name, concat(employees.SecondName,  " +
					"' ', employees.FirstName, ' ', employees.Patronymic), concat(clients.SecondName, ' ', " +
					"clients.FirstName, ' ', clients.Patronymic), waybills.Address, FullCost from waybills join suppliers on " +
					"waybills.Id_supplier = suppliers.Id_supplier left join clients on waybills.Id_client = clients.Id_client " +
					"left join employees on waybills.Id_employee = employees.Id_employee");
			GetToWaybills();
		}


		private void Waybills_button_Click(object sender, RoutedEventArgs e)
		{
			isCatalogs = true;
			Catalogs catalogs = new Catalogs();
			catalogs.Show();
			Close();
		}

		private void SearchWaybills_textBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			GetWaybills("select Id_waybill, DateTime, Type, suppliers.Name, concat(employees.SecondName,  " +
					"' ', employees.FirstName, ' ', employees.Patronymic), concat(clients.SecondName, ' ', " +
					"clients.FirstName, ' ', clients.Patronymic), waybills.Address, FullCost from waybills join suppliers on " +
					"waybills.Id_supplier = suppliers.Id_supplier left join clients on waybills.Id_client = clients.Id_client " +
					"left join employees on waybills.Id_employee = employees.Id_employee " +
					$"where Id_waybill like '{SearchWaybills_textBox.Text}%'");
		}

		private void Waybills_dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			DataRowView currentRow = Waybills_dataGrid.SelectedItem as DataRowView;
			if (currentRow != null)
			{
				DateTime_dateTimePicker.Text = currentRow[1].ToString();
				Type_comboBox.SelectedItem = currentRow[2].ToString();
				Supplier_comboBox.SelectedItem = currentRow[3].ToString();
				Client_comboBox.SelectedItem = currentRow[5].ToString();
				Address_textBox.Text = currentRow[6].ToString();
				FullCost_textBox.Text = currentRow[7].ToString();
			}
		}

		private void ChangeWaybill_button_Click(object sender, RoutedEventArgs e)
		{
			DataRowView currentRow = Waybills_dataGrid.SelectedItem as DataRowView;
			if (currentRow == null)
			{
				MessageBox.Show("Вы не выбрали накладную");
			}
			else if (Type_comboBox.SelectedIndex == -1 || Supplier_comboBox.SelectedIndex == -1 || (Type_comboBox.SelectedIndex == 1 && Address_textBox.Text == ""))
			{
				MessageBox.Show("Вы не заполнили обязательные поля");
			}
			else
			{
				string client = "null";
				if (Client_comboBox.SelectedIndex >= 0)
				{
					client = $"'{clients[Client_comboBox.SelectedIndex][0]}'";
				}
				MySqlCommand command = new MySqlCommand($"update waybills set DateTime = '{Convert.ToDateTime(DateTime_dateTimePicker.SelectedDate).ToString("yyyy-MM-dd HH:mm")}', " +
					$"Type = '{Type_comboBox.SelectedItem}', Id_supplier = '{idSuppliers[Supplier_comboBox.SelectedIndex]}', " +
					$"Id_client = {client}, Address = " +
					$"'{Address_textBox.Text}' where Id_waybill = '{currentRow[0]}'", con);
				con.Open();
				command.ExecuteNonQuery();
				con.Close();
				GetWaybills("select Id_waybill, DateTime, Type, suppliers.Name, concat(employees.SecondName,  " +
					"' ', employees.FirstName, ' ', employees.Patronymic), concat(clients.SecondName, ' ', " +
					"clients.FirstName, ' ', clients.Patronymic), waybills.Address, FullCost from waybills join suppliers on " +
					"waybills.Id_supplier = suppliers.Id_supplier left join clients on waybills.Id_client = clients.Id_client " +
					"left join employees on waybills.Id_employee = employees.Id_employee");
				SearchWaybills_textBox.Text = "";
				MessageBox.Show("Вы обновили накладную");
			}
		}

		private void DeleteWaybill_button_Click(object sender, RoutedEventArgs e)
		{
			DataRowView currentRow = Waybills_dataGrid.SelectedItem as DataRowView;
			if (currentRow == null)
			{
				MessageBox.Show("Вы не выбрали накладную");
			}
			else
			{
				MySqlCommand command = new MySqlCommand($"delete from waybills " +
					$"where Id_waybill = '{currentRow[0]}'", con);
				con.Open();
				command.ExecuteNonQuery();
				con.Close();
				GetWaybills("select Id_waybill, DateTime, Type, suppliers.Name, concat(employees.SecondName,  " +
					"' ', employees.FirstName, ' ', employees.Patronymic), concat(clients.SecondName, ' ', " +
					"clients.FirstName, ' ', clients.Patronymic), waybills.Address, FullCost from waybills join suppliers on " +
					"waybills.Id_supplier = suppliers.Id_supplier left join clients on waybills.Id_client = clients.Id_client " +
					"left join employees on waybills.Id_employee = employees.Id_employee");
				SearchWaybills_textBox.Text = "";
				MessageBox.Show("Вы удалили накладную");
			}
		}

		private void Report_button_Click(object sender, RoutedEventArgs e)
		{
			DataRowView currentRow = Waybills_dataGrid.SelectedItem as DataRowView;
			if (currentRow == null)
			{
				MessageBox.Show("Вы не выбрали накладную");
			}
			else
			{
				int fullCost = 0;
				List<string[]> goodsTable = new List<string[]>();
				MySqlCommand commmand = new MySqlCommand("select goods.Id_goods, Name, Manufacturer, Cost, Quantity " +
					"from goods join waybillgoods on goods.Id_goods = waybillgoods.Id_goods " +
					$"where Id_waybill = '{currentRow[0]}'", con);
				con.Open();
				MySqlDataReader reader = commmand.ExecuteReader();
				while (reader.Read())
				{
					int price = reader.GetInt32(3) * reader.GetInt32(4);
					fullCost += price;
					string[] row = new string[] { reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), price.ToString() };
					goodsTable.Add(row);
				}
				reader.Close();
				con.Close();
				try
				{
					Word.Application wordApp = new Word.Application
					{
						Visible = true
					};
					object missing = Type.Missing;
					Word._Document WordDoc = wordApp.Documents.Add(
						ref missing, ref missing, ref missing, ref missing);
					object start = 0, end = 0;
					Word.Range rng = WordDoc.Range(ref start, ref end);
					rng.Text = $"{Type_comboBox.SelectedItem.ToString().ToUpper()} № {currentRow[0]}\n";
					rng.Font.Size = 12;
					rng.Font.Bold = 1;
					rng.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;

					start = WordDoc.Range().End - 1; end = WordDoc.Range().End - 1;
					rng = WordDoc.Range(ref start, ref end);
					rng.Text = $"Дата и время: {currentRow[1]}\n" +
						$"Поставщик: {currentRow[3]}\n";
					rng.Font.Size = 12;
					rng.Font.Bold = 0;
					rng.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;


					start = WordDoc.Range().End - 1; end = WordDoc.Range().End - 1;
					rng = WordDoc.Range(ref start, ref end);
					Word.Table table = WordDoc.Tables.Add(rng, goodsTable.Count + 1, 6, missing, missing);
					table.Cell(1, 1).Range.Text = "Артикул";
					table.Cell(1, 2).Range.Text = "Наименование";
					table.Cell(1, 3).Range.Text = "Производитель";
					table.Cell(1, 4).Range.Text = "Цена за ед.";
					table.Cell(1, 5).Range.Text = "Количество";
					table.Cell(1, 6).Range.Text = "Общая цена";
					for (int i = 0; i < goodsTable.Count; i++)
					{
						for (int j = 0; j < 6; j++)
						{
							table.Cell(i + 2, j + 1).Range.Text = goodsTable[i][j];
						}
					}
					table.Range.Font.Size = 11;
					table.Range.Font.Name = "Times New Roman";
					table.Columns[1].SetWidth(55, Word.WdRulerStyle.wdAdjustFirstColumn);
					table.Columns[2].SetWidth(120, Word.WdRulerStyle.wdAdjustFirstColumn);
					table.Columns[3].SetWidth(90, Word.WdRulerStyle.wdAdjustFirstColumn);
					table.Columns[4].SetWidth(65, Word.WdRulerStyle.wdAdjustFirstColumn);
					table.Columns[5].SetWidth(70, Word.WdRulerStyle.wdAdjustFirstColumn);
					table.Columns[6].SetWidth(65, Word.WdRulerStyle.wdAdjustFirstColumn);

					table.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
					table.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;

					start = WordDoc.Range().End - 1; end = WordDoc.Range().End - 1;
					rng = WordDoc.Range(ref start, ref end);
					if (Type_comboBox.SelectedIndex == 1)
					{
						
					  rng.Text = $"\nОбщая стоимость: {fullCost}\n" +
						  $"Сдал: {currentRow[4]}\n" +
						  $"Принял: {currentRow[5]}\n" +
						  $"Адрес: {currentRow[6]}\n";
					}
					else 
					{
						rng.Text = $"\nОбщая стоимость: {fullCost}\n";
							}
					rng.Font.Size = 11;
					rng.Font.Bold = 0;
					rng.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
				}
				catch { }
			}
		}

		private void AddGoods_button_Click(object sender, RoutedEventArgs e)
		{

			if (Goods_comboBox.SelectedIndex >= 0)
			{
				int index = Goods_comboBox.SelectedIndex;
				decimal price = Convert.ToDecimal(goods[index][3]) * Convert.ToDecimal(Quantity_numeric.Value);
				Goods goodsItem = new Goods { IdGoods = goods[index][0], Name = goods[index][1], Manufacturer = goods[index][2], CostEd = goods[index][3], Quantity = Quantity_numeric.Value.ToString(), FullPrice = price.ToString() };
				goodsGrid.Add(goodsItem);
				Goods_dataGrid.Items.Add(goodsItem);
				fullCostNew = 0;
				for (int i = 0; i < Goods_dataGrid.Items.Count; i++)
				{
					fullCostNew += Convert.ToDecimal(goodsGrid[i].FullPrice);
				}
				FullCostNew_textBox.Text = fullCostNew.ToString();
			}
		}

		private void DeleteGoods_button_Click(object sender, RoutedEventArgs e)
		{
			DataRowView currentRow = Goods_dataGrid.SelectedItem as DataRowView;
			int index = Goods_dataGrid.SelectedIndex;
			if (index >= 0)
			{
				Goods_dataGrid.Items.RemoveAt(index);
				fullCostNew = 0;
				for (int i = 0; i < Goods_dataGrid.Items.Count; i++)
				{
					fullCostNew += Convert.ToDecimal(goodsGrid[i].FullPrice);
				}
				FullCostNew_textBox.Text = fullCostNew.ToString();
			}
		}

		private void Goods_comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (Goods_comboBox.SelectedIndex >= 0)
			{
				GoodsPrice_label.Content = $"Товар(цена: {goods[Goods_comboBox.SelectedIndex][3]})";
			}
		}

		private void ClientNew_comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (ClientNew_comboBox.SelectedIndex >= 0)
			{
				AddressNew_textBox.Text = clients[ClientNew_comboBox.SelectedIndex][2];
			}
		}

		private void DoWatbill_button_Click(object sender, RoutedEventArgs e)
		{
			if (Goods_dataGrid.Items.Count == 0)
			{
				MessageBox.Show("Вы не добавили товары");
			}
			else if (TypeNew_comboBox.SelectedIndex == -1 || SupplierNew_comboBox.SelectedIndex == -1 || (TypeNew_comboBox.SelectedIndex == 1 && AddressNew_textBox.Text == ""))
			{
				MessageBox.Show("Вы не заполнили обязательные поля");
			}
			else
			{
				int maxId = 1;
				MySqlCommand command = new MySqlCommand("select max(Id_waybill) from waybills", con);
				con.Open();
				MySqlDataReader reader = command.ExecuteReader();
				reader.Read();
				if (!reader.IsDBNull(0))
				{
					maxId = reader.GetInt32(0) + 1;
				}
				con.Close();
				string zaprosGoods = "";
				string clientNew = "null";
				if (ClientNew_comboBox.SelectedIndex >= 0)
				{
					clientNew = $"'{clients[ClientNew_comboBox.SelectedIndex][0]}'";
				}
				string sdal = "null";
				if (SdalNew_textBox.Text != "")
				{
					sdal = $"'{Authorisation.idEmployee}'";
				}
				for (int i = 0; i < Goods_dataGrid.Items.Count; i++)
				{
					zaprosGoods += "insert into waybillgoods (Id_goods, Id_waybill, Quantity) " +
						$"values('{goodsGrid[i].IdGoods}', '{maxId}', '{goodsGrid[i].Quantity}'); ";
				}
				command = new MySqlCommand("insert into waybills values (" +
				$"'{maxId}', '{Convert.ToDateTime(DateTime_dateTimePicker.SelectedDate).ToString("yyyy-MM-dd HH:mm")}', '{TypeNew_comboBox.SelectedItem}', " +
				$"'{idSuppliers[SupplierNew_comboBox.SelectedIndex]}', {sdal}, " +
				$"{clientNew}, '{AddressNew_textBox.Text}', '{fullCostNew}'); {zaprosGoods}", con);
				con.Open();
				command.ExecuteNonQuery();
				con.Close();
				GetOtherData();
				GetToWaybills();
				GetWaybills("select Id_waybill, DateTime, Type, suppliers.Name, concat(employees.SecondName,  " +
					"' ', employees.FirstName, ' ', employees.Patronymic), concat(clients.SecondName, ' ', " +
					"clients.FirstName, ' ', clients.Patronymic), waybills.Address, FullCost from waybills join suppliers on " +
					"waybills.Id_supplier = suppliers.Id_supplier left join clients on waybills.Id_client = clients.Id_client " +
					"left join employees on waybills.Id_employee = employees.Id_employee");
				MessageBox.Show("Вы сформировали накладную");
				Waybills_tabControl.SelectedIndex = 0;
			}
		}

		private void Exit_button_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void WaybillsWndw_Closed(object sender, EventArgs e)
		{
			if (!isCatalogs)
			{
				Application.Current.Windows[0].Show();
			}
		}

		private void TypeNew_comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			AddressNew_textBox.Clear();
			SdalNew_textBox.Clear();
			SdalNew_textBox.IsEnabled = false;
			AddressNew_textBox.IsEnabled = false;
			ClientNew_comboBox.IsEnabled = false;
			ClientNew_comboBox.SelectedIndex = -1;
			if (TypeNew_comboBox.SelectedIndex == 1)
			{
				ClientNew_comboBox.IsEnabled = true;
				if (ClientNew_comboBox.Items.Count > 0)
				{
					SdalNew_textBox.IsEnabled = true;
					SdalNew_textBox.Text = Authorisation.fullnameEmployee;
					ClientNew_comboBox.SelectedIndex = 0;
					AddressNew_textBox.IsEnabled = true;
				}
			}
		}

		private void Type_comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Address_textBox.Clear();
			Sdal_textBox.Clear();
			Sdal_textBox.IsEnabled = false;
			Address_textBox.IsEnabled = false;
			Client_comboBox.IsEnabled = false;
			Client_comboBox.SelectedIndex = -1;
			if (Type_comboBox.SelectedIndex == 1)
			{
				Client_comboBox.IsEnabled = true;
				if (Client_comboBox.Items.Count > 0)
				{
					DataRowView currentRow = Waybills_dataGrid.SelectedItem as DataRowView;
					Sdal_textBox.IsEnabled = true;
					Sdal_textBox.Text = currentRow[4].ToString();
					Client_comboBox.SelectedItem = currentRow[5].ToString();
					Address_textBox.IsEnabled = true;
					Address_textBox.Text = currentRow[6].ToString();
				}
			}
		}

		private void Client_comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

		}
	}

	class Goods
	{
		public string IdGoods { get; set; }
		public string Name { get; set; }
		public string Manufacturer { get; set; }
		public string CostEd { get; set; }
		public string Quantity { get; set; }
		public string FullPrice { get; set; }
	}
}
