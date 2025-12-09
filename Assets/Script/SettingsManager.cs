using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("UI Elemanlarý")]
    public Toggle fpsToggle; // Inspector'dan Toggle'ý buraya sürüklemeyi unutma!

    // OnEnable: Obje her aktifleþtiðinde (Sahne yüklenince, obje açýlýnca) çalýþýr.
    // Start veya Awake'ten daha garantidir UI güncellemeleri için.
    void OnEnable()
    {
        // 1. Önce VSync'i kapat (Mobil için þart)
        QualitySettings.vSyncCount = 0;

        // 2. Kayýtlý tercihi oku (Varsayýlan 0: Kapalý)
        int isHighFpsActive = PlayerPrefs.GetInt("HighFPS", 0);

        // 3. Toggle UI'ýný güncelle
        // ÖNEMLÝ: Toggle'ýn 'OnValueChanged' eventi tetiklenmesin diye önce listener'ý durdurabiliriz
        // ama þimdilik direkt eþitleyelim, event çalýþsa da sorun olmaz, fonksiyon ayný iþi yapýyor.

        if (fpsToggle != null)
        {
            if (isHighFpsActive == 1)
            {
                fpsToggle.isOn = true;
                SetMaxFPS(); // Ayarý da uygula
            }
            else
            {
                fpsToggle.isOn = false;
                Set60FPS(); // Ayarý da uygula
            }
        }
        else
        {
            // Eðer Toggle baðlý deðilse bile arka planda FPS ayarýný yapalým
            if (isHighFpsActive == 1) SetMaxFPS();
            else Set60FPS();
        }
    }

    public void OnFPSToggleChanged(bool isChecked)
    {
        if (isChecked)
        {
            PlayerPrefs.SetInt("HighFPS", 1);
            SetMaxFPS();
        }
        else
        {
            PlayerPrefs.SetInt("HighFPS", 0);
            Set60FPS();
        }
        PlayerPrefs.Save();
    }

    void Set60FPS()
    {
        Application.targetFrameRate = 60;
    }

    void SetMaxFPS()
    {
        // Unity sürümüne göre uygun olaný seç:

        // YENÝ UNITY SÜRÜMLERÝ (2022.2+)
        int refreshRate = (int)Screen.currentResolution.refreshRateRatio.value;

        // ESKÝ UNITY SÜRÜMLERÝ (Eðer üstteki hata verirse bunu aç)
        // int refreshRate = Screen.currentResolution.refreshRate;

        if (refreshRate < 60) refreshRate = 60;
        Application.targetFrameRate = refreshRate;
    }
}