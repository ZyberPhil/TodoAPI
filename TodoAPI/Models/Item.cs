using System;
using System.Collections.Generic;

namespace TodoAPI.Models;

public partial class Item
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Beschreibung { get; set; }

    public int? Status { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? DueDate { get; set; }

    public virtual ICollection<Appuser> AppUsers { get; set; } = new List<Appuser>();
}
