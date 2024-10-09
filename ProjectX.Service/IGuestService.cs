using ProjectX.Data.Model;
using ProjectX.Data.Model.dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace ProjectX.Service
{
    public interface IGuestService
    {
        Task<Donation> CreateDonation(DonationDTO donationDTO);
    }
}
