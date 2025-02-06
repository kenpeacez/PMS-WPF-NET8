using System;
using System.Data.SQLite;
using System.Collections.Generic;
using System.IO;

namespace PMS_WPF_NET8.functions.database
{
    internal class SQLiteDB
    {
        private readonly string _dbFilePath = "SQLitedb.db"; // Path to the SQLite DB file
        private SQLiteConnection _connection;

        // Constructor to initialize and open the connection
        public SQLiteDB()
        {
            if (!File.Exists(_dbFilePath))
            {
                SQLiteConnection.CreateFile(_dbFilePath); // Create the DB file if it doesn't exist
            }

            _connection = new SQLiteConnection($"Data Source={_dbFilePath};Version=3;");
            _connection.Open(); // Open the SQLite connection
        }

        // Method to create the Patients table if it doesn't exist
        public void CreateTable()
        {
            string createPatientsTableQuery = @"
        CREATE TABLE IF NOT EXISTS Patients (
            ID INTEGER PRIMARY KEY AUTOINCREMENT,
            Name TEXT NOT NULL,
            ICNo TEXT NOT NULL,
            DateRegistered TEXT NOT NULL
        );";

            string createDrugDetailsTableQuery = @"
        CREATE TABLE IF NOT EXISTS DrugDetails (
            ID INTEGER PRIMARY KEY AUTOINCREMENT,
            PatientId INTEGER NOT NULL,
            DrugIndex INTEGER NOT NULL,
            Dose TEXT,
            Frequency TEXT,
            Duration TEXT,
            Quantity TEXT,
            FOREIGN KEY(PatientId) REFERENCES Patients(ID)
        );";

            string createTableQuery = @"
        CREATE TABLE IF NOT EXISTS Drugs (
            DrugID INTEGER PRIMARY KEY AUTOINCREMENT,
            DrugName TEXT NOT NULL,
            Strength TEXT NOT NULL,
            Unit TEXT NOT NULL,
            DosageForm TEXT NOT NULL,
            PrescriberCategory TEXT NOT NULL,
            Remark TEXT,
            DefaultMaxQty INTEGER NOT NULL
        );";

            // Execute both queries
            using (SQLiteCommand command = new SQLiteCommand(createPatientsTableQuery, _connection))
            {
                command.ExecuteNonQuery();
            }

            using (SQLiteCommand command = new SQLiteCommand(createDrugDetailsTableQuery, _connection))
            {
                command.ExecuteNonQuery();
            }

            using (SQLiteCommand command = new SQLiteCommand(createTableQuery, _connection))
            {
                command.ExecuteNonQuery();
            }
        }


        // Method to insert a new patient
        public void InsertPatient(string name, string icNo, string dateRegistered)
        {
            string insertQuery = "INSERT INTO Patients (Name, ICNo, DateRegistered) VALUES (@name, @icNo, @dateRegistered)";

            using (SQLiteCommand command = new SQLiteCommand(insertQuery, _connection))
            {
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@icNo", icNo);
                command.Parameters.AddWithValue("@dateRegistered", dateRegistered);
                command.ExecuteNonQuery();
            }
        }

        // Method to read all patients from the database
        public List<string> GetPatients()
        {
            string selectQuery = "SELECT * FROM Patients";
            List<string> patients = new List<string>();

            using (SQLiteCommand command = new SQLiteCommand(selectQuery, _connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string patientInfo = $"ID: {reader["ID"]}, Name: {reader["Name"]}, IC: {reader["ICNo"]}, Date Registered: {reader["DateRegistered"]}";
                        patients.Add(patientInfo);
                    }
                }
            }

            return patients;
        }

        // Method to close the connection
        public void CloseConnection()
        {
            _connection.Close();
        }
    }
}
