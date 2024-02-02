using System.Collections.Generic;
using UnityEngine;

public class Queen_Piece : ChessPiece {

       public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCount_X, int tileCount_Y){
       
        List<Vector2Int> moves = new();
        

// --------- Down
        for(int i = currnet_Y - 1; i >=0; i --){

            if(board[currnet_X, i] == null)
                moves.Add(new Vector2Int(currnet_X,i));
            else{
                if(board[currnet_X, i].Team != Team)
                    moves.Add(new Vector2Int(currnet_X,i));
                
                break;
            }
        }

// --------- Up
        for(int i = currnet_Y + 1; i < tileCount_Y; i++ ){

            if(board[currnet_X, i] == null)
                moves.Add(new Vector2Int(currnet_X,i));
            else{
                if(board[currnet_X, i].Team != Team)
                    moves.Add(new Vector2Int(currnet_X,i));
                
                break;
            }
        }

// --------- Left
        for(int i = currnet_X - 1; i >=0; i --){

            if(board[i, currnet_Y] == null)
                moves.Add(new Vector2Int(i, currnet_Y));
            else{
                if(board[i, currnet_Y].Team != Team)
                    moves.Add(new Vector2Int(i, currnet_Y));
                
                break;
            }
        }


// --------- Right
        for(int i = currnet_X + 1; i < tileCount_X; i ++){

            if(board[i, currnet_Y] == null)
                moves.Add(new Vector2Int(i, currnet_Y));
            else{
                if(board[i, currnet_Y].Team != Team)
                    moves.Add(new Vector2Int(i, currnet_Y));
                
                break;
            }
        }


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