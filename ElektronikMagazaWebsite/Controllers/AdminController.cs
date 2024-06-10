using BusinessLibrary;
using ElektronikMagazaWebsite.Models;
using EntityFrameworkLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static ElektronikMagazaWebsite.ViewModel.AdminKategoriListe;
using static ElektronikMagazaWebsite.ViewModel.AdminBayiler;
using System.Data;
using System.Web.Mvc;
using ElektronikMagazaWebsite.ViewModel;
using System.Security.Cryptography;
using System.Web.Hosting;

namespace ElektronikMagazaWebsite.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetSiparisler()
        {
            // Veritabanından verileri çek
            using (ElektronikMagazaDBEntities db = new ElektronikMagazaDBEntities())
            {
                var data = db.SiparisKart.Select(x => new
                {
                    x.SiparisKartID,
                }).ToList();
                return Json(new { data = data }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Siparisler(AdminSiparisler mdl)
        {
            ElektronikMagazaDBEntities db = new ElektronikMagazaDBEntities();
            {
                AdminSiparisler admSip = new AdminSiparisler();

                var sipListe = db.SiparisKart.ToList();

                //var sipKul = sipListe.Distinct()

                if (!String.IsNullOrEmpty(mdl.Aranan))
                {

                    sipListe = sipListe.Where(w => w.bayiKod == mdl.Aranan).ToList();

                }

                if (mdl.tarih1.Year > 0001 && mdl.tarih2.Year > 0001)
                {
                    sipListe = sipListe.Where(w => w.SiparisKartTarih.Value.Date >= mdl.tarih1.Date && w.SiparisKartTarih.Value.Date <= mdl.tarih2.Date).ToList();
                }
                else if (mdl.tarih1.Year > 0001 && mdl.tarih2.Year == 0001)
                {
                    sipListe = sipListe.Where(w => w.SiparisKartTarih.Value.Date == mdl.tarih1.Date).ToList();
                }
                else if (mdl.tarih2.Year > 0001 && mdl.tarih1.Year == 0001)
                {
                    sipListe = sipListe.Where(w => w.SiparisKartTarih.Value.Date == mdl.tarih2.Date).ToList();
                }


                admSip.Liste = sipListe;
              

                return View(admSip);
            }
        }
       
        public ActionResult SiparisIncele(AdminSiparisler mdl,int id)
        {
            ElektronikMagazaDBEntities db = new ElektronikMagazaDBEntities();
            {
                AdminSiparisler admSip = new AdminSiparisler();
                var sipharListe = db.SiparisHareket.ToList();
                var sipkartListe = db.SiparisKart.ToList();
                if (!String.IsNullOrEmpty(mdl.Aranan))
                {

                    sipharListe = sipharListe.Where(w => w.SipHarUrunAdi.IndexOf(mdl.Aranan, StringComparison.OrdinalIgnoreCase) >= 0).ToList();

                }

                admSip.hareketListe = sipharListe.Where(x=>x.siparisKartID==id).ToList();
                admSip.Liste = sipkartListe.Where(x=>x.SiparisKartID==id).ToList();
             
             
                //admSip.hareketListe = db.SiparisHareket.Where(x => x.siparisKartID == id).ToList();


                return View(admSip);
            }
        }






        #region Kullanicilar
        [Route("Admin/Kullanicilar")]
        public ActionResult Kullanicilar()
        {
            ElektronikMagazaDBEntities db = new ElektronikMagazaDBEntities();
            {
                List<Kullanicilar> liste = db.Kullanicilar.ToList();
                return View(liste);
            }
        }
        [Route("Admin/KullaniciEkle")]
        public ActionResult KullaniciEkle()
        {
            ElektronikMagazaDBEntities db = new ElektronikMagazaDBEntities();
            {
                PageItem pi = new PageItem();
                pi.AdminBayiler = new List<vmAdminBayiler>();

                var dbbayi = db.Bayiler.ToList();


                foreach (var item in dbbayi)
                {
                    pi.AdminBayiler.Add(new vmAdminBayiler { BayiID = item.BayiID, BayiAdi = item.BayiUnvan });




                }


                return View(pi);
            }
        }


        [Route("Admin/KullaniciIslem")]
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult KullaniciIslem(Kullanicilar kullanicilar)
        {
            ElektronikMagazaDBEntities db = new ElektronikMagazaDBEntities();
            if (kullanicilar.KullaniciID == 0)
            {
                db.Kullanicilar.Add(kullanicilar);
                db.SaveChanges();
                return RedirectToAction("Kullanicilar");
            }
            else
            {
                var kullanici = db.Kullanicilar.Find(kullanicilar.KullaniciID);
                kullanici.KullaniciGrup = kullanicilar.KullaniciGrup;
                kullanici.KullaniciBayiId = kullanicilar.KullaniciBayiId;
                kullanici.KullaniciAdi = kullanicilar.KullaniciAdi;
                kullanici.KullaniciMail = kullanicilar.KullaniciMail;
                kullanici.KullaniciSifre = kullanicilar.KullaniciSifre;


                db.SaveChanges();
                return Redirect($"KullaniciDuzenle?id={kullanicilar.KullaniciID}");
            }
        }

        [Route("Admin/KullaniciDuzenle")]
        public ActionResult KullaniciDuzenle(int id)
        {
            PageItem pi = new PageItem();
            ElektronikMagazaDBEntities db = new ElektronikMagazaDBEntities();
            {

                pi.kullanici = db.Kullanicilar.Where(x => x.KullaniciID == id).FirstOrDefault();

                pi.AdminBayiler = new List<vmAdminBayiler>();

                var dbbayi = db.Bayiler.ToList();


                foreach (var item in dbbayi)
                {
                    pi.AdminBayiler.Add(new vmAdminBayiler { BayiID = item.BayiID, BayiAdi = item.BayiUnvan });

                }

            }

            return View(pi);
        }

        [Route("Admin/KullaniciSil")]
        public ActionResult KullaniciSil(int id)
        {
            ElektronikMagazaDBEntities db = new ElektronikMagazaDBEntities();
            {
                var kullanicisil = db.Kullanicilar.Find(id);
                db.Kullanicilar.Remove(kullanicisil);
                db.SaveChanges();
                return RedirectToAction("Kullanicilar");
            }
        }
        #endregion

        #region Bayiler
        [Route("Admin/Bayiler")]
        public ActionResult Bayiler()
        {
            ElektronikMagazaDBEntities db = new ElektronikMagazaDBEntities();
            {
                List<Bayiler> liste = db.Bayiler.ToList();
                return View(liste);
            }
        }
        [Route("Admin/BayiEkle")]
        public ActionResult BayiEkle()
        {
            ElektronikMagazaDBEntities db = new ElektronikMagazaDBEntities();
            {
                PageItem pi = new PageItem();






                return View(pi);
            }
        }


        [Route("Admin/BayiIslem")]
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult BayiIslem(Bayiler bayiler)
        {
            ElektronikMagazaDBEntities db = new ElektronikMagazaDBEntities();
            if (bayiler.BayiID == 0)
            {
                db.Bayiler.Add(bayiler);
                db.SaveChanges();
                return RedirectToAction("Bayiler");
            }
            else
            {
                var bayi = db.Bayiler.Find(bayiler.BayiID);
                bayi.BayiKod = bayiler.BayiKod;
                bayi.BayiUnvan = bayiler.BayiUnvan;
                bayi.BayiYetkiliKisi = bayiler.BayiYetkiliKisi;
                bayi.BayiSifre = bayiler.BayiSifre;


                db.SaveChanges();
                return Redirect($"BayiDuzenle?id={bayiler.BayiID}");
            }
        }

        [Route("Admin/BayiDuzenle")]
        public ActionResult BayiDuzenle(int id)
        {
            PageItem item = new PageItem();
            ElektronikMagazaDBEntities db = new ElektronikMagazaDBEntities();
            {

                item.bayi = db.Bayiler.Where(x => x.BayiID == id).FirstOrDefault();


            }
            return View(item);
        }

        [Route("Admin/BayiSil")]
        public ActionResult BayiSil(int id)
        {
            ElektronikMagazaDBEntities db = new ElektronikMagazaDBEntities();
            {
                var bayisil = db.Bayiler.Find(id);
                db.Bayiler.Remove(bayisil);
                db.SaveChanges();
                return RedirectToAction("Bayiler");
            }
        }
        #endregion



        [Route("Admin/Kategoriler")]
        public ActionResult Kategoriler()
        {
            ElektronikMagazaDBEntities db = new ElektronikMagazaDBEntities();
            {
                List<Kategoriler> liste = db.Kategoriler.ToList();
                return View(liste);
            }
        }

        [Route("Admin/KategoriEkle")]
        public ActionResult KategoriEkle()
        {
            ElektronikMagazaDBEntities db = new ElektronikMagazaDBEntities();
            {
                PageItem pi = new PageItem();
                pi.AdminDropDownKategoriler = new List<vmAdminKategoriler>();

                var dbAnaKat = db.Kategoriler.Where(w => w.KatUstID == 0).ToList();


                foreach (var item in dbAnaKat)
                {
                    pi.AdminDropDownKategoriler.Add(new vmAdminKategoriler { KatID = item.KategoriID, KatAdi = item.KategoriAdi });

                    //var altKat = db.Kategoriler.Where(w => w.KatUstId == item.KatId).ToList();
                    //foreach (var itemAlt in altKat)
                    //{
                    //    pi.AdminDropDownKategori.Add(new vmAdminKategori { 
                    //        KatId = itemAlt.KatId,
                    //        KategoriAdi = itemAlt.KatAdi,
                    //        KategoriYol = item.KatAdi + " >> " + itemAlt.KatAdi

                    //    });
                    //}


                }


                return View(pi);
            }
        }

        [Route("Admin/KategorilerIslem")]
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult KategorilerIslem(Kategoriler kategoriler, HttpPostedFileBase katresimfile)
        {

            ElektronikMagazaDBEntities db = new ElektronikMagazaDBEntities();
            if (kategoriler.KategoriID == 0)
            {
                kategoriler.KategoriResimUrl = katresimfile.ToFileUploadKaydet("../uploads/", kategoriler.KategoriResimUrl);
                db.Kategoriler.Add(kategoriler);
                db.SaveChanges();
                return RedirectToAction("Kategoriler");
            }
            else
            {
                var kategori = db.Kategoriler.Find(kategoriler.KategoriID);
                kategori.KategoriAdi = kategoriler.KategoriAdi;
                kategori.KatUstID = kategoriler.KatUstID;
                kategori.KategoriAciklama = kategoriler.KategoriAciklama;
                kategori.KatPopuler = kategoriler.KatPopuler;
                if (katresimfile != null)
                {
                    kategori.KategoriResimUrl = katresimfile.ToFileUploadKaydet("../uploads/", kategoriler.KategoriResimUrl);
                }
                else
                {
                    kategori.KategoriResimUrl = kategoriler.KategoriResimUrl;
                }

                db.SaveChanges();
                return Redirect($"KategoriDuzenle?id={kategoriler.KategoriID}");
            }
        }

        [Route("Admin/KategoriDuzenle")]
        public ActionResult KategoriDuzenle(int id)
        {
            PageItem item = new PageItem();
            ElektronikMagazaDBEntities db = new ElektronikMagazaDBEntities();
            {

                item.kategori = db.Kategoriler.Where(x => x.KategoriID == id).FirstOrDefault();
                if (string.IsNullOrEmpty(item.kategori.KategoriResimUrl))
                {
                    item.kategori.KategoriResimUrl = "../uploads/_blank.png";
                }


            }
            return View(item);
        }

        [Route("Admin/KategoriSil")]
        public ActionResult KategoriSil(int id)
        {
            ElektronikMagazaDBEntities db = new ElektronikMagazaDBEntities();
            {
                var katsil = db.Kategoriler.Find(id);
                db.Kategoriler.Remove(katsil);
                db.SaveChanges();
                return RedirectToAction("Kategoriler");
            }
        }

        #region Urunler


        [Route("Admin/Urunler")]
        public ActionResult Urunler()
        {
            ElektronikMagazaDBEntities db = new ElektronikMagazaDBEntities();
            {
                List<Urunler> liste = db.Urunler.ToList();
                return View(liste);
            }
        }

        [Route("Admin/UrunEkle")]
        public ActionResult UrunEkle()
        {
            ElektronikMagazaDBEntities db = new ElektronikMagazaDBEntities();
            {
                PageItem pi = new PageItem();
                pi.AdminDropDownKategoriler = new List<vmAdminKategoriler>();

                var dbAnaKat = db.Kategoriler.ToList();


                foreach (var item in dbAnaKat)
                {
                    pi.AdminDropDownKategoriler.Add(new vmAdminKategoriler { KatID = item.KategoriID, KatAdi = item.KategoriAdi });
                }


                return View(pi);
            }
        }


        [Route("Admin/UrunIslem")]
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult UrunIslem(Urunler urunler, HttpPostedFileBase urunResimFile1, HttpPostedFileBase urunResimFile2, HttpPostedFileBase urunResimFile3)
        {
            ElektronikMagazaDBEntities db = new ElektronikMagazaDBEntities();
            if (urunler.UrunID == 0)
            {
                urunler.UrunResimUrl1 = urunResimFile1.ToFileUploadKaydet("../uploads/", urunler.UrunResimUrl1);
                urunler.UrunResimUrl2 = urunResimFile2.ToFileUploadKaydet("../uploads/", urunler.UrunResimUrl2);
                urunler.UrunResimUrl3 = urunResimFile3.ToFileUploadKaydet("../uploads/", urunler.UrunResimUrl3);
                db.Urunler.Add(urunler);
                db.SaveChanges();
                return RedirectToAction("Urunler");
            }
            else
            {
                var urun = db.Urunler.Find(urunler.UrunID);
                urun.katid = urunler.katid;
                urun.UrunAdi = urunler.UrunAdi;
                urun.UrunAciklama = urunler.UrunAciklama;
                urun.UrunAltAciklama = urunler.UrunAltAciklama;
                urun.UrunFiyat = urunler.UrunFiyat;
                urun.UrunBayiFiyat = urunler.UrunBayiFiyat;
                urun.UrunPopuler = urunler.UrunPopuler;
                urun.UrunYeni = urunler.UrunYeni;
                if (urunResimFile1 != null)
                {
                    urun.UrunResimUrl1 = urunResimFile1.ToFileUploadKaydet("../uploads/", urunler.UrunResimUrl1);
                }
                else
                {
                    urun.UrunResimUrl1 = urunler.UrunResimUrl1;
                }

                if (urunResimFile2 != null)
                {
                    urun.UrunResimUrl2 = urunResimFile2.ToFileUploadKaydet("../uploads/", urunler.UrunResimUrl2);
                }
                else
                {
                    urun.UrunResimUrl2 = urunler.UrunResimUrl2;
                }
                if (urunResimFile3 != null)
                {
                    urun.UrunResimUrl3 = urunResimFile3.ToFileUploadKaydet("../uploads/", urunler.UrunResimUrl3);
                }
                else
                {
                    urun.UrunResimUrl3 = urunler.UrunResimUrl3;
                }



                db.SaveChanges();
                return Redirect($"UrunDuzenle?id={urunler.UrunID}");
            }
        }

        [Route("Admin/UrunDuzenle")]
        public ActionResult UrunDuzenle(int id)
        {
            PageItem item = new PageItem();
            ElektronikMagazaDBEntities db = new ElektronikMagazaDBEntities();
            {

                item.urun = db.Urunler.Where(x => x.UrunID == id).FirstOrDefault();
                item.kategoriler = db.Kategoriler.ToList();

                if (string.IsNullOrEmpty(item.urun.UrunResimUrl1))
                {
                    item.urun.UrunResimUrl1 = "../uploads/_blank.png";

                }
                if (string.IsNullOrEmpty(item.urun.UrunResimUrl2))
                {
                    item.urun.UrunResimUrl2 = "../uploads/_blank.png";

                }
                if (string.IsNullOrEmpty(item.urun.UrunResimUrl3))
                {
                    item.urun.UrunResimUrl3 = "../uploads/_blank.png";

                }

                return View(item);
            }


        }
        [Route("Admin/UrunSil")]
        public ActionResult UrunSil(int id)
        {
            ElektronikMagazaDBEntities db = new ElektronikMagazaDBEntities();
            {
                var urunsil = db.Urunler.Find(id);
                db.Urunler.Remove(urunsil);
                db.SaveChanges();
                return RedirectToAction("Urunler");
            }
        }
        #endregion



    }
}