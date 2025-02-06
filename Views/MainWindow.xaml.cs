using System.Data.SQLite;
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
using PMS_WPF_NET8.Functions.Patient;
using PMS_WPF_NET8.functions.database;
using PMS_WPF_NET8.functions.patient;
using PMS_WPF_NET8.Functions.Database;
using System.Data.Common;
using System.Windows.Automation.Text;
using System.Data.SqlClient;
using PMS_WPF_NET8.ViewModels;


namespace PMS_WPF_NET8.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 


    public partial class MainWindow : Window
    {
        private SQLiteConnection _connection;
        private PatientFunctions _patientFunctions = new PatientFunctions();
        private MainViewModel viewModel = new MainViewModel();
        private DrugSelectionViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            InitializeAll();
            // SetServer();
            // LoadDataToDrugComboBox();
            // DGV_Load_Medicine();


        }

        public void InitializeAll() 
        {
            SQLiteDB db = new SQLiteDB();
            db.CreateTable();  // Ensure the table exists

            // Initialize the connection to your database
            _connection = new SQLiteConnection("Data Source=./SQLitedb.db;Version=3;");
            _connection.Open();  // Open the connection to the database

            var viewModel = new MainViewModel();
            this.DataContext = viewModel;
            viewModel.LoadData();  // Load data after setting the DataContext



        }

        private void SavePatientButton_Click(object sender, RoutedEventArgs e)
        {
            List<DrugInfo> drugList = GetDrugList();

            _patientFunctions.SavePatient(
                txtName.Text,
                txtICNo.Text,
                dtpDateCollection.Text,
                dtpDateSaved.Text,
                dtpDateSeeDoctor.Text,
                chboxNoICNumber.IsChecked == true,
                drugList
            );
        }

        private List<DrugInfo> GetDrugList()
        {
            List<DrugInfo> drugList = new List<DrugInfo>();

            for (int i = 1; i <= 20; i++) // Assuming up to 20 drug inputs
            {
                var comboBox = FindName($"cbDrug{i}") as System.Windows.Controls.ComboBox;
                var doseBox = FindName($"txtDoseD{i}") as System.Windows.Controls.TextBox;
                var freqBox = FindName($"txtFreqD{i}") as System.Windows.Controls.TextBox;
                var durationBox = FindName($"txtDurationD{i}") as System.Windows.Controls.TextBox;
                var qtyBox = FindName($"txtQTYD{i}") as System.Windows.Controls.TextBox;

                if (comboBox != null && comboBox.SelectedIndex >= 0)
                {
                    drugList.Add(new DrugInfo(i, comboBox.Text, doseBox.Text, freqBox.Text, durationBox.Text, qtyBox.Text));
                }
            }

            return drugList;
        }

        private void btnAddNewDrug_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Create a new drug object from input fields
                NewDrugModal newDrug = new NewDrugModal
                {
                    DrugName = txtNewDrugName.Text,
                    Strength = txtDrugStr.Text,
                    Unit = txtDrugUnit.Text,
                    DosageForm = txtDrugDoseForm.Text,
                    PrescriberCategory = txtDrugPresCat.Text,
                    Remark = txtDrugRemark.Text,
                    DefaultMaxQty = int.Parse(txtDefaultMaxQty.Text)
                };

                // Initialize DrugFunctions with the database connection
                DrugFunctions drugFunctions = new DrugFunctions(_connection);

                // Save new drug to the database
                drugFunctions.SaveNewDrug(newDrug);

                // Refresh the DataGrid
                viewModel.LoadData();

                // Clear input fields after adding
                ClearDrugInputFields();

                // Show success message
                MessageBox.Show("New drug saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void UpdateDatabaseButton_Click(object sender, RoutedEventArgs e)
        {
            viewModel.IsReadOnly = !viewModel.IsReadOnly; // Toggle ReadOnly
        }

        private void DeleteSelectedRowButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LoadDrugs()
        {
            
            try
            {
                // Assuming _connection is already initialized
                DrugFunctions drugFunctions = new DrugFunctions(_connection);

                // Get all drugs from the database
                List<NewDrugInfo> drugs = drugFunctions.GetAllDrugs();

                // Set the DataGrid's ItemsSource to the list of drugs
                // DataGrid1.ItemsSource = drugs;

                // MessageBox.Show("Drugs loaded successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void ClearDrugInputFields()
        {
            txtNewDrugName.Text = string.Empty;
            txtDrugStr.Text = string.Empty;
            txtDrugUnit.Text = string.Empty;
            txtDrugDoseForm.Text = string.Empty;
            txtDrugPresCat.Text = string.Empty;
            txtDrugRemark.Text = string.Empty;
            txtDefaultMaxQty.Text = string.Empty; // Reset to default value (or string.Empty if needed)
        }



    }

}
