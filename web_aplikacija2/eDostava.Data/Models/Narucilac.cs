﻿using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace eDostava.Data.Models
{
   public class Narucilac:Korisnik
    {
 

        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Telefon { get; set; }
        [ForeignKey("Blok")]
        public int BlokID { get; set; }

        public Blok Blok { get; set; }

        public string Ime_prezime
        {
            get { return Ime + " " + Prezime; }
        }

        public string Prezime_ime
        {
            get { return Prezime + " " + Ime; }
        }

    }
}
