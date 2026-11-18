using System;
using System.Collections.Generic;

namespace WarehouseNFC.Data.Models;

public partial class OperationType
{
    public int OperationTypeId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Operation> Operations { get; set; } = new List<Operation>();
}
