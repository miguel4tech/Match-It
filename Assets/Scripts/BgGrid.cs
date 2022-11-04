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

        Destroy(pieces[4, 4].gameObject);
        SpawnNewPiece (4, 4, PieceType.OBSTACLE);

        StartCoroutine( Fill());

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
                                    diagX = x -diagX;
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
                                            //else if(!pieceAbove.IsMoveable() && pieceAbove.Type != Piece)
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

}
