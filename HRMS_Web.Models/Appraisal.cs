﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS_Web.Models
{
    public class Appraisal
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AppraisalId { get; set; }

        [Required]
        public DateTime AppraisalDate { get; set; }

        [Required]
        public string Status { get; set; } //active , inactive

        [Required]
        public string Employee { get; set; }

        [Required]
        public string Designation { get; set; }

        [Required]
        public string Department { get; set; }


        // Foreign key to ApplicationUser
        [Required]
        public string Id { get; set; }

        // Navigation property to ApplicationUser
        [ForeignKey("Id")]
        public ApplicationUser? User { get; set; }
    }
}