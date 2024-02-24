using System;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ChessBoard : MonoBehaviour
{
    private const int TILE_COUNT_X = 8;
    private const int TILE_COUNT_Y = 8;

    [Header("Prefabs && Materials")]

    [SerializeField] private GameObject whiteTile;
    [SerializeField] private GameObject blackTile;

    [SerializeField] private ChessPiece[] piecePrefabs;
    [SerializeField] private Material[] teamMaterials;

    [Header("UI")]
    [SerializeField] private GameObject victoryScreen;
    [SerializeField] private GameObject rematchIndicator;
    private Text winningTitle;
    private Text rematchText;


    [Header("Variables")]
    [SerializeField] private float y_Offset = 0, tileSize = 1, dragOffset = 1f;
    [SerializeField] private float deathSize = 0.5f, deathSpacing = 0.3f, death_yOffest = 0.7f;

    //Logic

    private ChessPiece[,] chessPieces;
    private GameObject[,] tiles;
    private List<Vector2Int[]> moves_List = new();
    private List<Vector2Int> availableMoves_List = new();
    private List<ChessPiece> deadWhites_List = new();
    private List<ChessPiece> deadBlacks_List = new();
    private Vector2Int currentHover;
    private GameState currentGameState;
    private ChessPiece currentPiece;
    private SpecialMoves specialMove;
    private Vector3 bounds;
    private Vector3 boardCenter;
    private bool isWhiteTurn;

    // -- Multiplayer Logic
    private int playerCount = 0;
    private ChessTeam currentTeam = ChessTeam.NONE;
    private bool isLocalGame = true;
    private bool[] playerRematch = new bool[2];



    private void Start()
    {

        victoryScreen.SetActive(false);
        rematchIndicator.SetActive(false);
        winningTitle = victoryScreen.transform.GetChild(0).GetComponent<Text>();
        rematchText = rematchIndicator.transform.GetChild(0).GetComponent<Text>();

        isWhiteTurn = true;

        GenerateTiles(tileSize, TILE_COUNT_X, TILE_COUNT_Y);
        GenerateAllPieces();
        PositionAllPieces();

        RegisterEvents();

    }

    private void Update()
    {
        if (currentGameState == GameState.PLAY)
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

    private Vector2Int LookUpIndex(GameObject hitInfo)
    {

        for (int x = 0; x < TILE_COUNT_X; x++)
            for (int y = 0; y < TILE_COUNT_Y; y++)
                if (tiles[x, y] == hitInfo)
                    return new Vector2Int(x, y);


        return -Vector2Int.one;

    }

    private void OnMouseDragging(Vector2Int hitTileIndex)
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (chessPieces[hitTileIndex.x, hitTileIndex.y] != null)
            {
                if ((chessPieces[hitTileIndex.x, hitTileIndex.y].Team == ChessTeam.WHITE && isWhiteTurn && currentTeam == ChessTeam.WHITE) || (chessPieces[hitTileIndex.x, hitTileIndex.y].Team == ChessTeam.BLACK && !isWhiteTurn && currentTeam == ChessTeam.BLACK))
                {
                    currentPiece = chessPieces[hitTileIndex.x, hitTileIndex.y];

                    // Get a list for available Moves to Highlight
                    availableMoves_List = currentPiece.GetAvailableMoves(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y);
                    // Get the list of Special Moves to Highlight
                    specialMove = currentPiece.GetSpecialMoves(ref chessPieces, ref moves_List, ref availableMoves_List);

                    PreventCheck();

                    HighlightTiles();
                }
            }
        }

    }

    private void OnMouseRelease(Vector2Int hitTileIndex)
    {
        if (currentPiece != null && Input.GetMouseButtonUp(0))
        {
            Vector2Int previousPos = new Vector2Int(currentPiece.currnet_X, currentPiece.currnet_Y);

            if (ContainsValidMoves(ref availableMoves_List, hitTileIndex))
            {
                MoveTo(previousPos, hitTileIndex);

                // -- Net Implementation
                NetMakeMove move = new();
                move.originalX = previousPos.x;
                move.originalY = previousPos.y;
                move.destinationX = hitTileIndex.x;
                move.destinationY = hitTileIndex.y;
                move.teamID = (int)currentTeam;
                Client.Instance.SendToServer(move);

            }
            else
            {
                currentPiece.SetPosition(GetTileCenter(previousPos.x, previousPos.y));
                currentPiece = null;
                RemoveHighlightTiles();
            }
            
        }
    }

    private void MoveTo(Vector2Int originalPos, Vector2Int targetPos)
    {
        ChessPiece piece = chessPieces[originalPos.x, originalPos.y];

        if (chessPieces[targetPos.x, targetPos.y] != null)
        {
            ChessPiece otherPiece = chessPieces[targetPos.x, targetPos.y];

            if (otherPiece.Team == piece.Team)
                return;

            PieceDeath(otherPiece);
        }

        chessPieces[targetPos.x, targetPos.y] = piece;
        chessPieces[originalPos.x, originalPos.y] = null;

        PositionSinglePiece(targetPos.x, targetPos.y);

        moves_List.Add(new Vector2Int[] { originalPos, targetPos });

        isWhiteTurn = !isWhiteTurn;

        if (isLocalGame)
            currentTeam = (currentTeam == ChessTeam.WHITE) ? ChessTeam.BLACK : ChessTeam.WHITE;

        ProcessSpecialMoves();

        switch (CheckForCheckMate())
        {
            default:
                break;
            case 1:
                CheckMate(piece.Team);
                break;
            case 2:
                CheckMate(ChessTeam.NONE);
                break;
        }

        if(currentPiece != null)
            currentPiece = null;
        RemoveHighlightTiles();

        return;
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

        playerRematch[0] = playerRematch[1] = false;
        isWhiteTurn = true;
        currentGameState = GameState.PLAY;
    }

    #endregion

    #region  SpecialMoves 

    private void ProcessSpecialMoves()
    {

        if (specialMove == SpecialMoves.ENPASSANT)
        {

            var newMove = moves_List[moves_List.Count - 1];
            ChessPiece currentPawn = chessPieces[newMove[1].x, newMove[1].y];
            var targetMove = moves_List[moves_List.Count - 2];
            ChessPiece otherPawn = chessPieces[targetMove[1].x, targetMove[1].y];

            if (currentPawn.currnet_X == otherPawn.currnet_X && Mathf.Abs(currentPawn.currnet_Y - otherPawn.currnet_Y) == 1)
                PieceDeath(otherPawn);
        }


        if (specialMove == SpecialMoves.CASTLING)
        {

            Vector2Int[] lastMove = moves_List[moves_List.Count - 1];

            //Left Rook
            if (lastMove[1].x == 2)
            {

                if (lastMove[1].y == 0)
                {      // White Team
                    ChessPiece rook = chessPieces[0, 0];
                    chessPieces[3, 0] = rook;
                    PositionSinglePiece(3, 0);
                    chessPieces[0, 0] = null;
                }
                else if (lastMove[1].y == 7)
                {      // Blac7 Team
                    ChessPiece rook = chessPieces[0, 7];
                    chessPieces[3, 7] = rook;
                    PositionSinglePiece(3, 7);
                    chessPieces[0, 7] = null;
                }
            }
            //Right Rook
            else if (lastMove[1].x == 6)
            {

                if (lastMove[1].y == 0)
                {      // White Team
                    ChessPiece rook = chessPieces[7, 0];
                    chessPieces[5, 0] = rook;
                    PositionSinglePiece(5, 0);
                    chessPieces[7, 0] = null;
                }
                else if (lastMove[1].y == 7)
                {      // Blac7 Team
                    ChessPiece rook = chessPieces[7, 7];
                    chessPieces[5, 7] = rook;
                    PositionSinglePiece(5, 7);
                    chessPieces[7, 7] = null;
                }
            }

        }


        if (specialMove == SpecialMoves.PROMOTION)
        {

            Vector2Int lastMove = moves_List[moves_List.Count - 1][1];
            ChessPiece targetPawn = chessPieces[lastMove.x, lastMove.y];

            if (targetPawn.Type == ChessPieceType.PAWN)
            {

                if (targetPawn.Team == ChessTeam.WHITE && lastMove.y == 7)
                {

                    ChessPiece newQueen = GenerateSinglePiece(ChessPieceType.QUEEN, ChessTeam.WHITE);
                    newQueen.transform.position = chessPieces[lastMove.x, lastMove.y].transform.position;
                    Destroy(chessPieces[lastMove.x, lastMove.y].gameObject);
                    chessPieces[lastMove.x, lastMove.y] = newQueen;
                    PositionSinglePiece(lastMove.x, lastMove.y);
                }
                else if (targetPawn.Team == ChessTeam.BLACK && lastMove.y == 0)
                {

                    ChessPiece newQueen = GenerateSinglePiece(ChessPieceType.QUEEN, ChessTeam.BLACK);
                    newQueen.transform.position = chessPieces[lastMove.x, lastMove.y].transform.position;
                    Destroy(chessPieces[lastMove.x, lastMove.y].gameObject);
                    chessPieces[lastMove.x, lastMove.y] = newQueen;
                    PositionSinglePiece(lastMove.x, lastMove.y);
                }
            }
        }
    }

    private void PreventCheck()
    {

        ChessPiece targetKing = null;

        for (int x = 0; x < TILE_COUNT_X; x++)
            for (int y = 0; y < TILE_COUNT_Y; y++)
                if (chessPieces[x, y] != null)
                    if (chessPieces[x, y].Type == ChessPieceType.KING && chessPieces[x, y].Team == currentPiece.Team)
                        targetKing = chessPieces[x, y];

        SimulateMoveForSinglePiece(currentPiece, ref availableMoves_List, targetKing);

    }

    private void SimulateMoveForSinglePiece(ChessPiece currentPiece, ref List<Vector2Int> moves, ChessPiece targetKing)
    {

        // -- Save the current values to reset
        int actual_X = currentPiece.currnet_X;
        int actual_Y = currentPiece.currnet_Y;
        List<Vector2Int> movesToRemove = new();

        // -- Simulating the moves and checking
        for (int i = 0; i < moves.Count; i++)
        {
            int simX = moves[i].x;
            int simY = moves[i].y;

            Vector2Int kingPositionSim = new Vector2Int(targetKing.currnet_X, targetKing.currnet_Y);

            // -- Did the simulation for king's move
            if (currentPiece.Type == ChessPieceType.KING)
                kingPositionSim = new Vector2Int(simX, simY);

            // -- Copy the [,] not ref
            ChessPiece[,] simulation = new ChessPiece[TILE_COUNT_X, TILE_COUNT_Y];
            List<ChessPiece> simAttackingPieces = new();

            for (int x = 0; x < TILE_COUNT_X; x++)
            {
                for (int y = 0; y < TILE_COUNT_Y; y++)
                {
                    if (chessPieces[x, y] != null)
                    {
                        simulation[x, y] = chessPieces[x, y];
                        if (simulation[x, y].Team != currentPiece.Team)
                            simAttackingPieces.Add(simulation[x, y]);
                    }
                }
            }

            // -- Simulate that move
            simulation[actual_X, actual_Y] = null;
            currentPiece.currnet_X = simX;
            currentPiece.currnet_Y = simY;
            simulation[simX, simY] = currentPiece;

            // -- Did any piece was taken down in simulation
            var deadPiece = simAttackingPieces.Find(d => d.currnet_X == simX && d.currnet_Y == simY);
            if (deadPiece != null)
                simAttackingPieces.Remove(deadPiece);

            // -- Get all simulated attacking Piece
            List<Vector2Int> simMoves = new();
            for (int m = 0; m < simAttackingPieces.Count; m++)
            {

                var pieceMoves = simAttackingPieces[m].GetAvailableMoves(ref simulation, TILE_COUNT_X, TILE_COUNT_Y);
                for (int n = 0; n < pieceMoves.Count; n++)
                    simMoves.Add(pieceMoves[n]);
            }

            // -- Remove the move where the king is in trouble
            if (ContainsValidMoves(ref simMoves, kingPositionSim))
                movesToRemove.Add(moves[i]);


            // -- Restore actual piece data
            currentPiece.currnet_X = actual_X;
            currentPiece.currnet_Y = actual_Y;

        }

        //Remove from the current avialable move list
        for (int i = 0; i < movesToRemove.Count; i++)
            moves.Remove(movesToRemove[i]);

    }

    private int CheckForCheckMate()
    {

        var lastMove = moves_List[moves_List.Count - 1];
        ChessTeam targetTeam = (chessPieces[lastMove[1].x, lastMove[1].y].Team == ChessTeam.WHITE) ? ChessTeam.BLACK : ChessTeam.WHITE;

        List<ChessPiece> attackingPieces = new();
        List<ChessPiece> defendingPieces = new();
        ChessPiece targetKing = null;

        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {
                if (chessPieces[x, y] != null)
                {
                    if (chessPieces[x, y].Team == targetTeam)
                    {
                        defendingPieces.Add(chessPieces[x, y]);
                        if (chessPieces[x, y].Type == ChessPieceType.KING)
                            targetKing = chessPieces[x, y];
                    }
                    else
                        attackingPieces.Add(chessPieces[x, y]);
                }
            }
        }

        // -- Is King attacked right now
        List<Vector2Int> currentAvailableMove = new();
        for (int i = 0; i < attackingPieces.Count; i++)
        {
            var pieceMoves = attackingPieces[i].GetAvailableMoves(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y);
            for (int b = 0; b < pieceMoves.Count; b++)
                currentAvailableMove.Add(pieceMoves[b]);
        }

        // -- Are we in check
        if (ContainsValidMoves(ref currentAvailableMove, new Vector2Int(targetKing.currnet_X, targetKing.currnet_Y)))
        {

            // -- King is under attack
            for (int i = 0; i < defendingPieces.Count; i++)
            {

                List<Vector2Int> defendingMoves = defendingPieces[i].GetAvailableMoves(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y);
                SimulateMoveForSinglePiece(defendingPieces[i], ref defendingMoves, targetKing);

                if (defendingMoves.Count != 0)
                    return 0;

            }
            return 1;
        }
        else   // -- stalemate Condition
        {
            for (int i = 0; i < defendingPieces.Count; i++)
            {
                List<Vector2Int> defendingMoves = defendingPieces[i].GetAvailableMoves(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y);
                SimulateMoveForSinglePiece(defendingPieces[i], ref defendingMoves, targetKing);
                if (defendingMoves.Count != 0)
                    return 0;
            }
            return 2; //staleMate Exit
        }
    }


    #endregion

    #region  CheckMate

    private void CheckMate(ChessTeam team)
    {
        DisplayVictory(team);
        currentGameState = GameState.PAUSE;
    }

    private void DisplayVictory(ChessTeam team)
    {
        victoryScreen.SetActive(true);

        switch (team)
        {
            case ChessTeam.NONE:
                winningTitle.text = "StaleMate !";
                break;
            case ChessTeam.WHITE:
                winningTitle.text = "White Wins !";
                break;
            case ChessTeam.BLACK:
                winningTitle.text = "Black Wins !";
                break;
        }
    }

    public void OnResetButton()
    {
        victoryScreen.SetActive(false);
        rematchIndicator.SetActive(false);
        //---- CleanUp
        BoardCleanUp();
    }

    public void OnExitButton() { Application.Quit(); }

    public void OnRematchButton()
    {
        if (isLocalGame)
        {
            NetRematch wRematch = new NetRematch();
            wRematch.teamID = (int)ChessTeam.WHITE;
            wRematch.wantRematch = 1;
            Client.Instance.SendToServer(wRematch);

            NetRematch bRematch = new NetRematch();
            bRematch.teamID = (int)ChessTeam.BLACK;
            bRematch.wantRematch = 1;
            Client.Instance.SendToServer(bRematch);
        }
        else
        {
            NetRematch rematch = new NetRematch();
            rematch.teamID = (int)currentTeam;
            rematch.wantRematch = 1;
            Client.Instance.SendToServer(rematch);
        }

    }

    public void OnMenuButton()
    {
        NetRematch rematch = new NetRematch();
        rematch.teamID = (int)currentTeam;
        rematch.wantRematch = 0;
        Client.Instance.SendToServer(rematch);

        OnResetButton();  // -- Reset the Board
        UIService.Instance.OnLeaveFromGameButton();

        Invoke(nameof(ShutDownRelay) , 1f);

        // -- Reset values
        playerCount = 0;
        currentTeam = ChessTeam.NONE;

    }


    #endregion

    #region  Events

    private void RegisterEvents()
    {
        NetUtility.S_WELCOME += OnWelcomeServer;
        NetUtility.S_MAKE_MOVE += OnMakeMoveServer;
        NetUtility.S_REMATCH += OnRematchServer;

        NetUtility.C_WELCOME += OnWelcomeClient;
        NetUtility.C_START_GAME += OnStartGame_Client;
        NetUtility.C_MAKE_MOVE += OnMakeMoveClient;
        NetUtility.C_REMATCH += OnRematchClient;

        UIService.Instance.SetLocalGame += OnSetLocalGame;
    }


    private void UnregisterEvents()
    {
        NetUtility.S_WELCOME -= OnWelcomeServer;
        NetUtility.S_MAKE_MOVE -= OnMakeMoveServer;
        NetUtility.S_REMATCH -= OnRematchServer;

        NetUtility.C_WELCOME -= OnWelcomeClient;
        NetUtility.C_START_GAME -= OnStartGame_Client;
        NetUtility.C_MAKE_MOVE -= OnMakeMoveClient;
        NetUtility.C_REMATCH -= OnRematchClient;

        UIService.Instance.SetLocalGame -= OnSetLocalGame;
    }


    private void OnSetLocalGame(bool c)
    {
        playerCount = 0;
        currentTeam = ChessTeam.NONE;
        isLocalGame = c;
    }

    private void OnWelcomeServer(NetMessage message, NetworkConnection connection)
    {
        // -- Client is connected, assign a team and return the message back to it
        NetWelcome welcome = (NetWelcome)message;

        // -- Assign a Team
        welcome.AssignedTeam = ++playerCount;

        // -- Return back 
        Server.Instance.SendToClient(connection, welcome);

        // -- If full, start the game
        if (playerCount == 2)
        {
            Server.Instance.Broadcast(new NetStartGame());
        }
    }
    
    private void OnMakeMoveServer(NetMessage message, NetworkConnection connection)
    {
        // -- Recieve message
        NetMakeMove move = message as NetMakeMove;

        // -- Can make Validation Check

        // -- Broadcat message
        Server.Instance.Broadcast(move);
    }

    private void OnRematchServer(NetMessage message, NetworkConnection connection)
    {
        Server.Instance.Broadcast(message);
    }


    private void OnWelcomeClient(NetMessage message)
    {
        // -- Recieve the connection message
        NetWelcome welcome = (NetWelcome)message;

        // -- Assign the team
        currentTeam = (ChessTeam)welcome.AssignedTeam;

        Debug.Log($" My assigned team is {welcome.AssignedTeam}");

        if (isLocalGame && currentTeam == ChessTeam.WHITE)
            Server.Instance.Broadcast(new NetStartGame());

    }

    private void OnStartGame_Client(NetMessage message)
    {
        // -- Set the Camera
        UIService.Instance.ChangeCamera(currentTeam);
    }

    private void OnMakeMoveClient(NetMessage message)
    {
        // -- Recieve message
        NetMakeMove move = message as NetMakeMove;

        Debug.Log($" Move : {(ChessTeam)move.teamID} : {move.originalX} , {move.originalY}  ->  {move.destinationX} , {move.destinationY} ");

        if (move.teamID != (int)currentTeam)
        {
            ChessPiece target = chessPieces[move.originalX, move.originalY];

            availableMoves_List = target.GetAvailableMoves(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y);
            specialMove = target.GetSpecialMoves(ref chessPieces, ref moves_List, ref availableMoves_List);

            Vector2Int originalPos = new Vector2Int(move.originalX, move.originalY);
            Vector2Int targetPos = new Vector2Int(move.destinationX, move.destinationY);

            MoveTo(originalPos, targetPos);

        }
    }

    private void OnRematchClient(NetMessage message)
    {
        // -- Recieve message
        NetRematch rematch = message as NetRematch;

        // -- Set the Bool for rematch
        playerRematch[rematch.teamID - 1] = rematch.wantRematch == 1;

        // -- Activate the piece of Ui
        if (rematch.teamID != (int)currentTeam)
        {
            rematchIndicator.SetActive(true);

            if (rematch.wantRematch == 1)
            {
                switch ((ChessTeam)rematch.teamID)
                {
                    case ChessTeam.WHITE:
                        rematchText.text = "White Team wants Rematch!";

                        break;
                    case ChessTeam.BLACK:
                        rematchText.text = "Black Team wants Rematch!";
                        break;
                }
                rematchText.color = Color.green;
            }
            else if(rematch.wantRematch == 0)
            {
                switch ((ChessTeam)rematch.teamID)
                {
                    case ChessTeam.WHITE:
                        rematchText.text = "White Team left the match!";

                        break;
                    case ChessTeam.BLACK:
                        rematchText.text = "Black Team left the match!";
                        break;
                }
                rematchText.color = Color.red;
            }
        }

        // -- Both team wants rematch
        if( playerRematch[0] &&  playerRematch[1])
            OnResetButton();

    }

    private void ShutDownRelay()
    {
        Client.Instance.ShutDown();
        Server.Instance.ShutDown();
    }

    #endregion

}