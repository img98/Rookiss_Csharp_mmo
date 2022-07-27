using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager // �̹� �̱��� �Ŵ����� ������, ��� ������Ʈ�� �ƴ� �Ϲ����� C#���Ϸ� �����
{
    public Action KeyAction = null; //������ delegate

    public void OnUpdate()
    {
        if (Input.anyKey == false)
            return;

        if (KeyAction != null)
            KeyAction.Invoke();
    }
}
