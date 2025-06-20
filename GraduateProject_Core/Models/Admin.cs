using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.Models
{
    public class Admin
    {
        [Key]
        public int AdminID { get; set; }
        public int UserId { get; set; }
        public string AccessLevel { get; set; }

        public Users User { get; set; }

    }
}
