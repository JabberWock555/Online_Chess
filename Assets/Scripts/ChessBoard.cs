using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChessBoard : MonoBehaviour
{

    [Header("Prefabs && Materials")]

    [SerializeField] private GameObject whiteTile;
    [SerializeField] private GameObject blackTile;

    [SerializeField] private ChessPiece[] piecePrefabs;
    [SerializeField] private Material[] teamMaterials;

    //Logic
    private const int TILE_COUNT_X = 8;
    private const int TILE_COUNT_Y = 8;

    private ChessPiece[,] chessPieces;
    private GameObject[,] tiles;
    private List<Vector2Int> availableMoves = new();
    private List<ChessPiece> deadWhites_List = new();
    private List<ChessPiece> deadBlacks_List = new();
    private Camera currentCamera;
    private Vector2Int currentHover;
    private ChessPiece currentPiece;
    private Vector3 bounds;
    [SerializeField] private float y_Offset = 0, tileSize = 1, dragOffset = 1f ;
    private Vector3 boardCenter;
    [SerializeField] private float deathSize = 0.5f,deathSpacing = 0.3f, death_yOffest = 0.7f;
    

    private void Awake() {
    GenerateTiles(tileSize,TILE_COUNT_X, TILE_COUNT_Y);
    GenerateAllPieces();
    PositionAllPieces();
   }
   
   private void Update() {   
        if(!currentCamera){
            currentCamera = Camera.main;
            return;
        }

        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit info, 200, LayerMask.GetMask("Tile", "Hover", "Highlight"))){

            Vector2Int hitTileIndex = LookUpIndex(info.transform.gameObject);
            // -------- No Previous Hover    
            if(currentHover == -Vector2Int.one){
                currentHover = hitTileIndex;
                tiles[hitTileIndex.x,hitTileIndex.y].layer = LayerMask.NameToLayer("Hover");
            }
            // ------- With Previous Hover
            if(currentHover != hitTileIndex){
                tiles[currentHover.x,currentHover.y].layer = ContainsValidMoves(ref availableMoves, currentHover) ? LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer("Tile");
                currentHover = hitTileIndex;
                tiles[hitTileIndex.x,hitTileIndex.y].layer = LayerMask.NameToLayer("Hover");
            }

// ---------- On Mouse Down - Dragging
            if(Input.GetMouseButtonDown(0)){
                if(chessPieces[hitTileIndex.x, hitTileIndex.y] != null){
                    if(true){
                        currentPiece = chessPieces[hitTileIndex.x, hitTileIndex.y];

                        // Get a list for available Moves to Highlight
                        availableMoves = currentPiece.GetAvailableMoves(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y);
                        HighlightTiles();
                    }
                }
            }
// ---------- On Mouse Up - Released
            if(currentPiece != null && Input.GetMouseButtonUp(0)){
                Vector2Int previousPos = new Vector2Int(currentPiece.currnet_X, currentPiece.currnet_Y);

                bool validMove = MoveTo(currentPiece, previousPos, hitTileIndex);
                if(!validMove)
                    currentPiece.SetPosition( GetTileCenter(previousPos.x, previousPos.y));
                    
                currentPiece = null;
                RemoveHighlightTiles();
            }
        }
        else{
            if(currentHover != -Vector2Int.one){
                tiles[currentHover.x,currentHover.y].layer = ContainsValidMoves(ref availableMoves, currentHover) ? LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer("Tile") ;
                currentHover = -Vector2Int.one;
            }

            if(currentPiece != null && Input.GetMouseButtonUp(0)){
                currentPiece.SetPosition(GetTileCenter(currentPiece.currnet_X,currentPiece.currnet_Y));
                currentPiece = null;
                RemoveHighlightTiles();
            }
        }

        if(currentPiece){
            Plane horizontalPlane = new (Vector3.up,Vector3.up * y_Offset );
            if(horizontalPlane.Raycast(ray, out float distance))
                currentPiece.SetPosition(ray.GetPoint(distance) + Vector3.up * dragOffset);

        }
   }


#region  Generating -- ChessBoard

    private void GenerateTiles(float tileSize, int tileCount_X,int tileCount_Y ){

    bounds = new Vector3((tileCount_X/2) * tileSize, 0, (tileCount_Y/2) * tileSize) + boardCenter;

    tiles = new GameObject[tileCount_X,tileCount_Y];

    for(int x =0; x< tileCount_X; x++)
        for(int y=0; y < tileCount_Y; y++)
              tiles[x,y] = GenerateSingleTile(tileSize, x,y);
        
    
   }

    private GameObject GenerateSingleTile(float tileSize, int x, int y)
    {
        GameObject blockPrefab = (x + y) % 2 == 0 ? whiteTile : blackTile;
        GameObject tileObject = Instantiate(blockPrefab, transform);
        tileObject.name =  string.Format("X:{0}, Y:{1}", x,y);
        
        tileObject.transform.position = new Vector3(x*tileSize, 0, y*tileSize) - bounds;
        
        tileObject.layer = LayerMask.NameToLayer("Tile");
        //tileObject.AddComponent<BoxCollider>(); 

        return tileObject;
    }

    private Vector2Int LookUpIndex(GameObject hitInfo){

        for(int x =0; x< TILE_COUNT_X; x++)
            for(int y=0; y < TILE_COUNT_Y; y++)
                if(tiles[x,y] == hitInfo)
                    return new Vector2Int(x,y);
        
    
       return -Vector2Int.one;

    }
#endregion

#region  Generating -- ChessPieces

private void GenerateAllPieces(){
    chessPieces = new ChessPiece[TILE_COUNT_X, TILE_COUNT_Y];

    chessPieces[0,0] = GenerateSinglePiece(ChessPieceType.ROOK, ChessTeam.WHITE);
    chessPieces[1,0] = GenerateSinglePiece(ChessPieceType.KNIGHT, ChessTeam.WHITE);
    chessPieces[2,0] = GenerateSinglePiece(ChessPieceType.BISHOP, ChessTeam.WHITE);
    chessPieces[3,0] = GenerateSinglePiece(ChessPieceType.QUEEN, ChessTeam.WHITE);
    chessPieces[4,0] = GenerateSinglePiece(ChessPieceType.KING, ChessTeam.WHITE);
    chessPieces[5,0] = GenerateSinglePiece(ChessPieceType.BISHOP, ChessTeam.WHITE);
    chessPieces[6,0] = GenerateSinglePiece(ChessPieceType.KNIGHT, ChessTeam.WHITE);
    chessPieces[7,0] = GenerateSinglePiece(ChessPieceType.ROOK, ChessTeam.WHITE);

    // for(int i =0 ; i< TILE_COUNT_X; i++ )
    //     chessPieces[i,1] = GenerateSinglePiece(ChessPieceType.PAWN, ChessTeam.WHITE);


    chessPieces[0,7] = GenerateSinglePiece(ChessPieceType.ROOK, ChessTeam.BLACK);
    chessPieces[1,7] = GenerateSinglePiece(ChessPieceType.KNIGHT, ChessTeam.BLACK);
    chessPieces[2,7] = GenerateSinglePiece(ChessPieceType.BISHOP, ChessTeam.BLACK);
    chessPieces[3,7] = GenerateSinglePiece(ChessPieceType.KING, ChessTeam.BLACK);
    chessPieces[4,7] = GenerateSinglePiece(ChessPieceType.QUEEN, ChessTeam.BLACK);
    chessPieces[5,7] = GenerateSinglePiece(ChessPieceType.BISHOP, ChessTeam.BLACK);
    chessPieces[6,7] = GenerateSinglePiece(ChessPieceType.KNIGHT, ChessTeam.BLACK);
    chessPieces[7,7] = GenerateSinglePiece(ChessPieceType.ROOK, ChessTeam.BLACK);

    // for(int i =0 ; i< TILE_COUNT_X; i++ )
    //     chessPieces[i,6] = GenerateSinglePiece(ChessPieceType.PAWN, ChessTeam.BLACK);
}

private ChessPiece GenerateSinglePiece(ChessPieceType pieceType, ChessTeam team){

    ChessPiece piece = Instantiate(piecePrefabs[(int)pieceType -1], transform);

    piece.Type = pieceType;
    piece.Team = team;

    piece.GetComponent<MeshRenderer>().material = teamMaterials[(int)team];

    return piece;
}

#endregion

#region  Positioning -- ChessPieces

private void PositionAllPieces(){

    for(int x = 0; x < TILE_COUNT_X; x++ )
        for(int y = 0; y < TILE_COUNT_Y; y++)
            if(chessPieces[x,y] != null)
                PositionSinglePiece(x,y, true);
}

    private void PositionSinglePiece(int x, int y, bool force = false)
    {
        chessPieces[x,y].currnet_X = x;
        chessPieces[x,y].currnet_Y = y;
        chessPieces[x,y].SetPosition( GetTileCenter(x,y), force);
    }

    private Vector3 GetTileCenter(int x, int y)
    {
        return new Vector3(x*tileSize, y_Offset, y*tileSize) - bounds ;
    }



    #endregion

#region  Highlighting -- ChessBoard

    private void HighlightTiles(){
        for(int i =0 ; i < availableMoves.Count; i++){
            tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("Highlight");
        }
    } 
    private void RemoveHighlightTiles(){
        for(int i =0 ; i < availableMoves.Count; i++){
            tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("Tile");
        }
        availableMoves.Clear();
    } 
#endregion

#region Operations -- ChessPiece

    private bool MoveTo(ChessPiece piece, Vector2Int previousPos, Vector2Int targetPos)
    {
        if(!ContainsValidMoves(ref availableMoves, targetPos))
            return false;

        if(chessPieces[targetPos.x,targetPos.y] != null){
            ChessPiece otherPiece = chessPieces[targetPos.x,targetPos.y];

            if(otherPiece.Team == piece.Team)
                return false;

            if(otherPiece.Team == ChessTeam.WHITE){
                deadWhites_List.Add(otherPiece);
                otherPiece.SetScale(deathSize);
                otherPiece.SetPosition(new Vector3(7.75f *tileSize, death_yOffest, -1.3f*tileSize) - bounds + (Vector3.forward * deathSpacing) * deadWhites_List.Count);
            }else{
                deadBlacks_List.Add(otherPiece);
                otherPiece.SetScale(deathSize);
                otherPiece.SetPosition(new Vector3(-0.75f *tileSize, death_yOffest, 8.3f *tileSize) - bounds + (Vector3.back * deathSpacing) * deadBlacks_List.Count);
            }
        }

        chessPieces[targetPos.x, targetPos.y] = piece;
        chessPieces[previousPos.x, previousPos.y] = null;

        PositionSinglePiece(targetPos.x, targetPos.y);

        return true;
    }

    private bool ContainsValidMoves( ref List<Vector2Int> moves, Vector2 pos){
        for(int i =0; i< moves.Count; i++)
            if(moves[i].x == pos.x && moves[i].y == pos.y)
                return true;

        return false;
    }

#endregion

}  