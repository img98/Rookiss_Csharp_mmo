using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager // �̹� �̱��� �Ŵ����� ������, ��� ������Ʈ�� �ƴ� �Ϲ����� C#���Ϸ� �����
{
    public Action KeyAction = null; //������ delegate
    public Action<Define.MouseEvent> MouseAction = null;

    bool _pressed = false; //MouseAction���� Ŭ������ �˱����� ����

    public void OnUpdate()
    {
        if (Input.anyKey && KeyAction != null) //anykey�� �ְ�(&&) keyAciton�� null�� �ƴϸ�
            KeyAction.Invoke();

        if(MouseAction!=null)
        {
            if(Input.GetMouseButton(0)) //���콺0 = ���ʹ�ư
            {
                MouseAction.Invoke(Define.MouseEvent.Press);
                _pressed = true; //���콺�� �������� true�� �ٲ��ְ�, ���ʹ�ư�� ���� GetMouseButton�� false�� ���״� else������ �̵��Ѵ�.
            }
            else
            {
                if (_pressed)//_pressed�� true��� = �� if���� ���� ���콺�� �����Ŷ�� Ŭ���̴ϱ�
                    MouseAction.Invoke(Define.MouseEvent.Click);
                _pressed = false;
            }
        }
    }
}
