using ElektronikMagazaWebsite.ViewModel;
using EntityFrameworkLibrary;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using Microsoft.Owin.Security;

namespace ElektronikMagazaWebsite.Controllers
{

    public class SecurityController : Controller
    {
        // GET: Security
        ElektronikMagazaDBEntities db = new ElektronikMagazaDBEntities();

        //[AllowAnonymous]
        //[Route("Security/Login")]
        //public ActionResult Login()
        //{

        //    return View();
        //}

        //[AllowAnonymous]
        //[HttpPost]
        //[Route("Security/Login")]
        //public ActionResult Login(ViewLoginModel user)
        //{
        //    var kullanici = db.Kullanicilar.FirstOrDefault(x => x.KullaniciAdi == user.KullaniciAdi && x.KullaniciSifre == user.KullaniciSifre);
        //    if (kullanici != null)
        //    {
        //        //JavaScriptSerializer serializer = new JavaScriptSerializer();
        //        //string userData = serializer.Serialize(user);

        //        FormsAuthentication.SetAuthCookie(user.KullaniciAdi, false);
        //        return RedirectToAction("Kullanicilar", "Admin");
        //    }
        //    else
        //    {
        //        ViewBag.Mesaj = "Geçersiz Kullanıcı";
        //    }

        //    return View();
        //}

        [AllowAnonymous]
        [Route("Security/UyeLogin")]
        public ActionResult UyeLogin()
        {
            if (User.Identity.IsAuthenticated)
            {
                RedirectToAction("Index", "Home");
            }

            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("Security/UyeLogin")]
        public ActionResult UyeLogin(ViewLoginModel user)
        {
           

            var kul = db.Kullanicilar.FirstOrDefault(x => x.KullaniciMail == user.Eposta);
            if (kul != null)
            {
                if (kul.KullaniciSifre == user.KullaniciSifre)
                {
                    var mdl = new kullaniciModel
                    {
                        kulId = kul.KullaniciID,
                        kulAdi = kul.KullaniciAdi,
                        grupId = kul.KullaniciGrup,

                    };

                    /*1 =admin 2=son kullanici 3=bayi*/
                    if (kul.KullaniciGrup == 1) {
                        var admin = db.Kullanicilar.FirstOrDefault(f => f.KullaniciID == kul.KullaniciID);
                        if (admin != null)
                        {
                            mdl.kulId = admin.KullaniciID;
                            mdl.kulAdi = admin.KullaniciAdi;
                            mdl.grupId = admin.KullaniciGrup;
                        }

                        
                    }
                    else if (kul.KullaniciGrup == 2)
                    {
                        var sonkullanici = db.Kullanicilar.FirstOrDefault(f => f.KullaniciID == kul.KullaniciID);
                        if (sonkullanici != null)
                        {
                            mdl.kulId = sonkullanici.KullaniciID;
                            mdl.kulAdi = sonkullanici.KullaniciAdi;
                            mdl.grupId = sonkullanici.KullaniciGrup;
                        }
                    }
                    else if (kul.KullaniciGrup == 3)
                    {
                        var bayi = db.Bayiler.FirstOrDefault(f => f.BayiID == kul.KullaniciBayiId);
                        if (bayi != null)
                        {
                            mdl.bayiId = bayi.BayiID;
                            mdl.bayiKod = bayi.BayiKod;
                            mdl.bayiUnvan = bayi.BayiUnvan;
                            mdl.bayiBakiye = bayi.BayiBakiye;
                        }
                    }



                        JavaScriptSerializer serializer = new JavaScriptSerializer();
                    string userData = serializer.Serialize(mdl);

                    var claims = new List<Claim>();
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, mdl.kulId.ToString()));
                    claims.Add(new Claim(ClaimTypes.Role, kul.KullaniciGrup.ToString()));
                    claims.Add(new Claim(ClaimTypes.UserData, userData));

                    var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
                    Request.GetOwinContext().Authentication.SignIn(
                        new AuthenticationProperties
                        {
                            AllowRefresh = true,
                            IsPersistent = true, //beni hatırla olayı :)
                            IssuedUtc = DateTime.UtcNow,
                            ExpiresUtc = DateTime.UtcNow.AddDays(1)
                        }, identity
                    );


                    if (kul.KullaniciGrup == 1)
                    {
                        return RedirectToAction("Kullanicilar", "Admin");
                    }
                    else {
                        return RedirectToAction("Index", "Home");
                    }
                        
                }
                else
                {
                    ViewBag.Mesaj = "Geçersiz Kullanıcı";
                }
            }
            else
            {
                ViewBag.Mesaj = "Geçersiz Kullanıcı";
            }

            return View();
        }




        [Route("Security/Logout")]
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            Request.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            return Redirect("/Home/Index");
        }
    }
}