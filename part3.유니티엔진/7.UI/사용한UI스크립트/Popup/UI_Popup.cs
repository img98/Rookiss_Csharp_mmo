using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Popup : UI_Base
{
    public override void Init()
    {
        Managers.UI.SetCanvas(gameObject, true);
    }
    public virtual void ClosePopupUI() //UI_Popup�� ��ӹ��� �ֵ��� ClosePopupUI�� ���� �����Ҽ��ֵ���, ÷����.(�̰� extension�ΰ�?)
    {
        Managers.UI.ClosePopupUI(this);
    }
}
