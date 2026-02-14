using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AgriManager.API.Models;

[Table("Income")]
public partial class Income
{
    [Key]
    public int IncomeId { get; set; }

    public int CropId { get; set; }

    [Column(TypeName = "decimal(12, 2)")]
    public decimal? Quantity { get; set; }

    [Column(TypeName = "decimal(12, 2)")]
    public decimal? PricePerUnit { get; set; }

    [Column(TypeName = "decimal(14, 2)")]
    public decimal TotalAmount { get; set; }

    public DateOnly SaleDate { get; set; }

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
