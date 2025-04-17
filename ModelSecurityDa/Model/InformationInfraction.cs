namespace Entity.Model
{
    public class InformationInfraction
    {
        public int Id { get; set; }
        public int Numer_smldv { get; set; }
        public decimal MinimumWage { get; set; }
        public decimal Value_smldv { get; set; }
        public decimal TotalValue { get; set; }
        public virtual ICollection<TypeInfraction>? TypeInfraction { get; set; }
        public virtual ICollection<PaymentHistory>? PaymentHistory { get; set; }
    }
}