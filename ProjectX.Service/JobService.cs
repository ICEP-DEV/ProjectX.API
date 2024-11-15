using ProjectX.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.Service
{
    public class JobService: IJobService
    {

        private readonly List<Jobs> job = new List<Jobs>();

        public void UploadJobs(Jobs newJob) => job.Add(newJob);

        public IEnumerable<Jobs> GetJob() => job;
        public IEnumerable<Jobs> GetJobsByFaculty(string faculty)
        {
            // Filter the jobs based on the faculty name
            return job.Where(j => j.Faculty.Equals(faculty, StringComparison.OrdinalIgnoreCase));
        }


    }
}
