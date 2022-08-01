using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{
    protected override void Init()
    {
        base.Init(); //start가 없지만 BaseScene에 Awake로 Init을 돌리는 구문이 있어서, 여기도 start없어도 작동한다.

        SceneType = Define.Scene.Game;

        Managers.UI.ShowSceneUI<UI_Inven>();
    }

    public override void Clear()
    {

    }
}
