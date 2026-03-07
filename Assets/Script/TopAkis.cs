using UnityEngine;

public class TopAkis : MonoBehaviour
{
    [SerializeField] private GameManager _GameManager;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("TopGirdi"))
        {
            _GameManager.DevamEt(transform.position);

            // Potanýn can azaltmasýný engellemek için haber ver
            // (Script kovanýn ana objesinde olduðu için parent'a bakýyoruz)
            ObjeKaydirici kaydirici = collision.GetComponentInParent<ObjeKaydirici>();
            if (kaydirici != null)
            {
                kaydirici.PuanAlindi();
            }

            // Potayý sahnede biraz daha tutup yok et (Görsellik için)
            // collision.gameObject tetikleyiciyi, parent kovanýn tamamýný siler
            Destroy(collision.transform.parent.gameObject, 0.8f);
        }
        else if (collision.gameObject.CompareTag("OyunBitti"))
        {
            _GameManager.OyunBitti();
        }
    }
}