using System;
using System.Collections.Generic;

namespace Universal.Universal.MainDataNova;

public partial class Medium
{
    public int MediaId { get; set; }

    public string Src { get; set; } = null!;

    public int? UsersId { get; set; }

    public int MediaTypeId { get; set; }

    public int? Postition {  get; set; }

    public string? Url { get; set; }

    public virtual MediaType MediaType { get; set; } = null!;

    public virtual User Users { get; set; } = null!;
}
