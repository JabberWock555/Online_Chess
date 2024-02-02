using System.Collections.Generic;
using UnityEngine;

public class King_Piece : ChessPiece {

    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCount_X, int tileCount_Y){
       
        List<Vector2Int> moves = new();
        
// ---------- Right

        if(currnet_X + 1 < tileCount_X){

            //------ Top 
            if(currnet_Y + 1 < tileCount_Y)
                if(board[currnet_X + 1, currnet_Y + 1] == null  || board[currnet_X + 1, currnet_Y + 1].Team != Team)
                    moves.Add(new Vector2Int(currnet_X +1, currnet_Y + 1));
            
            // ------ Right
            if(board[currnet_X + 1, currnet_Y] == null  || board[currnet_X + 1, currnet_Y].Team != Team)
                moves.Add(new Vector2Int(currnet_X +1, currnet_Y));

            // ------ Bottom
            if(currnet_Y - 1 >= 0)
                if(board[currnet_X + 1, currnet_Y - 1] == null  || board[currnet_X + 1, currnet_Y - 1].Team != Team)
                    moves.Add(new Vector2Int(currnet_X +1, currnet_Y - 1));

        }

// ---------- Left

        if(currnet_X - 1 >= 0){

            //------ Top 
            if(currnet_Y + 1 < tileCount_Y)
                if(board[currnet_X - 1, currnet_Y + 1] == null  || board[currnet_X - 1, currnet_Y + 1].Team != Team)
                    moves.Add(new Vector2Int(currnet_X - 1, currnet_Y + 1));
            
            // ------ Left
            if(board[currnet_X - 1, currnet_Y] == null  || board[currnet_X - 1, currnet_Y].Team != Team)
                moves.Add(new Vector2Int(currnet_X - 1, currnet_Y));

            // ------ Bottom
            if(currnet_Y - 1 >= 0)
                if(board[currnet_X - 1, currnet_Y - 1] == null  || board[currnet_X - 1, currnet_Y - 1].Team != Team)
                    moves.Add(new Vector2Int(currnet_X - 1, currnet_Y - 1));

        }

// ---------- Up

        if(currnet_Y + 1 < tileCount_Y)
            if(board[currnet_X, currnet_Y + 1] == null  || board[currnet_X, currnet_Y + 1].Team != Team)
                moves.Add(new Vector2Int(currnet_X, currnet_Y + 1));

// ---------- Down

        if(currnet_Y - 1 >= 0)
            if(board[currnet_X , currnet_Y - 1] == null  || board[currnet_X, currnet_Y - 1].Team != Team)
                moves.Add(new Vector2Int(currnet_X , currnet_Y - 1));
        

        return moves;
    }

    public override SpecialMoves GetSpecialMoves(ref ChessPiece[,] board, ref List<Vector2Int[]> moveList, ref List<Vector2Int> availableMoves)
    {
        SpecialMoves specialMove = SpecialMoves.NONE;

        var kingMove = moveList.Find(  m => m[0].x == 4 && m[0].y == ((Team == ChessTeam.WHITE) ? 0 : 7));
        var leftRookMove = moveList.Find(  m => m[0].x == 0 && m[0].y == ((Team == ChessTeam.WHITE) ? 0 : 7));
        var rightRookMove = moveList.Find(  m => m[0].x == 7 && m[0].y == ((Team == ChessTeam.WHITE) ? 0 : 7));

        if( kingMove == null && currnet_X == 4 ){

            //---- White Team
            if(Team == ChessTeam.WHITE)
            {
                //---- LeftRook
                if(leftRookMove == null
                && board[0,0].Type == ChessPieceType.ROOK 
                && board[0,0].Team == ChessTeam.WHITE
                && board[3,0] == null
                && board[2,0] == null
                && board[1,0] == null){
                    availableMoves.Add(new Vector2Int(2,0));
                    specialMove = SpecialMoves.CASTLING;
                    
                }

                //---- RightRook
                if(rightRookMove == null 
                && board[7,0].Type == ChessPieceType.ROOK 
                && board[7,0].Team == ChessTeam.WHITE
                && board[5,0] == null
                && board[6,0] == null){
                    availableMoves.Add(new Vector2Int(6,0));
                    specialMove = SpecialMoves.CASTLING;
                    
                }
            }
            else
            {
                //---- LeftRook
                if(leftRookMove == null 
                && board[0,7].Type == ChessPieceType.ROOK 
                && board[0,7].Team == ChessTeam.BLACK
                && board[3,7] == null
                && board[2,7] == null
                && board[1,7] == null){
                    availableMoves.Add(new Vector2Int(2,7));
                    specialMove = SpecialMoves.CASTLING;
                
                }

                //---- RightRook
                if(rightRookMove == null 
                && board[7,7].Type == ChessPieceType.ROOK 
                && board[7,7].Team == ChessTeam.BLACK
                && board[5,7] == null
                && board[6,7] == null){
                    availableMoves.Add(new Vector2Int(6,7));
                    specialMove = SpecialMoves.CASTLING;
                
                }
            }
        }
        
        return specialMove;
    }
}