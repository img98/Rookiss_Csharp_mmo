using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Login;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q)) //일단 씬넘어가는걸 q누르면 넘어가게 하드코딩
        {
            Managers.Scene.LoadScene(Define.Scene.Game);
        }
    }


    public override void Clear()
    {
        Debug.Log("Login Scene Clear!");
    }
    
}
