using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class explosion : MonoBehaviour
{
    public float areaExplosao = 3;
    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.tag == "terreno")
        {
            Vector3 pos = other.GetContact(0).point;
            dados.Explodir(other.GetContact(0).point, areaExplosao);
            Collider[] c = Physics.OverlapSphere(pos, areaExplosao + 1);
            foreach(Collider g in c)
            {
                if(g.GetComponent<chunk>())
                    g.GetComponent<chunk>().Recriar();
            }
        }
    }
}
