using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovimentoGrid : MonoBehaviour
{

    public GridManager grid;

    Vector2Int celula;

    void Start()
    {
        celula = grid.CelulaInicialCamera();
        transform.position = grid.CelulaParaMundo(celula);
    }

    void Update()
    {
        Vector2Int dir = Vector2Int.zero;

        var teclado = Keyboard.current;
        if (teclado != null)
        {
            if (teclado.wKey.wasPressedThisFrame || teclado.upArrowKey.wasPressedThisFrame) dir = Vector2Int.up;
            else if (teclado.sKey.wasPressedThisFrame || teclado.downArrowKey.wasPressedThisFrame) dir = Vector2Int.down;
            else if (teclado.aKey.wasPressedThisFrame || teclado.leftArrowKey.wasPressedThisFrame) dir = Vector2Int.left;
            else if (teclado.dKey.wasPressedThisFrame || teclado.rightArrowKey.wasPressedThisFrame) dir = Vector2Int.right;
        }

        if (dir != Vector2Int.zero) TentarMover(dir);
    }

    void TentarMover(Vector2Int dir)
    {
        Vector2Int destino = celula + dir;
        if (!grid.Andavel(destino)) return;

        celula = destino;
        transform.position = grid.CelulaParaMundo(destino);
    }

    public void Up()    => TentarMover(Vector2Int.up);
    public void Down()  => TentarMover(Vector2Int.down);
    public void Left()  => TentarMover(Vector2Int.left);
    public void Right() => TentarMover(Vector2Int.right);
}