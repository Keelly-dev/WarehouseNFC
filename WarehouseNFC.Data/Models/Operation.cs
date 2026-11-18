using System;
using System.Collections.Generic;

namespace WarehouseNFC.Data.Models;

public partial class Operation
{
    public int OperationId { get; set; }

    public int OperationTypeId { get; set; }

    public int EmployeeId { get; set; }

    public DateTime? OperationDate { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual ICollection<OperationItem> OperationItems { get; set; } = new List<OperationItem>();

    public virtual OperationType OperationType { get; set; } = null!;
}
