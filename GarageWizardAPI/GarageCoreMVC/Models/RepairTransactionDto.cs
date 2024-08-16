namespace GarageCoreMVC.Models
{
    public class RepairTransactionDto
    {
        public string TransactionDate { get; set; } = string.Empty;
        public string? JobName { get; set; }
        public int Charge {  get; set; }
        public double Hours {  get; set; }
        public bool IsCompleted { get; set; } = false;
    }
}

