using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int largura = 10;
    public int altura = 10;
    public float tamanhoCelula = 1f;

    public Camera cameraAlvo;

    public int extraColunas = 2; // colunas extras fora da câmera, em cada lado (esquerda/direita)
    public int extraLinhas = 2;  // linhas extras fora da câmera, em cada lado (cima/baixo)

    readonly HashSet<Vector2Int> bloqueadas = new HashSet<Vector2Int>();

    void Start()
    {
        CalcularTamanhoPelaCamera();
        AlinharComCamera();
    }

    public void CalcularTamanhoPelaCamera()
    {
        Camera cam = cameraAlvo != null ? cameraAlvo : Camera.main;
        if (cam == null || !cam.orthographic) return;

        float alturaMundo = cam.orthographicSize * 2f;
        float larguraMundo = alturaMundo * cam.aspect;

        int colunasVisiveis = Mathf.CeilToInt(larguraMundo / tamanhoCelula);
        int linhasVisiveis = Mathf.CeilToInt(alturaMundo / tamanhoCelula);

        largura = colunasVisiveis + extraColunas * 2;
        altura = linhasVisiveis + extraLinhas * 2;
    }

    public void AlinharComCamera()
    {
        Camera cam = cameraAlvo != null ? cameraAlvo : Camera.main;
        if (cam == null) return;

        Vector3 cantoInferiorEsquerdo = cam.ViewportToWorldPoint(new Vector3(0f, 0f, cam.nearClipPlane));
        cantoInferiorEsquerdo.z = transform.position.z;

        Vector3 offset = new Vector3(extraColunas * tamanhoCelula, extraLinhas * tamanhoCelula, 0f);
        transform.position = cantoInferiorEsquerdo - offset;
    }

    public Vector2Int CelulaInicialCamera() => new Vector2Int(extraColunas, extraLinhas);

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