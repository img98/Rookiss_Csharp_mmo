using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{
    protected override void Init()
    {
        base.Init(); //start�� ������ BaseScene�� Awake�� Init�� ������ ������ �־, ���⵵ start��� �۵��Ѵ�.

        SceneType = Define.Scene.Game;

        Managers.UI.ShowSceneUI<UI_Inven>();
    }

    public override void Clear()
    {

    }
}
