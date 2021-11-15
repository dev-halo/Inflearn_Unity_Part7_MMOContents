using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class ArrowController : CreatureController
{
    protected override void Init()
    {
        switch (Dir)
        {
            case MoveDir.Up:
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                break;
            case MoveDir.Down:
                transform.rotation = Quaternion.Euler(0f, 0f, 180f);
                break;
            case MoveDir.Left:
                transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                break;
            case MoveDir.Right:
                transform.rotation = Quaternion.Euler(0f, 0f, 270f);
                break;
            default:
                break;
        }

        State = CreatureState.Moving;

        base.Init();
    }

    protected override void UpdateAnimation()
    {

    }
}
