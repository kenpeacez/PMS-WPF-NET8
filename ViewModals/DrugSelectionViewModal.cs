using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SQLite;
using System.Runtime.CompilerServices;
using System.Windows;

public class DrugSelectionViewModel : INotifyPropertyChanged
{
    // ObservableCollection for ComboBox items
    public ObservableCollection<string> DrugNames { get; set; } = new ObservableCollection<string>();

    // Dictionary to store drug details
    public Dictionary<string, NewDrugModal> DrugDetails { get; set; } = new Dictionary<string, NewDrugModal>();

    // Drug 1 properties
    private string _selectedDrug1;
    private string _strength1;
    private string _unit1;
    private string _prescriberCategory1;

    public string SelectedDrug1
    {
        get => _selectedDrug1;
        set
        {
            _selectedDrug1 = value;
            OnPropertyChanged();
            UpdateDrugDetails1();
        }
    }

    public string Strength1
    {
        get => _strength1;
        set { _strength1 = value; OnPropertyChanged(); }
    }

    public string Unit1
    {
        get => _unit1;
        set { _unit1 = value; OnPropertyChanged(); }
    }

    public string PrescriberCategory1
    {
        get => _prescriberCategory1;
        set { _prescriberCategory1 = value; OnPropertyChanged(); }
    }

    // Drug 2 properties
    private string _selectedDrug2;
    private string _strength2;
    private string _unit2;
    private string _prescriberCategory2;

    public string SelectedDrug2
    {
        get => _selectedDrug2;
        set
        {
            _selectedDrug2 = value;
            OnPropertyChanged();
            UpdateDrugDetails2();
        }
    }

    public string Strength2
    {
        get => _strength2;
        set { _strength2 = value; OnPropertyChanged(); }
    }

    public string Unit2
    {
        get => _unit2;
        set { _unit2 = value; OnPropertyChanged(); }
    }

    public string PrescriberCategory2
    {
        get => _prescriberCategory2;
        set { _prescriberCategory2 = value; OnPropertyChanged(); }
    }

    // Method to update drug details for Drug 1
    private void UpdateDrugDetails1()
    {
        if (DrugDetails.ContainsKey(SelectedDrug1))
        {
            Strength1 = DrugDetails[SelectedDrug1].Strength;
            Unit1 = DrugDetails[SelectedDrug1].Unit;
            PrescriberCategory1 = DrugDetails[SelectedDrug1].PrescriberCategory;
        }
    }

    // Method to update drug details for Drug 2
    private void UpdateDrugDetails2()
    {
        if (DrugDetails.ContainsKey(SelectedDrug2))
        {
            Strength2 = DrugDetails[SelectedDrug2].Strength;
            Unit2 = DrugDetails[SelectedDrug2].Unit;
            PrescriberCategory2 = DrugDetails[SelectedDrug2].PrescriberCategory;
        }
    }

    // Load drug data from database
    public void LoadData()
    {
        try
        {
            using (SQLiteConnection con = new SQLiteConnection("Data Source=./SQLitedb.db;Version=3;"))
            {
                con.Open();
                string query = "SELECT * FROM Drugs";
                using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    DrugNames.Clear();
                    DrugDetails.Clear();

                    while (reader.Read())
                    {
                        string drugName = reader["DrugName"].ToString();
                        DrugNames.Add(drugName);

                        var drug = new NewDrugModal
                        {
                            DrugID = Convert.ToInt32(reader["DrugID"]),
                            DrugName = drugName,
                            Strength = reader["Strength"].ToString(),
                            Unit = reader["Unit"].ToString(),
                            PrescriberCategory = reader["PrescriberCategory"].ToString()
                        };

                        DrugDetails[drugName] = drug;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error loading data: " + ex.Message);
        }
    }

    // Notify property changes
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
