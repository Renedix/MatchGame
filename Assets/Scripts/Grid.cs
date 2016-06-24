using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour {

	public enum PieceType
	{
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

                // Game Piece Prefab
                GameObject newPiece = (GameObject)Instantiate(piecePrefabDict[PieceType.NORMAL], Vector3.zero, Quaternion.identity);
                newPiece.name = "Piece("+x+","+y+")";
                newPiece.transform.parent = transform;

                // Custom Game Object
                gamePieces[x, y] = newPiece.GetComponent<GamePiece>(); // Get the custom game piece class from the prefab!
                gamePieces[x, y].Init(x, y, this, PieceType.NORMAL);

                if (gamePieces[x, y].IsMovable())
                    gamePieces[x, y].MovableComponent.Move(x, y);

                if (gamePieces[x, y].IsColored())
                    gamePieces[x, y].ColoredComponent.SetColor((ColoredPiece.ColorType)Random.Range(0, gamePieces[x, y].ColoredComponent.NumberOfColors()));
            }
		}
    }
	
	public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(this.transform.position.x-xDim/2.0f+x, this.transform.position.y + yDim / 2.0f - y, 0);
    }
}
