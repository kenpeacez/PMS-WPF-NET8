public class NewDrugModal
{
    public int DrugID { get; set; }  // Unique ID
    public string DrugName { get; set; }
    public string Strength { get; set; }
    public string Unit { get; set; }
    public string DosageForm { get; set; }
    public string PrescriberCategory { get; set; }
    public string Remark { get; set; }
    public int DefaultMaxQty { get; set; }
}