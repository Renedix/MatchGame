using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour {

	public enum PieceType
	{
        EMPTY,
		NORMAL,
        BUBBLE,
		COUNT,
	};

	[System.Serializable]
	public struct PiecePrefab
	{
		public PieceType type;
		public GameObject prefab;
	};

	public int xDim;
	public int yDim;
    public float pieceMovement;

	public PiecePrefab[] piecePrefabs;
	public GameObject backgroundPrefab;

	private Dictionary<PieceType, GameObject> piecePrefabDict;
    private GamePiece[,] gamePieces;
    private bool reverse = false;

    private GamePiece pressPiece;
    private GamePiece enterPiece;

	// Use this for initialization
	void Start () {
		piecePrefabDict = new Dictionary<PieceType, GameObject> ();

		for (int i = 0; i < piecePrefabs.Length; i++) {
			if (!piecePrefabDict.ContainsKey (piecePrefabs [i].type)) {
				piecePrefabDict.Add (piecePrefabs [i].type, piecePrefabs [i].prefab);
			}
		}

        gamePieces = new GamePiece[xDim, yDim];
        for (int x = 0; x < xDim; x++) {
			for (int y = 0; y < yDim; y++) {
                // Background
				GameObject background = (GameObject)Instantiate (backgroundPrefab, GetWorldPosition(x, y), Quaternion.identity);
				background.transform.parent = transform;

                SpawnGamePiece(x, y, PieceType.EMPTY);
            }
		}

        Destroy(gamePieces[4, 4].gameObject);
        SpawnGamePiece(4, 4, PieceType.BUBBLE);

        Destroy(gamePieces[3, 4].gameObject);
        SpawnGamePiece(3, 4, PieceType.BUBBLE);

        Destroy(gamePieces[2, 4].gameObject);
        SpawnGamePiece(2, 4, PieceType.BUBBLE);

        Destroy(gamePieces[5, 4].gameObject);
        SpawnGamePiece(5, 4, PieceType.BUBBLE);

        Destroy(gamePieces[6, 3].gameObject);
        SpawnGamePiece(6, 3, PieceType.BUBBLE);

        StartCoroutine(fillBoard());
    }
	
	public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(this.transform.position.x-xDim/2.0f+x, this.transform.position.y + yDim / 2.0f - y, 0);
    }

    private GamePiece SpawnGamePiece(int x, int y, PieceType pieceType)
    {
        GameObject newPiece = (GameObject)Instantiate(piecePrefabDict[pieceType], GetWorldPosition(x, y), Quaternion.identity);
        // This new piece is a child of the grid
        newPiece.transform.parent = transform;

        gamePieces[x, y] = newPiece.GetComponent<GamePiece>();
        gamePieces[x, y].Init(x, y, this, pieceType);

        return gamePieces[x, y];
    }

    IEnumerator fillBoard()
    {
        // Continue to fill the board until no piece has moved
        while (fillBoardStep()) {
            reverse = !reverse;
            yield return new WaitForSeconds(pieceMovement);
        }
    }

    private bool fillBoardStep()
    {
        bool pieceMoved = false;

        // Go through each coordinate and move it down (if possible)
        for(int y = yDim - 2; y >= 0; y--)
        {
            for(int xLoop = 0; xLoop < xDim; xLoop++)
            {
                int x = xLoop;

                if (reverse)
                {
                    x = xDim - xLoop - 1;
                }

                GamePiece piece = gamePieces[x, y];
                if (piece.IsMovable())
                {
                    GamePiece pieceBelow = gamePieces[x, y + 1];
                    if (pieceBelow.PieceType == PieceType.EMPTY)
                    {
                        Destroy(pieceBelow.gameObject);
                        piece.MovableComponent.Move(x, y + 1,pieceMovement);
                        gamePieces[x, y + 1] = piece;
                        SpawnGamePiece(x, y, PieceType.EMPTY);
                        pieceMoved = true;
                    }else
                    {
                        // Diagonal fill

                        GamePiece pieceOnLeft = null;
                        if (x > 0)
                        {
                            pieceOnLeft = gamePieces[x - 1, y];
                        }

                        GamePiece pieceOnRight = null;
                        if (x < xDim - 2)
                        {
                            pieceOnRight = gamePieces[x + 1, y];
                        }

                        foreach(GamePiece adjacentPiece in new GamePiece[2] { pieceOnLeft, pieceOnRight })
                        {
                            if (adjacentPiece != null)
                            {
                                // If the piece on the side is not movable..
                                if (!adjacentPiece.IsMovable())
                                {
                                    pieceBelow = gamePieces[adjacentPiece.X, adjacentPiece.Y + 1];
                                    // And the piece below the piece on the side is empty
                                    if (pieceBelow.PieceType == PieceType.EMPTY)
                                    {
                                        // Fill It!
                                        Destroy(pieceBelow.gameObject);
                                        piece.MovableComponent.Move(adjacentPiece.X, y + 1, pieceMovement);
                                        gamePieces[adjacentPiece.X, y + 1] = piece;
                                        SpawnGamePiece(x, y, PieceType.EMPTY);
                                        pieceMoved = true;
                                        break;
                                    }
                                }
                            }
                        }

                    }
                }
            }
        }

        // Spawn new pieces
        for (int x = 0; x < xDim; x++)
        {
            GamePiece topGamePiece = gamePieces[x, 0];

            if (topGamePiece.PieceType == PieceType.EMPTY)
            {
                Destroy(topGamePiece.gameObject);
                // Spawn new piece
                GameObject newPiece = (GameObject)Instantiate(piecePrefabDict[PieceType.NORMAL], GetWorldPosition(x, -1), Quaternion.identity);
                // This new piece is a child of the grid
                newPiece.transform.parent = transform;

                // Set the game piece instance on the grid
                gamePieces[x, 0] = newPiece.GetComponent<GamePiece>();
                // Initialize the game object itself
                gamePieces[x, 0].Init(x, -1, this, PieceType.NORMAL);
                // Set to random color
                gamePieces[x, 0].ColoredComponent.SetColor((ColoredPiece.ColorType)Random.Range(0, gamePieces[x, 0].ColoredComponent.NumberOfColors()));
                // Move the piece to the first row
                gamePieces[x, 0].MovableComponent.Move(x, 0, pieceMovement);

                pieceMoved = true;
            }
        }

        return pieceMoved;
    }


    public bool IsAdjacent(GamePiece piece1, GamePiece piece2)
    {
        return (piece1.X == piece2.X && (piece1.Y == piece2.Y - 1 || piece1.Y == piece2.Y + 1))
                ||
                (piece1.Y == piece2.Y && (piece1.X == piece2.X - 1 || piece1.X == piece2.X + 1));
    }

    public void SwapPieces(GamePiece piece1, GamePiece piece2)
    {
        if (piece1.IsMovable() && piece2.IsMovable())
        {
            int piece1X = piece1.X;
            int piece1Y = piece1.Y;

            gamePieces[piece1X, piece1X] = piece2;
            gamePieces[piece2.X, piece2.Y] = piece1;

            piece1.MovableComponent.Move(piece2.X, piece2.Y, pieceMovement);
            piece2.MovableComponent.Move(piece1X, piece1Y, pieceMovement);
        }
        
    }

    public void PressPiece(GamePiece gamePiece)
    {
        pressPiece = gamePiece;
    }

    public void EnterPiece(GamePiece gamePiece)
    {
        enterPiece = gamePiece;
    }

    public void ReleasePiece()
    {
        if (IsAdjacent(pressPiece, enterPiece))
        {
            SwapPieces(pressPiece, enterPiece);
        }
    }

}
