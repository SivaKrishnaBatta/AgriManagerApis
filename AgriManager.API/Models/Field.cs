using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AgriManager.API.Models;

[Table("Field")]
public partial class Field
{
    [Key]
    public int FieldId { get; set; }

    public int FarmId { get; set; }

    [StringLength(50)]
    public string FieldName { get; set; } = null!;

    [StringLength(200)]
    public string? Area { get; set; }

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
