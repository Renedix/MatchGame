﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Grid : MonoBehaviour {

	public enum PieceType
	{
        EMPTY,
		NORMAL,
        BUBBLE,
        ROW_CLEARING,
        COLUMN_CLEARING,
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
        Destroy(gamePieces[2, 3].gameObject);
        SpawnGamePiece(2, 3, PieceType.BUBBLE);

        Destroy(gamePieces[5, 4].gameObject);
        SpawnGamePiece(5, 4, PieceType.BUBBLE);

        Destroy(gamePieces[6, 3].gameObject);
        SpawnGamePiece(6, 3, PieceType.BUBBLE);

        StartCoroutine(FillBoard());
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

    IEnumerator FillBoard()
    {
        bool needsFilling = true;

        while (needsFilling)
        {
            yield return new WaitForSeconds(pieceMovement);
            // Continue to fill the board until no piece has moved
            while (FillBoardStep())
            {
                reverse = !reverse;
                yield return new WaitForSeconds(pieceMovement);
            }

            needsFilling = RemoveAnyMatchingPieces();
        }

    }

    private bool FillBoardStep()
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

                        GamePiece pieceOnLeft = GetPieceFromDirection(piece, Direction.LEFT);

                        GamePiece pieceOnRight = GetPieceFromDirection(piece, Direction.RIGHT);

                        foreach (GamePiece adjacentPiece in new GamePiece[2] { pieceOnLeft, pieceOnRight })
                        {
                            if (adjacentPiece != null)
                            {
                                // If the piece on the side is not movable..
                                if (!adjacentPiece.IsMovable())
                                {
                                    pieceBelow = GetPieceFromDirection(adjacentPiece,Direction.DOWN);
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
                gamePieces[x, 0].ColoredComponent.SetColor((ColoredPiece.ColorType)UnityEngine.Random.Range(0, gamePieces[x, 0].ColoredComponent.NumberOfColors()));
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

            piece1.MovableComponent.Move(piece2.X, piece2.Y, pieceMovement);
            piece2.MovableComponent.Move(piece1X, piece1Y, pieceMovement);

            gamePieces[piece1.X, piece1.Y] = piece1;
            gamePieces[piece2.X, piece2.Y] = piece2;

            if (GetMatch(piece1).Count>0 || GetMatch(piece2).Count > 0)
            {
                RemoveAnyMatchingPieces();
                enterPiece = null;
                pressPiece = null;
                StartCoroutine(FillBoard());
            }
            else
            {
                piece1X = piece1.X;
                piece1Y = piece1.Y;

                piece1.MovableComponent.Move(piece2.X, piece2.Y, pieceMovement);
                piece2.MovableComponent.Move(piece1X, piece1Y, pieceMovement);

                gamePieces[piece1.X, piece1.Y] = piece1;
                gamePieces[piece2.X, piece2.Y] = piece2;
            }

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

    private enum Direction {
        UP,
        DOWN,
        LEFT,
        RIGHT
    };

    private GamePiece GetPieceFromDirection(GamePiece piece, Direction direction)
    {
        if (direction == Direction.UP)
        {
            if (piece.Y > 0)
            {
                return gamePieces[piece.X, piece.Y - 1];
            }
        }else if(direction == Direction.RIGHT)
        {
            if (piece.X < xDim - 1)
            {
                return gamePieces[piece.X + 1, piece.Y];
            }
        }else if (direction == Direction.DOWN)
        {
            if (piece.Y < yDim - 1)
            {
                return gamePieces[piece.X, piece.Y + 1];
            }
        }
        else if(direction == Direction.LEFT)
        {
            if (piece.X > 0)
            {
                return gamePieces[piece.X - 1, piece.Y];
            }
        }

        return null;
    }


    private List<GamePiece> GetMatch(GamePiece piece)
    {
        
        List<GamePiece> matches = new List<GamePiece>();
        if (piece.IsColored())
        {
            GamePiece targetPiece = null;
            int startX = piece.X;
            int startY = piece.Y;
            ColoredPiece.ColorType color = piece.ColoredComponent.Color;

            List<GamePiece> leftPieces = new List<GamePiece>();
            int x = startX - 1;
            for (; x >= 0; x--)
            {
                targetPiece = gamePieces[x, startY];
                if (targetPiece.IsColored()
                    && targetPiece.ColoredComponent.Color == color)
                {
                    leftPieces.Add(targetPiece);
                }
                else
                {
                    break;
                }
            }

            List<GamePiece> rightPieces = new List<GamePiece>();
            x = startX + 1;
            for (; x < xDim; x++)
            {
                targetPiece = gamePieces[x, startY];
                if (targetPiece.IsColored()
                    && targetPiece.ColoredComponent.Color == color)
                {
                    rightPieces.Add(targetPiece);
                }
                else
                {
                    break;
                }
            }

            List<GamePiece> upPieces = new List<GamePiece>();
            int y = startY - 1;
            for (; y >= 0; y--)
            {
                targetPiece = gamePieces[startX, y];
                if (targetPiece.IsColored()
                    && targetPiece.ColoredComponent.Color == color)
                {
                    upPieces.Add(targetPiece);
                }
                else
                {
                    break;
                }
            }

            List<GamePiece> downPieces = new List<GamePiece>();
            y = startY + 1;
            for (; y < yDim; y++)
            {
                targetPiece = gamePieces[startX, y];
                if (targetPiece.IsColored()
                    && targetPiece.ColoredComponent.Color == color)
                {
                    downPieces.Add(targetPiece);
                }
                else
                {
                    break;
                }
            }

            // If there is more than 1 matching piece, then we have a line!

            // Horizontal Line
            if ((rightPieces.Count + leftPieces.Count) > 1)
            {
                matches.AddRange(rightPieces);
                matches.AddRange(leftPieces);
            }

            // Veritcal line
            if ((upPieces.Count + downPieces.Count) > 1)
            {
                matches.AddRange(upPieces);
                matches.AddRange(downPieces);
            }

            if (matches.Count > 0)
            {
                matches.Add(piece);
            }

        }

        return matches;
    }

    private bool RemoveAnyMatchingPieces()
    {
        bool piecesRemoved = false;

        for(int x = 0; x < xDim; x++)
        {
            for(int y = 0;y<yDim;y++)
            {
                if (gamePieces[x, y].IsRemovable())
                {
                    List<GamePiece> matchingPieces = GetMatch(gamePieces[x, y]);
                    if (matchingPieces.Count > 0)
                    {

                        PieceType specialPieceType = PieceType.COUNT;
                        int newSpecialPieceX = 0;
                        int newSpecialPieceY = 0;

                        if (matchingPieces.Count == 4)
                        {
                            if (pressPiece!=null && enterPiece != null)
                            {
                                // We know the direction of the piece
                                if (pressPiece.X == enterPiece.Y)
                                {
                                    // Same X means different Y, COLUMN PIECE!
                                    specialPieceType = PieceType.COLUMN_CLEARING;
                                }
                                else
                                {
                                    // Same Y means different X, ROW PIECE!
                                    specialPieceType = PieceType.ROW_CLEARING;
                                }
                            }
                            else
                            {
                                // randomly set the direction of the special piece
                                specialPieceType = (PieceType)UnityEngine.Random.Range(0, 1) + 3;

                                // If we did not swap the piece, select the first coordinate on the matching pieces to replace with a special piece
                                newSpecialPieceX = matchingPieces[0].X;
                                newSpecialPieceY = matchingPieces[0].Y;
                            }
                        }

                        foreach (GamePiece aPiece in matchingPieces)
                        {
                            if (ClearPiece(aPiece.X, aPiece.Y))
                            {
                                piecesRemoved = true;
                                
                                if (aPiece == pressPiece || aPiece == enterPiece)
                                {
                                    // If we swapped a piece, then we need to place one of the swapped pieces with a special piece!
                                    newSpecialPieceX = aPiece.X;
                                    newSpecialPieceY = aPiece.Y;
                                }
                            }
                        }

                        if (specialPieceType != PieceType.COUNT)
                        {
                            SpawnGamePiece(newSpecialPieceX, newSpecialPieceY, specialPieceType);
                            gamePieces[newSpecialPieceX, newSpecialPieceY].ColoredComponent.SetColor(matchingPieces[0].ColoredComponent.Color);
                        }
                    }
                }
            }
        }

        return piecesRemoved;
    }

    public bool ClearPiece(int x, int y)
    {
        if (gamePieces[x,y].IsRemovable() && !gamePieces[x, y].RemovableComponent.IsBeingRemoved)
        {
            gamePieces[x, y].RemovableComponent.ClearPiece();
            SpawnGamePiece(x, y, PieceType.EMPTY);

            RemoveAnyObstacles(gamePieces[x,y]);

            return true;
        }
        return false;
    }

    private void RemoveAnyObstacles(GamePiece piece)
    {

        List<GamePiece> potentialObstaclesToRemove = new List<GamePiece>();
        potentialObstaclesToRemove.Add(GetPieceFromDirection(piece, Direction.LEFT));
        potentialObstaclesToRemove.Add(GetPieceFromDirection(piece, Direction.RIGHT));
        potentialObstaclesToRemove.Add(GetPieceFromDirection(piece, Direction.UP));
        potentialObstaclesToRemove.Add(GetPieceFromDirection(piece, Direction.DOWN));

        foreach(GamePiece aPiece in potentialObstaclesToRemove)
        {
            if (aPiece != null && aPiece.IsRemovable() && aPiece.PieceType == PieceType.BUBBLE)
            {
                gamePieces[aPiece.X, aPiece.Y].RemovableComponent.ClearPiece();
                SpawnGamePiece(aPiece.X, aPiece.Y, PieceType.EMPTY);
            }
        }

    }

    public void RemovePiecesInColumn(int x)
    {
        for (int y = 0; y < yDim; y++)
        {
            ClearPiece(x, y);
        }
    }

    public void RemovePiecesInRow(int y)
    {
        for (int x = 0; x < xDim; x++)
        {
            ClearPiece(x, y);
        }
    }
}
