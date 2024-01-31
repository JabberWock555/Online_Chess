using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessBoard : MonoBehaviour
{
    [SerializeField] private GameObject whiteTile;
    [SerializeField] private GameObject blackTile;
    //Logic
    private const int TILE_COUNT_X = 8;
    private const int TILE_COUNT_Y = 8;

    private GameObject[,] tiles;
    private Camera currentCamera;
    private Vector2Int currentHover;
    Vector3 bounds;
    private Vector3 boardCenter;

    private void Awake() {
    GenerateTiles(1,TILE_COUNT_X, TILE_COUNT_Y);
   }
   
   private void Update() {   
    if(!currentCamera){
        currentCamera = Camera.main;
        return;
    }

    RaycastHit info;
    Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
    if(Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile"))){

        Vector2Int hitTileiIndex = LookUpIndex(info.transform.gameObject);

        if(currentHover == -Vector2Int.one){
            currentHover = hitTileiIndex;
            tiles[hitTileiIndex.x,hitTileiIndex.y].layer = LayerMask.NameToLayer("Hover");
        }

        if(currentHover != hitTileiIndex){
            tiles[currentHover.x,currentHover.y].layer = LayerMask.NameToLayer("Tile");
            currentHover = hitTileiIndex;
            tiles[hitTileiIndex.x,hitTileiIndex.y].layer = LayerMask.NameToLayer("Hover");
        }
    }
    else{
        if(currentHover != -Vector2Int.one){
            tiles[currentHover.x,currentHover.y].layer = LayerMask.NameToLayer("Tile");
            currentHover = -Vector2Int.one;
        }
    }
   }


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
        tileObject.AddComponent<BoxCollider>(); 

        return tileObject;
    }

    private Vector2Int LookUpIndex(GameObject hitInfo){

        for(int x =0; x< TILE_COUNT_X; x++)
            for(int y=0; y < TILE_COUNT_Y; y++)
                if(tiles[x,y] == hitInfo)
                    return new Vector2Int(x,y);
        
        return -Vector2Int.one;

    }


}