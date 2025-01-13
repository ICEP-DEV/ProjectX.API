using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ProjectX.Data.Model.dto
{
    public class ProfilePhotoDTO
    {
        [Required]
        public int AlumnusId { get; set; }

        [Required]
        public string ProfilePhoto { get; set; } // Base64 string of uploaded image
    }
}

