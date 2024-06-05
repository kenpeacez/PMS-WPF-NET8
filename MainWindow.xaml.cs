using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace PMS_WPF_NET8
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 


    public partial class MainWindow : Window
    {



        public MainWindow()
        {
            InitializeComponent();
            InitializeAll();
            SetServer();
            LoadDataToDrugComboBox();
            DGV_Load_Medicine();

        }

        public void InitializeAll() 
        {
            txtDBServerAddress.Text = Properties.Settings.Default.dbServerAddress;
            txtDBUserID.Text = Properties.Settings.Default.dbUserID;
            txtDBPassword.Password = Properties.Settings.Default.dbPassword;
            txtDBName.Text = Properties.Settings.Default.dbName;


        }

        public static class DB
        {
            static string connstring = string.Format("Server={0}; database={1}; UID={2}; password={3}",
            Properties.Settings.Default.dbServerAddress,
            Properties.Settings.Default.dbName,
            Properties.Settings.Default.dbUserID,
            Properties.Settings.Default.dbPassword);
            public static MySqlConnection conn { get; set; } = new MySqlConnection(connstring);

            public static MySqlCommand cmd { get; set; } = new MySqlCommand();

            
            
        }
  

        public static void SetServer()
        {
            
            try
            {
                var dbCon = DBConnection.Instance();
                if (dbCon.DBConnect())
                {
                    //Connection to Database successful
                    //MessageBox.Show("Connection Success");
                    dbCon.Close();
                }
                
            }
            catch (Exception e) { MessageBox.Show(e.Message, "Database Initialization Error"); }
        }

        public void LoadDataToDrugComboBox() {
            //BUGFIXED#1: Temporary stores the current selected index to prevent any loses
            //and restore it after changes made to drug list to prevent 
            var tempDrug1 = cbDrug1.SelectedIndex;
            var tempDrug2 = cbDrug2.SelectedIndex;
            var tempDrug3 = cbDrug3.SelectedIndex;
            var tempDrug4 = cbDrug4.SelectedIndex;
            var tempDrug5 = cbDrug5.SelectedIndex;
            var tempDrug6 = cbDrug6.SelectedIndex;
            var tempDrug7 = cbDrug7.SelectedIndex;
            var tempDrug8 = cbDrug8.SelectedIndex;
            var tempDrug9 = cbDrug9.SelectedIndex;
            var tempDrug10 = cbDrug10.SelectedIndex;
            cbDrug1.Items.Clear();
            cbDrug2.Items.Clear();
            cbDrug3.Items.Clear();
            cbDrug4.Items.Clear();
            cbDrug5.Items.Clear();
            cbDrug6.Items.Clear();
            cbDrug7.Items.Clear();
            cbDrug8.Items.Clear();
            cbDrug9.Items.Clear();
            cbDrug10.Items.Clear();

            // Assuming 'conn' is a valid and open MySqlConnection
            string connstring = string.Format("Server={0}; database={1}; UID={2}; password={3}",
                Properties.Settings.Default.dbServerAddress,
                Properties.Settings.Default.dbName,
                Properties.Settings.Default.dbUserID,
                Properties.Settings.Default.dbPassword);


            using (MySqlConnection conn = new MySqlConnection(connstring))
            {
                try
                {
                    conn.Open();

                    MySqlCommand cmd = new MySqlCommand("SELECT DrugName FROM drugtable WHERE DrugName not like '%Insulin%'", conn);
                    DataTable dt = new DataTable();
                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                    string drugnamedb;

                    da.Fill(dt);
                    foreach (DataRow row in dt.Rows)
                    {
                        drugnamedb = row["DrugName"].ToString();
                        //col.Add(drugnamedb);

                        cbDrug1.Items.Add(drugnamedb);
                        cbDrug2.Items.Add(drugnamedb);
                        cbDrug3.Items.Add(drugnamedb);
                        cbDrug4.Items.Add(drugnamedb);
                        cbDrug5.Items.Add(drugnamedb);
                        cbDrug6.Items.Add(drugnamedb);
                        cbDrug7.Items.Add(drugnamedb);
                        cbDrug8.Items.Add(drugnamedb);
                        cbDrug9.Items.Add(drugnamedb);
                        cbDrug10.Items.Add(drugnamedb);
                    }

                    conn.Close();
                    da.Dispose();
                }

                catch (Exception ex) { conn.Close(); MessageBox.Show("An error occured: " + ex.Message); }
            }
        }
        private void SavePatientButton_Click(object sender, RoutedEventArgs e)
        {
            SavePatient();
        }
        public void SavePatient() { 
        string PatientName = txtName.Text;
        string ICNo = txtICNo.Text;
        string DateCollect = dtpDateCollection.Text;
        string DateRegister = dtpDateSaved.Text;
        string DateSeeDoctor = dtpDateSeeDoctor.Text;

        string ICREGEX = @"^((\d{2}(?!0229))|([02468][048]|[13579][26])(?=0229))(0[1-9]|1[0-2])(0[1-9]|[12]\d|(?<!02)30|(?<!02|4|6|9|11)31)-(\d{2})-(\d{4})$";
        //Perform validations check
        //Name Check
        if (PatientName.Length == 0)
            {
                MessageBox.Show("Error. Name cannot be empty");
                return;
            }
        //IC Check
          if (chboxNoICNumber.IsChecked == false)
            {
                if (ICNo.Length < 14)
                {
                    MessageBox.Show("Error. IC Number must be 12 digit with dash");
                    return;
                }
                else if (Regex.IsMatch(ICNo,ICREGEX) == false)
                {
                    MessageBox.Show("Error. IC No. Regex Formatting is Invalid. Please check and try again");
                    return;
                }
            }
          //Drug input check
          if (cbDrug1.SelectedIndex >= 0)
            {
                if (txtDoseD1.Text.Length == 0 || txtFreqD1.Text.Length == 0 || txtDurationD1.Text.Length == 0 || txtQTYD1.Text.Length == 0)
                {
                    MessageBox.Show("Drug 1 input error.", "Drug 1 Error");
                }
            }
        
        }
        private void UpdateDatabaseButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateDatabase();
        }
        private void UpdateDatabase()
        {
            string connstring = string.Format("Server={0}; database={1}; UID={2}; password={3}",
            Properties.Settings.Default.dbServerAddress,
            Properties.Settings.Default.dbName,
            Properties.Settings.Default.dbUserID,
            Properties.Settings.Default.dbPassword);


            using (MySqlConnection conn = new MySqlConnection(connstring)) {
                try
                {
                    conn.ClearAllPoolsAsync();
                    conn.Close();
                    conn.Open();

                    foreach (var item in DataGrid1.Items)
                    {
                        // Assuming the DataGrid's ItemsSource is a DataTable or a list of a custom class
                        //var row = item as DataRowView;
                        if (item is not DataRowView row) continue;
                        //if (row == null) continue;

                        string drugName = row["Drug Name"]?.ToString();
                        string strength = row["Strength"]?.ToString();
                        string unit = row["Unit"]?.ToString();
                        string dosageForm = row["Dosage Form"]?.ToString();
                        string prescriberCategory = row["Prescriber Category"]?.ToString();
                        string defaultMaxQTY = row["Default Max QTY"]?.ToString();
                        string remark = row["Remark"]?.ToString();


                        string query = "UPDATE drugtable SET Strength=@Strength, Unit=@Unit, DosageForm=@DosageForm, PrescriberCategory=@PrescriberCategory, DefaultMaxQTY=@DefaultMaxQTY, Remark=@Remark WHERE DrugName=@DrugName";

                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@DrugName", drugName);
                            cmd.Parameters.AddWithValue("@Strength", strength);
                            cmd.Parameters.AddWithValue("@Unit", unit);
                            cmd.Parameters.AddWithValue("@DosageForm", dosageForm);
                            cmd.Parameters.AddWithValue("@PrescriberCategory", prescriberCategory);
                            cmd.Parameters.AddWithValue("@DefaultMaxQTY", defaultMaxQTY);
                            cmd.Parameters.AddWithValue("@Remark", remark);

                            cmd.ExecuteNonQuery();
                            //cmd.Parameters.Clear();
                        }
                    }
                    //conn.Dispose();
                    conn.Close();
                    MessageBox.Show("Data updated successfully.");
                    DataGrid1.Items.Refresh();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message);
                }
            }    
        }
        private void DeleteSelectedRowButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteSelectedRow();
        }
        private void DeleteSelectedRow()
        {
            string connstring = string.Format("Server={0}; database={1}; UID={2}; password={3}",
            Properties.Settings.Default.dbServerAddress,
            Properties.Settings.Default.dbName,
            Properties.Settings.Default.dbUserID,
            Properties.Settings.Default.dbPassword);
            
            //dbCon.Connection.Dispose();
            // MySqlConnection conn = dbCon.Connection;

            // Get the selected row
            if (DataGrid1.SelectedItem is DataRowView selectedRow)
            {
                string drugName = selectedRow["Drug Name"]?.ToString();
                
                using (MySqlConnection conn = new MySqlConnection(connstring))
                
                {
                    try
                    {
                        //conn.ClearAllPoolsAsync();
                        //conn.Close();
                        //conn.CloseAsync();
                        conn.Open();
                        
                        string query = "DELETE FROM drugtable WHERE DrugName=@DrugName";

                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@DrugName", drugName);
                            cmd.ExecuteNonQuery();
                        }
                        

                        // Remove the row from the DataGrid
                        //DataGrid1.ItemsSource.Rows.Remove(selectedRow.Row);
                        
                        MessageBox.Show("Row deleted successfully.");
                        //DataGrid1.Items.Refresh();
                        conn.Close();
                        DGV_Load_Medicine();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a row to delete.");
            }
        }


        public void DGV_Load_Medicine()
        {
            string connstring = string.Format("Server={0}; database={1}; UID={2}; password={3}",
        Properties.Settings.Default.dbServerAddress,
        Properties.Settings.Default.dbName,
        Properties.Settings.Default.dbUserID,
        Properties.Settings.Default.dbPassword);

            //conn.Open();
            // Assuming 'DataGridView1' is a valid DataGridView control on your form
            DataGrid1.ItemsSource = null;
            DataGrid1.Items.Clear();
            DataGrid1.Columns.Clear();
            DataGrid1.Items.Refresh();
            
            
            
            DataTable newTable = new DataTable();

            // Add columns
            DataColumn drugNameColumn = new DataColumn("Drug Name");
            drugNameColumn.ReadOnly = true;
            newTable.Columns.Add(drugNameColumn);

            //newTable.Columns.Add("Drug Name");
            newTable.Columns.Add("Strength");
            newTable.Columns.Add("Unit");
            newTable.Columns.Add("Dosage Form");
            newTable.Columns.Add("Prescriber Category");
            newTable.Columns.Add("Default Max QTY");
            newTable.Columns.Add("Remark");
            

            // Data Grid View Method to Get Data from MYSQL Database
            using (MySqlConnection conn = new MySqlConnection(connstring))
                try
            {
                // Assuming 'conn' is a valid MySqlConnection
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM drugtable", conn);
                MySqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    newTable.Rows.Add(dr["DrugName"], dr["Strength"], dr["Unit"], dr["DosageForm"], dr["PrescriberCategory"], dr["DefaultMaxQTY"], dr["Remark"]);
                }

                DataGrid1.ItemsSource = newTable.DefaultView;
                dr.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        public void execSqlRead(String req)
        {
            var dbCon = DBConnection.Instance();
            var conn = dbCon.Connection;
            MySqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = req;
            MySqlDataReader myReader;
            myReader = cmd.ExecuteReader();  //stop here
            try
            {
                while (myReader.Read())
                {
                    Console.WriteLine(myReader.GetString(0));
                }
            }
            catch (Exception e) { MessageBox.Show(e.Message, "Database Error"); }
            finally
            {
                Console.WriteLine("Success");
                dbCon.Close();
            }

            //return myReader;
        }

    }

}
