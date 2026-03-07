using UnityEngine;

public class AutoCameraScaler : MonoBehaviour
{
    // Unity Editörde oyunu tasarlarken kullandýðýn çözünürlük (Game ekranýndaki ayarýn)
    // Genelde 1080 x 1920 (Portrait) standarttýr.
    public Vector2 referenceResolution = new Vector2(1080, 1920);

    // Kameranýn tasarým anýndaki orijinal boyutu
    private float targetSize;

    void Start()
    {
        // Baþlangýçtaki kamera boyutunu kaydet (Editörde ayarladýðýn size)
        targetSize = Camera.main.orthographicSize;

        AdjustCamera();
    }

    void AdjustCamera()
    {
        // 1. Tasarladýðýn ekranýn oraný (Örn: 9/16 = 0.5625)
        float targetAspect = referenceResolution.x / referenceResolution.y;

        // 2. Þu anki telefonun ekran oraný
        float windowAspect = (float)Screen.width / (float)Screen.height;

        // 3. Oran Karþýlaþtýrmasý
        if (windowAspect < targetAspect)
        {
            // DURUM A: Telefon senin tasarýmýndan daha ÝNCE (Uzun Telefonlar)
            // Geniþlik sýðmýyor demektir. Geniþliði sýðdýrmak için kamerayý büyütmeliyiz.

            float scaleHeight = targetAspect / windowAspect;
            Camera.main.orthographicSize = targetSize * scaleHeight;
        }
        else
        {
            // DURUM B: Telefon senin tasarýmýndan daha GENÝÞ (Tabletler)
            // Geniþlik sorunu yok. Orijinal yüksekliði (Size) koru.
            // Yanlardan daha fazla alan görünecek ama oyun bozulmayacak.
            Camera.main.orthographicSize = targetSize;
        }
    }
}