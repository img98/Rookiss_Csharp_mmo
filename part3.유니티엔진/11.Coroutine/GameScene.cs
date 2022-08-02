using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{

    Coroutine co;
    protected override void Init()
    {
        base.Init(); //start�� ������ BaseScene�� Awake�� Init�� ������ ������ �־, ���⵵ start��� �۵��Ѵ�.

        SceneType = Define.Scene.Game;

        Managers.UI.ShowSceneUI<UI_Inven>();

        //Coroutine�ǽ�
        Coroutine co = StartCoroutine("ExplodeAfterSeconds", 2.0f);
        StartCoroutine("CoStopExplode",1.0f);
    }

    //Coroutine�ǽ�
    IEnumerator CoStopExplode(float seconds) //�ڷ�ƾ�̶�°� ǥ���ϱ����� �̸��տ� Co�� �־���� 
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
    