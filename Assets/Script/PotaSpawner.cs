using UnityEngine;

public class PotaSpawner : MonoBehaviour
{
    public GameObject potaPrefab;
    public float olusmaSuresi = 1.5f;
    private bool spawnerCalisiyor = false;

    public void SpawneriBaslat()
    {
        if (!spawnerCalisiyor)
        {
            InvokeRepeating("PotaUret", 0.5f, olusmaSuresi);
            spawnerCalisiyor = true;
        }
    }

    public void SpawneriDurdur()
    {
        CancelInvoke("PotaUret");
        spawnerCalisiyor = false;
    }

    void PotaUret()
    {
        float randomX = Random.Range(-2.2f, 2.2f);
        // Potalar ekranýn en altýndan doðar
        Instantiate(potaPrefab, new Vector3(randomX, -8f, 0), Quaternion.identity);
    }
}