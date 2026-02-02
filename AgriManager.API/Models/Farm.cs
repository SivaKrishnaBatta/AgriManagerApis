using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AgriManager.API.Models;

[Table("Farm")]
public partial class Farm
{
    [Key]
    public int FarmId { get; set; }

    [StringLength(50)]
    public string FarmName { get; set; } = null!;

    [StringLength(50)]
    public string? Location { get; set; }

    public int? TotalFields { get; set; }

    [StringLength(200)]
    public string? Notes { get; set; }

    public int CustomerId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    public int CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedAt { get; set; }

    public int? ModifiedBy { get; set; }
}
