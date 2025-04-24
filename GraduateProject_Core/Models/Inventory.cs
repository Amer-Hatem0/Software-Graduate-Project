using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.Models
{
    public class Inventory
    {
        [Key]
        public int ItemID { get; set; }

        [Required]
        [StringLength(100)]
        public string ItemName { get; set; }

        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        [Range(0, int.MaxValue)]
        public int MinThreshold { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;


    }
}
