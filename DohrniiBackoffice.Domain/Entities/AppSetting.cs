using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DohrniiBackoffice.Domain.Entities
{
    [Table("AppSetting")]
    public partial class AppSetting
    {
        [Key]
        public int Id { get; set; }
        public int XpToJelly { get; set; }
    }
}
