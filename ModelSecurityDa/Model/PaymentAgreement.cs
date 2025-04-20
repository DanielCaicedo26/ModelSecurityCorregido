namespace Entity.Model
{
    public class PaymentAgreement
    {
        public int Id { get; set; }
        public string Address { get; set; } = null!;
        public string? Neighborhood { get; set; }
        public decimal FinanceAmount { get; set; }
        public string? AgreementDescription { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<PaymentUser>? PaymentUsers { get; set; }
        public virtual ICollection<Bill>? Bills { get; set; }

    }
}