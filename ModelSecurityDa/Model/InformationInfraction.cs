namespace Entity.Model
{
    public class InformationInfraction
    {
        public int Id { get; set; }
        public int Numer_smldv { get; set; }
        public decimal MinimumWage { get; set; }
        public decimal Value_smldv { get; set; }
        public decimal TotalValue { get; set; }
        public virtual ICollection<TypeInfraction>? TypeInfractions { get; set; }
        public virtual ICollection<PaymentHistory>? PaymentHistories { get; set; }
    }
}