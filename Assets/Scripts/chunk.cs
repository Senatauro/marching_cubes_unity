using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chunk : MonoBehaviour
{  
    public float distanciaPontos { get { return dados.distanciaPontos;} }
    int tamX, tamY, tamZ;

    float ultimoTerreno = -1f;  //variavel para permitir mudar o nivel do terreno e tempo real
    public bool terrenoLiso { get { return dados.terrenoLiso; }}
    public bool flatShader { get { return dados.flatShader; }}
    public Material mat { get{ return dados.mat; }}

    //Lista de todos os vertices do terreno
    List<Vector3> vertices = new List<Vector3>();
    //Lista de todos os triangulos do terreno
    List<int> triangulos = new List<int>();
    float[,,] mapaTerreno;
    MeshFilter filter;
    MeshRenderer renderer;
    MeshCollider collider;
    public GameObject chunkObj;
    public Vector3 chunkPos;
    
    Vector3Int tamanhoCenario { get { return dados.tamanhoCenario; }}
    float terreno { get { return dados.corte; } }

    public void Iniciar(Vector3 pos)
    {
        //chunkObj = new GameObject();
        chunkPos = pos;
        this.transform.position = pos;
        filter = this.gameObject.AddComponent<MeshFilter>();
        renderer = this.gameObject.AddComponent<MeshRenderer>();
        collider = this.gameObject.AddComponent<MeshCollider>();
        renderer.material = mat;
        this.tag = "terreno";
        SamplingTerreno();
        CriarCubos();
        ConstruirMesh();
    }

    public bool analisarTerreno = false;
    public bool definirIndices = false;
    public bool criarTerreno = false;
    public bool limparTerreno = false;

    void Update()
    {
        /* 1° - Pega pontos do terreno */
        if(analisarTerreno)
        {
            analisarTerreno = false;
            SamplingTerreno();
        }

        /* 2° - Criar um cubo imaginario baseado na posição atual sendo analisada 
        e  3° - Define os vertices e indices de triangulos utilizando a técnica marching cubes*/
        if(definirIndices)
        {
            definirIndices = false;
            CriarCubos();
        }
        
        /* 4° - Constroi o mesh */
        if(criarTerreno)
        {
            criarTerreno = false;
            ConstruirMesh();
        }

        /* Extra - Limpa o terreno caso necessário*/
        if(limparTerreno)
        {
            limparTerreno = false;
            LimparDadosMesh();
        }

        if(ultimoTerreno != terreno)
        {
            ultimoTerreno = terreno;
            //LimparDadosMesh();
            //SamplingTerreno();
            //CriarCubos();
            //ConstruirMesh();
        }
        /*
        if(criarTerreno)
        {
            criarTerreno = false;
            mapaTerreno = new float[tamanho.x + 1, tamanho.y + 1, tamanho.z + 1];
            LimparDadosMesh();
            PopularTerreno();
            CriarDadosMesh();
            renderer.material = mat;
        }
        */
    }

    public void Recriar()
    {
        print("Recriou");
        LimparDadosMesh();
        SamplingTerreno();
        CriarCubos();
        ConstruirMesh();
    }

    public float min = 100, max = -100;
    public float noise = 0.9f;
    public float amplitude = 0.1f;
    [Range(0f, 0.1f)]
    public float varAmplitude = 0.1f;
    public float aleatoriedade = 1;
    public float aleAmplitude = 0.1f;
    public float resAleatoriedade = 0;
    public float extraX = 0;
    public float extraY = 0;
    public float extraZ = 0;
    /// <summary>
    /// Função que vai determinar o valor dos pontos no terreno utilizando uma função de perlin noise
    /// </summary>
    void SamplingTerreno()
    {
        tamX = (int)((float)tamanhoCenario.x / distanciaPontos); tamY = (int)((float)tamanhoCenario.y / distanciaPontos); tamZ = (int)((float)tamanhoCenario.z / distanciaPontos);
        mapaTerreno = new float[tamX, tamY, tamZ];
        
        for(int x = 0; x < tamX; x++)
        {
            for(int y = 0; y < tamY; y++)
            {
                for(int z = 0; z < tamZ; z++)
                {
                    //mapaTerreno[x,y,z] = (float) y - dados.PegarAlturaTerreno(x + (chunkPos.x/distanciaPontos), y, z + (chunkPos.z/distanciaPontos));
                    //print("X: " + (int)((chunkPos.x / distanciaPontos) + x));
                    //print("Z: " + (int)((chunkPos.z / distanciaPontos) + z));
                    mapaTerreno[x,y,z] = dados.mapa[(int)(chunkPos.x / distanciaPontos) + x, y, (int)(chunkPos.z / distanciaPontos) + z];
                    //print(mapaTerreno[x,y,z]);
                    //    (new Vector2Int((int)(chunkPos.x / distanciaPontos), (int)(chunkPos.z / distanciaPontos)))];
                }
            }
        }
    }

    /// <summary>
    /// Função que vai pegar todos os pontos analisados e criar cubos imaginarios baseados neles
    /// </summary>
    void CriarCubos()
    {
        LimparDadosMesh();
        tamX = (int)((float)tamanhoCenario.x / distanciaPontos); tamY = (int)((float)tamanhoCenario.y / distanciaPontos); tamZ = (int)((float)tamanhoCenario.z / distanciaPontos);
        // Faz um loop por todos os "cubos" do terreno
        for(int x = 0; x < tamX - 1; x++)  //Todos os valores precisa de -1 pois os vertices da borda não tem como criar cubos
        {
            for(int y = 0; y < tamY - 1; y++)
            {
                for(int z = 0; z < tamZ - 1; z++)
                {
                    //Caso precise mudar a distancia entre pontos de um cubo, mudar aqui
                    MarchCube(new Vector3Int(x, y, z)); // Passa a posição do cubo para analisar e criar
                }
            }
        }
    }

    /// <summary>
    /// Função principal. Essa função que faz a criação do terreno analisando vertices de cubo um a um.
    /// </summary>
    /// <param name="posicao">Posição do cubo atual sendo analisado</param>
    void MarchCube(Vector3Int posicao)
    {
        // Sample terrain values at each corner of the cube.
        float[] cubo = new float[8];
        for (int i = 0; i < 8; i++) 
        {
            cubo[i] = mapaTerreno[posicao.x + dados.CornerTable[i].x, posicao.y + dados.CornerTable[i].y, posicao.z +  + dados.CornerTable[i].z];
        }

        int indiceDeConfiguracao = pegarConfCubo(cubo);

        //significa que o cubo atualmente sendo analisado não tem conexão com nenhum outro, logo não precisa continuar
        if(indiceDeConfiguracao == 0 || indiceDeConfiguracao == 255)
            return;


        int indiceAresta = 0;

        // Menor que 5 porque não tem configuração de cubo com mais que 5 triangulos
        for(int i = 0; i < 5; i++)
        {
            // Menor que 3 porque todo triangulo só tem 3 lados
            for(int j = 0; j < 3; j++)
            {
                // Pega na tabela de configuração de triangulos utilizando o indice de configuração qual aresta deve ser conectada
                int indice = dados.triangleTable[indiceDeConfiguracao, indiceAresta];

                //if(posicao.y == tamanhoCenario.y - 1)
                    //print("Chegou no limite do mapa");
                
                // Se a posição do indice atual for -1 não tem mais vertices na tabela, logo pode sair da função
                if (indice == -1)
                    return;

                // Pega os vertices do inicio e fim dessa aresta
                Vector3 v1 = posicao + dados.CornerTable[dados.EdgeIndexes[indice, 0]];  // Vertice do inicio
                Vector3 v2 = posicao + dados.CornerTable[dados.EdgeIndexes[indice, 1]];  //Vertice do fim
                v1 *= distanciaPontos;
                v2 *= distanciaPontos;
                Vector3 vMeio;

                if(terrenoLiso)
                {
                    // Pega o valor do terreno tanto no vertice que estamos agora quanto no outro vertice da aresta
                    float v1temp = cubo[dados.EdgeIndexes[indice, 0]];
                    float v2temp = cubo[dados.EdgeIndexes[indice, 1]];

                    // Calcula a diferença entre os niveis de terreno
                    float diferenca = v2temp - v1temp;
                    
                    if (diferenca == 0)     // Se a diferença for 0, então o terreno passa pelo meio
                        diferenca = terreno;
                    else                    // Caso contrário faz uma interpolação para definir onde deveria passar
                        diferenca = (terreno - v1temp) / diferenca;

                    // Calcula o ponto na aresta que vai passar
                    vMeio = v1 + ((v2 - v1) * diferenca);
                }
                else
                    vMeio = (v1 + v2)/2f;

                // Add to our vertices and triangles list and incremement the edgeIndex.
                if (flatShader) {

                    vertices.Add(vMeio);
                    triangulos.Add(vertices.Count - 1);

                } else
                    triangulos.Add(VerticeParaIndice(vMeio));

                indiceAresta++;
            }
        }
    }

    /// <summary>
    /// Função que serve para pegar a configuração que o cubo sendo analisado deve ter.
    /// </summary>
    /// <param name="cubo">Cubo sendo analisado</param>
    /// <returns></returns>
    int pegarConfCubo(float[] cubo)
    {
        // Starting with a configuration of zero, loop through each point in the cube and check if it is below the terrain surface.
        int indexConf = 0;
        for (int i = 0; i < 8; i++)
        {
            // If it is, use bit-magic to the set the corresponding bit to 1. So if only the 3rd point in the cube was below
            // the surface, the bit would look like 00100000, which represents the integer value 32.
            if (cubo[i] > terreno)
                indexConf |= 1 << i;
        }
        return indexConf;
    }

    /// <summary>
    /// Função que serve para retornar o indice de um vertice
    /// </summary>
    /// <param name="v">vertice para verificar</param>
    /// <returns>Indice do vertice</returns>
    int VerticeParaIndice (Vector3 v) 
    {
        // Roda por todos os vertices na lista de vertices
        for (int i = 0; i < vertices.Count; i++)
        {
            // Se já existe um vertice no ponto que queremos, retornamos o indice deste
            if (vertices[i] == v)
                return i;
        }

        // Caso não encontre um vertice na lista, adicona e retorna o mesmo
        vertices.Add(v);
        return vertices.Count - 1;

    }

    public void AdicionarTerreno(Vector3 pos)
    {
        pos /= distanciaPontos;
        Vector3Int posInt = new Vector3Int(Mathf.CeilToInt((pos.x - chunkPos.x) * distanciaPontos) , Mathf.CeilToInt(pos.y), Mathf.CeilToInt((pos.z - chunkPos.z) * distanciaPontos));
        mapaTerreno[posInt.x, posInt.y, posInt.z] = 0f;
        CriarCubos();
        ConstruirMesh();
    }

    public void RemoverTerreno(Vector3 pos)
    {

    }

    public void RemoverTerreno(Vector3 pos, float explosao)
    {
        pos /= distanciaPontos;
        //Vector3Int posInt = new Vector3Int(Mathf.CeilToInt(pos.x - chunkPos.x))
        //Vector3Int posInt = new Vector3Int(Mathf.CeilToInt((pos.x - chunkPos.x) * distanciaPontos) , Mathf.CeilToInt(pos.y), Mathf.CeilToInt((pos.z - chunkPos.z) * distanciaPontos));
        //mapaTerreno[posInt.x, posInt.y, posInt.z] = dados.corte + 1;
        CriarCubos();
        ConstruirMesh();
    }

    /// <summary>
    /// Função utilizada para resetar as informações que essa mesh tem
    /// </summary>
    public void LimparDadosMesh()
    {
        vertices.Clear();
        triangulos.Clear();
        filter.mesh = null;
    }

    /// <summary>
    /// Função utilizada para construir a mesh baseada nas variaveis vertices e triangulos e associa essa mesh ao meshfilter
    /// </summary>
    void ConstruirMesh()
    {
        Mesh m = new Mesh();
        m.vertices = vertices.ToArray();
        m.triangles = triangulos.ToArray();
        m.RecalculateNormals();
        filter.mesh = m;
        //Debug.Log("Tamanho vertices: " + vertices.Count);
        //Debug.Log("Tamanho triangulos: " + triangulos.Count);
        renderer.material = mat;
        collider.sharedMesh = m;
        //print("Construiu mesh");
    }
}
