using System.Collections.Generic;
using UnityEngine;

public enum ChessPieceType{
    NONE,
    PAWN,
    ROOK,
    KNIGHT,
    BISHOP,
    QUEEN,
    KING
}

public enum ChessTeam{
    WHITE,
    BLACK
}

public class ChessPiece : MonoBehaviour {
    
    public ChessTeam Team;
    public ChessPieceType Type;

    public int currnet_X;
    public int currnet_Y;

    private  Vector3 targetPosition;
    private Vector3 desiredScale = Vector3.one;

    private void Update(){

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime*10f);
        transform.localScale = Vector3.Lerp(transform.localScale, desiredScale, Time.deltaTime*10f);
    }


    public virtual List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCount_X, int tileCount_Y){
        List<Vector2Int> moves = new()
        {
            new Vector2Int(3, 3),
            new Vector2Int(3, 4),
            new Vector2Int(4, 3),
            new Vector2Int(4, 4)
        };

        return moves;
    }
    public virtual void SetPosition(Vector3 targetPos, bool force = false){
        targetPosition = targetPos;
        if(force)
            transform.position = targetPosition;

    }

    public virtual void SetScale(float newScale, bool force = false){
        desiredScale = Vector3.one * newScale;
        if(force) 
            transform.localScale = desiredScale;

    }
}