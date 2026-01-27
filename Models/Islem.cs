namespace FinansTakipSistemi.Models
{
    public class Islem
    {
        public int Id { get; set; }
        public decimal Miktar { get; set; }
        public string Kategori { get; set; }
        public DateTime Tarih { get; set; }
        public bool GelirMi { get; set; } // true: Gelir, false: Gider
        public string Aciklama { get; set; }
    }
}