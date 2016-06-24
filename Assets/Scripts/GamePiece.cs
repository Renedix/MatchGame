using UnityEngine;
using System.Collections;

public class GamePiece : MonoBehaviour {

    private int x;
    private int y;
    private Grid gridRef;
    private Grid.PieceType pieceType;
    private MovablePiece movableComponent;
    private ColoredPiece coloredComponent;

    public int X
    {
        get
        {
            return x;
        }
        set
        {
            if (IsMovable())
            {
                x = value;
            }
        }
    }

    public int Y
    {
        get
        {
            return y;
        }
        set
        {
            if (IsMovable())
            {
                y = value;
            }
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

    public MovablePiece MovableComponent
    {
        get
        {
            return movableComponent;
        }
    }

    public ColoredPiece ColoredComponent
    {
        get
        {
            return coloredComponent;
        }

    }

    public void Init(int x, int y, Grid gridRef, Grid.PieceType pieceType) {
        this.x = x;
        this.y = y;
        this.gridRef = gridRef;
        this.pieceType = pieceType;
        movableComponent = GetComponent<MovablePiece>();
        coloredComponent = GetComponent<ColoredPiece>();
    }

    public bool IsMovable()
    {
        return MovableComponent != null;
    }

    public bool IsColored()
    {
        return ColoredComponent != null;
    }

    void Start() { }
    void Update() { }
}
