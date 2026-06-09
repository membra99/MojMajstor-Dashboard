using Universal.DTO.ODTO;

namespace Universal.Universal.MainDataNova;

public partial class Problem
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
    public ProblemStatus Status { get; set; } = ProblemStatus.Open;
}
