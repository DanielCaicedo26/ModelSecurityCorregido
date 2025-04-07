namespace Entity.Dto
{
    public class PaymentAgreementDto
    {
        public int Id { get; set; }
        public string Address { get; set; } = null!;
        public string? Neighborhood { get; set; }
        public decimal FinanceAmount { get; set; }
        public string? AgreementDescription { get; set; }
    }
}