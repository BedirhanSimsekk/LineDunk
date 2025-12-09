using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class AdMobManager : MonoBehaviour
{
    public static AdMobManager Instance { get; private set; }

    [Header("Ayarlar")]
    [Tooltip("Kaç defada bir geçiþ reklamý gösterilsin?")]
    public int gecisReklamSikligi = 3;
    private const string REKLAM_SAYAC_KEY = "GecisReklamSayisi";

    // Reklam Deðiþkenleri
    private InterstitialAd _interstitialAd;
    private RewardedAd _rewardedAd;
    private BannerView _bannerView; // Banner deðiþkeni eklendi

    // --- ID TANIMLAMALARI ---
#if UNITY_ANDROID
    private string _interstitialId = "ca-app-pub-3940256099942544/1033173712";
    private string _rewardedId = "ca-app-pub-3940256099942544/5224354917";
    private string _bannerId = "ca-app-pub-3940256099942544/6300978111"; // Android Test ID
#elif UNITY_IPHONE
    private string _interstitialId = "ca-app-pub-3940256099942544/4411468910";
    private string _rewardedId = "ca-app-pub-3940256099942544/1712485313";
    private string _bannerId = "ca-app-pub-3940256099942544/2934735716"; // iOS Test ID
#else
    private string _interstitialId = "unused";
    private string _rewardedId = "unused";
    private string _bannerId = "unused";
#endif

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        MobileAds.Initialize(initStatus =>
        {
            Debug.Log("AdMob SDK Baþlatýldý.");
            LoadInterstitial();
            LoadRewarded();
            RequestBanner(); // Baþlar baþlamaz banner'ý çaðýrýyoruz
        });
    }

    #region BANNER REKLAM (YENÝ EKLENDÝ)

    public void RequestBanner()
    {
        // Eðer daha önce oluþturulmuþ bir banner varsa temizle
        if (_bannerView != null)
        {
            _bannerView.Destroy();
        }

        // 1. Banner boyutunu belirle (Standart Banner: 320x50)
        AdSize adSize = AdSize.Banner;

        // 2. Banner'ý oluþtur (ID, Boyut, Konum: Alt Kýsým)
        _bannerView = new BannerView(_bannerId, adSize, AdPosition.Bottom);

        // 3. Ýsteði oluþtur ve yükle
        AdRequest request = new AdRequest();
        _bannerView.LoadAd(request);

        Debug.Log("Banner reklam isteði gönderildi.");
    }

    // Eðer banner'ý bir sahnede gizlemek istersen bu fonksiyonu kullanabilirsin
    public void DestroyBanner()
    {
        if (_bannerView != null)
        {
            _bannerView.Destroy();
            _bannerView = null;
        }
    }

    #endregion

    #region INTERSTITIAL (GEÇÝÞ REKLAMI)

    public void LoadInterstitial()
    {
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }

        AdRequest request = new AdRequest();

        InterstitialAd.Load(_interstitialId, request, (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("Geçiþ reklamý yüklenemedi: " + error);
                return;
            }
            _interstitialAd = ad;
        });
    }

    public void ShowInterstitial(Action onAdClosed)
    {
        int currentCount = PlayerPrefs.GetInt(REKLAM_SAYAC_KEY, 0);
        currentCount++;

        bool showAd = (currentCount >= gecisReklamSikligi) &&
                      (_interstitialAd != null && _interstitialAd.CanShowAd());

        if (showAd)
        {
            Debug.Log("Reklam gösteriliyor...");

            Action closeHandler = null;
            closeHandler = () =>
            {
                if (_interstitialAd != null) _interstitialAd.OnAdFullScreenContentClosed -= closeHandler;

                PlayerPrefs.SetInt(REKLAM_SAYAC_KEY, 0);
                LoadInterstitial();

                Debug.Log("Reklam kapatýldý.");
                onAdClosed?.Invoke();
            };

            _interstitialAd.OnAdFullScreenContentClosed += closeHandler;
            _interstitialAd.Show();
        }
        else
        {
            if (currentCount < gecisReklamSikligi)
            {
                PlayerPrefs.SetInt(REKLAM_SAYAC_KEY, currentCount);
            }
            else
            {
                LoadInterstitial();
            }
            onAdClosed?.Invoke();
        }
    }

    #endregion

    #region REWARDED (ÖDÜLLÜ REKLAM)

    public void LoadRewarded()
    {
        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }

        AdRequest request = new AdRequest();

        RewardedAd.Load(_rewardedId, request, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("Ödüllü reklam yüklenemedi: " + error);
                return;
            }

            _rewardedAd = ad;
            _rewardedAd.OnAdFullScreenContentClosed += () =>
            {
                LoadRewarded();
            };
        });
    }

    public void ShowRewarded(Action<Reward> onRewardEarned)
    {
        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            _rewardedAd.Show((Reward reward) =>
            {
                onRewardEarned?.Invoke(reward);
            });
        }
        else
        {
            LoadRewarded();
        }
    }

    #endregion
}