﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using eDostava.Data;
using eDostava.Data.Models;
using eDostava.Web.Areas.Api.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using eDostava.Web.Areas.Api.Helper;
using Microsoft.AspNetCore.Http;
using eDostava.Web.Helper;
using RS1_Ispit_2017.Helper;

namespace eDostava.Web.Areas.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/Auth")]
    public class AuthController : Controller
    {
        private readonly MojContext _context;
        private readonly IHostingEnvironment _appEnvironment;

        public AuthController(MojContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _appEnvironment = hostingEnvironment;
        }

        // POST: api/Auth
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] AuthLoginVM postAccount)
        {
            Narucilac user = await _context.Narucioci
                .Include(n => n.Blok)
                .ThenInclude(n => n.Grad)
                .Where(n => n.Username == postAccount.Username && n.Password == postAccount.Password)
                .SingleOrDefaultAsync();

            if (user == null)
            {
                return BadRequest("Pogrešan username ili password");
            }

            string token = Guid.NewGuid().ToString();
            UserModelResponse model = new UserModelResponse
            {
                Id = user.KorisnikID,
                Ime = user.Ime,
                Prezime = user.Prezime,
                Username = user.Username,
                Password = user.Password,
                Blok = user.Blok,
                DatumKreiranja = user.DatumKreiranja,
                NarudzbeCount = _context.Narudzbe.Where(n => n.NarucilacID == user.KorisnikID).Count(),
                ZadnjiLogin = DateTime.Now,
                Token = token,
                Adresa = user.Adresa,
                ImageUrl = user.ImageUrl != null && user.ImageUrl.Length > 0 ? user.ImageUrl : MyApiConfig.IMAGE_DIR + "/" + MyApiConfig.DEFAULT_IMAGE,
            };

            await _context.AuthTokeni.AddAsync(new AuthToken
            {
                Value = token,
                NarucilacId = user.KorisnikID,
                DatumGenerisanja = DateTime.Now,
                Ip = HttpContext.Connection.RemoteIpAddress + ":" + HttpContext.Connection.RemotePort
            });

            await _context.SaveChangesAsync();

            return Ok(model);
        }

        [HttpGet]
        [Route("Logout")]
        public IActionResult Logout()
        {
            string token = HttpContext.GetMyAuthToken();
            AuthToken tokenDb = _context.AuthTokeni.Where(t => t.Value == token).Single();
            if (tokenDb != null)
            {
                _context.Remove(tokenDb);
                _context.SaveChanges();
            }

            return Ok();
        }

        // POST: api/Auth/Register
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterRequest Model)
        {
            Narucilac existing = await _context.Narucioci.FindAsync(Model.Id); // provjeri je li postoji user
            if (existing != null)
            {
                return BadRequest("Username " + Model.Username + " je već zauzet");
            }

            if (Model.Id == 0)
            {
                string token = Guid.NewGuid().ToString();
                Narucilac newUser = new Narucilac
                {
                    Ime = Model.Ime,
                    Prezime = Model.Prezime,
                    Password = Model.Password,
                    Username = Model.Username,
                    ImageUrl = Model.ImageUrl != null && Model.ImageUrl.Length > 0 ? Model.ImageUrl : MyApiConfig.IMAGE_DIR + "/" + MyApiConfig.DEFAULT_IMAGE,
                    Adresa = Model.Adresa,
                    BadgeID = 1,
                    DatumKreiranja = DateTime.Now,
                    Blok = _context.Blokovi.Include(b => b.Grad).DefaultIfEmpty(_context.Blokovi.First()).First(b => b.BlokID == Model.BlokID)
                };
                
                await _context.Narucioci.AddAsync(newUser);

                AuthToken tokenDb = new AuthToken
                {
                    Value = token,
                    NarucilacId = newUser.KorisnikID,
                    DatumGenerisanja = DateTime.Now,
                    Ip = HttpContext.Connection.RemoteIpAddress + ":" + HttpContext.Connection.RemotePort
                };

                await _context.AuthTokeni.AddAsync(tokenDb);

                await _context.SaveChangesAsync();

                return Ok(new UserModelResponse
                {
                    Id = newUser.KorisnikID,
                    Ime = newUser.Ime,
                    Prezime = newUser.Prezime,
                    Username = newUser.Username,
                    Password = newUser.Password,
                    Blok = newUser.Blok,
                    DatumKreiranja = newUser.DatumKreiranja,
                    ZadnjiLogin = DateTime.Now,
                    Token = token,
                    Adresa = newUser.Adresa,
                    ImageUrl = newUser.ImageUrl,
                    NarudzbeCount = _context.Narudzbe.Where(n => n.NarucilacID == newUser.KorisnikID).Count()
                });
            }

            return BadRequest("Provjerite podatke");
        }

        // PUT: api/Auth/Update
        [HttpPost]
        [Route("Update")]
        public async Task<IActionResult> Update([FromBody] UserRegisterRequest Model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (Model.Id != 0)
            {
                Narucilac user = await _context.Narucioci.FindAsync(Model.Id);
                user.Ime = Model.Ime ?? user.Ime;
                user.Prezime = Model.Prezime ?? user.Prezime;
                user.Password = Model.Password ?? user.Password;
                user.Username = Model.Username ?? user.Username;
                user.ImageUrl = Model.ImageUrl != null && Model.ImageUrl.Length > 0 ? Model.ImageUrl : MyApiConfig.IMAGE_DIR + "/" + MyApiConfig.DEFAULT_IMAGE;
                user.Adresa = Model.Adresa ?? user.Adresa;
                user.Blok = _context.Blokovi.Include(b => b.Grad).DefaultIfEmpty(_context.Blokovi.First()).First(b => b.BlokID == Model.BlokID);

                await _context.SaveChangesAsync();

                UserModelResponse authUserVM = new UserModelResponse
                {
                    Id = user.KorisnikID,
                    Ime = user.Ime,
                    Prezime = user.Prezime,
                    Username = user.Username,
                    Password = user.Password,
                    Blok = user.Blok,
                    DatumKreiranja = user.DatumKreiranja,
                    ZadnjiLogin = DateTime.Now,
                    Token = "",
                    Adresa = user.Adresa,
                    ImageUrl = user.ImageUrl,
                    NarudzbeCount = _context.Narudzbe.Where(n => n.NarucilacID == user.KorisnikID).Count()
                };

                return CreatedAtAction("Update", new { id = authUserVM.Id }, authUserVM);
            }

            return BadRequest("Update failed, morate proslijediti validan postojeći PK-ID i korisnički račun!");
        }

        // DELETE: api/Auth/Delete
        [HttpPost]
        [Route("Delete")]
        public async Task<IActionResult> Delete([FromBody] UserRegisterRequest Model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Narucilac narucilac = await _context.Narucioci.SingleOrDefaultAsync(m => m.KorisnikID == Model.Id);
            if (narucilac == null)
            {
                return NotFound();
            }

            _context.Narucioci.Remove(narucilac);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // UPLOAD PROFILE IMAGE: api/Auth/User/Image
        [HttpPost]
        [Route("User/Image")]
        public async Task<IActionResult> UploadImage([FromBody] UserImageRequest Model)
        {
            if (Model.EncodedImageBase64 == null || !(Model.EncodedImageBase64.Length > 0))
            {
                return BadRequest("Nedostaje slika.");
            }

            Narucilac user = await _context.Narucioci.FirstAsync(u => u.Username == Model.credentials.Username && u.Password == Model.credentials.Password);

            if (user != null)
            {
                try
                {
                    string Filename = Model.FileName + "_" + Model.credentials.Username + "_" + Guid.NewGuid().ToString().Substring(0, 4) + ".jpeg";
                    string Uploads = Path.Combine(_appEnvironment.WebRootPath, MyApiConfig.IMAGE_DIR);
                    string FilePath = Path.Combine(Uploads, Filename); // Pripremi path i ime slike

                    byte[] imageBytes = Convert.FromBase64String(Model.EncodedImageBase64);
                    System.IO.File.WriteAllBytes(FilePath, imageBytes);

                    user.ImageUrl = MyApiConfig.IMAGE_DIR + "/" + Filename;
                    await _context.SaveChangesAsync();

                    return Ok(user.ImageUrl);
                }
                catch ( Exception e )
                {
                    // handle ili samo pust da akcija vrati bad request?
                    return BadRequest("Dogodila se greska pri konverziji base64 u sliku.");
                }
            }

            return BadRequest("Pogrešan username ili password.");
        }
    }
}