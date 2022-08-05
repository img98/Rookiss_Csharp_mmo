using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{

    protected override void Init()
    {
        base.Init(); //start�� ������ BaseScene�� Awake�� Init�� ������ ������ �־, ���⵵ start��� �۵��Ѵ�.

        SceneType = Define.Scene.Game;
        
        //Managers.UI.ShowSceneUI<UI_Inven>(); //���ӽ��� ������ ������ �κ��丮UI�� �������ϴ� �ڵ�. ��� ������ �������� ���⿡ �������ȴ�.
        
        Dictionary<int, Data.Stat> dict = Managers.Data.StatDict;
        gameObject.GetOrAddComponent<CursorController>();

        GameObject player = Managers.Game.Spawn(Define.WorldObject.Player, "UnityChan");
        Camera.main.gameObject.GetOrAddComponent<CameraController>().SetPlayer(player);

        //Managers.Game.Spawn(Define.WorldObject.Monster, "Knight"); //���� �Ѹ��� �������� ������ �ʰ�, ������Ǯ�� ����Ұ��̴�.
        GameObject go = new GameObject { name = "SpawningPool" }; //������Ǯ�̶� ������Ʈ�� �����
        SpawningPool pool = go.GetOrAddComponent<SpawningPool>(); //�� ������Ʈ�� ������ SpawningPool��ũ��Ʈ�� �޾��ش�. �̷��� ����ִ� ������Ǯ�� �����Ȱ�.
        pool.SetKeepMonsterCount(5);
    }



    public override void Clear()
    {

    }
}
    