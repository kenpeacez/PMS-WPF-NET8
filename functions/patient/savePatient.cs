using System;
using System.Windows;
using System.Text.RegularExpressions;
using System.Data.SQLite;
using PMS_WPF_NET8.functions.patient;

namespace PMS_WPF_NET8.Functions.Patient
{
    public class PatientFunctions
    {
        private readonly string _dbFilePath = "SQLitedb.db"; // Path to SQLite DB file (with .db extension)

        public void SavePatient(
        string patientName, string icNo, string dateCollect, string dateRegister, string dateSeeDoctor,
        bool noICNumberChecked, List<DrugInfo> drugList)
        {
            string ICREGEX = @"^((\d{2}(?!0229))|([02468][048]|[13579][26])(?=0229))(0[1-9]|1[0-2])(0[1-9]|[12]\d|(?<!02)30|(?<!02|4|6|9|11)31)-(\d{2})-(\d{4})$";

            // Perform validations
            if (string.IsNullOrWhiteSpace(patientName))
            {
                MessageBox.Show("Error. Name cannot be empty.");
                return;
            }

            if (!noICNumberChecked) // If "No IC Number" checkbox is unchecked
            {
                if (icNo.Length < 14)
                {
                    MessageBox.Show("Error. IC Number must be 12 digits with dashes.");
                    return;
                }
                else if (!Regex.IsMatch(icNo, ICREGEX))
                {
                    MessageBox.Show("Error. IC No. Regex formatting is invalid. Please check and try again.");
                    return;
                }
            }

            // Save patient data to SQLite database
            SavePatientToDatabase(patientName, icNo, dateCollect, dateRegister, dateSeeDoctor, drugList);

            MessageBox.Show("Patient data is valid and saved successfully.");
        }


        private void SavePatientToDatabase(string patientName, string icNo, string dateCollect, string dateRegister, string dateSeeDoctor, List<DrugInfo> drugList)
        {
            using (SQLiteConnection connection = new SQLiteConnection("Data Source=SQLitedb.db;Version=3;"))
            {
                connection.Open();

                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Insert Patient Data
                        string insertPatientQuery = @"
                    INSERT INTO Patients (Name, ICNo, DateCollect, DateRegister, DateSeeDoctor)
                    VALUES (@Name, @ICNo, @DateCollect, @DateRegister, @DateSeeDoctor);
                    SELECT last_insert_rowid();";

                        long patientId;

                        using (SQLiteCommand patientCmd = new SQLiteCommand(insertPatientQuery, connection))
                        {
                            patientCmd.Parameters.AddWithValue("@Name", patientName);
                            patientCmd.Parameters.AddWithValue("@ICNo", icNo);
                            patientCmd.Parameters.AddWithValue("@DateCollect", dateCollect);
                            patientCmd.Parameters.AddWithValue("@DateRegister", dateRegister);
                            patientCmd.Parameters.AddWithValue("@DateSeeDoctor", dateSeeDoctor);

                            patientId = (long)patientCmd.ExecuteScalar();
                        }

                        // Insert Drug Data
                        string insertDrugQuery = @"
                    INSERT INTO Drugs (PatientID, DrugIndex, DrugName, Dose, Frequency, Duration, Quantity)
                    VALUES (@PatientID, @DrugIndex, @DrugName, @Dose, @Frequency, @Duration, @Quantity);";

                        foreach (var drug in drugList)
                        {
                            using (SQLiteCommand drugCmd = new SQLiteCommand(insertDrugQuery, connection))
                            {
                                drugCmd.Parameters.AddWithValue("@PatientID", patientId);
                                drugCmd.Parameters.AddWithValue("@DrugIndex", drug.DrugIndex);
                                drugCmd.Parameters.AddWithValue("@DrugName", drug.DrugName);
                                drugCmd.Parameters.AddWithValue("@Dose", drug.Dose);
                                drugCmd.Parameters.AddWithValue("@Frequency", drug.Frequency);
                                drugCmd.Parameters.AddWithValue("@Duration", drug.Duration);
                                drugCmd.Parameters.AddWithValue("@Quantity", drug.Quantity);

                                drugCmd.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Error saving patient: " + ex.Message);
                    }
                }
            }
        }

    }
}
