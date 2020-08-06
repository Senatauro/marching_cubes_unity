using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class geradorMapa : MonoBehaviour
{
    [Tooltip("Ate que valor que um vertice precisa ter para ser considerado terreno")]
    public float corte = 0.5f;
    [Tooltip("Dimensoes do cenário")]
    public Vector3Int tamanhoCenario;
    public float tamanhoMinimo;
    public float tamanhoExtra;
    public Vector2Int tamanhoMapa;
    public Material mat;
    public float distanciaPontos = 0.5f;
    public bool terrenoLiso;
    public bool flatShader;
    public float noise = 0.9f;
    public float amplitude = 0.1f;
    public Vector2 frequencia;
    [Range(0f, 0.1f)]
    public float varAmplitude = 0.1f;
    public float aleatoriedade = 1;
    public float aleAmplitude = 0.1f;


    public float persistencia = 0.5f;
    public float lacunaridade = 2f;
    public float rotacoes = 3;
    
    void Start()
    {
        SetarInformacoes();
        for(int x = 0; x < tamanhoMapa.x; x++)
        {
            for(int z = 0; z < tamanhoMapa.y; z++)
            {
                GameObject g = new GameObject();
                g.AddComponent<chunk>();
                g.GetComponent<chunk>().Iniciar(new Vector3(x * tamanhoCenario.x - (x * distanciaPontos), 0, z * tamanhoCenario.z - (z * distanciaPontos)));
                dados.chunks[x, z] = g;
                //chunks.Add(g);
                //chunk c = new chunk(new Vector3(x * tamanhoCenario.x - (x * distanciaPontos), 0, z * tamanhoCenario.z - (z * distanciaPontos)));
                //c.chunkObj.AddComponent<chunkRef>();
                //c.chunkObj.GetComponent<chunkRef>().referencia = c;
                //chunks.Add(c.chunkObj.GetComponent<chunkRef>());
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetarInformacoes()
    {
        dados.corte = corte;
        dados.tamanhoCenario = tamanhoCenario;
        dados.mat = mat;
        dados.distanciaPontos = distanciaPontos;
        dados.terrenoLiso = terrenoLiso;
        dados.flatShader = flatShader;
        dados.noise = noise;
        dados.tamanhoMinimoDoCenario = tamanhoMinimo;
        dados.tamanhoExtraDoCenario = tamanhoExtra;
        dados.frequencia = frequencia;
        dados.persistencia = persistencia;
        dados.lacunaridade = lacunaridade;
        dados.rotacoes = rotacoes;
        dados.sizeChunks = tamanhoMapa;
        dados.chunks = new GameObject[tamanhoMapa.x, tamanhoMapa.y];
        dados.SamplingGeralMapa();
    }
}
