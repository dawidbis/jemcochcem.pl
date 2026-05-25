namespace FitApp.Domain.Entities
{
    public class WaterLog
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime Date { get; set; }
        public int AmountMl { get; set; } // Zalogowana ilość wody w mililitrach

        // Konstruktor bezparametrowy dla Entity Framework
        public WaterLog() { }

        public WaterLog(Guid userId, DateTime date, int amountMl)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Date = date.Date; // Zapisujemy samą datę bez godzin
            AmountMl = amountMl;
        }
    }
}