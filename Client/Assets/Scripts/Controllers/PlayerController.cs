using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PlayerController : MonoBehaviour
{
    public Grid grid;
    public float speed = 5f;

    Vector3Int cellPos = Vector3Int.zero;
    MoveDir dir = MoveDir.None;
    bool isMoving = false;

    void Start()
    {
        Vector3 pos = grid.CellToWorld(cellPos) + new Vector3(0.5f, 0.5f);
        transform.position = pos;
    }

    void Update()
    {
        GetDirInput();
        UpdatePosition();
        UpdateIsMoving();
    }

    void GetDirInput()
    {
        if (Input.GetKey(KeyCode.W))
        {
            dir = MoveDir.Up;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            dir = MoveDir.Down;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            dir = MoveDir.Left;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            dir = MoveDir.Right;
        }
        else
        {
            dir = MoveDir.None;
        }
    }

    void UpdatePosition()
    {
        if (isMoving == false)
            return;

        Vector3 destPos = grid.CellToWorld(cellPos) + new Vector3(0.5f, 0.5f);
        Vector3 moveDir = destPos - transform.position;

        float dist = moveDir.magnitude;
        if (dist < speed * Time.deltaTime)
        {
            transform.position = destPos;
            isMoving = false;
        }
        else
        {
            transform.position += moveDir.normalized * speed * Time.deltaTime;
            isMoving = true;
        }
    }

    void UpdateIsMoving()
    {
        if (isMoving == false)
        {
            switch (dir)
            {
                case MoveDir.Up:
                    cellPos += Vector3Int.up;
                    isMoving = true;
                    break;
                case MoveDir.Down:
                    cellPos += Vector3Int.down;
                    isMoving = true;
                    break;
                case MoveDir.Left:
                    cellPos += Vector3Int.left;
                    isMoving = true;
                    break;
                case MoveDir.Right:
                    cellPos += Vector3Int.right;
                    isMoving = true;
                    break;
                default:
                    break;
            }
        }
    }
}
