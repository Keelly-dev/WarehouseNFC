using System;
using System.Collections.Generic;

namespace WarehouseNFC.Data.Models;

public partial class ProductItem
{
    public int ItemId { get; set; }

    public int ProductId { get; set; }

    public string? Batch { get; set; }

    public string? SerialNumber { get; set; }

    public DateOnly? ProductionDate { get; set; }

    public int PositionId { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<NfcTag> NfcTags { get; set; } = new List<NfcTag>();

    public virtual ICollection<OperationItem> OperationItems { get; set; } = new List<OperationItem>();

    public virtual StoragePosition Position { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
