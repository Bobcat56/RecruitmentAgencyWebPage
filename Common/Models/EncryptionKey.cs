using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Common.Models
{
    public class EncryptionKey
    {
        [Key]
        [ForeignKey("IdentityUser")]
        public required string UserId { get; set; }
        public virtual IdentityUser IdentityUser { get; set; }

        public required string PublicKey { get; set; }
        public required string PrivateKey { get; set; }
    }
}
