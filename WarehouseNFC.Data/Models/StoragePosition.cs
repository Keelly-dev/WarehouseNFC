using System;
using System.Collections.Generic;

namespace WarehouseNFC.Data.Models;

public partial class StoragePosition
{
    public int PositionId { get; set; }

    public int ShelfNumber { get; set; }

    public int ShelfLevel { get; set; }

    public virtual ICollection<NfcTag> NfcTags { get; set; } = new List<NfcTag>();

    public virtual ICollection<ProductItem> ProductItems { get; set; } = new List<ProductItem>();
    public string DisplayName => $"Стеллаж {ShelfNumber}, полка {ShelfLevel}";
}
