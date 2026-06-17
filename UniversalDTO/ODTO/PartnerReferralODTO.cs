namespace Universal.DTO.ODTO
{
    public class PartnerReferralODTO
    {
        public int PartnerReferralId { get; set; }
        public string Name { get; set; } = null!;
        public string ReferalCode { get; set; } = null!;
        public int TokenAmount { get; set; }
        public int UseCount { get; set; }
    }
}
