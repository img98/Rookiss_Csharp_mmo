using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager // 이미 싱글톤 매니저가 있으니, 얘는 컴포넌트가 아닌 일반적인 C#파일로 만들것
{
    public Action KeyAction = null; //일종의 delegate

    public void OnUpdate()
    {
        if (Input.anyKey == false)
            return;

        if (KeyAction != null)
            KeyAction.Invoke();
    }
}
