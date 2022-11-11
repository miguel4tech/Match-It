using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgGrid : MonoBehaviour
{
    public int xDim;
    public int yDim;
    public float fillTime = 0.1f;

    public enum PieceType
    {
        EMPTY,
        OBSTACLE,
        NORMAL,
        COUNT,
    };

    [System.Serializable]
    public struct PiecePrefab
    {
        public PieceType type;
        public GameObject prefab;
    };

    public PiecePrefab[] piecePrefabs;
    public GameObject backgroundPrefabs;

    private GamePiece[,] pieces; //the comma notation creates the array as 2D

    private bool inverse = false;

    private GamePiece pressedPiece;
    private GamePiece enteredPiece;


    private Dictionary<PieceType, GameObject> piecePrefabDict;
    // Start is called before the first frame update
    void Start()
    {
        piecePrefabDict = new Dictionary<PieceType, GameObject>();

        for (int i = 0; i < piecePrefabs.Length; i++)
        {
            if (!piecePrefabDict.ContainsKey(piecePrefabs[i].type))
            {
                piecePrefabDict.Add(piecePrefabs[i].type, piecePrefabs[i].prefab);
            }
        }

        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                GameObject background = (GameObject)Instantiate(backgroundPrefabs, GetWorldPosition(x, y), Quaternion.identity);
                background.transform.parent = transform;
            }
        }

        pieces = new GamePiece [xDim, yDim];
        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                //Spawn new empty piece
                SpawnNewPiece(x, y, PieceType.EMPTY);
                #region Old Code to spawn normal piece as new
                //GameObject newPieces = (GameObject)Instantiate(piecePrefabDict[PieceType.NORMAL], Vector3.zero, Quaternion.identity);
                //newPieces.name = "Piece(" + x + "," + y + ")";
                //newPieces.transform.parent = transform;

                //pieces[x,y] = newPieces.GetComponent<GamePiece>();
                //pieces[x,y].Init(x,y, this, PieceType.NORMAL);

                //if (pieces[x, y].IsMoveable())
                //{
                //    pieces[x, y].MoveableComponent.Move(x, y);
                //}

                //if(pieces[x, y].IsColored())
                //{
                //    pieces[x, y].ColorComponent.SetColor((ColorPiece.ColorType)Random.Range(0, pieces[x, y].ColorComponent.NumColors));
                //}
                #endregion
            }

        }
        #region Obstacle OBJ
        Destroy(pieces[1, 4].gameObject);
        SpawnNewPiece(1, 4, PieceType.OBSTACLE);

        Destroy(pieces[2, 4].gameObject);
        SpawnNewPiece(2, 4, PieceType.OBSTACLE);

        Destroy(pieces[3, 4].gameObject);
        SpawnNewPiece(3, 4, PieceType.OBSTACLE);

        Destroy(pieces[4, 0].gameObject);
        SpawnNewPiece(4, 0, PieceType.OBSTACLE);

        Destroy(pieces[5, 4].gameObject);
        SpawnNewPiece(5, 4, PieceType.OBSTACLE);

        Destroy(pieces[6, 4].gameObject);
        SpawnNewPiece(6, 4, PieceType.OBSTACLE);

        Destroy(pieces[7, 4].gameObject);
        SpawnNewPiece(7, 4, PieceType.OBSTACLE);

        StartCoroutine( Fill());
        #endregion
    }

    private void Update()
    {

    }

    public IEnumerator Fill()
    {
        while (FillStep())
        {
            inverse = !inverse;
            yield return new WaitForSeconds (fillTime);
        }
    }

    public bool FillStep()
    {
        bool movedPiece = false;

        for(int y = yDim-2; y >=0; y--)
        {
            for(int loopX=0; loopX < xDim; loopX++)
            {
                int x = loopX;

                if (inverse)
                {
                    x = xDim - 1 - loopX;
                }


                GamePiece piece = pieces[x, y];

                if (piece.IsMoveable())
                {
                    GamePiece pieceBelow = pieces[x, y + 1];

                    if(pieceBelow.Type == PieceType.EMPTY)
                    {
                        Destroy(pieceBelow.gameObject);
                        piece.MoveableComponent.Move(x, y + 1, fillTime);
                        pieces [x, y + 1] = piece;
                        SpawnNewPiece(x,y, PieceType.EMPTY);
                        movedPiece = true;
                    }
                    else
                    {
                        for(int diag = -1; diag <= 1; diag++)
                        {
                            if(diag != 0)
                            {
                                int diagX = x + diag;

                                if (inverse)
                                {
                                    diagX = x -diag;
                                }

                                if(diagX >= 0 && diagX < xDim)
                                {
                                    GamePiece diagonalPiece = pieces[diagX, y + 1];

                                    if(diagonalPiece.Type == PieceType.EMPTY)
                                    {
                                        bool hasPieceAbove = true;

                                        for (int aboveY = y; aboveY >= 0; aboveY--)
                                        {
                                            GamePiece pieceAbove = pieces[diagX, aboveY];

                                            if (pieceAbove.IsMoveable())
                                            {
                                                break;
                                            }
                                            else if(!pieceAbove.IsMoveable() && pieceAbove.Type != PieceType.EMPTY)
                                            {
                                                hasPieceAbove = false;
                                                break;
                                            }
                                            
                                        }
                                        
                                        if(!hasPieceAbove)
                                        {
                                            Destroy (diagonalPiece.gameObject);
                                            piece.MoveableComponent.Move (diagX, y + 1, fillTime);
                                            pieces [diagX, y + 1] = piece;
                                            SpawnNewPiece (x, y, PieceType.EMPTY);
                                            movedPiece = true;
                                            
                                            break;
                                        }
                                    }
                                }
                            }
                        }    
                    }
                }
            }
        }
        for (int x = 0; x < xDim; x++)
        {
            GamePiece pieceBelow = pieces[x, 0];

            if(pieceBelow.Type == PieceType.EMPTY)
            {
                Destroy(pieceBelow.gameObject);
                GameObject newPiece = (GameObject)Instantiate(piecePrefabDict[PieceType.NORMAL], GetWorldPosition(x, -1), Quaternion.identity);
                newPiece.transform.parent = transform;

                pieces[x, 0] = newPiece.GetComponent<GamePiece>();
                pieces[x, 0].Init(x, -1, this, PieceType.NORMAL);
                pieces[x, 0].MoveableComponent.Move(x, 0, fillTime);
                pieces[x, 0].ColorComponent.SetColor((ColorPiece.ColorType)Random.Range(0, pieces[x, 0].ColorComponent.NumColors));

                movedPiece = true;
            }
        }

        return movedPiece;
    }

    public Vector2 GetWorldPosition(int x, int y)
    {
         return new Vector2(transform.position.x - xDim/2.0f + x, transform.position.y + yDim / 2.0f - y);
    }

    public GamePiece SpawnNewPiece(int x, int y, PieceType type)
    {
        GameObject newPiece = (GameObject)Instantiate(piecePrefabDict[type], GetWorldPosition(x, y), Quaternion.identity);
        newPiece.transform.parent = transform;

        pieces[x,y] = newPiece.GetComponent<GamePiece>();
        pieces[x,y].Init (x,y, this, type);

        return pieces[x,y];
    }

    public bool IsAdjacent(GamePiece piece1, GamePiece piece2)
    {
        return (piece1.X == piece2.X && (int)Mathf.Abs(piece1.Y - piece2.Y) == 1 || (piece1.Y == piece2.Y && (int)Mathf.Abs (piece1.X - piece2.X) == 1));
    }

    public void SwapPieces(GamePiece piece1, GamePiece piece2)
    {
        if(piece1.IsMoveable () && piece2.IsMoveable())
        {
            pieces[piece1.X, piece1.Y] = piece2;
            pieces[piece2.X, piece2.Y] = piece1;

            if(GetMatch (piece1, piece2.X, piece2.Y) !=null || GetMatch (piece2, piece1.X, piece1.Y) != null)
            {
                int piece1X = piece1.X;
                int piece1Y = piece1.Y;

                piece1.MoveableComponent.Move(piece2.X, piece2.Y, fillTime);
                piece2.MoveableComponent.Move(piece1X, piece1Y, fillTime);
            }
            else
            {
                pieces[piece1.X, piece1.Y] = piece1;
                pieces[piece2.X, piece2.Y] = piece2;
            }
        }
    }

    public void PressPiece(GamePiece piece)
    {
        pressedPiece = piece;
    }

    public void EnterPiece(GamePiece piece)
    {
        enteredPiece = piece;
    }

    public void ReleasePiece()
    {
        if(IsAdjacent (pressedPiece, enteredPiece))
        {
            SwapPieces(pressedPiece, enteredPiece);
        }
    }

    public List<GamePiece> GetMatch(GamePiece piece, int newX, int newY)
    {
        if (piece.IsColored())
        {
            ColorPiece.ColorType color = piece.ColorComponent.Color;
            List<GamePiece> horizontalPieces = new List<GamePiece> ();
            List<GamePiece> verticalPieces = new List<GamePiece> ();
            List<GamePiece> matchingPieces = new List<GamePiece>();

            //First Check horizontal
            horizontalPieces.Add(piece);

            for (int dir = 0; dir <= 1; dir++)
            {
                for (int xOffset = 0; xOffset < xDim; xOffset++)
                {
                    int x;

                    if(dir == 0)
                    {
                        x = newX - xOffset;//left
                    }
                    else
                    {
                        x = newY + xOffset;//right
                    }

                    if(x < 0 || x >= xDim)
                    {
                        break;
                    }

                    if (pieces[x, newY].IsColored() && pieces[x, newY].ColorComponent.Color == color)
                    {
                        horizontalPieces.Add(pieces[x, newY]);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            if(horizontalPieces.Count >= 3) 
            {
                for (int i = 0; i < horizontalPieces.Count; i++)
                {
                    matchingPieces.Add(horizontalPieces[i]);
                }
            }

            //Tranverse vertically if we found a match (for L and T shape)
            if(horizontalPieces.Count >= 3)
            {
                for (int i = 0; i < horizontalPieces.Count; i++)
                {
                    for (int dir = 0; dir <= 1; dir++)
                    {
                        for (int yOffset = 0; yOffset < yDim; yOffset++)
                        {
                            int y; 
                            if(dir == 0) //Up
                            {
                                y = newY - yOffset;
                            }
                            else //Down
                            {
                                y = newY + yOffset;
                            }

                            if(y < 0 || y >= yDim) //if outside of the needed dimension, breakout
                            {
                                break;
                            }

                            if (pieces[horizontalPieces[i].X, y].IsColored() && pieces[horizontalPieces[i].X, y].ColorComponent.Color == color)
                            {
                                verticalPieces.Add(pieces[horizontalPieces[i].X, y]);
                            }
                            else //if the piece doesn't match, we break out
                            {
                                break;
                            }
                        }
                    }

                    if(verticalPieces.Count < 2)
                    {
                        verticalPieces.Clear();//if we don't have enough vertical pieces to form a match we clear the list
                    }
                    else
                    {
                        for (int j = 0; j < verticalPieces.Count; j++)
                        {
                            matchingPieces.Add(verticalPieces[j]);
                        }
                        break;
                    }
                }
            }

            if(matchingPieces.Count >= 3)
            {
                return matchingPieces;
            }

            //Didn't find anything going horizontally first
            //So now check vertically
            horizontalPieces.Clear();
            verticalPieces.Clear();
            verticalPieces.Add(piece);

            for (int dir = 0; dir <= 1; dir++)
            {
                for (int yOffset = 0; yOffset < yDim; yOffset++)
                {
                    int y;

                    if (dir == 0)
                    {
                        y = newX - yOffset;//Up
                    }
                    else
                    {
                        y = newY + yOffset;//Down
                    }

                    if (y < 0 || y >= yDim)
                    {
                        break;
                    }

                    if (pieces[newX, y].IsColored() && pieces[newX, y].ColorComponent.Color == color)
                    {
                        verticalPieces.Add(pieces[newX, y]);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            if (verticalPieces.Count >= 3)
            {
                for (int i = 0; i < verticalPieces.Count; i++)
                {
                    matchingPieces.Add(verticalPieces[i]);
                }
            }

            //Tranverse horizontally if we found a match (for L and T shape)
            if (verticalPieces.Count >= 3)
            {
                for (int i = 0; i < verticalPieces.Count; i++)
                {
                    for (int dir = 0; dir <= 1; dir++)
                    {
                        for (int xOffset = 0; xOffset < xDim; xOffset++)
                        {
                            int x;
                            if (dir == 0) //Left
                            {
                                x = newX - xOffset;
                            }
                            else //Right
                            {
                                x = newX + xOffset;
                            }

                            if (x < 0 || x >= xDim) //if outside of the needed dimension, breakout
                            {
                                break;
                            }

                            if (pieces[x, verticalPieces[i].Y].IsColored() && pieces[x, verticalPieces[i].Y].ColorComponent.Color == color)
                            {
                                verticalPieces.Add(pieces[x, verticalPieces[i].Y]);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    if (horizontalPieces.Count < 2)
                    {
                        horizontalPieces.Clear();//if we don't have enough horizontal pieces to form a match we clear the list
                    }
                    else
                    {
                        for (int j = 0; j < horizontalPieces.Count; j++)
                        {
                            matchingPieces.Add(horizontalPieces[j]);
                        }
                        break;
                    }
                }
            }

            if (matchingPieces.Count >= 3)
            {
                return matchingPieces;
            }
        }
        return null; 
    }

}
