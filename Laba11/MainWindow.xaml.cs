using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Data.SqlClient;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections;
using System.IO;
using Microsoft.Win32;

namespace Laba11
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string insertData = @"INSERT INTO [Table]
                         (Number, Type, Balance, [Date Of Open], [Sms Information], [Internet Banking], Pasport, FLS, [Date Of Birthday],
                            Photo, [Type Of Operation], Sum, Date)
                          VALUES(@Number, @Type, @Balance, @Date_Of_Open, @Sms_Information, @Internet_Banking, @Pasport, @FLS, @Date_Of_Birthday,
                          @Photo, @Type_Of_Operation, @Sum, @Date)";
        private string deleteData = @"DELETE FROM [Table] where Number =@deleteNumber";
        private string saveChanges = @"UPDATE       [Table]
                    SET Type =@type, Balance =@balance, [Date Of Open] =@open, [Sms Information] =@sms, [Internet Banking] =@banking,
                    Pasport =@pasport, FLS =@fls, [Date Of Birthday] =@birthday, Photo =@photo, [Type Of Operation] =@typeOper, Sum =@sum, 
                    Date =@date where Number =@number";
        private SqlConnection connection = null;
        private string orderbySum = "Select * from [Table] order by Sum";
        private string orderbyFLS = "Select * from [Table] order by FLS";
        private string orderbyDate = "Select * from [Table] order by [Date Of Open]";

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                SqlConnectionStringBuilder sqlConnection = new SqlConnectionStringBuilder();
                sqlConnection.DataSource = @".\SQLEXPRESS";
                sqlConnection.InitialCatalog = "Laba11";
                sqlConnection.IntegratedSecurity = true;
                connection = new SqlConnection(sqlConnection.ConnectionString);
                try
                {
                    connection.Open();
                }
                catch (Exception ex)
                {
                    SqlConnectionStringBuilder sqlConnection1 = new SqlConnectionStringBuilder();
                    sqlConnection1.DataSource = @".\SQLEXPRESS";
                    sqlConnection1.InitialCatalog = "master";
                    sqlConnection1.IntegratedSecurity = true;
                    connection = new SqlConnection(sqlConnection1.ConnectionString);
                    try
                    {
                        connection.Open();
                        if (connection.State == ConnectionState.Open)
                        {
                            string temp = "CREATE DATABASE Laba11";
                            SqlCommand sqlCommand = new SqlCommand(temp, connection);
                            sqlCommand.ExecuteNonQuery();
                            temp = "use Laba11";
                            sqlCommand = new SqlCommand(temp, connection);
                            sqlCommand.ExecuteNonQuery();
                            temp = @"CREATE TABLE [dbo].[Table]
                            (
                            	[Number] INT NOT NULL PRIMARY KEY, 
                                [Type] NCHAR(10), 
                                [Balance] INT, 
                                [Date Of Open] DATE, 
                                [Sms Information] BIT, 
                                [Internet Banking] BIT , 
                                [Pasport] NCHAR(10),
                            	[FLS] NCHAR(30), 
                                [Date Of Birthday] DATE,
                            	[Photo] Image,
                            	[Type Of Operation] NCHAR(10), 
                                [Sum] INT, 
                                [Date] DATE
                            )";
                            sqlCommand = new SqlCommand(temp, connection);
                            sqlCommand.ExecuteNonQuery();

                        }
                    }
                    catch (Exception exeption)
                    {
                        MessageBox.Show(exeption.Message);
                    }

                }

                string command = @"SELECT [Table].* FROM [Table]";

                SqlDataAdapter dataAdapter = new SqlDataAdapter(command, connection);
                DataSet dataSet = new DataSet();
                dataAdapter.Fill(dataSet);
                DataView dataView = new DataView(dataSet.Tables[0]);

                dataGrid.ItemsSource = dataView;
                dataGrid.CanUserAddRows = true;
                dataGrid.CanUserSortColumns = true;
                dataGrid.IsReadOnly = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }



        private void AddPhoto_Click(object sender, EventArgs e)
        {
            if (dataGrid.SelectedItems.Count == 1)
            {
                string path = "";
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "jpg files (.jpg)|*.jpg";
                if (openFileDialog.ShowDialog() == true)
                {
                    path = openFileDialog.FileName;
                    FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read);
                    long lenght = file.Length;
                    byte[] image = new byte[lenght];
                    file.Read(image, 0, (int)lenght);
                    ((DataRowView)(dataGrid.SelectedItems[0])).Row.SetField(9, image);
                }
            }
        }

        private void ShowPhoto_Click(object sender, EventArgs e)
        {
            if (dataGrid.SelectedItems.Count == 1)
            {
                new Window_1(((byte[])((DataRowView)(dataGrid.SelectedItems[0])).Row["Photo"])).ShowDialog();
            }
        }

        private void Save_Click(object sender, EventArgs e)
        {
            SqlCommand sqlCommand = new SqlCommand(saveChanges, connection);
            if (dataGrid.SelectedItems.Count == 1)
            {

                sqlCommand.Parameters.AddWithValue("type",
                    ((DataRowView)(dataGrid.SelectedItems[0])).Row["Type"].ToString());
                sqlCommand.Parameters.AddWithValue("balance",
                    ((DataRowView)(dataGrid.SelectedItems[0])).Row["Balance"].ToString());
                sqlCommand.Parameters.AddWithValue("open",
                    ((DataRowView)(dataGrid.SelectedItems[0])).Row["Date Of Open"].ToString());
                sqlCommand.Parameters.AddWithValue("sms",
                    ((DataRowView)(dataGrid.SelectedItems[0])).Row["Sms Information"].ToString());
                sqlCommand.Parameters.AddWithValue("banking",
                    ((DataRowView)(dataGrid.SelectedItems[0])).Row["Internet Banking"].ToString());
                sqlCommand.Parameters.AddWithValue("pasport",
                    ((DataRowView)(dataGrid.SelectedItems[0])).Row["Pasport"].ToString());
                sqlCommand.Parameters.AddWithValue("fls",
                    ((DataRowView)(dataGrid.SelectedItems[0])).Row["FLS"].ToString());
                sqlCommand.Parameters.AddWithValue("birthday",
                    ((DataRowView)(dataGrid.SelectedItems[0])).Row["Date Of Birthday"].ToString());
                sqlCommand.Parameters.AddWithValue("photo",
                    ((DataRowView)(dataGrid.SelectedItems[0])).Row["Photo"]);
                sqlCommand.Parameters.AddWithValue("typeOper",
                    ((DataRowView)(dataGrid.SelectedItems[0])).Row["Type Of Operation"].ToString());
                sqlCommand.Parameters.AddWithValue("sum",
                    ((DataRowView)(dataGrid.SelectedItems[0])).Row["Sum"].ToString());
                sqlCommand.Parameters.AddWithValue("date",
                    ((DataRowView)(dataGrid.SelectedItems[0])).Row["Date"].ToString());
                sqlCommand.Parameters.AddWithValue("number",
                    ((DataRowView)(dataGrid.SelectedItems[0])).Row["Number"].ToString());
                sqlCommand.Parameters["Photo"].SqlDbType = SqlDbType.Image;

                sqlCommand.ExecuteNonQuery();

                string command = @"SELECT [Table].* FROM [Table]";

                SqlDataAdapter dataAdapter = new SqlDataAdapter(command, connection);
                DataSet dataSet = new DataSet();
                dataAdapter.Fill(dataSet);
                DataView dataView = new DataView(dataSet.Tables[0]);

                dataGrid.ItemsSource = dataView;
            }
        }


        private void Insert_Click(object sender, EventArgs e)
        {
            SqlCommand sqlCommand = new SqlCommand(insertData, connection);
            if (dataGrid.SelectedItems.Count == 1)
            {
                sqlCommand.Parameters.AddWithValue("Number",
                ((DataRowView)(dataGrid.SelectedItems[0])).Row["Number"].ToString());
                sqlCommand.Parameters.AddWithValue("Type",
                    ((DataRowView)(dataGrid.SelectedItems[0])).Row["Type"].ToString());
                sqlCommand.Parameters.AddWithValue("Balance",
                    ((DataRowView)(dataGrid.SelectedItems[0])).Row["Balance"].ToString());
                sqlCommand.Parameters.AddWithValue("Date_Of_Open",
                    ((DataRowView)(dataGrid.SelectedItems[0])).Row["Date Of Open"].ToString());
                sqlCommand.Parameters.AddWithValue("Sms_Information",
                    ((DataRowView)(dataGrid.SelectedItems[0])).Row["Sms Information"].ToString());
                sqlCommand.Parameters.AddWithValue("Internet_Banking",
                    ((DataRowView)(dataGrid.SelectedItems[0])).Row["Internet Banking"].ToString());
                sqlCommand.Parameters.AddWithValue("Pasport",
                    ((DataRowView)(dataGrid.SelectedItems[0])).Row["Pasport"].ToString());
                sqlCommand.Parameters.AddWithValue("FLS",
                    ((DataRowView)(dataGrid.SelectedItems[0])).Row["FLS"].ToString());
                sqlCommand.Parameters.AddWithValue("Date_Of_Birthday",
                    ((DataRowView)(dataGrid.SelectedItems[0])).Row["Date Of Birthday"].ToString());
                sqlCommand.Parameters.AddWithValue("Photo",
                    (((DataRowView)(dataGrid.SelectedItems[0])).Row["Photo"]));
                sqlCommand.Parameters.AddWithValue("Type_Of_Operation",
                    ((DataRowView)(dataGrid.SelectedItems[0])).Row["Type Of Operation"].ToString());
                sqlCommand.Parameters.AddWithValue("Sum",
                    ((DataRowView)(dataGrid.SelectedItems[0])).Row["Sum"].ToString());
                sqlCommand.Parameters.AddWithValue("Date",
                    ((DataRowView)(dataGrid.SelectedItems[0])).Row["Date"].ToString());
                sqlCommand.Parameters["Photo"].SqlDbType = SqlDbType.Image;
                sqlCommand.ExecuteNonQuery();

                string command = @"SELECT [Table].* FROM [Table]";

                SqlDataAdapter dataAdapter = new SqlDataAdapter(command, connection);
                DataSet dataSet = new DataSet();
                dataAdapter.Fill(dataSet);
                DataView dataView = new DataView(dataSet.Tables[0]);

                dataGrid.ItemsSource = dataView;
            }
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            SqlCommand sqlCommand = new SqlCommand(deleteData, connection);
            if (dataGrid.SelectedItems.Count == 1)
            {
                sqlCommand.Parameters.AddWithValue("deleteNumber",
                ((DataRowView)(dataGrid.SelectedItems[0])).Row["Number"].ToString());
                sqlCommand.ExecuteNonQuery();

                string command = @"SELECT [Table].* FROM [Table]";

                SqlDataAdapter dataAdapter = new SqlDataAdapter(command, connection);
                DataSet dataSet = new DataSet();
                dataAdapter.Fill(dataSet);
                DataView dataView = new DataView(dataSet.Tables[0]);

                dataGrid.ItemsSource = dataView;
            }
        }

        private void DateOfOpen_Click(object sender, EventArgs e)
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter(orderbyDate, connection);
            DataSet dataSet = new DataSet();
            dataAdapter.Fill(dataSet);
            DataView dataView = new DataView(dataSet.Tables[0]);

            dataGrid.ItemsSource = dataView;

        }
        private void Sum_Click(object sender, EventArgs e)
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter(orderbySum, connection);
            DataSet dataSet = new DataSet();
            dataAdapter.Fill(dataSet);
            DataView dataView = new DataView(dataSet.Tables[0]);

            dataGrid.ItemsSource = dataView;
        }

        private void FLS_Click(object sender, EventArgs e)
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter(orderbyFLS, connection);
            DataSet dataSet = new DataSet();
            dataAdapter.Fill(dataSet);
            DataView dataView = new DataView(dataSet.Tables[0]);

            dataGrid.ItemsSource = dataView;
        }
        private void Prev_Click(object sender, EventArgs e)
        {
            scroll.LineLeft();
        }

        private void Next_Click(object sender, EventArgs e)
        {
            scroll.LineRight();

        }
    }

}
