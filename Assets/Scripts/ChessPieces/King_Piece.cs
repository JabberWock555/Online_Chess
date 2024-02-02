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
    
}