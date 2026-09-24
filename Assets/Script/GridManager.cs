using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int largura = 10;
    public int altura = 10;
    public float tamanhoCelula = 1f;

    readonly HashSet<Vector2Int> bloqueadas = new HashSet<Vector2Int>();

    public bool DentroDoGrid(Vector2Int c) =>
        c.x >= 0 && c.y >= 0 && c.x < largura && c.y < altura;

    public bool Andavel(Vector2Int c) => DentroDoGrid(c) && !bloqueadas.Contains(c);

    public void Bloquear(Vector2Int c) => bloqueadas.Add(c);

    public void Desbloquear(Vector2Int c) => bloqueadas.Remove(c);

    public Vector3 CelulaParaMundo(Vector2Int c)
    {
        float x = (c.x + 0.5f) * tamanhoCelula;
        float y = (c.y + 0.5f) * tamanhoCelula;
        return transform.position + new Vector3(x, y, 0f);
    }

    public Vector2Int MundoParaCelula(Vector3 pos)
    {
        Vector3 l = pos - transform.position;
        return new Vector2Int(Mathf.FloorToInt(l.x / tamanhoCelula),
                              Mathf.FloorToInt(l.y / tamanhoCelula));
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;
        for (int x = 0; x < largura; x++)
            for (int y = 0; y < altura; y++)
            {
                Vector3 centro = CelulaParaMundo(new Vector2Int(x, y));
                Gizmos.DrawWireCube(centro, new Vector3(tamanhoCelula, tamanhoCelula, 0.01f));
            }
    }
}