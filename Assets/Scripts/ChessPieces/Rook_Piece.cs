using System.Collections.Generic;
using UnityEngine;

public class Rook_Piece : ChessPiece {

    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCount_X, int tileCount_Y){
       
        List<Vector2Int> moves = new();

// --------- Down
        for(int i = currnet_Y - 1; i >=0; i --){

            if(board[currnet_X, i] == null)
                moves.Add(new Vector2Int(currnet_X,i));
            
            if(board[currnet_X, i] != null){

                if(board[currnet_X, i].Team != Team)
                    moves.Add(new Vector2Int(currnet_X,i));
                
                break;
            }
        }

// --------- Up
        for(int i = currnet_Y + 1; i < tileCount_Y; i++ ){

            if(board[currnet_X, i] == null)
                moves.Add(new Vector2Int(currnet_X,i));
            
            if(board[currnet_X, i] != null){

                if(board[currnet_X, i].Team != Team)
                    moves.Add(new Vector2Int(currnet_X,i));
                
                break;
            }
        }

// --------- Left
        for(int i = currnet_X - 1; i >=0; i --){

            if(board[i, currnet_Y] == null)
                moves.Add(new Vector2Int(i, currnet_Y));
            
            if(board[i, currnet_Y] != null){

                if(board[i, currnet_Y].Team != Team)
                    moves.Add(new Vector2Int(i, currnet_Y));
                
                break;
            }
        }


// --------- Right
        for(int i = currnet_X + 1; i < tileCount_X; i ++){

            if(board[i, currnet_Y] == null)
                moves.Add(new Vector2Int(i, currnet_Y));
            
            if(board[i, currnet_Y] != null){

                if(board[i, currnet_Y].Team != Team)
                    moves.Add(new Vector2Int(i, currnet_Y));
                
                break;
            }
        }

        return moves;
    }
    
}