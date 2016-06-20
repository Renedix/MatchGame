using UnityEngine;
using System.Collections;

public class GamePiece : MonoBehaviour {

    private int x;
    private int y;
    private Grid gridRef;
    private Grid.PieceType pieceType;

    public int X
    {
        get
        {
            return x;
        }
    }

    public int Y
    {
        get
        {
            return y;
        }
    }

    public Grid GridRef
    {
        get
        {
            return gridRef;
        }
    }

    public Grid.PieceType PieceType
    {
        get
        {
            return pieceType;
        }
    }

    public void Init(int x, int y, Grid gridRef, Grid.PieceType pieceType) {
        this.x = x;
        this.y = y;
        this.gridRef = gridRef;
        this.pieceType = pieceType;
    }

    void Start() { }
    void Update() { }
}
