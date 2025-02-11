using Common.Models;
using DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class CvRepository
    {
        private RecruitmentContext _context;
        public CvRepository(RecruitmentContext context)
        {
            _context = context;
        }

        public void AddCv(CV cv)
        {
            _context.CVs.Add(cv);
            _context.SaveChanges();
        }
        
        public IQueryable<CV> GetAllCv()
        {
            return _context.CVs;
        }
        
        //Get a list of cv's the user has submitted
        public IQueryable<CV> GetCvsByUserId(string userId)
        {
            return _context.CVs.Where(cv => cv.UserId == userId);
        }

        //Get a list of cv's users have submitted to your job application
        public IQueryable<CV> GetCvsByEmployerId(string employerId)
        {
            return _context.CVs.Where(cv => cv.EmployerId == employerId);
        }

        public void RemoveCv(CV cv)
        {
            _context.CVs.Remove(cv);
            _context.SaveChanges();
        }
    }
}
