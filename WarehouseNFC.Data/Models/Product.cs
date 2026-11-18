using System;
using System.Collections.Generic;

namespace WarehouseNFC.Data.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string Name { get; set; } = null!;

    public string? Article { get; set; }

    public string? Unit { get; set; }

    public string? PackageType { get; set; }

    public virtual ICollection<ProductItem> ProductItems { get; set; } = new List<ProductItem>();
}
