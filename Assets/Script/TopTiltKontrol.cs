using UnityEngine;

public class TopTiltKontrol : MonoBehaviour
{
    public float hareketHizi = 15f;
    private Rigidbody2D rb;
    private GameManager _gameManager;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Sahnedeki GameManager'ý buluyoruz
        _gameManager = Object.FindObjectOfType<GameManager>();
    }

    void Update()
    {
        // EÐER GameManager bulunamadýysa veya oyun baþlamadýysa hareket etme!
        if (_gameManager == null || !_gameManager.oyunBasladiMi)
        {
            rb.velocity = Vector2.zero; // Hýzý sýfýrla
            return;
        }

        // --- WEBGL ÝÇÝN DEÐÝÞTÝRÝLEN KISIM ---
        // Input.acceleration.x yerine Input.GetAxis("Horizontal") kullanýyoruz.
        // Bu kod otomatik olarak A ve D tuþlarýna (ayný zamanda sað-sol yön tuþlarýna) duyarlýdýr.
        float xEkseni = Input.GetAxis("Horizontal");

        rb.velocity = new Vector2(xEkseni * hareketHizi, 0);

        // Ekran sýnýrlarý (Kameraya göre kýsýtla)
        float sinir = 2.5f;
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -sinir, sinir), transform.position.y, 0);
    }
}