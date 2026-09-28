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

    public int passoColunas = 4; // quantas colunas a câmera anda quando o boneco sai pela esquerda/direita
    public int passoLinhas = 4;  // quantas linhas a câmera anda quando o boneco sai por cima/baixo

    int colunasVisiveis;
    int linhasVisiveis;
    Vector2Int origem; // coordenada global da célula local (0,0)

    readonly HashSet<Vector2Int> bloqueadas = new HashSet<Vector2Int>(); // coordenadas globais

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

        colunasVisiveis = Mathf.CeilToInt(larguraMundo / tamanhoCelula);
        linhasVisiveis = Mathf.CeilToInt(alturaMundo / tamanhoCelula);

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

    public bool Andavel(Vector2Int c) => DentroDoGrid(c) && !bloqueadas.Contains(c + origem);

    public void Bloquear(Vector2Int c) => bloqueadas.Add(c + origem);

    public void Desbloquear(Vector2Int c) => bloqueadas.Remove(c + origem);

    // Se a célula saiu da área visível, diz quanto a câmera deve andar (o passo configurado, na direção da saída)
    public Vector2Int DeslocamentoParaVoltarATela(Vector2Int c)
    {
        int passoX = Mathf.Clamp(passoColunas, 1, colunasVisiveis);
        int passoY = Mathf.Clamp(passoLinhas, 1, linhasVisiveis);
        int dx = 0, dy = 0;

        if (c.x < extraColunas) dx = -passoX;
        else if (c.x >= extraColunas + colunasVisiveis) dx = passoX;

        if (c.y < extraLinhas) dy = -passoY;
        else if (c.y >= extraLinhas + linhasVisiveis) dy = passoY;

        return new Vector2Int(dx, dy);
    }

    // Move a câmera instantaneamente até a próxima tela e atualiza o grid junto
    public void Deslocar(Vector2Int d)
    {
        Camera cam = cameraAlvo != null ? cameraAlvo : Camera.main;
        if (cam == null) return;

        Vector3 delta = new Vector3(d.x, d.y, 0f) * tamanhoCelula;

        origem += d;
        transform.position += delta;
        cam.transform.position += delta;
    }

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