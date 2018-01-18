﻿using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace eDostava.Data.Models
{
   public class Blok
    {
        [Key]
        public int BlokID { get; set; }

        public int Naziv { get; set; }
        [ForeignKey("Grad")]
        public int GradID { get; set; }

        public Grad Grad { get; set; }
    }
}