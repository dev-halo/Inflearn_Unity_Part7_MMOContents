using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class CreatureController : MonoBehaviour
{
    public float speed = 5f;

    public Vector3Int CellPos { get; set; } = Vector3Int.zero;

    protected Animator animator;
    protected SpriteRenderer sprite;

    CreatureState state = CreatureState.Idle;
    public CreatureState State
    {
        get { return state; }
        set
        {
            if (state == value)
                return;

            state = value;
            UpdateAnimation();
        }
    }

    MoveDir lastDir = MoveDir.Down;
    MoveDir dir = MoveDir.Down;
    public MoveDir Dir
    {
        get { return dir; }
        set
        {
            if (dir == value)
                return;

            dir = value;
            if (value != MoveDir.None)
                lastDir = value;

            UpdateAnimation();
        }
    }

    protected virtual void UpdateAnimation()
    {
        if (state == CreatureState.Idle)
        {
            switch (lastDir)
            {
                case MoveDir.None:
                    break;
                case MoveDir.Up:
                    animator.Play("IDLE_BACK");
                    sprite.flipX = false;
                    break;
                case MoveDir.Down:
                    animator.Play("IDLE_FRONT");
                    sprite.flipX = false;
                    break;
                case MoveDir.Left:
                    animator.Play("IDLE_RIGHT");
                    sprite.flipX = true;
                    break;
                case MoveDir.Right:
                    animator.Play("IDLE_RIGHT");
                    sprite.flipX = false;
                    break;
                default:
                    break;
            }
        }
        else if (state == CreatureState.Moving)
        {
            switch (dir)
            {
                case MoveDir.None:

                    break;
                case MoveDir.Up:
                    animator.Play("WALK_BACK");
                    sprite.flipX = false;
                    break;
                case MoveDir.Down:
                    animator.Play("WALK_FRONT");
                    sprite.flipX = false;
                    break;
                case MoveDir.Left:
                    animator.Play("WALK_RIGHT");
                    sprite.flipX = true;
                    break;
                case MoveDir.Right:
                    animator.Play("WALK_RIGHT");
                    sprite.flipX = false;
                    break;
                default:
                    break;
            }
        }
        else if (state == CreatureState.Skill)
        {
        }
        else
        {
        }
    }

    void Start()
    {
        Init();
    }

    void Update()
    {
        UpdateController();
    }

    protected virtual void Init()
    {
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        Vector3 pos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        transform.position = pos;
    }

    protected virtual void UpdateController()
    {
        UpdatePosition();
        UpdateIsMoving();
    }

    void UpdatePosition()
    {
        if (State != CreatureState.Moving)
            return;

        Vector3 destPos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        Vector3 moveDir = destPos - transform.position;

        float dist = moveDir.magnitude;
        if (dist < speed * Time.deltaTime)
        {
            transform.position = destPos;
            state = CreatureState.Idle;
            if (dir == MoveDir.None)
                UpdateAnimation();
        }
        else
        {
            transform.position += moveDir.normalized * speed * Time.deltaTime;
            State = CreatureState.Moving;
        }
    }

    void UpdateIsMoving()
    {
        if (State == CreatureState.Idle && dir != MoveDir.None)
        {
            Vector3Int destPos = CellPos;

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

            State = CreatureState.Moving;

            if (Managers.Map.CanGo(destPos))
            {
                if (Managers.Object.Find(destPos) == null)
                {
                    CellPos = destPos;
                }
            }
        }
    }
}
