using EntityFrameworkLibrary;
using ElektronikMagazaWebsite.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static ElektronikMagazaWebsite.ViewModel.AdminKategoriListe;
using static ElektronikMagazaWebsite.ViewModel.AdminBayiler;

namespace ElektronikMagazaWebsite.Models
{
    public class PageItem
    {
        public List<vmAdminKategoriler> AdminDropDownKategoriler = new List<vmAdminKategoriler>();
        public List<vmAdminBayiler> AdminBayiler = new List<vmAdminBayiler>();


        public List<Kullanicilar> kullanicilar = new List<Kullanicilar>();
        public Kullanicilar kullanici = new Kullanicilar();

        public List<Kategoriler> kategoriler = new List<Kategoriler>();
        public Kategoriler kategori = new Kategoriler();

        public List<Urunler> urunler = new List<Urunler>();
        public Urunler urun = new Urunler();

        public List<Bayiler> bayiler = new List<Bayiler>();
        public Bayiler bayi = new Bayiler();

        public SepetModel Sepet { get; set; } = new SepetModel();
        

    }
}