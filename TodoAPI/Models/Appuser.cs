using System;
using System.Collections.Generic;

namespace TodoAPI.Models;

public partial class Appuser
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? Klasse { get; set; }

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();
}
