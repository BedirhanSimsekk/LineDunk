using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CizgiCiz : MonoBehaviour
{
    public GameObject LinePrefab;
    public GameObject Cizgi;

    public LineRenderer lineRenderer;
    public EdgeCollider2D EdgeCollider;
    public List<Vector2> ParmakPozisyonListesi;

    public List<GameObject> Cizgiler;
    bool CizmekMumkunmu;
    int CizmeHakki;
    [SerializeField] private TextMeshProUGUI CizmeHakkiText;

    private void Start()
    {
        // --- DÜZELTME BAÞLANGICI ---
        // Listeleri kullanmadan önce mutlaka "new" ile oluþturmalýsýn.
        ParmakPozisyonListesi = new List<Vector2>();
        Cizgiler = new List<GameObject>();
        // --- DÜZELTME BÝTÝÞÝ ---

        CizmekMumkunmu = false;
        CizmeHakki = 3;
        // Text atamasý yapýlmamýþsa hata vermesin diye kontrol ekledim (opsiyonel güvenlik)
        if(CizmeHakkiText != null) 
            CizmeHakkiText.text = CizmeHakki.ToString();
    }

    void Update()
    {
        if (CizmekMumkunmu && CizmeHakki != 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                CizgiOlustur();
            }
            if (Input.GetMouseButton(0))
            {
                // Çizgi yoksa iþlem yapma (Hata önleyici)
                if (Cizgi == null) return;

                Vector2 ParmakPozisyonu = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                // Liste boþsa hata vermemesi için kontrol
                if (ParmakPozisyonListesi.Count > 0 && Vector2.Distance(ParmakPozisyonu, ParmakPozisyonListesi[^1]) > .1f)
                {
                    CizgiyiGuncelle(ParmakPozisyonu);
                }
            }
        }

        if (Cizgiler.Count != 0 && CizmeHakki != 0)
        {
            if (Input.GetMouseButtonUp(0))
            {
                CizmeHakki--;
                if(CizmeHakkiText != null)
                    CizmeHakkiText.text = CizmeHakki.ToString();
            }
        }
    }

    void CizgiOlustur()
    {
        Cizgi = Instantiate(LinePrefab, Vector2.zero, Quaternion.identity);
        Cizgiler.Add(Cizgi);
        lineRenderer = Cizgi.GetComponent<LineRenderer>();
        EdgeCollider = Cizgi.GetComponent<EdgeCollider2D>();
        
        ParmakPozisyonListesi.Clear();
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        // Listenin boþ kalmamasý için ilk noktalarý ekliyoruz
        ParmakPozisyonListesi.Add(mousePos);
        ParmakPozisyonListesi.Add(mousePos);
        
        lineRenderer.SetPosition(0, ParmakPozisyonListesi[0]);
        lineRenderer.SetPosition(1, ParmakPozisyonListesi[1]);
        EdgeCollider.points = ParmakPozisyonListesi.ToArray();
    }

    void CizgiyiGuncelle(Vector2 GelenParmakPozisyonu)
    {
        ParmakPozisyonListesi.Add(GelenParmakPozisyonu);
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, GelenParmakPozisyonu);
        EdgeCollider.points = ParmakPozisyonListesi.ToArray();
    }

    public void DevamEt()
    {
        // Not: TopAtar scriptinin proðanda var olduðu varsayýlmýþtýr.
        if (TopAtar.AtilanTopSayisi == 0)
        {
            foreach (var item in Cizgiler)
            {
                Destroy(item.gameObject);
            }
            Cizgiler.Clear();
            CizmeHakki = 3;
            if(CizmeHakkiText != null)
                CizmeHakkiText.text = CizmeHakki.ToString();
        }
    }

    public void CizmeyiDurdur()
    {
        CizmekMumkunmu = false;
    }

    public void CizmeyiBaslat()
    {
        CizmeHakki = 3;
        CizmekMumkunmu = true;
    }
}