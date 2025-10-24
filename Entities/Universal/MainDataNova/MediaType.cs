using System;
using System.Collections.Generic;

namespace Universal.Universal.MainDataNova;

public partial class MediaType
{
    public int MediaTypeId { get; set; }

    public string MediaTypeName { get; set; } = null!;

    public virtual ICollection<Medium> Media { get; set; } = new List<Medium>();
}
