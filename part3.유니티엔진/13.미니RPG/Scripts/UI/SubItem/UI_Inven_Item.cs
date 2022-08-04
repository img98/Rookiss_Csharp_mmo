using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inven_Item : UI_Base // ��� �������� ���絵 �ƴϰ�, �ϳ��� �����ϴ� �͵� �ƴϱ⿡ UI_Scene�� �ƴ� UI_Base�� ����Ѵ�.
{
    enum GameObjects
    {
        //��� ���� ������ �ʾƼ�, text�� Image�ε��� �׳� go�� ���ױ׷��� ��Ƶ��ȴ�.
        ItemIcon,
        ItemNameText,
    }

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));//�̷��� ���� text������Ʈ�� image������Ʈ�� �����ִ°� �������� ã�ƴ��ִ°� �ƴ϶�, �׳� ������Ʈ�� �̸��� �������� �Ǻ��ؼ� ���ε�����.����� ������ ������ �������� �ٸ���.
        Get<GameObject>((int)GameObjects.ItemNameText).GetComponent<Text>().text = _name;//������ ���ߵ�, ������Ʈ�� ���ε��Ѱ� �ƴϱ⿡, text�� �ٲ��ְ������ �ش� �̸��� ������Ʈ�� Component�� Get�ؼ� ��������ߵ�

        Get<GameObject>((int)GameObjects.ItemIcon).BindEvent((PointerEventData) => { Debug.Log($"������ Ŭ��! : {_name}"); }); //���ε�� ������ ������ ��������, Get���� �������� ������ �αװ� �������ϴ� �ڵ�
    }

    string _name;
    public void SetInfo(string name) //�����ۺ� �̸��� �����ؼ� ����ϱ�����(������ ���� �޾ƿð��� ���� ������ ������.)
    {
        _name = name;
    }
}
