﻿using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace eDostava.Data.Models
{

    public enum Stanje
    {
        [Display(Name = "Narudžba nije validna!")]
        NijeValidna,
        [Display(Name = "Narudžba isporučena! Ispraznili smo vašu korpu ukoliko želite poručiti još nešto.")]
        Isporucena,
        [Display(Name = "Dodane stavke:")]
        Aktivna,
        [Display(Name = "Korpa je prazna. Pregledajte restorane i jelovnike, izaberite i dodajte u korpu nešto fino!")]
        Prazna
    }
    public class Narudzba
    {
        [Key]
        public int NarudzbaID { get; set; }
        public Guid Sifra { get; set; } = Guid.NewGuid();
        public DateTime DatumVrijeme { get; set; } = DateTime.UtcNow;
        public double UkupnaCijena { get; set; } = 0;
        public Stanje Status { get; set; } = Stanje.Prazna;
        [ForeignKey("Kupon")]
        public int? KuponID { get; set; }
        public Kupon Kupon { get; set; }
        public string GetKuponOpis => Kupon == null ? "~" : (Kupon.Opis ?? "~");
    }
}
