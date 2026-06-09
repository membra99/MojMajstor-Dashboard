namespace Universal.DTO.ODTO
{
    public class ProblemODTO
    {
        public int ProblemID { get; set; }
        public string? Email { get; set; }
        public string? ActualBehavior { get; set; }
        public string? ExpectedBehavior { get; set; }
        public string? StepsToReproduce { get; set; }
        public string? Screenshots { get; set; }
        public string? Video { get; set; }
        public string? AppVersion { get; set; }
        public string? Os { get; set; }
        public string? OsVersion { get; set; }
        public string? DeviceID { get; set; }
        public ProblemStatus Status { get; set; }

        // Presigned URLs generated server-side
        public List<string> ScreenshotUrls { get; set; } = new();
        public string? VideoUrl { get; set; }
    }
}
