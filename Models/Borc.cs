namespace FinansTakipSistemi.Models
{
    public class Borc
    {
        public string Kisi { get; set; }
        public decimal Miktar { get; set; }
        public DateTime SonOdemeTarihi { get; set; }
        public bool AlacakMi { get; set; } // true ise alacak, false ise borç
        public bool OdendiMi { get; set; }
    }
}