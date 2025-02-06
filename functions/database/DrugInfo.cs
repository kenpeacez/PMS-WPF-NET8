namespace PMS_WPF_NET8.functions.patient
{
    public class DrugInfo
    {
        // Properties for drug information
        public int DrugIndex { get; set; }  // Unique identifier (e.g., cbDrug1, cbDrug2)
        public string DrugName { get; set; }
        public string Dose { get; set; }
        public string Frequency { get; set; }
        public string Duration { get; set; }
        public string Quantity { get; set; }


        // Constructor to initialize drug information with necessary parameters
        // Constructor to initialize the drug data
        public DrugInfo(int drugIndex, string drugName, string dose, string frequency, string duration, string quantity)
        {
            DrugIndex = drugIndex;
            DrugName = drugName;
            Dose = dose;
            Frequency = frequency;
            Duration = duration;
            Quantity = quantity;
        }

        // ToString method for representing the DrugInfo object in string format
        public override string ToString()
        {
            return $"{DrugName} - {Dose}, {Frequency}, {Duration}, {Quantity}";
        }
    }
}
