using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElektronikMagazaWebsite.ViewModel
{
    public partial class ModelBayi
    {
        public int BayiID { get; set; }
        public string BayiKodu { get; set; }
        public string BayiUnvan { get; set; }
        public string BayiYetkiliKisi { get; set; }

    }
    public class modelTalepSonuc
    {
        public modelTalepSonuc(bool durum, string mesaj) : this(durum, mesaj, null)
        {

        }
        public modelTalepSonuc(bool durum, string mesaj, object veri)
        {
            Basarili = durum;
            Mesaj = mesaj;
            Veri = veri;
        }

        public bool Basarili { get; set; }
        public string Mesaj { get; set; } = "";
        public object Veri { get; set; }
    }
}