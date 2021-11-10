using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PlayerController : CreatureController
{
    Coroutine coSkill;

    protected override void Init()
    {
        base.Init();
    }

    protected override void UpdateController()
    {
        switch (State)
        {
            case CreatureState.Idle:
                GetDirInput();
                GetIdleInput();
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

    void GetIdleInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            State = CreatureState.Skill;
            coSkill = StartCoroutine(CoStartPunch());
        }
    }

    IEnumerator CoStartPunch()
    {
        GameObject go = Managers.Object.Find(GetFrontCellPos());
        if (go != null)
        {
            Debug.Log(go.name);
        }

        yield return new WaitForSeconds(0.5f);
        State = CreatureState.Idle;
        coSkill = null;
    }
}
