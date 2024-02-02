using System.Collections.Generic;
using UnityEngine;

public class ChessPiece : MonoBehaviour {
    
    public ChessTeam Team;
    public ChessPieceType Type;

    public int currnet_X;
    public int currnet_Y;

    private  Vector3 targetPosition;
    private Vector3 desiredScale = Vector3.one;

    void Start()
    {
        transform.rotation = Quaternion.Euler((Team == ChessTeam.WHITE) ? new Vector3(0,-90,0) : new Vector3(0,90,0) );        
    }

    private void Update(){

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime*10f);
        transform.localScale = Vector3.Lerp(transform.localScale, desiredScale, Time.deltaTime*10f);
    }


    public virtual List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCount_X, int tileCount_Y){
        return null;
    }
    public virtual void SetPosition(Vector3 targetPos, bool force = false){
        targetPosition = targetPos;
        if(force)
            transform.position = targetPosition;

    }

    public virtual void SetScale(float newScale, bool force = false){
        desiredScale = Vector3.one * newScale;
        if(force) 
            transform.localScale = desiredScale;

    }
}