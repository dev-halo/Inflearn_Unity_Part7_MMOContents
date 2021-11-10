using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class ArrowController : CreatureController
{
    protected override void Init()
    {
        switch (lastDir)
        {
            case MoveDir.None:
                break;
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

        base.Init();
    }

    protected override void UpdateAnimation()
    {

    }

    protected override void UpdateIdle()
    {
        if (dir != MoveDir.None)
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
                GameObject go = Managers.Object.Find(destPos);
                if (go == null)
                {
                    CellPos = destPos;
                }
                else
                {
                    CreatureController cc = go.GetComponent<CreatureController>();
                    if (cc != null)
                        cc.OnDamaged();

                    Debug.Log(go.name);
                    Managers.Resource.Destroy(gameObject);
                }
            }
            else
            {
                Managers.Resource.Destroy(gameObject);
            }
        }
    }
}
