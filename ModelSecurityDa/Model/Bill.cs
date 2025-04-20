namespace Entity.Model
{
    public class Bill
    {
        public int Id { get; set; }
        public string Barcode { get; set; } = null!;
        public DateTime IssueDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public decimal TotalValue { get; set; }
        public string? State { get; set; }

        public bool IsActive { get; set; }

        public int? PaymentAgreementId { get; set; }
        public virtual ICollection<PaymentUser>? PaymentUsers { get; set; }
        public virtual PaymentAgreement? PaymentAgreement { get; set; }

    }
}