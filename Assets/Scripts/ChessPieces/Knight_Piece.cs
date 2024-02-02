using System.Collections.Generic;
using UnityEngine;

public class Knight_Piece : ChessPiece {
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCount_X, int tileCount_Y){
       
        List<Vector2Int> moves = new();

//---------  Top Right
        int x = currnet_X +1;
        int y = currnet_Y +2;
        if(x < tileCount_X && y< tileCount_Y)
            if(board[x,y] == null || board[x,y].Team != Team)
                moves.Add(new Vector2Int(x,y));

        x = currnet_X +2;
        y = currnet_Y +1;
        if(x < tileCount_X && y< tileCount_Y)
            if(board[x,y] == null || board[x,y].Team != Team)
                moves.Add(new Vector2Int(x,y));

//---------  Top Left
        x = currnet_X -1;
        y = currnet_Y +2;
        if(x >= 0 && y< tileCount_Y)
            if(board[x,y] == null || board[x,y].Team != Team)
                moves.Add(new Vector2Int(x,y));

        x = currnet_X -2;
        y = currnet_Y +1;
        if(x >=0 && y< tileCount_Y)
            if(board[x,y] == null || board[x,y].Team != Team)
                moves.Add(new Vector2Int(x,y));

//---------  Bottom Right

        x = currnet_X +1;
        y = currnet_Y -2;
        if(x < tileCount_X && y >= 0)
            if(board[x,y] == null || board[x,y].Team != Team)
                moves.Add(new Vector2Int(x,y));


        x = currnet_X +2;
        y = currnet_Y -1;
        if(x < tileCount_X && y >= 0)
            if(board[x,y] == null || board[x,y].Team != Team)
                moves.Add(new Vector2Int(x,y));


//---------  Bottom Left
        x = currnet_X -1;
        y = currnet_Y -2;
        if(x >=0 && y>=0)
            if(board[x,y] == null || board[x,y].Team != Team)
                moves.Add(new Vector2Int(x,y));
        
        x = currnet_X -2;
        y = currnet_Y -1;
        if(x >=0 && y>=0)
            if(board[x,y] == null || board[x,y].Team != Team)
                moves.Add(new Vector2Int(x,y));

        return moves;
    }
    
    
}