using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AgriManager.API.Models;

[Table("CropStatus")]
public partial class CropStatus
{
    [Key]
    public int CropStatusId { get; set; }

    [StringLength(50)]
    public string CropStatusName { get; set; } = null!;

    public bool IsActive { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    public int CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedAt { get; set; }

    public int? ModifiedBy { get; set; }
}
