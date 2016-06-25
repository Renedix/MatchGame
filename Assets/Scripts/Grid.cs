using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour {

	public enum PieceType
	{
        EMPTY,
		NORMAL,
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

	public PiecePrefab[] piecePrefabs;
	public GameObject backgroundPrefab;

	private Dictionary<PieceType, GameObject> piecePrefabDict;
    private GamePiece[,] gamePieces;

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

        fillBoard();
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

    private void fillBoard()
    {
        // Continue to fill the board until no piece has moved
        while (fillBoardStep()) { }
    }

    private bool fillBoardStep()
    {
        bool pieceMoved = false;

        // Go through each coordinate and move it down (if possible)
        for(int y = yDim - 2; y >= 0; y--)
        {
            for(int x = 0; x < xDim; x++)
            {
                GamePiece piece = gamePieces[x, y];
                if (piece.IsMovable())
                {
                    GamePiece pieceBelow = gamePieces[x, y + 1];
                    if (pieceBelow.PieceType == PieceType.EMPTY)
                    {
                        piece.MovableComponent.Move(x, y + 1);
                        gamePieces[x, y + 1] = piece;
                        SpawnGamePiece(x, y, PieceType.EMPTY);
                        pieceMoved = true;
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
                gamePieces[x, 0].MovableComponent.Move(x, 0);

                pieceMoved = true;
            }
        }

        return pieceMoved;
    }
}
