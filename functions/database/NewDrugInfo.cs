namespace PMS_WPF_NET8.functions.patient
{
    public class NewDrugInfo
    {
        public string NewDrugName { get; set; }          // Drug Name
        public string Strength { get; set; }             // Strength (e.g., 500mg)
        public string Unit { get; set; }                 // Unit (e.g., tablet, capsule)
        public string DosageForm { get; set; }           // Dosage Form (e.g., Oral, Injectable)
        public string PrescriberCategory { get; set; }   // Prescriber Category (e.g., Specialist, General Practitioner)
        public string Remark { get; set; }               // Remark (e.g., Use with food)
        public int DefaultMaxQty { get; set; }           // Default Maximum Quantity (e.g., 30)

        // Constructor to initialize the properties
        public NewDrugInfo(string newDrugName, string strength, string unit, string dosageForm,
                            string prescriberCategory, string remark, int defaultMaxQty)
        {
            NewDrugName = newDrugName;
            Strength = strength;
            Unit = unit;
            DosageForm = dosageForm;
            PrescriberCategory = prescriberCategory;
            Remark = remark;
            DefaultMaxQty = defaultMaxQty;
        }

        // Optional: Override ToString for easier debugging or display in UI
        public override string ToString()
        {
            return $"{NewDrugName} - {Strength} {Unit}, Dosage Form: {DosageForm}, " +
                   $"Prescriber: {PrescriberCategory}, Remark: {Remark}, Max Qty: {DefaultMaxQty}";
        }
    }
}
