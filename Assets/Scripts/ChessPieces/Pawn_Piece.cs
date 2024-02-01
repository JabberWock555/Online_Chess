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
    
}