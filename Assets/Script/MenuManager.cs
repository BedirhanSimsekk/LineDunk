using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public GameObject infoPanel;
    [Header("Yükleme Ekraný Elemanlarý")]
    public GameObject loadingPanel; // Az önce oluþturduðun Panel
    public Slider loadingSlider;    // Varsa Slider
    public TextMeshProUGUI loadingText; // Varsa yüzde yazýsý


    public void KlasikModuAc()
    {
        StartCoroutine(SahneleriYukle(1)); // Klasik Mod Index: 1
    }

    public void AkisModunaGit()
    {
        StartCoroutine(SahneleriYukle(2)); // Akýþ Modu Index: 2
    }

    IEnumerator SahneleriYukle(int sceneIndex)
    {
        // 1. Yükleme panelini aç
        loadingPanel.SetActive(true);

        // 2. Sahneyi arka planda yüklemeye baþla
        AsyncOperation operasyon = SceneManager.LoadSceneAsync(sceneIndex);

        // 3. Sahne tamamen yüklenene kadar döngüde kal
        while (!operasyon.isDone)
        {
            // progress deðeri 0 ile 0.9 arasýndadýr. 0.9 olduðunda sahne yüklenmiþ demektir.
            float ilerleme = Mathf.Clamp01(operasyon.progress / 0.9f);

            // Slider'ý güncelle
            if (loadingSlider != null)
                loadingSlider.value = ilerleme;

            // Yüzde yazýsýný güncelle
            if (loadingText != null)
                loadingText.text = "%" + (ilerleme * 100f).ToString("F0");

            yield return null;
        }
    }

    // Diðer fonksiyonlarýn ayný kalsýn...
    public void InfoPanelAc() { infoPanel.SetActive(true); }
    public void InfoPanelKapat() { infoPanel.SetActive(false); }
    public void OyunuKapat() { Application.Quit(); }
}