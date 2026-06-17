namespace Universal.DTO.IDTO
{
    public class PartnerReferralIDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string ReferalCode { get; set; } = null!;
        public int TokenAmount { get; set; }
    }
}
