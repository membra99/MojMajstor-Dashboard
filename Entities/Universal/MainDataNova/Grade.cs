using System;
using System.Collections.Generic;

namespace Universal.Universal.MainDataNova;

public partial class Grade
{
    public int GradesId { get; set; }

    public bool Positive { get; set; }

    public bool Negative { get; set; }

    public string? Comment { get; set; }

    public bool IsClient { get; set; }

    public int UserLeftCommentId { get; set; }

    public int UserReceiveCommentId { get; set; }

    public virtual User UserLeftComment { get; set; } = null!;

    public virtual User UserReceiveComment { get; set; } = null!;
}
