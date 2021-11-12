using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MonsterController : CreatureController
{
    Coroutine coSkill;
    Coroutine coPatrol;
    Coroutine coSearch;

    [SerializeField]
    Vector3Int destCellPos;

    [SerializeField]
    GameObject target;

    [SerializeField]
    float searchRange = 10f;

    [SerializeField]
    float skillRange = 1f;

    [SerializeField]
    bool rangedSkill = false;

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
        rangedSkill = (Random.Range(0, 2) == 0 ? true : false);

        if (rangedSkill)
            skillRange = 10f;
        else
            skillRange = 1f;
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

            Vector3Int dir = destPos - CellPos;
            if (dir.magnitude <= skillRange && (dir.x == 0 || dir.y == 0))
            {
                Dir = GetDirFromVec(dir);
                State = CreatureState.Skill;

                if (rangedSkill)
                    coSkill = StartCoroutine(CoStartShootArrow());
                else
                    coSkill = StartCoroutine(CoStartPunch());

                return;
            }
        }

        List<Vector3Int> path = Managers.Map.FindPath(CellPos, destPos, ignoreDestCollision: true);
        if (path.Count < 2 || (target != null && path.Count > 20))
        {
            target = null;
            State = CreatureState.Idle;
            return;
        }

        Vector3Int nextPos = path[1];
        Vector3Int moveCellDir = nextPos - CellPos;

        Dir = GetDirFromVec(moveCellDir);

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

        Managers.Object.Remove(Id);
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

    IEnumerator CoStartPunch()
    {
        GameObject go = Managers.Object.Find(GetFrontCellPos());
        if (go != null)
        {
            CreatureController cc = go.GetComponent<CreatureController>();
            if (cc != null)
                cc.OnDamaged();
        }

        yield return new WaitForSeconds(0.5f);
        State = CreatureState.Moving;
        coSkill = null;
    }

    IEnumerator CoStartShootArrow()
    {
        GameObject go = Managers.Resource.Instantiate("Creature/Arrow");
        ArrowController ac = go.GetComponent<ArrowController>();
        ac.Dir = lastDir;
        ac.CellPos = CellPos;

        yield return new WaitForSeconds(0.3f);
        State = CreatureState.Moving;
        coSkill = null;
    }
}
