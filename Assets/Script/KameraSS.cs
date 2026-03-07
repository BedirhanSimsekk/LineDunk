using UnityEngine;

public class KameraSS : MonoBehaviour
{
    // Hangi tuþla çekeceðini buradan seçebilirsin
    public KeyCode cekmeTusu = KeyCode.K;

    // Çözünürlük çarpaný (1 = Ekran boyutu, 2 = 2 kat büyük, 4 = 4 kat büyük)
    // Maðaza görseli için 2 veya 3 yapabilirsin.
    public int cozunurlukCarpani = 1;

    void Update()
    {
        // Belirlediðin tuþa basýldýðýnda çalýþýr
        if (Input.GetKeyDown(cekmeTusu))
        {
            ResimCek();
        }
    }

    void ResimCek()
    {
        // Dosya ismini tarih ve saat ile oluþturuyoruz ki üst üste yazmasýn
        string tarih = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string dosyaIsmi = "OyunSS_" + tarih + ".png";

        // Görüntüyü al ve kaydet
        ScreenCapture.CaptureScreenshot(dosyaIsmi, cozunurlukCarpani);

        Debug.Log("Fotoðraf çekildi: " + dosyaIsmi);
    }
}