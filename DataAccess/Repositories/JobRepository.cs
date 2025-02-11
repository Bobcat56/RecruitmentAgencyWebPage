using Common.Models;
using DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class JobRepository
    {
        private RecruitmentContext _context;
        public JobRepository(RecruitmentContext context) 
        {
            _context = context;
        }

        public void AddJob(Job j)
        {
            _context.Jobs.Add(j);
            _context.SaveChanges();
        }

        public IQueryable<Job> GetAllJobs()
        {
            return _context.Jobs;
        }

        public void DeleteJob(Job job)
        {
            _context.Jobs.Remove(job);
            _context.SaveChanges();
        }
    }
}
