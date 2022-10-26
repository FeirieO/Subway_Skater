using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceSpawner : MonoBehaviour
{
    public PieceType type;
    private Piece currentPiece;

    public void Spawn()
    {
        int amtObjs = 0;
        switch (type)
        {
            case PieceType.jump:
                amtObjs = LevelManager.Instance.jumps.Count;
                break;
            case PieceType.slide:
                amtObjs = LevelManager.Instance.slides.Count;
                break;
            case PieceType.longblock:
                amtObjs = LevelManager.Instance.longblocks.Count;
                break;
            case PieceType.ramp:
                amtObjs = LevelManager.Instance.ramps.Count;
                break;
        }
        currentPiece = LevelManager.Instance.GetPiece(type, Random.Range(0, amtObjs));
        currentPiece.gameObject.SetActive(true);
        currentPiece.transform.SetParent(transform, false);
    }

    public void DeSpawn()
    {
        //currentPiece = Get a new Piece from the pool;
        currentPiece.gameObject.SetActive(false);
    }
}