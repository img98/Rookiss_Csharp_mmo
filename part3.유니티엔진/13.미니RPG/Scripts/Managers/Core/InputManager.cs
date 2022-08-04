using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager // �̹� �̱��� �Ŵ����� ������, ��� ������Ʈ�� �ƴ� �Ϲ����� C#���Ϸ� �����
{
    public Action KeyAction = null; //������ delegate
    public Action<Define.MouseEvent> MouseAction = null;

    bool _pressed = false; //MouseAction���� Ŭ������ �˱����� ����
    float _pressedTime = 0; //Click�� PressedUp�� �����Ҽ� �ֱ�����, ���ʵ��� �������־����� ���

    public void OnUpdate()
    {
        if (EventSystem.current.IsPointerOverGameObject()) //UI�� �����ٸ� return;
            return;

        if (Input.anyKey && KeyAction != null) //anykey�� �ְ�(&&) keyAciton�� null�� �ƴϸ�
            KeyAction.Invoke();

        if(MouseAction!=null)
        {
            if(Input.GetMouseButton(0)) //���콺0 = ���ʹ�ư
            {
                if(!_pressed)
                {
                    MouseAction.Invoke(Define.MouseEvent.PointerDown);
                    _pressedTime = Time.time;
                }
                MouseAction.Invoke(Define.MouseEvent.Press);
                _pressed = true; //���콺�� �������� true�� �ٲ��ְ�, ���ʹ�ư�� ���� GetMouseButton�� false�� ���״� else������ �̵��Ѵ�.
            }
            else
            {
                if(_pressed)
                {
                    if(Time.time<_pressedTime + 0.2f) //����������� ������ 0.2�� �ȿ� ������ Ŭ������ �����ϰڴ�.
                        MouseAction.Invoke(Define.MouseEvent.Click);
                    MouseAction.Invoke(Define.MouseEvent.PointerUp);
                }

                _pressed = false;
                _pressedTime = 0;
            }
        }
    }
    public void Clear()
    {
        KeyAction = null;
        MouseAction = null;
    }
}
