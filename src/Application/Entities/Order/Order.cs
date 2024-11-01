using System;
using System.Collections.Generic;

namespace API;

public partial class Order
{
    public int OrderId { get; set; }

    public int CustomerId { get; set; }

    public string Country { get; set; } = null!;

    public string Street { get; set; } = null!;

    public string City { get; set; } = null!;

    public string ZipCode { get; set; } = null!;

    public decimal Total { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
