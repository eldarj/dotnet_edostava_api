﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using eDostava.Data;
using eDostava.Data.Models;
using eDostava.Web.ViewModels;

namespace eDostava.Web.Controllers
{
    public class BlokController : Controller
    {
        private MojContext context;
        public BlokController (MojContext db)
        {
            context = db;
        }
        public IActionResult Index(int gradId)
        {
            BlokPrikazVM Model = new BlokPrikazVM();
            Model.GradId = gradId;
            Model.Blokovi = context.Blokovi
                .Where(x => x.GradID == gradId)
                .Select(x => new BlokPrikazVM.BlokPrikazInfo()
                {
                    BlokId = x.BlokID,
                    Naziv = x.Naziv,
                    BrojNarucioca = context.Narucioci.Where(s=>s.BlokID == x.BlokID).Count()
                })
                .ToList();

            return PartialView(Model);
        }
        public IActionResult Dodaj(int gradId)
        {
            BlokDodajVM Model = new BlokDodajVM();
            Model.GradId = gradId;

            return PartialView("Uredi", Model);
        }
        public IActionResult Uredi(int blokId)
        {
            Blok x = context.Blokovi.Find(blokId);
            return PartialView(new BlokDodajVM
            {
                GradId = x.GradID,
                BlokId = x.BlokID,
                nazivBloka = x.Naziv
            });
        }
        public IActionResult Snimi(int blokId, int gradId, string naziv)
        {
            Blok blok;
            if (blokId == 0)
            {
                blok = new Blok();
                blok.GradID = gradId;
                context.Blokovi.Add(blok);
            }
            else
            {
                blok = context.Blokovi.Find(blokId);
            }
            blok.Naziv = naziv;

            context.SaveChanges();
            return Redirect("/Blok/Index?gradId=" + gradId);
        }

        public IActionResult Obrisi(int blokId)
        {
            Blok x = context.Blokovi.Find(blokId);
            int gradId = x.GradID;
            context.Blokovi.Remove(x);
            context.SaveChanges();

            return Redirect("/Blok/Index?Id=" + gradId);
        }
    }
}