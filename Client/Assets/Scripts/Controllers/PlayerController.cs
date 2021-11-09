using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;

    Vector3Int cellPos = Vector3Int.zero;
    bool isMoving = false;
    Animator animator;

    MoveDir dir = MoveDir.Down;
    public MoveDir Dir
    {
        get { return dir; }
        set
        {
            if (dir == value)
                return;

            switch (value)
            {
                case MoveDir.None:
                    if (dir == MoveDir.Up)
                    {
                        animator.Play("IDLE_BACK");
                        transform.localScale = Vector3.one;
                    }
                    else if (dir == MoveDir.Down)
                    {
                        animator.Play("IDLE_FRONT");
                        transform.localScale = Vector3.one;
                    }
                    else if (dir == MoveDir.Left)
                    {
                        animator.Play("IDLE_RIGHT");
                        transform.localScale = new Vector3(-1f, 1f, 1f);
                    }
                    else
                    {
                        animator.Play("IDLE_RIGHT");
                        transform.localScale = Vector3.one;
                    }
                    break;
                case MoveDir.Up:
                    animator.Play("WALK_BACK");
                    transform.localScale = Vector3.one;
                    break;
                case MoveDir.Down:
                    animator.Play("WALK_FRONT");
                    transform.localScale = Vector3.one;
                    break;
                case MoveDir.Left:
                    animator.Play("WALK_RIGHT");
                    transform.localScale = new Vector3(-1f, 1f, 1f);
                    break;
                case MoveDir.Right:
                    animator.Play("WALK_RIGHT");
                    transform.localScale = Vector3.one;
                    break;
                default:
                    break;
            }

            dir = value;
        }
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        Vector3 pos = Managers.Map.CurrentGrid.CellToWorld(cellPos) + new Vector3(0.5f, 0.5f);
        transform.position = pos;
    }

    void Update()
    {
        GetDirInput();
        UpdatePosition();
        UpdateIsMoving();
    }

    void LateUpdate()
    {
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10f);
    }

    void GetDirInput()
    {
        if (Input.GetKey(KeyCode.W))
        {
            Dir = MoveDir.Up;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            Dir = MoveDir.Down;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            Dir = MoveDir.Left;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            Dir = MoveDir.Right;
        }
        else
        {
            Dir = MoveDir.None;
        }
    }

    void UpdatePosition()
    {
        if (isMoving == false)
            return;

        Vector3 destPos = Managers.Map.CurrentGrid.CellToWorld(cellPos) + new Vector3(0.5f, 0.5f);
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
        if (isMoving == false && dir != MoveDir.None)
        {
            Vector3Int destPos = cellPos;

            switch (dir)
            {
                case MoveDir.Up:
                    destPos += Vector3Int.up;
                    break;
                case MoveDir.Down:
                    destPos += Vector3Int.down;
                    break;
                case MoveDir.Left:
                    destPos += Vector3Int.left;
                    break;
                case MoveDir.Right:
                    destPos += Vector3Int.right;
                    break;
                default:
                    break;
            }

            if (Managers.Map.CanGo(destPos))
            {
                cellPos = destPos;
                isMoving = true;
            }
        }
    }
}
