using System;
using System.Collections.Generic;

namespace WarehouseNFC.Data.Models;

public partial class NfcTag
{
    public string TagId { get; set; } = null!;

    public int ItemId { get; set; }

    public int PositionId { get; set; }

    public virtual ProductItem Item { get; set; } = null!;

    public virtual StoragePosition Position { get; set; } = null!;
}
