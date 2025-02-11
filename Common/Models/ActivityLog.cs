using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class ActivityLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("IdentityUser")]
        public string UserId { get; set; }
        public virtual IdentityUser IdentityUser { get; set; }

        public required string ActivityType { get; set; }
        public DateTime ActivityDate { get; set; }
        public required string Description { get; set; }
    }
}
