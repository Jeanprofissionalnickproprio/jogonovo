using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovimentoGrid : MonoBehaviour
{

    public GridManager grid;
    public Vector2Int celulaInicial = Vector2Int.zero;
    public float velocidade = 6f;

    Vector2Int celula;
    bool movendo;

    void Start()
    {
        celula = celulaInicial;
        transform.position = grid.CelulaParaMundo(celula);
    }

    void Update()
    {
        if (movendo) return;

        Vector2Int dir = Vector2Int.zero;

        var teclado = Keyboard.current;
        if (teclado != null)
        {
            if (teclado.wKey.wasPressedThisFrame || teclado.upArrowKey.wasPressedThisFrame) dir = Vector2Int.up;
            else if (teclado.sKey.wasPressedThisFrame || teclado.downArrowKey.wasPressedThisFrame) dir = Vector2Int.down;
            else if (teclado.aKey.wasPressedThisFrame || teclado.leftArrowKey.wasPressedThisFrame) dir = Vector2Int.left;
            else if (teclado.dKey.wasPressedThisFrame || teclado.rightArrowKey.wasPressedThisFrame) dir = Vector2Int.right;
        }

        // Opcional: clique do mouse em célula vizinha (2D, câmera ortográfica)
        var mouse = Mouse.current;
        if (dir == Vector2Int.zero && mouse != null && mouse.leftButton.wasPressedThisFrame)
        {
            Vector3 mundo = Camera.main.ScreenToWorldPoint(mouse.position.ReadValue());
            Vector2Int clicada = grid.MundoParaCelula(mundo);
            Vector2Int diff = clicada - celula;

            if (Mathf.Abs(diff.x) + Mathf.Abs(diff.y) == 1)
                dir = diff;
        }

        if (dir == Vector2Int.zero) return;

        Vector2Int destino = celula + dir;
        if (!grid.Andavel(destino)) return;

        StartCoroutine(Mover(destino));
    }

    IEnumerator Mover(Vector2Int destino)
    {
        movendo = true;
        celula = destino;
        Vector3 alvo = grid.CelulaParaMundo(destino);

        while ((transform.position - alvo).sqrMagnitude > 0.0001f)
        {
            transform.position = Vector3.MoveTowards(transform.position, alvo, velocidade * Time.deltaTime);
            yield return null;
        }

        transform.position = alvo;
        movendo = false;
    }
}