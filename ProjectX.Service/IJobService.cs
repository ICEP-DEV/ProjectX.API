using ProjectX.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.Service
{
    public interface IJobService
    {
         void UploadJobs(Jobs newJob);

         IEnumerable<Jobs> GetJob();
        IEnumerable<Jobs> GetJobsByFaculty(string faculty); // New method to filter jobs by faculty
    }

}
