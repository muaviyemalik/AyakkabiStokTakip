using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.Json;
class Program
{
    // Veritabanı niyetine kullandığımız liste
    static List<Ayakkabi> envanter = new List<Ayakkabi>();
    // Verilerin saklanacağı dosya adı
    static string dosyaYolu = "envanter.json";

    static void Main(string[] args)
    {
        // Öncesinde Kayıtlı verileri yükle
        VerileriYukle();

        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Ayakkabı Envanter Sistemi ===");
            Console.WriteLine("1. İsimle Arama Yap");
            Console.WriteLine("2. Yeni Ayakkabı Ekle");
            Console.WriteLine("3. Stok Ekle/Sil");
            Console.WriteLine("4. Tüm Listeyi Göster");
            Console.WriteLine("5. Çıkış");
            Console.Write("Seçiminiz: ");
            
            string secim = Console.ReadLine();

            switch (secim)
            {
                case "1":
                    AyakkabiAra();
                    break;
                case "2":
                    YeniAyakkabiEkle();
                    break;
                case "3":
                    StokGuncelle();
                    break;
                case "4":
                    TumListeyiGoster();
                    break;
                case "5":
                    return;
                default:
                    Console.WriteLine("Geçersiz seçim, lütfen tekrar deneyin.");
                    break;
            }

            Console.WriteLine("\nDevam etmek için bir tuşa basın...");
            Console.ReadKey();
        }
    }

    // --- 1. AYAKKABI ARAMA METODU ---
    static void AyakkabiAra()
    {
        Console.Write("Aranacak ayakkabı adını girin: ");
        string arananAd = Console.ReadLine().ToLower();

        // LINQ kullanarak arama işlemi
        var bulunanAyakkabilar = envanter.Where(x => x.Ad.ToLower().Contains(arananAd)).ToList();

        if (bulunanAyakkabilar.Count == 0)
        {
            Console.WriteLine("Hiçbir ayakkabı bulunamadı.");
        }
        else
        {
            Console.WriteLine("Bulunan Ayakkabılar:");
            Console.WriteLine($"{"Marka",-20} | {"Renk",-10} | {"Numara",-6} | {"Fiyat",-10} | {"Stok Sayısı",-5}");
            Console.WriteLine(new string('-', 60));
            foreach (var ayakkabi in bulunanAyakkabilar)
            {
                // Tablo görünümü için formatlı yazdırma
                Console.WriteLine($"{ayakkabi.Ad,-20} | {ayakkabi.Renk,-10} | {ayakkabi.Numara,-6} | {ayakkabi.Fiyat + " TL",-10} | {ayakkabi.StokAdedi,-5}");
            }
        }
    }

    // --- 2. YENİ AYAKKABI EKLEME METODU ---
    static void YeniAyakkabiEkle()
    {
        Console.WriteLine("\n--- Yeni Ayakkabı Kaydı ---");
        Ayakkabi yeniAyakkabi = new Ayakkabi();

        // Ad Girişi
        Console.Write("Ayakkabı Adı: ");
        yeniAyakkabi.Ad = Console.ReadLine();

        // Numara Girişi (TryParse Kontrolü)
        Console.Write("Numara: ");
        int numara;
        while (!int.TryParse(Console.ReadLine(), out numara) || numara <= 0)
        {
            Console.Write("Lütfen geçerli bir numara girin: ");
        }
        yeniAyakkabi.Numara = numara;

        // Renk Girişi (Boş geçilemez kontrolü)
        Console.Write("Renk: ");
        string girilenRenk = Console.ReadLine();
        while (string.IsNullOrWhiteSpace(girilenRenk))
        {
            Console.WriteLine("Renk boş bırakılamaz, tekrar deneyin.");
            Console.Write("Renk: ");
            girilenRenk = Console.ReadLine();
        }
        yeniAyakkabi.Renk = girilenRenk;

        // Fiyat Girişi (Güvenli While Döngüsü)
        decimal fiyat;
        Console.Write("Fiyat: ");
        while (!decimal.TryParse(Console.ReadLine(), out fiyat) || fiyat <= 0)
        {
            Console.WriteLine("Hatalı giriş! Lütfen geçerli bir fiyat yazın (Örn: 500).");
            Console.Write("Fiyat: ");
        }
        yeniAyakkabi.Fiyat = fiyat;

        // Stok Girişi (Güvenli While Döngüsü)
        int stok;
        Console.Write("Stok Adedi: ");
        while (!int.TryParse(Console.ReadLine(), out stok) || stok < 0)
        {
            Console.WriteLine("Hatalı giriş! Lütfen rakamlarla bir tam sayı girin.");
            Console.Write("Stok Adedi: ");
        }
        yeniAyakkabi.StokAdedi = stok;

        // Listeye Ekleme
        envanter.Add(yeniAyakkabi);
        // Verileri kaydet
        VerileriKaydet();
        Console.WriteLine("Yeni ayakkabı başarıyla eklendi.");
    }

    // --- 3. STOK GÜNCELLEME METODU ---
    static void StokGuncelle()
    {
        Console.WriteLine("\n--- Stok Güncelleme ---");
        
        Console.Write("Güncellenecek ayakkabı adını girin: ");
        string ad = Console.ReadLine().ToLower();
        
        Console.Write("Numarasını girin: ");
        int numara;
        while (!int.TryParse(Console.ReadLine(), out numara))
        {
            Console.Write("Geçerli bir numara girin: ");
        }

        // Ayakkabıyı bul
        var ayakkabi = envanter.FirstOrDefault(x => x.Ad.Equals(ad, StringComparison.OrdinalIgnoreCase) && x.Numara == numara);

        if (ayakkabi == null)
        {
            Console.WriteLine("HATA!! Ayakkabı bulunamadı.");
            return;
        }
        else
        {
            Console.WriteLine($"\nBulunan Ayakkabı: {ayakkabi.Ad}, Numara: {ayakkabi.Numara}");
            Console.WriteLine($"Mevcut Stok: {ayakkabi.StokAdedi} | Fiyat: {ayakkabi.Fiyat} TL");

            Console.Write("\nEklenecek (+) veya Silinecek (-) miktar girin: ");
            int miktar;
            while (!int.TryParse(Console.ReadLine(), out miktar))
            {
                Console.Write("Lütfen sadece sayı girin: ");
            }

            // Stok yeterli mi kontrolü
            if (ayakkabi.StokAdedi + miktar < 0)
            {
                Console.WriteLine("HATA!! Yetersiz stok. İşlem iptal edildi.");
                return;
            }
            else
            {
                ayakkabi.StokAdedi += miktar;
                // Verileri kaydet
                VerileriKaydet();
                Console.WriteLine("Stok başarıyla güncellendi.");
                if(miktar >= 0)
                    Console.WriteLine($"{miktar} adet stok eklendi.");
                else
                    Console.WriteLine($"{-miktar} adet stok silindi.");
                Console.WriteLine($"\nGüncellenen Ayakkabı: {ayakkabi.Ad}, Numara: {ayakkabi.Numara}");
                Console.WriteLine($"Mevcut Stok: {ayakkabi.StokAdedi} | Fiyat: {ayakkabi.Fiyat} TL");
            }
        }
    }

    // --- 4. TÜM LİSTEYİ GÖSTERME METODU ---
    static void TumListeyiGoster()
    {
        Console.WriteLine("\n--- Tüm Ayakkabı Envanteri ---");
        Console.WriteLine($"{"Ad",-20} | {"Numara",-6} | {"Fiyat",-10} | {"Stok",-5} | {"Renk",-10}");
        Console.WriteLine(new string('-', 60));

        foreach (var ayakkabi in envanter)
        {
            Console.WriteLine($"{ayakkabi.Ad,-20} | {ayakkabi.Numara,-6} | {ayakkabi.Fiyat + " TL",-10} | {ayakkabi.StokAdedi,-5} | {ayakkabi.Renk,-10}");
        }
    }
    // --- VERİLERİ YÜKLEME METODU ---
    static void VerileriYukle()
    {
        if(File.Exists(dosyaYolu))
        {
            string okunanJson = File.ReadAllText(dosyaYolu);
            envanter = JsonSerializer.Deserialize<List<Ayakkabi>>(okunanJson);
        }
    }

    // --- VERİLERİ KAYDETME METODU ---
    static void VerileriKaydet()
    {
        // Türkçe karakterleri bozmadan ve okunaklı kaydetmek için ayar
        var ayarlar = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(envanter, ayarlar);
        File.WriteAllText(dosyaYolu, json);
    }

    // --- AYAKKABI SINIFI (CLASS) ---
    class Ayakkabi
    {
        public int Numara { get; set; }
        public string Renk { get; set; }
        public string Ad { get; set; }
        public decimal Fiyat { get; set; }
        public int StokAdedi { get; set; }
    }
}