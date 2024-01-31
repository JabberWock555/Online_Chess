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

    private  Vector3 targertPosition;
    private Vector3 desiredScale;
}