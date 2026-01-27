namespace FinansTakipSistemi.Models
{
    public class Hedef
    {
        public string HedefAdi { get; set; }
        public decimal HedeflenenTutar { get; set; }
        public decimal MevcutBirikim { get; set; }

        public decimal YuzdeHesapla()
        {
            if (HedeflenenTutar == 0) return 0;
            return (MevcutBirikim / HedeflenenTutar) * 100;
        }
    }
}