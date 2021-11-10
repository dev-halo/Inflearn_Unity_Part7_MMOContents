using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MonsterController : CreatureController
{
    Coroutine coPatrol;
    Coroutine coSearch;

    [SerializeField]
    Vector3Int destCellPos;

    [SerializeField]
    GameObject target;

    [SerializeField]
    float searchRange = 5f;

    public override CreatureState State
    {
        get { return state; }
        set
        {
            if (state == value)
                return;

            base.State = value;

            if (coPatrol != null)
            {
                StopCoroutine(coPatrol);
                coPatrol = null;
            }

            if (coSearch != null)
            {
                StopCoroutine(coSearch);
                coSearch = null;
            }
        }
    }

    protected override void Init()
    {
        base.Init();

        State = CreatureState.Idle;
        Dir = MoveDir.None;

        speed = 3f;
    }

    protected override void UpdateIdle()
    {
        base.UpdateIdle();

        if (coPatrol == null)
        {
            coPatrol = StartCoroutine(CoPatrol());
        }

        if (coSearch == null)
        {
            coSearch = StartCoroutine(CoSearch());
        }
    }

    protected override void MoveToNextPos()
    {
        Vector3Int destPos = destCellPos;
        if (target != null)
        {
            destPos = target.GetComponent<CreatureController>().CellPos;
        }

        List<Vector3Int> path = Managers.Map.FindPath(CellPos, destPos, ignoreDestCollision: true);
        if (path.Count < 2 || (target != null && path.Count > 10))
        {
            target = null;
            State = CreatureState.Idle;
            return;
        }

        Vector3Int nextPos = path[1];
        Vector3Int moveCellDir = nextPos - CellPos;

        if (moveCellDir.x > 0)
            Dir = MoveDir.Right;
        else if (moveCellDir.x < 0)
            Dir = MoveDir.Left;
        else if (moveCellDir.y > 0)
            Dir = MoveDir.Up;
        else if (moveCellDir.y < 0)
            Dir = MoveDir.Down;
        else
            Dir = MoveDir.None;

        if (Managers.Map.CanGo(nextPos) && Managers.Object.Find(nextPos) == null)
        {
            CellPos = nextPos;
        }
        else
        {
            State = CreatureState.Idle;
        }
    }

    public override void OnDamaged()
    {
        GameObject effect = Managers.Resource.Instantiate("Effect/DieEffect");
        effect.transform.position = transform.position;
        effect.GetComponent<Animator>().Play("START");
        GameObject.Destroy(effect, 0.5f);

        Managers.Object.Remove(gameObject);
        Managers.Resource.Destroy(gameObject);
    }

    IEnumerator CoPatrol()
    {
        int waitSeconds = Random.Range(1, 4);
        yield return new WaitForSeconds(waitSeconds);

        for (int i = 0; i < 10; ++i)
        {
            int xRange = Random.Range(-5, 6);
            int yRange = Random.Range(-5, 6);
            Vector3Int randPos = CellPos + new Vector3Int(xRange, yRange, 0);

            if (Managers.Map.CanGo(randPos) && Managers.Object.Find(randPos) == null)
            {
                destCellPos = randPos;
                State = CreatureState.Moving;
                yield break;
            }
        }

        State = CreatureState.Idle;
    }

    IEnumerator CoSearch()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            if (target != null)
                continue;

            target = Managers.Object.Find((go) =>
            {
                PlayerController pc = go.GetComponent<PlayerController>();
                if (pc == null)
                    return false;

                Vector3Int dir = (pc.CellPos - CellPos);
                if (dir.magnitude > searchRange)
                    return false;

                return true;
            });
        }
    }
}
