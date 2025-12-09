using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopAtar : MonoBehaviour
{
    [SerializeField] private GameObject[] Toplar;
    [SerializeField] private GameObject TopAtarMerkezi;
    [SerializeField] private GameObject Kova;
    [SerializeField] private GameObject[] KovaNoktalari;
    int AktifTopIndex;
    int RandomKovaPointIndex;
    bool Kilit;

    public static int AtilanTopSayisi;
    public static int TopAtisSayisi;

    private void Start()
    {
        TopAtisSayisi = 0;
        AtilanTopSayisi = 0;
    }
    public void OyunBaslasin()
    {
        StartCoroutine(TopAtisSistemi());
    }
    IEnumerator TopAtisSistemi()
    {
        while (true)
        {

            if (!Kilit)
            {
                yield return new WaitForSeconds(.5f);                 

                if (TopAtisSayisi != 0 && TopAtisSayisi % 5 == 0)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        TopAtisveAyarlama();
                    }
                    AtilanTopSayisi = 2;
                    TopAtisSayisi++;
                }
                else
                {
                    TopAtisveAyarlama();
                    AtilanTopSayisi = 1;
                    TopAtisSayisi++;
                }               

                yield return new WaitForSeconds(0.7f);
                RandomKovaPointIndex = Random.Range(0, KovaNoktalari.Length - 1);
                Kova.transform.position = KovaNoktalari[RandomKovaPointIndex].transform.position;
                Kova.SetActive(true);
                Kilit = true;
                Invoke("TopuKontrolEt",5f);
            }
            else
            {
                yield return null;
            }
        }
    }
    public void DevamEt()
    {
        if (AtilanTopSayisi==1)
        {
            Kilit = false;
            Kova.SetActive(false);
            CancelInvoke();
            AtilanTopSayisi--;
        }
        else
        {
            AtilanTopSayisi--;
        }       
    }
     float AciVer(float deger1,float deger2)
    {
       return Random.Range(deger1, deger2);
    }
    Vector3 PozisyonVer(float GelenAci)
    {
       return Quaternion.AngleAxis(GelenAci, Vector3.forward) * Vector3.right;
    }
    public void TopAtmaDurdur ()
    {
        StopAllCoroutines();
    }
    void TopuKontrolEt()
    {
        if (Kilit)
            GetComponent<GameManager>().OyunBitti();
    }
    void TopAtisveAyarlama()
    {

        Toplar[AktifTopIndex].transform.position = TopAtarMerkezi.transform.position;
        Toplar[AktifTopIndex].SetActive(true);
        Toplar[AktifTopIndex].GetComponent<Rigidbody2D>().AddForce(750 * PozisyonVer(AciVer(70f, 110f)));

        if (AktifTopIndex != Toplar.Length - 1)
            AktifTopIndex++;
        else
            AktifTopIndex = 0;

    }
}
