using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChessBoard : MonoBehaviour
{

    [Header("Prefabs && Materials")]

    [SerializeField] private GameObject whiteTile;
    [SerializeField] private GameObject blackTile;

    [SerializeField] private ChessPiece[] piecePrefabs;
    [SerializeField] private Material[] teamMaterials;

    [Header("UI")]
    [SerializeField] private GameObject victoryScreen;
    private Text winningTitle;

    [Header("Variables")]
    [SerializeField] private float y_Offset = 0, tileSize = 1, dragOffset = 1f;
    [SerializeField] private float deathSize = 0.5f, deathSpacing = 0.3f, death_yOffest = 0.7f;

    //Logic
    private const int TILE_COUNT_X = 8;
    private const int TILE_COUNT_Y = 8;

    private ChessPiece[,] chessPieces;
    private GameObject[,] tiles;
    private List<Vector2Int[]> moves_List = new();
    private List<Vector2Int> availableMoves_List = new();
    private List<ChessPiece> deadWhites_List = new();
    private List<ChessPiece> deadBlacks_List = new();
    private Vector2Int currentHover;
    private ChessPiece currentPiece;
    private SpecialMoves specialMove;
    private Vector3 bounds;
    private Vector3 boardCenter;
    private bool isWhiteTurn;

    //private Camera currentCamera;

    private void Awake()
    {

        victoryScreen.SetActive(false);
        winningTitle = victoryScreen.transform.GetChild(0).GetComponent<Text>();

        isWhiteTurn = true;

        GenerateTiles(tileSize, TILE_COUNT_X, TILE_COUNT_Y);
        GenerateAllPieces();
        PositionAllPieces();


    }

    private void Update()
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit info, 200, LayerMask.GetMask("Tile", "Hover", "Highlight")))
        {

            Vector2Int hitTileIndex = LookUpIndex(info.transform.gameObject);
            // -------- No Previous Hover    
            if (currentHover == -Vector2Int.one)
            {
                currentHover = hitTileIndex;
                tiles[hitTileIndex.x, hitTileIndex.y].layer = LayerMask.NameToLayer("Hover");
            }
            // ------- With Previous Hover
            if (currentHover != hitTileIndex)
            {
                tiles[currentHover.x, currentHover.y].layer = ContainsValidMoves(ref availableMoves_List, currentHover) ? LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer("Tile");
                currentHover = hitTileIndex;
                tiles[hitTileIndex.x, hitTileIndex.y].layer = LayerMask.NameToLayer("Hover");
            }

            // ---------- On Mouse Down - Dragging
            OnMouseDragging(hitTileIndex);

            // ---------- On Mouse Up - Released
            OnMouseRelease(hitTileIndex);
        }
        else
        {
            if (currentHover != -Vector2Int.one)
            {
                tiles[currentHover.x, currentHover.y].layer = ContainsValidMoves(ref availableMoves_List, currentHover) ? LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer("Tile");
                currentHover = -Vector2Int.one;
            }

            if (currentPiece != null && Input.GetMouseButtonUp(0))
            {
                currentPiece.SetPosition(GetTileCenter(currentPiece.currnet_X, currentPiece.currnet_Y));
                currentPiece = null;
                RemoveHighlightTiles();
            }
        }

        if (currentPiece)
        {
            Plane horizontalPlane = new(Vector3.up, Vector3.up * y_Offset);
            if (horizontalPlane.Raycast(ray, out float distance))
                currentPiece.SetPosition(ray.GetPoint(distance) + Vector3.up * dragOffset);

        }
    }


    #region  Generating -- ChessBoard

    private void GenerateTiles(float tileSize, int tileCount_X, int tileCount_Y)
    {

        bounds = new Vector3((tileCount_X / 2) * tileSize, 0, (tileCount_Y / 2) * tileSize) + boardCenter;

        tiles = new GameObject[tileCount_X, tileCount_Y];

        for (int x = 0; x < tileCount_X; x++)
            for (int y = 0; y < tileCount_Y; y++)
                tiles[x, y] = GenerateSingleTile(tileSize, x, y);


    }

    private GameObject GenerateSingleTile(float tileSize, int x, int y)
    {
        GameObject blockPrefab = (x + y) % 2 == 0 ? whiteTile : blackTile;
        GameObject tileObject = Instantiate(blockPrefab, transform);
        tileObject.name = string.Format("X:{0}, Y:{1}", x, y);

        tileObject.transform.position = new Vector3(x * tileSize, 0, y * tileSize) - bounds;

        tileObject.layer = LayerMask.NameToLayer("Tile");
        //tileObject.AddComponent<BoxCollider>(); 

        return tileObject;
    }

  
    #endregion

    #region  Generating -- ChessPieces

    private void GenerateAllPieces()
    {

        chessPieces = new ChessPiece[TILE_COUNT_X, TILE_COUNT_Y];

        chessPieces[0, 0] = GenerateSinglePiece(ChessPieceType.ROOK, ChessTeam.WHITE);
        chessPieces[1, 0] = GenerateSinglePiece(ChessPieceType.KNIGHT, ChessTeam.WHITE);
        chessPieces[2, 0] = GenerateSinglePiece(ChessPieceType.BISHOP, ChessTeam.WHITE);
        chessPieces[3, 0] = GenerateSinglePiece(ChessPieceType.QUEEN, ChessTeam.WHITE);
        chessPieces[4, 0] = GenerateSinglePiece(ChessPieceType.KING, ChessTeam.WHITE);
        chessPieces[5, 0] = GenerateSinglePiece(ChessPieceType.BISHOP, ChessTeam.WHITE);
        chessPieces[6, 0] = GenerateSinglePiece(ChessPieceType.KNIGHT, ChessTeam.WHITE);
        chessPieces[7, 0] = GenerateSinglePiece(ChessPieceType.ROOK, ChessTeam.WHITE);

        for (int i = 0; i < TILE_COUNT_X; i++)
            chessPieces[i, 1] = GenerateSinglePiece(ChessPieceType.PAWN, ChessTeam.WHITE);


        chessPieces[0, 7] = GenerateSinglePiece(ChessPieceType.ROOK, ChessTeam.BLACK);
        chessPieces[1, 7] = GenerateSinglePiece(ChessPieceType.KNIGHT, ChessTeam.BLACK);
        chessPieces[2, 7] = GenerateSinglePiece(ChessPieceType.BISHOP, ChessTeam.BLACK);
        chessPieces[3, 7] = GenerateSinglePiece(ChessPieceType.QUEEN, ChessTeam.BLACK);
        chessPieces[4, 7] = GenerateSinglePiece(ChessPieceType.KING, ChessTeam.BLACK);
        chessPieces[5, 7] = GenerateSinglePiece(ChessPieceType.BISHOP, ChessTeam.BLACK);
        chessPieces[6, 7] = GenerateSinglePiece(ChessPieceType.KNIGHT, ChessTeam.BLACK);
        chessPieces[7, 7] = GenerateSinglePiece(ChessPieceType.ROOK, ChessTeam.BLACK);

        for (int i = 0; i < TILE_COUNT_X; i++)
            chessPieces[i, 6] = GenerateSinglePiece(ChessPieceType.PAWN, ChessTeam.BLACK);
    }

    private ChessPiece GenerateSinglePiece(ChessPieceType pieceType, ChessTeam team)
    {

        ChessPiece piece = Instantiate(piecePrefabs[(int)pieceType - 1], transform);

        piece.Type = pieceType;
        piece.Team = team;

        piece.GetComponent<MeshRenderer>().material = teamMaterials[(int)team];

        return piece;
    }

    #endregion

    #region  Positioning -- ChessPieces

    private void PositionAllPieces()
    {

        for (int x = 0; x < TILE_COUNT_X; x++)
            for (int y = 0; y < TILE_COUNT_Y; y++)
                if (chessPieces[x, y] != null)
                    PositionSinglePiece(x, y, true);
    }

    private void PositionSinglePiece(int x, int y, bool force = false)
    {
        chessPieces[x, y].currnet_X = x;
        chessPieces[x, y].currnet_Y = y;
        chessPieces[x, y].SetPosition(GetTileCenter(x, y), force);
    }

    private Vector3 GetTileCenter(int x, int y)
    {
        return new Vector3(x * tileSize, y_Offset, y * tileSize) - bounds;
    }

    #endregion

    #region  Highlighting -- ChessBoard

    private void HighlightTiles()
    {
        for (int i = 0; i < availableMoves_List.Count; i++)
        {
            tiles[availableMoves_List[i].x, availableMoves_List[i].y].layer = LayerMask.NameToLayer("Highlight");
        }
    }
    private void RemoveHighlightTiles()
    {
        for (int i = 0; i < availableMoves_List.Count; i++)
        {
            tiles[availableMoves_List[i].x, availableMoves_List[i].y].layer = LayerMask.NameToLayer("Tile");
        }
        availableMoves_List.Clear();
    }
    #endregion

    #region  Operations -- ChessPiece

    private void OnMouseRelease(Vector2Int hitTileIndex)
    {
        if (currentPiece != null && Input.GetMouseButtonUp(0))
        {
            Vector2Int previousPos = new Vector2Int(currentPiece.currnet_X, currentPiece.currnet_Y);

            bool validMove = MoveTo(currentPiece, previousPos, hitTileIndex);
            if (!validMove)
                currentPiece.SetPosition(GetTileCenter(previousPos.x, previousPos.y));

            currentPiece = null;
            RemoveHighlightTiles();
        }
    }

    private void OnMouseDragging(Vector2Int hitTileIndex)
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (chessPieces[hitTileIndex.x, hitTileIndex.y] != null)
            {
                if ((chessPieces[hitTileIndex.x, hitTileIndex.y].Team == ChessTeam.WHITE && isWhiteTurn) || (chessPieces[hitTileIndex.x, hitTileIndex.y].Team == ChessTeam.BLACK && !isWhiteTurn))
                {
                    currentPiece = chessPieces[hitTileIndex.x, hitTileIndex.y];

                    // Get a list for available Moves to Highlight
                    availableMoves_List = currentPiece.GetAvailableMoves(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y);
                    // Get the list of Special Moves to Highlight
                    specialMove = currentPiece.GetSpecialMoves(ref chessPieces, ref moves_List, ref availableMoves_List);
                    HighlightTiles();
                }
            }
        }

    }

    private Vector2Int LookUpIndex(GameObject hitInfo)
    {

        for (int x = 0; x < TILE_COUNT_X; x++)
            for (int y = 0; y < TILE_COUNT_Y; y++)
                if (tiles[x, y] == hitInfo)
                    return new Vector2Int(x, y);


        return -Vector2Int.one;

    }
   
    private bool MoveTo(ChessPiece piece, Vector2Int previousPos, Vector2Int targetPos)
    {
        if (!ContainsValidMoves(ref availableMoves_List, targetPos))
            return false;

        if (chessPieces[targetPos.x, targetPos.y] != null)
        {
            ChessPiece otherPiece = chessPieces[targetPos.x, targetPos.y];

            if (otherPiece.Team == piece.Team)
                return false;

            PieceDeath(otherPiece);
        }

        chessPieces[targetPos.x, targetPos.y] = piece;
        chessPieces[previousPos.x, previousPos.y] = null;

        PositionSinglePiece(targetPos.x, targetPos.y);

        moves_List.Add(new Vector2Int[] { previousPos, targetPos });
        isWhiteTurn = !isWhiteTurn;

        ProcessSpecialMoves();

        return true;
    }

    private void PieceDeath(ChessPiece otherPiece)
    {
        if (otherPiece.Team == ChessTeam.WHITE)
        {

            if (otherPiece.Type == ChessPieceType.KING)
                CheckMate(ChessTeam.BLACK);

            deadWhites_List.Add(otherPiece);
            otherPiece.SetScale(deathSize);
            otherPiece.SetPosition(new Vector3(7.75f * tileSize, death_yOffest, -1.3f * tileSize) - bounds + (Vector3.forward * deathSpacing) * deadWhites_List.Count);
        }
        else
        {

            if (otherPiece.Type == ChessPieceType.KING)
                CheckMate(ChessTeam.WHITE);

            deadBlacks_List.Add(otherPiece);
            otherPiece.SetScale(deathSize);
            otherPiece.SetPosition(new Vector3(-0.75f * tileSize, death_yOffest, 8.3f * tileSize) - bounds + (Vector3.back * deathSpacing) * deadBlacks_List.Count);
        }
    }

    private bool ContainsValidMoves(ref List<Vector2Int> moves, Vector2 pos)
    {
        for (int i = 0; i < moves.Count; i++)
            if (moves[i].x == pos.x && moves[i].y == pos.y)
                return true;

        return false;
    }

    private void BoardCleanUp()
    {

        currentPiece = null;
        availableMoves_List.Clear();
        moves_List.Clear();

        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {
                if (chessPieces[x, y] != null)
                    Destroy(chessPieces[x, y].gameObject);

                chessPieces[x, y] = null;
            }
        }

        for (int i = 0; i < deadBlacks_List.Count; i++)
            Destroy(deadBlacks_List[i].gameObject);
        for (int i = 0; i < deadWhites_List.Count; i++)
            Destroy(deadWhites_List[i].gameObject);

        deadBlacks_List.Clear();
        deadWhites_List.Clear();

        ResetBoard();
    }

    private void ResetBoard()
    {
        GenerateAllPieces();
        PositionAllPieces();
        isWhiteTurn = true;
    }

    #endregion

    #region  SpecialMoves

    private void ProcessSpecialMoves()
    {

        if(specialMove == SpecialMoves.ENPASSANT)
        {

            var newMove = moves_List[moves_List.Count - 1];
            ChessPiece currentPawn = chessPieces[newMove[1].x, newMove[1].y];
            var targetMove = moves_List[moves_List.Count - 2];
            ChessPiece otherPawn = chessPieces[targetMove[1].x, targetMove[1].y];

            if (currentPawn.currnet_X == otherPawn.currnet_X && Mathf.Abs(currentPawn.currnet_Y - otherPawn.currnet_Y) == 1)
                PieceDeath(otherPawn);
        }


        if( specialMove == SpecialMoves.CASTLING){

            Vector2Int[] lastMove = moves_List[moves_List.Count -1];

            //Left Rook
            if(lastMove[1].x  == 2){

                if(lastMove[1].y == 0){      // White Team
                    ChessPiece rook  = chessPieces[0,0];
                    chessPieces[3,0] = rook;
                    PositionSinglePiece(3,0);
                    chessPieces[0,0] = null;
                }
                else if(lastMove[1].y == 7){      // Blac7 Team
                    ChessPiece rook  = chessPieces[0,7];
                    chessPieces[3,7] = rook;
                    PositionSinglePiece(3,7);
                    chessPieces[0,7] = null;
                }
            }
            //Right Rook
            else  if(lastMove[1].x  == 6){

                if(lastMove[1].y == 0){      // White Team
                    ChessPiece rook  = chessPieces[7,0];
                    chessPieces[5,0] = rook;
                    PositionSinglePiece(5,0);
                    chessPieces[7,0] = null;
                }
                else if(lastMove[1].y == 7){      // Blac7 Team
                    ChessPiece rook  = chessPieces[7,7];
                    chessPieces[5,7] = rook;
                    PositionSinglePiece(5,7);
                    chessPieces[7,7] = null;
                }
            }

        }
    

        if( specialMove == SpecialMoves.PROMOTION){

            Vector2Int lastMove = moves_List[moves_List.Count - 1][1];
            ChessPiece targetPawn = chessPieces[lastMove.x, lastMove.y];

            if(targetPawn.Type == ChessPieceType.PAWN){

                if(targetPawn.Team == ChessTeam.WHITE && lastMove.y == 7){
                    
                    ChessPiece newQueen = GenerateSinglePiece(ChessPieceType.QUEEN, ChessTeam.WHITE);
                    newQueen.transform.position = chessPieces[lastMove.x, lastMove.y].transform.position;
                    Destroy(chessPieces[lastMove.x, lastMove.y].gameObject);
                    chessPieces[lastMove.x, lastMove.y] = newQueen;
                    PositionSinglePiece(lastMove.x, lastMove.y);
                }
                else if(targetPawn.Team == ChessTeam.BLACK && lastMove.y == 0){
                    
                    ChessPiece newQueen = GenerateSinglePiece(ChessPieceType.QUEEN, ChessTeam.BLACK);
                    newQueen.transform.position = chessPieces[lastMove.x, lastMove.y].transform.position;
                    Destroy(chessPieces[lastMove.x, lastMove.y].gameObject);
                    chessPieces[lastMove.x, lastMove.y] = newQueen;
                    PositionSinglePiece(lastMove.x, lastMove.y);
                }
            }
        }
    }

    #endregion

    #region  CheckMate

    private void CheckMate(ChessTeam team)
    {
        DisplayVictory(team);
    }

    private void DisplayVictory(ChessTeam team)
    {
        victoryScreen.SetActive(true);
        if (team == ChessTeam.WHITE)
            winningTitle.text = "White Wins !";
        else
            winningTitle.text = "Black Wins !";
    }

    public void OnResetButton()
    {
        victoryScreen.SetActive(false);

        //---- CleanUp
        BoardCleanUp();
    }

    public void OnExitButton() { Application.Quit(); }

    #endregion

}