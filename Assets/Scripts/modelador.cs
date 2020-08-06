using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class modelador : MonoBehaviour
{
    Camera camera;
    // Update is called once per frame
    void Start()
    {
        camera = GetComponent<Camera>();
    }
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            print("clicou");
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if(hit.transform.tag == "terreno")
                    hit.transform.GetComponent<chunk>().AdicionarTerreno(hit.point);
                else                
                    print("não deu certo");
            }

        }
    }
}
