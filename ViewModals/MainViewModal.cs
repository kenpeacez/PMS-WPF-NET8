using PMS_WPF_NET8.Commands;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SQLite;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace PMS_WPF_NET8.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {

        private bool _isReadOnly = true;
        private string _selectedDrugName;
        private string _strength;
        private string _unit;
        private string _prescriberCategory;

        public ObservableCollection<NewDrugModal> DrugList { get; set; } = new ObservableCollection<NewDrugModal>();
        public ObservableCollection<string> DrugNames { get; set; } = new ObservableCollection<string>();
        public Dictionary<string, NewDrugModal> DrugDetails { get; set; } = new Dictionary<string, NewDrugModal>();

        public bool IsReadOnly
        {
            get => _isReadOnly;
            set
            {
                _isReadOnly = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand ToggleEditCommand => new RelayCommand(ToggleEdit);

        private void ToggleEdit()
        {
            if (IsReadOnly)
            {
                IsReadOnly = false;
            }
            else
            {
                SaveData();
                IsReadOnly = true;
            }
        }

        public string SelectedDrugName
        {
            get => _selectedDrugName;
            set
            {
                _selectedDrugName = value;
                OnPropertyChanged();
                UpdateDrugDetails();
            }
        }

        public string Strength
        {
            get => _strength;
            set
            {
                _strength = value;
                OnPropertyChanged();
            }
        }

        public string Unit
        {
            get => _unit;
            set
            {
                _unit = value;
                OnPropertyChanged();
            }
        }

        public string PrescriberCategory
        {
            get => _prescriberCategory;
            set
            {
                _prescriberCategory = value;
                OnPropertyChanged();
            }
        }

        // ✅ FIXED: Populate `DrugDetails` dictionary
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
                        DrugList.Clear();
                        DrugNames.Clear();
                        DrugDetails.Clear(); // Ensure it's cleared before adding

                        while (reader.Read())
                        {
                            string drugName = reader["DrugName"].ToString();

                            var drug = new NewDrugModal
                            {
                                DrugID = Convert.ToInt32(reader["DrugID"]),
                                DrugName = drugName,
                                Strength = reader["Strength"].ToString(),
                                Unit = reader["Unit"].ToString(),
                                DosageForm = reader["DosageForm"].ToString(),
                                PrescriberCategory = reader["PrescriberCategory"].ToString(),
                                Remark = reader["Remark"].ToString(),
                                DefaultMaxQty = Convert.ToInt32(reader["DefaultMaxQty"])
                            };

                            DrugList.Add(drug);
                            DrugNames.Add(drugName);
                            DrugDetails[drugName] = drug; // Store details for lookup
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }
        }

        // ✅ FIXED: Ensure `SelectedDrugName` updates label values
        private void UpdateDrugDetails()
        {
            // MessageBox.Show(SelectedDrugName);
            if (!string.IsNullOrEmpty(SelectedDrugName) && DrugDetails.ContainsKey(SelectedDrugName))
            {
                var selectedDrug = DrugDetails[SelectedDrugName];
                Strength = selectedDrug.Strength;
                Unit = selectedDrug.Unit;
                PrescriberCategory = selectedDrug.PrescriberCategory;
            }
            else
            {
                Strength = "";
                Unit = "";
                PrescriberCategory = "";
            }
        }

        public void SaveData()
        {
            try
            {
                using (SQLiteConnection con = new SQLiteConnection("Data Source=./SQLitedb.db;Version=3;"))
                {
                    con.Open();
                    foreach (var drug in DrugList)
                    {
                        string query = @"
                                        UPDATE Drugs 
                                        SET DrugName = @DrugName, 
                                            Strength = @Strength, 
                                            Unit = @Unit, 
                                            DosageForm = @DosageForm, 
                                            PrescriberCategory = @PrescriberCategory, 
                                            Remark = @Remark, 
                                            DefaultMaxQty = @DefaultMaxQty
                                        WHERE DrugID = @DrugID";

                        using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                        {
                            cmd.Parameters.AddWithValue("@DrugID", drug.DrugID);
                            cmd.Parameters.AddWithValue("@DrugName", drug.DrugName);
                            cmd.Parameters.AddWithValue("@Strength", drug.Strength);
                            cmd.Parameters.AddWithValue("@Unit", drug.Unit);
                            cmd.Parameters.AddWithValue("@DosageForm", drug.DosageForm);
                            cmd.Parameters.AddWithValue("@PrescriberCategory", drug.PrescriberCategory);
                            cmd.Parameters.AddWithValue("@Remark", drug.Remark);
                            cmd.Parameters.AddWithValue("@DefaultMaxQty", drug.DefaultMaxQty);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                MessageBox.Show("Database updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating database: " + ex.Message);
            }
        }
    }
}
