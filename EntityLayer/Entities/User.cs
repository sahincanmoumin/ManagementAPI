using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace EntityLayer.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public decimal Balance { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

       
        public List<Farm> Farms { get; set; }
    }
}
