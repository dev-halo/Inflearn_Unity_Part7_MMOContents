using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class CreatureController : MonoBehaviour
{
    HpBar hpBar;

    public int Id { get; set; }

    StatInfo stat = new StatInfo();
    public StatInfo Stat
    {
        get { return stat; }
        set
        {
            if (stat.Equals(value))
                return;

            stat.Hp = value.Hp;
            stat.MaxHp = value.MaxHp;
            stat.Speed = value.Speed;
            UpdateHpBar();
        }
    }

    public float Speed
    {
        get { return Stat.Speed; }
        set { Stat.Speed = value; }
    }

    public int Hp
    {
        get { return Stat.Hp; }
        set
        {
            Stat.Hp = value;
            UpdateHpBar();
        }
    }

    protected bool updated = false;

    PositionInfo positionInfo = new PositionInfo();
    public PositionInfo PosInfo
    {
        get { return positionInfo; }
        set
        {
            if (positionInfo.Equals(value))
                return;

            CellPos = new Vector3Int(value.PosX, value.PosY, 0);
            State = value.State;
            Dir = value.MoveDir;
        }
    }

    protected void AddHpBar()
    {
        GameObject go = Managers.Resource.Instantiate("UI/HpBar", transform);
        go.transform.localPosition = new Vector3(0f, 0.5f);
        go.name = "HpBar";
        hpBar = go.GetComponent<HpBar>();
        UpdateHpBar();
    }

    void UpdateHpBar()
    {
        if (hpBar == null)
            return;

        float ratio = 0f;
        if (Stat.MaxHp > 0)
            ratio = (float)Hp / Stat.MaxHp;

        hpBar.SetHpBar(ratio);
    }

    public void SyncPos()
    {
        Vector3 destPos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        transform.position = destPos;
    }

    public Vector3Int CellPos
    {
        get
        {
            return new Vector3Int(PosInfo.PosX, PosInfo.PosY, 0);
        }
        set
        {
            if (PosInfo.PosX == value.x && PosInfo.PosY == value.y)
                return;

            PosInfo.PosX = value.x;
            PosInfo.PosY = value.y;
            updated = true;
        }
    }

    protected Animator animator;
    protected SpriteRenderer sprite;

    public virtual CreatureState State
    {
        get { return PosInfo.State; }
        set
        {
            if (PosInfo.State == value)
                return;

            PosInfo.State = value;
            UpdateAnimation();
            updated = true;
        }
    }

    public MoveDir Dir
    {
        get { return PosInfo.MoveDir; }
        set
        {
            if (PosInfo.MoveDir == value)
                return;

            PosInfo.MoveDir = value;

            UpdateAnimation();
            updated = true;
        }
    }

    public MoveDir GetDirFromVec(Vector3Int dir)
    {
        if (dir.x > 0)
            return MoveDir.Right;
        else if (dir.x < 0)
            return MoveDir.Left;
        else if (dir.y > 0)
            return MoveDir.Up;
        else
            return MoveDir.Down;
    }

    public Vector3Int GetFrontCellPos()
    {
        Vector3Int cellPos = CellPos;

        switch (Dir)
        {
            case MoveDir.Up:
                cellPos += Vector3Int.up;
                break;
            case MoveDir.Down:
                cellPos += Vector3Int.down;
                break;
            case MoveDir.Left:
                cellPos += Vector3Int.left;
                break;
            case MoveDir.Right:
                cellPos += Vector3Int.right;
                break;
            default:
                break;
        }

        return cellPos;
    }

    protected virtual void UpdateAnimation()
    {
        if (State == CreatureState.Idle)
        {
            switch (Dir)
            {
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
        else if (State == CreatureState.Moving)
        {
            switch (Dir)
            {
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
        else if (State == CreatureState.Skill)
        {
            switch (Dir)
            {
                case MoveDir.Up:
                    animator.Play("ATTACK_BACK");
                    sprite.flipX = false;
                    break;
                case MoveDir.Down:
                    animator.Play("ATTACK_FRONT");
                    sprite.flipX = false;
                    break;
                case MoveDir.Left:
                    animator.Play("ATTACK_RIGHT");
                    sprite.flipX = true;
                    break;
                case MoveDir.Right:
                    animator.Play("ATTACK_RIGHT");
                    sprite.flipX = false;
                    break;
                default:
                    break;
            }
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

        State = CreatureState.Idle;
        Dir = MoveDir.Down;
        UpdateAnimation();
    }

    protected virtual void UpdateController()
    {
        switch (State)
        {
            case CreatureState.Idle:
                UpdateIdle();
                break;
            case CreatureState.Moving:
                UpdateMoving();
                break;
            case CreatureState.Skill:
                break;
            case CreatureState.Dead:
                break;
            default:
                break;
        }
    }

    protected virtual void UpdateIdle()
    {
    }

    protected virtual void UpdateMoving()
    {
        Vector3 destPos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        Vector3 moveDir = destPos - transform.position;

        float dist = moveDir.magnitude;
        if (dist < Speed * Time.deltaTime)
        {
            transform.position = destPos;
            MoveToNextPos();
        }
        else
        {
            transform.position += moveDir.normalized * Speed * Time.deltaTime;
            State = CreatureState.Moving;
        }
    }

    protected virtual void MoveToNextPos()
    {

    }

    protected virtual void UpdateSkill()
    {
    }

    protected virtual void UpdateDead()
    {
    }

    public virtual void OnDamaged()
    {

    }
}
