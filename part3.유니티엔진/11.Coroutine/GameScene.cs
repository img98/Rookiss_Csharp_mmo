using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{

    Coroutine co;
    protected override void Init()
    {
        base.Init(); //start가 없지만 BaseScene에 Awake로 Init을 돌리는 구문이 있어서, 여기도 start없어도 작동한다.

        SceneType = Define.Scene.Game;

        Managers.UI.ShowSceneUI<UI_Inven>();

        //Coroutine실습
        Coroutine co = StartCoroutine("ExplodeAfterSeconds", 2.0f);
        StartCoroutine("CoStopExplode",1.0f);
    }

    //Coroutine실습
    IEnumerator CoStopExplode(float seconds) //코루틴이라는걸 표시하기위해 이름앞에 Co를 넣어줬다 
    {
        Debug.Log("Stop enter");
        yield return new WaitForSeconds(seconds);
        Debug.Log("Explode stop");
        if (co != null)
        {
            StopCoroutine(co);
            co = null;
        }
    }
    IEnumerator ExplodeAfterSeconds(float seconds)
    {
        Debug.Log("Explode enter");
        yield return new WaitForSeconds(seconds);
        Debug.Log("Boom!");
        co = null;
    }


    public override void Clear()
    {

    }
}
    