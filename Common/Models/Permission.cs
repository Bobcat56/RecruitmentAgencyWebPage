using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel;

namespace Common.Models
{
    public class Permission
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("CV")]
        public int CVId { get; set; }
        public virtual CV CV { get; set; }

        [ForeignKey("IdentityUser")]
        public string UserId { get; set; }
        public virtual IdentityUser User { get; set; }

        public string Role { get; set; }

    }
}
