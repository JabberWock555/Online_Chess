using System.Collections.Generic;
using UnityEngine;

public class Bishop_Piece : ChessPiece {
      public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCount_X, int tileCount_Y){
       
        List<Vector2Int> moves = new();
        
        //--------- Top Right
        for(int x = currnet_X + 1, y = currnet_Y +1; x < tileCount_X && y < tileCount_Y; x++ ,y++){

            if(board[x, y] == null)
                moves.Add(new Vector2Int(x,y));
            else{
                if(board[x, y].Team != Team)
                    moves.Add(new Vector2Int(x, y));
                break;
            }
        }

        //--------- Top left
        for(int x = currnet_X - 1, y = currnet_Y +1; x >= 0 && y < tileCount_Y; x-- ,y++){

            if(board[x, y] == null)
                moves.Add(new Vector2Int(x,y));
            else{
                if(board[x, y].Team != Team)
                    moves.Add(new Vector2Int(x, y));
                break;
            }
        }

         //--------- Bottom Left
        for(int x = currnet_X - 1, y = currnet_Y - 1; x >= 0 && y >= 0; x-- ,y--){

            if(board[x, y] == null)
                moves.Add(new Vector2Int(x,y));
            else{
                if(board[x, y].Team != Team)
                    moves.Add(new Vector2Int(x, y));
                break;
            }
        }

          //--------- Bottom Right
        for(int x = currnet_X + 1, y = currnet_Y - 1; x < tileCount_X && y >= 0; x++ ,y--){

            if(board[x, y] == null)
                moves.Add(new Vector2Int(x,y));
            else{
                if(board[x, y].Team != Team)
                    moves.Add(new Vector2Int(x, y));
                break;
            }
        }


        
        return moves;
      }
    
}

  