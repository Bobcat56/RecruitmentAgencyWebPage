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
    public class CV
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [FileExtensions(Extensions = ".pdf,.docx")]
        public required string FileName { get; set; }

        [ForeignKey("UserIdentity")]
        public required string UserId { get; set; }
        public virtual IdentityUser UserIdentity { get; set; }

        [ForeignKey("EmployerIdentity")]
        public required string EmployerId { get; set; }
        public virtual IdentityUser EmployerIdentity { get; set; }

        public required byte[] DigtalSignature { get; set; }

    }
}
