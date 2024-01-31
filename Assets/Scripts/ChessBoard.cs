using System;
using System.Collections;
using System.Collections.Generic;
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
    private Camera currentCamera;
    private Vector2Int currentHover;
    private Vector3 bounds;
    [SerializeField] private float y_Offset = 0, tileSize = 1 ;
    private Vector3 boardCenter;

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
        if (Physics.Raycast(ray, out RaycastHit info, 200, LayerMask.GetMask("Tile", "Hover"))){

        Vector2Int hitTileIndex = LookUpIndex(info.transform.gameObject);

        if(currentHover == -Vector2Int.one){
            Debug.Log("Hover_0" + hitTileIndex);
            currentHover = hitTileIndex;
            tiles[hitTileIndex.x,hitTileIndex.y].layer = LayerMask.NameToLayer("Hover");
        }
        if(currentHover != hitTileIndex){
            Debug.Log("Hover_1"+ hitTileIndex);
            tiles[currentHover.x,currentHover.y].layer = LayerMask.NameToLayer("Tile");
            currentHover = hitTileIndex;
            tiles[hitTileIndex.x,hitTileIndex.y].layer = LayerMask.NameToLayer("Hover");
        }
    }
    else{
        if(currentHover != -Vector2Int.one){
            Debug.Log("Hover_2");
            tiles[currentHover.x,currentHover.y].layer = LayerMask.NameToLayer("Tile");
            currentHover = -Vector2Int.one;
        }
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

    for(int i =0 ; i< TILE_COUNT_X; i++ )
        chessPieces[i,1] = GenerateSinglePiece(ChessPieceType.PAWN, ChessTeam.WHITE);


    chessPieces[0,7] = GenerateSinglePiece(ChessPieceType.ROOK, ChessTeam.BLACK);
    chessPieces[1,7] = GenerateSinglePiece(ChessPieceType.KNIGHT, ChessTeam.BLACK);
    chessPieces[2,7] = GenerateSinglePiece(ChessPieceType.BISHOP, ChessTeam.BLACK);
    chessPieces[3,7] = GenerateSinglePiece(ChessPieceType.KING, ChessTeam.BLACK);
    chessPieces[4,7] = GenerateSinglePiece(ChessPieceType.QUEEN, ChessTeam.BLACK);
    chessPieces[5,7] = GenerateSinglePiece(ChessPieceType.BISHOP, ChessTeam.BLACK);
    chessPieces[6,7] = GenerateSinglePiece(ChessPieceType.KNIGHT, ChessTeam.BLACK);
    chessPieces[7,7] = GenerateSinglePiece(ChessPieceType.ROOK, ChessTeam.BLACK);

    for(int i =0 ; i< TILE_COUNT_X; i++ )
        chessPieces[i,6] = GenerateSinglePiece(ChessPieceType.PAWN, ChessTeam.BLACK);
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
        chessPieces[x,y].transform.position = GetTileCenter(x,y);
    }

    private Vector3 GetTileCenter(int x, int y)
    {
        return new Vector3(x*tileSize, y_Offset, y*tileSize) - bounds ;
    }



    #endregion

}