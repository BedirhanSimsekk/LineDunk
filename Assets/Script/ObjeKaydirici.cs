using UnityEngine;

public class ObjeKaydirici : MonoBehaviour
{
    public float kaymaHizi = 6f;
    private GameManager _gameManager;
    private bool puanAlindi = false;

    void Start()
    {
        _gameManager = Object.FindObjectOfType<GameManager>();
    }

    void Update()
    {
        // Obje sürekli yukarý akar
        transform.Translate(Vector3.up * kaymaHizi * Time.deltaTime);
    }

    // Bu fonksiyon TopAkis tarafýndan potaya girildiðinde çaðrýlacak
    public void PuanAlindi()
    {
        puanAlindi = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Pota ekranýn üstündeki Finish hattýna çarptýðýnda
        if (collision.CompareTag("Finish"))
        {
            // Eðer bu potadan puan ALINMADIYSA can azalt
            if (!puanAlindi)
            {
                if (_gameManager != null) _gameManager.CanAzalt();
            }

            // Her durumda objeyi temizle
            Destroy(gameObject);
        }
    }
}