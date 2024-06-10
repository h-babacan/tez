using EntityFrameworkLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElektronikMagazaWebsite
{
    public class UiData
    {
        public static List<Kategoriler> GetKategoriler(int katUstId=0)
        {
            ElektronikMagazaDBEntities db = new ElektronikMagazaDBEntities();
            {
                var item = db.Kategoriler.Where(w => w.KatUstID == katUstId).ToList();

                return item;
            }

        }
        public static List<Urunler> GetUrunler()
        {
            ElektronikMagazaDBEntities db = new ElektronikMagazaDBEntities();
            {
                var item = db.Urunler.ToList();

                return item;
            }

        }
        //public static List<Urunler> GetUrunler(string dilKod)
        //{
        //    GiyimDBEntities db = new GiyimDBEntities();
        //    {
        //        var item = db.Urunler.Where(x => x.UrunDilKod == dilKod).ToList();

        //        return item;
        //    }

        //}

        //public static List<Kategoriler> GetAltKategoriler(int ustKatId)
        //{
        //    GiyimDBEntities db = new GiyimDBEntities();
        //    {
        //        var item = db.Kategoriler.Where(x => x.KatUstID == ustKatId).ToList();

        //        return item;
        //    }

        //}
    }
}