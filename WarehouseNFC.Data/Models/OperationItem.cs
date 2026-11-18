using System;
using System.Collections.Generic;

namespace WarehouseNFC.Data.Models;

public partial class OperationItem
{
    public int OperationItemId { get; set; }

    public int OperationId { get; set; }

    public int ItemId { get; set; }

    public string? Result { get; set; }

    public virtual ProductItem Item { get; set; } = null!;

    public virtual Operation Operation { get; set; } = null!;
}
