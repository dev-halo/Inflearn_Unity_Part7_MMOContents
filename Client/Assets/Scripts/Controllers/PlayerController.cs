using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PlayerController : CreatureController
{
    Coroutine coSkill;
    bool rangedSkill = false;

    protected override void Init()
    {
        base.Init();
    }

    protected override void UpdateAnimation()
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
            switch (lastDir)
            {
                case MoveDir.None:
                    break;
                case MoveDir.Up:
                    animator.Play(rangedSkill ? "ATTACK_WEAPON_BACK" : "ATTACK_BACK");
                    sprite.flipX = false;
                    break;
                case MoveDir.Down:
                    animator.Play(rangedSkill ? "ATTACK_WEAPON_FRONT" : "ATTACK_FRONT");
                    sprite.flipX = false;
                    break;
                case MoveDir.Left:
                    animator.Play(rangedSkill ? "ATTACK_WEAPON_RIGHT" : "ATTACK_RIGHT");
                    sprite.flipX = true;
                    break;
                case MoveDir.Right:
                    animator.Play(rangedSkill ? "ATTACK_WEAPON_RIGHT" : "ATTACK_RIGHT");
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

    protected override void UpdateController()
    {
        switch (State)
        {
            case CreatureState.Idle:
                GetDirInput();
                break;
            case CreatureState.Moving:
                GetDirInput();
                break;
            case CreatureState.Skill:
                break;
            case CreatureState.Dead:
                break;
            default:
                break;
        }

        base.UpdateController();
    }

    void LateUpdate()
    {
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10f);
    }

    protected override void UpdateIdle()
    {
        if (Dir != MoveDir.None)
        {
            State = CreatureState.Moving;
            return;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            State = CreatureState.Skill;
            //coSkill = StartCoroutine(CoStartPunch());
            coSkill = StartCoroutine(CoStartShootArrow());
        }
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

    IEnumerator CoStartPunch()
    {
        GameObject go = Managers.Object.Find(GetFrontCellPos());
        if (go != null)
        {
            CreatureController cc = go.GetComponent<CreatureController>();
            if (cc != null)
                cc.OnDamaged();
        }

        rangedSkill = false;
        yield return new WaitForSeconds(0.5f);
        State = CreatureState.Idle;
        coSkill = null;
    }

    IEnumerator CoStartShootArrow()
    {
        GameObject go = Managers.Resource.Instantiate("Creature/Arrow");
        ArrowController ac = go.GetComponent<ArrowController>();
        ac.Dir = lastDir;
        ac.CellPos = CellPos;

        rangedSkill = true;
        yield return new WaitForSeconds(0.3f);
        State = CreatureState.Idle;
        coSkill = null;
    }

    public override void OnDamaged()
    {
        Debug.Log("Player HIT !");
    }
}
