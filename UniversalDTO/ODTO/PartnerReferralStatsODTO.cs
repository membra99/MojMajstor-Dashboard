namespace Universal.DTO.ODTO
{
    public class PartnerReferralStatsODTO
    {
        public int PartnerReferralId { get; set; }
        public string Name { get; set; } = null!;
        public string ReferalCode { get; set; } = null!;
        public int TokenAmount { get; set; }
        public int TotalUses { get; set; }
        public int TotalAllTime { get; set; }
        public List<ReferralDailyStatODTO> DailyStats { get; set; } = new();
    }

    public class ReferralDailyStatODTO
    {
        public string Date { get; set; } = null!;
        public int Count { get; set; }
    }
}
