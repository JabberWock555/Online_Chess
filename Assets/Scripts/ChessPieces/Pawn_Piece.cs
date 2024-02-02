using System.Collections.Generic;
using UnityEngine;

public class Pawn_Piece : ChessPiece {

    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCount_X, int tileCount_Y){
       
        List<Vector2Int> moves = new();

        int direction = (Team == ChessTeam.WHITE) ? 1 : -1;

        // One Step Front Move
        if(board[currnet_X, currnet_Y + direction] == null)
            moves.Add(new Vector2Int(currnet_X, currnet_Y + direction));

        //Two Step Start Move
        if((Team == ChessTeam.WHITE && currnet_Y ==1) || (Team == ChessTeam.BLACK && currnet_Y == 6)){
            if(board[currnet_X, currnet_Y + direction * 2] == null)
                moves.Add(new Vector2Int(currnet_X, currnet_Y + (direction * 2)));
        } 

        //Diagonal Move - Kill
        if(currnet_X != 0)
            if(board[currnet_X - 1, currnet_Y + direction] != null && board[currnet_X - 1, currnet_Y + direction].Team != Team )
                moves.Add(new Vector2Int(currnet_X - 1, currnet_Y + direction));
        
        if(currnet_X != tileCount_X - 1)
            if(board[currnet_X + 1, currnet_Y + direction] != null && board[currnet_X + 1, currnet_Y + direction].Team != Team )
                moves.Add(new Vector2Int(currnet_X + 1, currnet_Y + direction));

        return moves;
    }

    public override SpecialMoves GetSpecialMoves(ref ChessPiece[,] board, ref List<Vector2Int[]> moveList, ref List<Vector2Int> availableMoves)
    {
        int direction = (Team == ChessTeam.WHITE) ? 1 : -1;
     
        // -------------- Promotion
        if( (Team == ChessTeam.WHITE && currnet_Y == 6) || (Team == ChessTeam.BLACK && currnet_Y == 1))
            return SpecialMoves.PROMOTION;


        // -------------- En Passant
        if(moveList.Count > 0){

            Vector2Int[] lastMove = moveList[moveList.Count - 1];
            if(board[lastMove[1].x, lastMove[1].y].Type == ChessPieceType.PAWN){       // If The Last moved piece was PAWN
                
                if((Mathf.Abs(lastMove[0].y - lastMove[1].y )== 2)                     // If the last move was +2 in both direction
                && (board[lastMove[1].x, lastMove[1].y].Team != Team)                  // If the last move was from another team
                    && ( lastMove[1].y == currnet_Y)){                                 // If both the pawns are on same Y
 
                   if( lastMove[1].x == currnet_X - 1){                                // Landed Left
                        availableMoves.Add(new Vector2Int(currnet_X - 1, currnet_Y + direction));
                        return SpecialMoves.ENPASSANT;
                       }
                    if( lastMove[1].x == currnet_X + 1){                               // Landed Right
                        availableMoves.Add(new Vector2Int(currnet_X + 1, currnet_Y + direction));
                        return SpecialMoves.ENPASSANT;
                    }
                }
                
            }    
        }


        return base.GetSpecialMoves(ref board, ref moveList, ref availableMoves);
    }
}
