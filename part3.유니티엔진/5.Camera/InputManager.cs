using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager // 이미 싱글톤 매니저가 있으니, 얘는 컴포넌트가 아닌 일반적인 C#파일로 만들것
{
    public Action KeyAction = null; //일종의 delegate
    public Action<Define.MouseEvent> MouseAction = null;

    bool _pressed = false; //MouseAction에서 클릭인지 알기위한 변수

    public void OnUpdate()
    {
        if (Input.anyKey && KeyAction != null) //anykey가 있고(&&) keyAciton이 null이 아니면
            KeyAction.Invoke();

        if(MouseAction!=null)
        {
            if(Input.GetMouseButton(0)) //마우스0 = 왼쪽버튼
            {
                MouseAction.Invoke(Define.MouseEvent.Press);
                _pressed = true; //마우스를 눌렀으면 true로 바꿔주고, 왼쪽버튼을 때면 GetMouseButton이 false가 될테니 else문으로 이동한다.
            }
            else
            {
                if (_pressed)//_pressed가 true라면 = 위 if문을 돌고 마우스가 떼진거라면 클릭이니까
                    MouseAction.Invoke(Define.MouseEvent.Click);
                _pressed = false;
            }
        }
    }
}
