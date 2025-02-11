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
    public class Job
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string Requirments { get; set; }
        public required DateOnly PostingDate { get; set;}

        [ForeignKey("IdentityUser")]
        public required string EmployerId { get; set; }
        public virtual IdentityUser IdentityUser { get; set; }
    }
}
