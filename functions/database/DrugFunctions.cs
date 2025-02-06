using PMS_WPF_NET8.functions.patient;
using System.Data.SQLite;

namespace PMS_WPF_NET8.Functions.Database
{
    public class DrugFunctions
    {
        private SQLiteConnection _connection;

        // Constructor accepting SQLiteConnection
        public DrugFunctions(SQLiteConnection connection)
        {
            _connection = connection;
        }

        public void SaveNewDrug(NewDrugModal drug)
        {
            string insertDrugQuery = @"
                INSERT INTO Drugs (DrugName, Strength, Unit, DosageForm, PrescriberCategory, Remark, DefaultMaxQty)
                VALUES (@DrugName, @Strength, @Unit, @DosageForm, @PrescriberCategory, @Remark, @DefaultMaxQty);";

            using (SQLiteCommand command = new SQLiteCommand(insertDrugQuery, _connection))
            {
                command.Parameters.AddWithValue("@DrugName", drug.DrugName);
                command.Parameters.AddWithValue("@Strength", drug.Strength);
                command.Parameters.AddWithValue("@Unit", drug.Unit);
                command.Parameters.AddWithValue("@DosageForm", drug.DosageForm);
                command.Parameters.AddWithValue("@PrescriberCategory", drug.PrescriberCategory);
                command.Parameters.AddWithValue("@Remark", drug.Remark ?? (object)DBNull.Value); // If remark is null, set it to DBNull
                command.Parameters.AddWithValue("@DefaultMaxQty", drug.DefaultMaxQty);

                command.ExecuteNonQuery();
            }
        }

        // Fetch current drugs from the database
        public List<NewDrugInfo> GetAllDrugs()
        {
            List<NewDrugInfo> drugs = new List<NewDrugInfo>();

            string selectDrugsQuery = @"
                SELECT DrugName, Strength, Unit, DosageForm, PrescriberCategory, Remark, DefaultMaxQty 
                FROM Drugs;";

            using (SQLiteCommand command = new SQLiteCommand(selectDrugsQuery, _connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        NewDrugInfo drug = new NewDrugInfo(
                            reader.GetString(0),  // DrugName
                            reader.GetString(1),  // Strength
                            reader.GetString(2),  // Unit
                            reader.GetString(3),  // DosageForm
                            reader.GetString(4),  // PrescriberCategory
                            reader.IsDBNull(5) ? null : reader.GetString(5),  // Remark
                            reader.GetInt32(6)   // DefaultMaxQty
                        );
                        drugs.Add(drug);
                    }
                }
            }

            return drugs;
        }

        public void CloseConnection()
        {
            _connection.Close();
        }
    }
}
