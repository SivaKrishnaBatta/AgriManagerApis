using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AgriManager.API.Models;

[Table("Crop")]
public partial class Crop
{
    [Key]
    public int CropId { get; set; }

    public int FarmId { get; set; }

    public int FieldId { get; set; }

    [StringLength(50)]
    public string CropName { get; set; } = null!;

    public int CropStatusId { get; set; }

    [StringLength(50)]
    public string? Season { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? ExpectedEndDate { get; set; }

    [StringLength(100)]
    public string? ExpectedYield { get; set; }

    public int CustomerId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    public int CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedAt { get; set; }

    public int? ModifiedBy { get; set; }
}
