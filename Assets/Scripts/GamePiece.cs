using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePiece : MonoBehaviour
{
    private int x;
    private int y;

    public int X
    {
        get { return x; }
        set { if(IsMoveable())
                x = value;
        }
    }

    public int Y
    {
        get { return y; }
        set
        {
            if (IsMoveable())
                y = value;
        }
    }
    private BgGrid.PieceType type;

    public BgGrid.PieceType Type
    {
        get { return type; }
    }
    private BgGrid bgGrid;

    public BgGrid GridRef
    {
        get { return bgGrid; }
    }

    private MoveablePiece moveableComponent;

    public MoveablePiece MoveableComponent
    {
        get { return moveableComponent; }
    }

    private ColorPiece colorComponent;

    public ColorPiece ColorComponent
    {
        get
        {
            return colorComponent;
        }
    }

    private void Awake()
    {
        moveableComponent = gameObject.GetComponent<MoveablePiece>();
        colorComponent = gameObject.GetComponent<ColorPiece>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(int _x, int _y, BgGrid _grid, BgGrid.PieceType _type)
    {
        x = _x;
        y = _y;
        bgGrid = _grid;
        type = _type;

    }
    void OnMouseEnter()
    {
        bgGrid.EnterPiece(this);
    }

    void OnMouseDown()
    {
        bgGrid.PressPiece(this);
    }

    void OnMouseUp()
    {
        bgGrid.ReleasePiece();
    }

    public bool IsMoveable()
    {
        return moveableComponent != null;
    }

    public bool IsColored()
    {
        return colorComponent != null;
    }
}
