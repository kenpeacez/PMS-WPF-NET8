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
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool _isReadOnly = true;
        public bool IsReadOnly
        {
            get => _isReadOnly;
            set
            {
                _isReadOnly = value;
                OnPropertyChanged();
            }
        }
        private string _selectedDrugName;
        private string _selectedDrugName1;
        private string _strength1;
        private string _unit1;
        private string _prescriberCategory1;
        private string _selectedDrugName2;
        private string _strength2;
        private string _unit2;
        private string _prescriberCategory2;

        public ObservableCollection<NewDrugModal> DrugList { get; set; } = new ObservableCollection<NewDrugModal>();
        public ObservableCollection<string> DrugNames { get; set; } = new ObservableCollection<string>();
        public Dictionary<string, NewDrugModal> DrugDetails { get; set; } = new Dictionary<string, NewDrugModal>();

        public void CalculateTotalQuantity(string strengthContent, string doseText, string freqText, string durationText, out string totalQuantity)
        {
            try
            {
                double strength = double.TryParse(strengthContent, out double s) ? s : 0;
                double dose = double.TryParse(doseText, out double d) ? d : 0;
                double frequency = double.TryParse(freqText, out double f) ? f : 0;
                double duration = double.TryParse(durationText, out double dur) ? dur : 0;

                double total = (dose / strength) * frequency * duration;
                totalQuantity = total.ToString();
            }
            catch
            {
                totalQuantity = "Error";
            }
        }
        public string Strength1
        {
            get => _strength1;
            set
            {
                _strength1 = value;
                OnPropertyChanged();
            }
        }

        public string Unit1
        {
            get => _unit1;
            set
            {
                _unit1 = value;
                OnPropertyChanged();
            }
        }

        public string PrescriberCategory1
        {
            get => _prescriberCategory1;
            set
            {
                _prescriberCategory1 = value;
                OnPropertyChanged();
            }
        }
        public string SelectedDrugName1
        {
            get => _selectedDrugName1;
            set
            {
                _selectedDrugName1 = value;
                OnPropertyChanged();
                UpdateDrugDetails1();
            }
        }
        private void UpdateDrugDetails1()
        {
            if (!string.IsNullOrEmpty(SelectedDrugName1) && DrugDetails.ContainsKey(SelectedDrugName1))
            {
                var selectedDrug = DrugDetails[SelectedDrugName1];
                Strength1 = selectedDrug.Strength;
                Unit1 = selectedDrug.Unit;
                PrescriberCategory1 = selectedDrug.PrescriberCategory;
            }
            else
            {
                Strength1 = "";
                Unit1 = "";
                PrescriberCategory1 = "";
            }
        }

        public string SelectedDrugName2
        {
            get => _selectedDrugName2;
            set
            {
                _selectedDrugName2 = value;
                OnPropertyChanged();
                UpdateDrugDetails2();
            }
        }

        public string Strength2
        {
            get => _strength2;
            set
            {
                _strength2 = value;
                OnPropertyChanged();
            }
        }

        public string Unit2
        {
            get => _unit2;
            set
            {
                _unit2 = value;
                OnPropertyChanged();
            }
        }

        public string PrescriberCategory2
        {
            get => _prescriberCategory2;
            set
            {
                _prescriberCategory2 = value;
                OnPropertyChanged();
            }
        }

        private void UpdateDrugDetails2()
        {
            if (!string.IsNullOrEmpty(SelectedDrugName2) && DrugDetails.ContainsKey(SelectedDrugName2))
            {
                var selectedDrug = DrugDetails[SelectedDrugName2];
                Strength2 = selectedDrug.Strength;
                Unit2 = selectedDrug.Unit;
                PrescriberCategory2 = selectedDrug.PrescriberCategory;
            }
            else
            {
                Strength2 = "";
                Unit2 = "";
                PrescriberCategory2 = "";
            }
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
                        DrugDetails.Clear();

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
