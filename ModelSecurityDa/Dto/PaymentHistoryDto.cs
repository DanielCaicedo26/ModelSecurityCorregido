namespace Entity.Dto
{
    public class PaymentHistoryDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }

        public bool IsActive { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}