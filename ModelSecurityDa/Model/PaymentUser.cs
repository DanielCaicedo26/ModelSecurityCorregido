namespace Entity.Model
{
    public class PaymentUser
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime CreatedAt { get; set; }

        // Nuevas propiedades
        public int? BillId { get; set; }
        public int? PaymentAgreementId { get; set; }
        public int? TypePaymentId { get; set; }

        // Propiedades de navegación
        public virtual Person Person { get; set; } = null!;
        public virtual Bill? Bill { get; set; }
        public virtual PaymentAgreement? PaymentAgreement { get; set; }
        public virtual TypePayment? TypePayment { get; set; }
    }
}
