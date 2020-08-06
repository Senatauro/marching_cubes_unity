using UnityEngine;

public static class Perlin3D
{
    /// <summary>
    /// Função perlin noise 3D. Uma função perlin noise precisa de entradas que não sejam inteiras, se entrar com valores inteiros o retorno sempre sera o mesmo numero
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public static float Noise(float x, float y, float z)
    {
        //Debug.Log("Valores: " + x + "\t" + y + "\t" + z);
        float ab = Mathf.PerlinNoise(x, y);
        float ac = Mathf.PerlinNoise(x, z);
        float bc = Mathf.PerlinNoise(y, z);

        float cb = Mathf.PerlinNoise(z, y);
        float ca = Mathf.PerlinNoise(z, x);
        float ba = Mathf.PerlinNoise(y, x);

        float abc = ab + bc + ac + cb + ca + ba;
        //Debug.Log("Valor ABC: " + abc);
        return abc/6f;
    }
}