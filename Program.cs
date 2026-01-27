using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq; 
using FinansTakipSistemi.Models;

namespace FinansTakipSistemi
{
    class Program
    {
        static string aktifKullanici = "";
        static string islemDosyasi => $"{aktifKullanici}_islemler.json";
        static string hedefDosyasi => $"{aktifKullanici}_hedef.json";
        static string bekleyenDosyasi => $"{aktifKullanici}_bekleyen.json";

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            GirisEkrani();
            
            List<Islem> islemler = VerileriYukle<List<Islem>>(islemDosyasi);
            List<Islem> bekleyenler = VerileriYukle<List<Islem>>(bekleyenDosyasi);
            Hedef hedef = VerileriYukle<Hedef>(hedefDosyasi) ?? new Hedef { HedefAdi = "Genel Birikim", HedeflenenTutar = 50000 };

            bool devamEt = true;
            while (devamEt)
            {
                Console.Clear();
                decimal bakiye = HesaplaBakiye(islemler);
                
                // ÜST PANEL
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
                Console.WriteLine($"║ KULLANICI: {aktifKullanici.ToUpper().PadRight(15)} | BAKİYE: {bakiye.ToString("N2").PadRight(15)} TL ║");
                Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
                
                var yaklasanlar = bekleyenler.Count(x => x.Tarih <= DateTime.Now.AddDays(3));
                if(yaklasanlar > 0)
                {
                    Console.BackgroundColor = ConsoleColor.Red; Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"  ⚡ KRİTİK: 3 gün içinde {yaklasanlar} adet ödemeniz yaklaşıyor!  ");
                    Console.ResetColor();
                }

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("\n1. 🟢 PARA GİRİŞİ (Maaş, Yatırım, Satış)");
                Console.WriteLine("2. 🔴 PARA ÇIKIŞI (Fatura, Market, Eğlence)");
                Console.WriteLine("3. 📋 HAREKET DÖKÜMÜ (Detaylı Liste)");
                Console.WriteLine("4. 🎯 BİRİKİM HEDEFİ DURUMU");
                Console.WriteLine("5. 🧠 YAPAY ZEKA ANALİZİ & ÖNERİLER");
                Console.WriteLine("6. 📅 FATURA & ÖDEME TAKVİMİ");
                Console.WriteLine("7. 🚪 KAYDET VE GÜVENLİ ÇIKIŞ");
                Console.Write("\nİşlem Seçiniz: ");

                switch (Console.ReadLine())
                {
                    case "1": IslemGerceklestir(islemler, hedef, true); break;
                    case "2": IslemGerceklestir(islemler, hedef, false); break;
                    case "3": HareketleriListele(islemler); break;
                    case "4": HedefGoster(hedef); break;
                    case "5": AkilliAnaliz(islemler); break;
                    case "6": GelecekPlanlayici(bekleyenler, islemler, hedef); break;
                    case "7": Kaydet(islemler, hedef, bekleyenler); devamEt = false; break;
                }
            }
        }

        static void IslemGerceklestir(List<Islem> liste, Hedef hedef, bool gelirMi)
        {
            Console.Clear();
            Console.ForegroundColor = gelirMi ? ConsoleColor.Green : ConsoleColor.Magenta;
            Console.WriteLine(gelirMi ? "=== [ GELİR KAYNAĞI SEÇİN ] ===" : "=== [ GİDER KATEGORİSİ SEÇİN ] ===");
            
            if (gelirMi) {
                Console.WriteLine("1. Maaş/Hakediş      2. Freelance/Ek İş   3. Borsa/Hisse Senedi");
                Console.WriteLine("4. Kripto Kârı       5. Döviz Arbitraj    6. İkinci El Satış");
                Console.WriteLine("7. Kira Geliri       8. Faiz/Repo         9. Hediye/İkramiye");
                Console.WriteLine("10. Vergi İadesi     11. Diğer Gelir");
            } else {
                Console.WriteLine("1. Kira/Mortgage     2. Mutfak/Market     3. Elektrik/Su/Gaz");
                Console.WriteLine("4. İnternet/TV       5. Telefon Faturası  6. Dışarıda Yemek/Kahve");
                Console.WriteLine("7. Akaryakıt/Ulaşım  8. Araç Bakım/Sigorta 9. Kıyafet/Aksesuar");
                Console.WriteLine("10. Eğitim/Kurs      11. Sağlık/Eczane    12. Sinema/Konser/Hobi");
                Console.WriteLine("13. Dijital Abonelik 14. Evcil Hayvan     15. Kişisel Bakım/Kozmetik");
                Console.WriteLine("16. Borç/Taksit      17. Bağış/Yardım     18. Beklenmedik Gider");
            }
            Console.ResetColor();

            Console.Write("\nKategori No: "); string s = Console.ReadLine();
            Console.Write("Miktar (TL): ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal miktar)) return;

            string kat = (gelirMi, s) switch {
                (true, "1") => "Maaş", (true, "2") => "Freelance", (true, "3") => "Borsa", (true, "4") => "Kripto",
                (true, "5") => "Döviz", (true, "6") => "Eski Eşya Satış", (true, "7") => "Kira Geliri", (true, "8") => "Faiz",
                (true, "9") => "Hediye", (true, "10") => "Vergi İadesi", (false, "1") => "Barınma", (false, "2") => "Mutfak",
                (false, "3") => "Enerji", (false, "4") => "İnternet", (false, "5") => "Mobil", (false, "6") => "Gastronomi",
                (false, "7") => "Ulaşım", (false, "8") => "Araç", (false, "9") => "Moda", (false, "10") => "Gelişim",
                (false, "11") => "Sağlık", (false, "12") => "Sosyal Aktiviteler", (false, "13") => "Abonelikler",
                (false, "14") => "Pet", (false, "15") => "Bakım", (false, "16") => "Finansal Geri Ödeme",
                (false, "17") => "Yardım", _ => "Genel"
            };

            liste.Add(new Islem { Miktar = miktar, GelirMi = gelirMi, Kategori = kat, Tarih = DateTime.Now });
            if (gelirMi) hedef.MevcutBirikim += miktar; else hedef.MevcutBirikim -= miktar;

            Console.WriteLine("\n" + new string('-', 40));
            Console.ForegroundColor = gelirMi ? ConsoleColor.Green : ConsoleColor.Red;
            Console.WriteLine(gelirMi ? $"💰 HARİKA! +{miktar:N2} TL cüzdana eklendi." : $"💸 DİKKAT! -{miktar:N2} TL çıkış yapıldı.");
            Console.ResetColor();
            Console.WriteLine($"Kategori: {kat} | Yeni Bakiyeniz: {HesaplaBakiye(liste):N2} TL");
            Console.WriteLine(new string('-', 40));
            Console.ReadKey();
        }

        static void AkilliAnaliz(List<Islem> liste)
        {
            Console.Clear();
            var giderler = liste.Where(x => !x.GelirMi).ToList();
            if (!giderler.Any()) { Console.WriteLine("⚠️ Analiz için harcama yapmanız gerekiyor."); Console.ReadKey(); return; }
            
            decimal toplamGider = giderler.Sum(x => x.Miktar);
            var katOzet = giderler.GroupBy(x => x.Kategori)
                                  .Select(g => new { Isim = g.Key, Toplam = g.Sum(x => x.Miktar), Yuzde = (g.Sum(x => x.Miktar) / toplamGider) * 100 })
                                  .OrderByDescending(x => x.Toplam);

            Console.WriteLine("📊 DETAYLI HARCAMA DAĞILIMI");
            Console.WriteLine("---------------------------------------------");
            foreach (var k in katOzet) {
                string bar = new string('■', (int)k.Yuzde / 5);
                Console.WriteLine($"{k.Isim,-18} | {k.Toplam,8:N2} TL | %{k.Yuzde,5:F1} {bar}");
            }
            
            Console.WriteLine("\n💡 ASİSTAN ÖNERİSİ:");
            var enCok = katOzet.First();
            if(enCok.Isim == "Gastronomi" && enCok.Yuzde > 20) 
                Console.WriteLine("👉 Dışarıda yemek masrafın bütçenin %20'sini aşmış. Biraz evde yemek yapmaya ne dersin?");
            else if(toplamGider > HesaplaBakiye(liste.Where(x => x.GelirMi).ToList()))
                Console.WriteLine("👉 Harcamaların gelirini aşmak üzere! Acil tasarruf moduna geçmelisin.");
            else
                Console.WriteLine("👉 Finansal durumun dengeli görünüyor, böyle devam et Merve!");

            Console.ReadKey();
        }

        static void GirisEkrani()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("***************************************");
            Console.WriteLine("* SMART FINANCE MANAGER v2.0       *");
            Console.WriteLine("***************************************");
            Console.ResetColor();
            Console.Write("\nKimlik Doğrulama (İsim): ");
            aktifKullanici = Console.ReadLine()?.ToLower() ?? "merve";
        }

        static void HareketleriListele(List<Islem> liste)
        {
            Console.Clear();
            Console.WriteLine("{0,-12} | {1,-20} | {2,12}", "TARİH", "KATEGORİ", "TUTAR");
            Console.WriteLine(new string('═', 50));
            foreach (var i in liste.OrderByDescending(x => x.Tarih)) {
                Console.ForegroundColor = i.GelirMi ? ConsoleColor.Green : ConsoleColor.Red;
                Console.WriteLine($"{i.Tarih:dd.MM.yyyy} | {i.Kategori,-20} | {(i.GelirMi ? "+" : "-")}{i.Miktar,10:N2} TL");
            }
            Console.ResetColor(); Console.ReadKey();
        }

        static void HedefGoster(Hedef h)
        {
            Console.Clear();
            decimal yuzde = h.YuzdeHesapla();
            Console.WriteLine($"🎯 HEDEF: {h.HedefAdi}");
            Console.WriteLine($"💰 Durum: {h.MevcutBirikim:N2} / {h.HedeflenenTutar:N2} TL");
            int doluluk = (int)(Math.Clamp(yuzde, 0, 100) / 5);
            Console.Write("["); Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(new string('█', doluluk) + new string('░', 20 - doluluk));
            Console.ResetColor(); Console.WriteLine($"] %{yuzde:F2}");
            Console.ReadKey();
        }

        static void GelecekPlanlayici(List<Islem> bekleyenler, List<Islem> islemler, Hedef h)
        {
            Console.Clear();
            Console.WriteLine("1. Ödeme Hatırlatıcı Ekle | 2. Ödeme Gerçekleştir");
            string s = Console.ReadLine();
            if (s == "1") {
                Console.Write("Fatura/Ödeme Adı: "); string k = Console.ReadLine();
                Console.Write("Tutar: "); decimal m = decimal.Parse(Console.ReadLine());
                Console.Write("Vade (GG.AA.YYYY): "); DateTime t = DateTime.Parse(Console.ReadLine());
                bekleyenler.Add(new Islem { Kategori = k, Miktar = m, Tarih = t, GelirMi = false });
            } else if (s == "2") {
                for (int i = 0; i < bekleyenler.Count; i++)
                    Console.WriteLine($"{i+1}. [{bekleyenler[i].Tarih:dd.MM.yyyy}] {bekleyenler[i].Kategori} - {bekleyenler[i].Miktar:N2} TL");
                Console.Write("\nÖdenen No: ");
                if(int.TryParse(Console.ReadLine(), out int idx) && idx > 0 && idx <= bekleyenler.Count) {
                    var o = bekleyenler[idx-1]; o.Tarih = DateTime.Now;
                    islemler.Add(o); h.MevcutBirikim -= o.Miktar; bekleyenler.RemoveAt(idx-1);
                    Console.WriteLine($"\n✅ Ödeme başarıyla sisteme işlendi.");
                }
            }
            Console.ReadKey();
        }

        static void Kaydet(object i, object h, object b) {
            File.WriteAllText(islemDosyasi, JsonSerializer.Serialize(i));
            File.WriteAllText(hedefDosyasi, JsonSerializer.Serialize(h));
            File.WriteAllText(bekleyenDosyasi, JsonSerializer.Serialize(b));
        }

        static T VerileriYukle<T>(string dosya) where T : new() {
            if (!File.Exists(dosya)) return new T();
            return JsonSerializer.Deserialize<T>(File.ReadAllText(dosya)) ?? new T();
        }

        static decimal HesaplaBakiye(List<Islem> l) => l.Where(x => x.GelirMi).Sum(x => x.Miktar) - l.Where(x => !x.GelirMi).Sum(x => x.Miktar);
    }
}