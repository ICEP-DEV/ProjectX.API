﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.Data.Model
{
    public class AlumnusProfile
    {
        public AlumnusProfile()
        {
            FirstName = string.Empty;   
            LastName = string.Empty;
            Course = string.Empty;
            Campus = string.Empty;
            Faculty = string.Empty;
        }

        [Key]
        public int AlumnusProfId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Course { get; set; }
        public DateOnly GraduationYears { get; set; }
        public string Campus { get; set; }
        public string Faculty { get; set; }
        public byte ProfilePicture { get; set; }


        [ForeignKey("Alumnus")]
        public int AlumnusId { get; set;}

    }
}