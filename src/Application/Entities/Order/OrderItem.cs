using System;
using System.Collections.Generic;

namespace API;

public partial class OrderItem
{
    public int OrderItemId { get; set; }

    public decimal Price { get; set; }

    public string Description { get; set; } = null!;

    public int Quantity { get; set; }

    public int OrderId { get; set; }

    public virtual Order Order { get; set; } = null!;
}
