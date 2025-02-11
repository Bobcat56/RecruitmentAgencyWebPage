using Common.Models;
using DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class EncryptionKeyRepository
    {
        private RecruitmentContext _context;
        public EncryptionKeyRepository(RecruitmentContext context)
        {
            _context = context;
        }
        public EncryptionKey GetKeyById(string userId)
        {
            return _context.EncryptionKeys.SingleOrDefault(key => key.UserId == userId);
        }
    }
}
