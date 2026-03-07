using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    [Header("----MOD AYARI")]
    [SerializeField] private bool isAkisModu = true;

    [Header("----TOP VE TEKNIK OBJELER")]
    [SerializeField] private TopAtar _TopAtar;
    [SerializeField] private CizgiCiz _CizgiCiz;
    [SerializeField] private PotaSpawner _PotaSpawner;

    [Header("----GENEL OBJELER")]
    [SerializeField] private ParticleSystem KovaGirme;
    [SerializeField] private ParticleSystem BestScoreGecis;
    [SerializeField] private AudioSource[] Sesler;

    [Header("----UI OBJELER")]
    [SerializeField] private GameObject[] Paneller;
    [SerializeField] private TextMeshProUGUI[] ScoreTextleri;
    // ScoreTextleri[0] -> Ana Menüdeki Rekor
    // ScoreTextleri[1] -> Oyun Sonu Panelindeki Rekor
    // ScoreTextleri[2] -> Oyun İçindeki Anlık Skor

    [Header("----UI SES BUTONLARI")]
    [SerializeField] private Image[] MuzikButonResimleri;
    [SerializeField] private Image[] SFXButonResimleri;
    [SerializeField] private Sprite SesAcikSprite;
    [SerializeField] private Sprite SesKapaliSprite;

    [Header("----CAN SISTEMI")]
    public int kalanCan = 3;
    [SerializeField] private GameObject[] CanIkonlari;

    [Header("----SES KAYNAKLARI")]
    [SerializeField] private AudioSource GameSound;
    [SerializeField] private AudioSource[] SFXSound;

    int GirenTopSayisi;
    private string scoreKey;
    private bool isMusicMuted = false;
    private bool isSFXMuted = false;
    public bool oyunBasladiMi = false;

    void Start()
    {
        GirenTopSayisi = 0;
        oyunBasladiMi = false;

        // Modu belirle
        scoreKey = isAkisModu ? "BestScore_Akis" : "BestScore_Klasik";


        // --- DÜZELTME 1: Başlangıçta anlık skoru sıfırla ---
        if (ScoreTextleri.Length > 2)
            ScoreTextleri[2].text = "0";

        // Rekoru yükle ve yaz
        int currentBest = PlayerPrefs.GetInt(scoreKey, 0);
        if (ScoreTextleri.Length > 0) ScoreTextleri[0].text = currentBest.ToString();
        if (ScoreTextleri.Length > 1) ScoreTextleri[1].text = currentBest.ToString();

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
                if (EventSystem.current.IsPointerOverGameObject()) return;
                if (Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) return;

                if (Paneller.Length > 3 && Paneller[3].activeSelf || (Paneller.Length > 1 && Paneller[1].activeSelf)) return;

                OyunBaslasin();
            }
        }
    }

    public void DevamEt(Vector2 Pos)
    {
        if (KovaGirme != null)
        {
            KovaGirme.transform.position = Pos;
            KovaGirme.gameObject.SetActive(true);
            KovaGirme.Play();
        }

        GirenTopSayisi++;

        // --- DÜZELTME 2: Anlık skor textini güncelle ---
        if (ScoreTextleri.Length > 2)
            ScoreTextleri[2].text = GirenTopSayisi.ToString();

        if (!isSFXMuted && Sesler != null && Sesler.Length > 0) Sesler[0].Play();

        if (_TopAtar != null) _TopAtar.DevamEt();
        if (_CizgiCiz != null) _CizgiCiz.DevamEt();
    }

    public void CanAzalt()
    {
        kalanCan--;
        if (kalanCan >= 0 && kalanCan < CanIkonlari.Length)
        {
            CanIkonlari[kalanCan].SetActive(false);
        }

        if (kalanCan <= 0)
        {
            OyunBitti();
        }
    }

    public void OyunBitti()
    {
        CizgiCiz.oyunDevamEdiyor = false;
        oyunBasladiMi = false;

        if (_PotaSpawner != null) _PotaSpawner.SpawneriDurdur();

        if (!isSFXMuted && Sesler != null && Sesler.Length > 1) Sesler[1].Play();

        if (Paneller.Length > 1) Paneller[1].SetActive(true);
        if (Paneller.Length > 2) Paneller[2].SetActive(false);

        // --- DÜZELTME 3: Rekor Kontrolü ve Kaydı ---
        int currentBest = PlayerPrefs.GetInt(scoreKey, 0);
        if (GirenTopSayisi > currentBest)
        {
            PlayerPrefs.SetInt(scoreKey, GirenTopSayisi);
            currentBest = GirenTopSayisi; // Yeni rekoru değişkene eşitle
            PlayerPrefs.Save(); // Kaydı garantiye al

            if (BestScoreGecis != null)
            {
                BestScoreGecis.gameObject.SetActive(true);
                BestScoreGecis.Play();
            }
        }

        // Oyun sonu panelindeki rekor textini güncelle
        if (ScoreTextleri.Length > 1)
            ScoreTextleri[1].text = currentBest.ToString();

        if (_TopAtar != null) _TopAtar.TopAtmaDurdur();
        if (_CizgiCiz != null) _CizgiCiz.CizmeyiDurdur();
    }

    public void OyunBaslasin()
    {
        if (Paneller.Length > 3 && Paneller[3].activeSelf) return;

        oyunBasladiMi = true;

        if (Paneller.Length > 0) Paneller[0].SetActive(false);

        if (_PotaSpawner != null) _PotaSpawner.SpawneriBaslat();

        if (_TopAtar != null) _TopAtar.OyunBaslasin();
        if (_CizgiCiz != null) _CizgiCiz.CizmeyiBaslat();
        if (Paneller.Length > 2) Paneller[2].SetActive(true);
    }

    public void TekrarOyna()
    {
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        
    }

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
        foreach (var img in MuzikButonResimleri) { if (img != null) img.sprite = ikon; }
    }

    public void SFXAcKapat()
    {
        isSFXMuted = !isSFXMuted;
        PlayerPrefs.SetInt("SFXMute", isSFXMuted ? 1 : 0);
        SFXDurumunuGuncelle();
    }

    private void SFXDurumunuGuncelle()
    {
        foreach (var audioSrc in SFXSound) { if (audioSrc != null) audioSrc.mute = isSFXMuted; }
        foreach (var audioSrc in Sesler) { if (audioSrc != null) audioSrc.mute = isSFXMuted; }
        Sprite ikon = isSFXMuted ? SesKapaliSprite : SesAcikSprite;
        foreach (var img in SFXButonResimleri) { if (img != null) img.sprite = ikon; }
    }

    public void Ayarlar(int Olay)
    {
        if (Olay == 1 && Paneller.Length > 3) Paneller[3].SetActive(true);
        else if (Paneller.Length > 3) Paneller[3].SetActive(false);

        if (Olay == 2) SceneManager.LoadScene(0);
    }
}