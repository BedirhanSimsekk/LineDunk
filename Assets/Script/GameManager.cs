using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    [Header("----TOP VE TEKNİK OBJELER")]
    [SerializeField] private TopAtar _TopAtar;
    [SerializeField] private CizgiCiz _CizgiCiz;

    [Header("----GENEL OBJELER")]
    [SerializeField] private ParticleSystem KovaGirme;
    [SerializeField] private ParticleSystem BestScoreGecis;
    [SerializeField] private AudioSource[] Sesler;

    [Header("----UI OBJELER")]
    [SerializeField] private GameObject[] Paneller;
    [SerializeField] private TextMeshProUGUI[] ScoreTextleri;

    [Header("----SES KAYNAKLARI")]
    [SerializeField] private AudioSource GameSound;
    [SerializeField] private AudioSource[] SFXSound;

    [Header("----UI SES BUTONLARI")]
    [SerializeField] private Image[] MuzikButonResimleri;
    [SerializeField] private Image[] SFXButonResimleri;

    [SerializeField] private Sprite SesAcikSprite;
    [SerializeField] private Sprite SesKapaliSprite;

    int GirenTopSayisi;
    private bool isMusicMuted = false;
    private bool isSFXMuted = false;

    // Oyunun başlayıp başlamadığını kontrol etmek için
    private bool oyunBasladiMi = false;

    void Start()
    {
        GirenTopSayisi = 0;
        oyunBasladiMi = false;

        // -----------------------------------------------------------------
        // YENİ EKLENEN KISIM: Her oyun başladığında Banner'ı zorla çağır
        // -----------------------------------------------------------------
        if (AdMobManager.Instance != null)
        {
            AdMobManager.Instance.RequestBanner();
        }
        // -----------------------------------------------------------------

        // --- BEST SCORE ---
        if (PlayerPrefs.HasKey("BestScore"))
        {
            ScoreTextleri[0].text = PlayerPrefs.GetInt("BestScore").ToString();
            ScoreTextleri[1].text = PlayerPrefs.GetInt("BestScore").ToString();
        }
        else
        {
            PlayerPrefs.SetInt("BestScore", 0);
            ScoreTextleri[0].text = "0";
            ScoreTextleri[1].text = "0";
        }

        // --- SES AYARLARINI YÜKLEME ---
        isMusicMuted = PlayerPrefs.GetInt("MusicMute", 0) == 1;
        isSFXMuted = PlayerPrefs.GetInt("SFXMute", 0) == 1;

        MuzikDurumunuGuncelle();
        SFXDurumunuGuncelle();
    }

    void Update()
    {
        if (!oyunBasladiMi)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // UI Tıklama Kontrolleri
                if (EventSystem.current.IsPointerOverGameObject()) return;
                if (Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) return;
                if (Paneller[3].activeSelf) return;

                OyunBaslasin();
            }
        }
    }

    public void DevamEt(Vector2 Pos)
    {
        KovaGirme.transform.position = Pos;
        KovaGirme.gameObject.SetActive(true);
        KovaGirme.Play();

        GirenTopSayisi++;
        if (!isSFXMuted) Sesler[0].Play();

        _TopAtar.DevamEt();
        _CizgiCiz.DevamEt();
    }

    public void OyunBitti()
    {
        oyunBasladiMi = false;

        if (!isSFXMuted) Sesler[1].Play();

        Paneller[1].SetActive(true);
        Paneller[2].SetActive(false);

        ScoreTextleri[1].text = PlayerPrefs.GetInt("BestScore").ToString();
        ScoreTextleri[2].text = GirenTopSayisi.ToString();

        if (GirenTopSayisi > PlayerPrefs.GetInt("BestScore"))
        {
            PlayerPrefs.SetInt("BestScore", GirenTopSayisi);
            BestScoreGecis.gameObject.SetActive(true);
            BestScoreGecis.Play();
        }
        _TopAtar.TopAtmaDurdur();
        _CizgiCiz.CizmeyiDurdur();
    }

    public void OyunBaslasin()
    {
        if (Paneller[3].activeSelf) return;

        oyunBasladiMi = true;

        Paneller[0].SetActive(false);
        _TopAtar.OyunBaslasin();
        _CizgiCiz.CizmeyiBaslat();
        Paneller[2].SetActive(true);
    }

    // ------------------------------------------------------------
    // BURAYI GÜNCELLEDİM: AdMobManager KULLANIMI
    // ------------------------------------------------------------
    public void TekrarOyna()
    {
        // AdMobManager varsa, sahne yükleme kodunu "Action" olarak içine atıyoruz
        if (AdMobManager.Instance != null)
        {
            AdMobManager.Instance.ShowInterstitial(() =>
            {
                // BU KISIM SADECE REKLAM İŞİ BİTİNCE ÇALIŞIR
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            });
        }
        else
        {
            // Yönetici yoksa direkt yükle (Hata almamak için)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    // ------------------------------------------------------------

    public void MuzikAcKapat()
    {
        isMusicMuted = !isMusicMuted;
        PlayerPrefs.SetInt("MusicMute", isMusicMuted ? 1 : 0);
        MuzikDurumunuGuncelle();
    }

    private void MuzikDurumunuGuncelle()
    {
        if (GameSound != null) GameSound.mute = isMusicMuted;

        Sprite ikon = isMusicMuted ? SesKapaliSprite : SesAcikSprite;
        foreach (var img in MuzikButonResimleri)
        {
            if (img != null) img.sprite = ikon;
        }
    }

    public void SFXAcKapat()
    {
        isSFXMuted = !isSFXMuted;
        PlayerPrefs.SetInt("SFXMute", isSFXMuted ? 1 : 0);
        SFXDurumunuGuncelle();
    }

    private void SFXDurumunuGuncelle()
    {
        foreach (var audioSrc in SFXSound)
        {
            if (audioSrc != null) audioSrc.mute = isSFXMuted;
        }

        foreach (var audioSrc in Sesler)
        {
            if (audioSrc != null) audioSrc.mute = isSFXMuted;
        }

        Sprite ikon = isSFXMuted ? SesKapaliSprite : SesAcikSprite;
        foreach (var img in SFXButonResimleri)
        {
            if (img != null) img.sprite = ikon;
        }
    }

    public void Ayarlar(int Olay)
    {
        if (Olay == 1)
            Paneller[3].SetActive(true);
        else
            Paneller[3].SetActive(false);
    }
}