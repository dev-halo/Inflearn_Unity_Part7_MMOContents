using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Game;

        Managers.Map.LoadMap(1);

        Screen.SetResolution(640, 480, false);

        //GameObject player = Managers.Resource.Instantiate("Creature/Player");
        //player.name = "Player";
        //Managers.Object.Add(player);

        //for (int i = 0; i < 5; i++)
        //{
        //    Vector3Int pos = new Vector3Int()
        //    {
        //        x = Random.Range(-20, 20),
        //        y = Random.Range(-10, 10)
        //    };

        //    if (!Managers.Map.CanGo(pos))
        //    {
        //        --i;
        //        continue;
        //    }

        //    GameObject monster = Managers.Resource.Instantiate("Creature/monster");
        //    monster.name = $"Monster_{i + 1}";

        //    MonsterController mc = monster.GetComponent<MonsterController>();
        //    mc.CellPos = pos;

        //    Managers.Object.Add(monster);
        //}
    }

    public override void Clear()
    {

    }
}
