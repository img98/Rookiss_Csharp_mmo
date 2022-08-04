using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Inven : UI_Scene
{
    enum GameObjects
    {
        GridPanel,
    }


    public override void Init()
    {
        base.Init();

        Bind<GameObject>(typeof(GameObjects)); //�������� �ߴ� ���ε��Լ�

        GameObject gridPanel = Get<GameObject>((int)GameObjects.GridPanel);
        foreach (Transform child in gridPanel.transform) //�� transform�� ����ִ� ��� �ڽĵ��� ��ȸ
            Managers.Resource.Destroy(child.gameObject); // ���� �����տ��� ���ÿ����� ����ִ� ��� �ڽĵ�(�κ�_������)�� �������ּ���=�ʱ�ȭ
        
        //������� �гο� ������ ä��� ����, ���� �κ��丮 ������ �����ؼ� �ۼ��ϸ��
        for(int i=0;i<8;i++) //���� ������� �Ұŵ������� �׳� 8�������� ������.
        {
            // GameObject item = Managers.Resource.Instantiate("UI/SubItem/UI_Inven_Item"); //�̷��� �������� ��θ� ��� ���°� �������� ShowPopupItemó�� subItem�� ����� �Լ��� ���� �����Ű��
            GameObject item = Managers.UI.MakeSubItem<UI_Inven_Item>(gridPanel.transform).gameObject; //MakeSubItem������ ������Ʈ�� �̾ƿͼ�, ���� ����(gameObject)�� �̾Ƽ� Item�� ����
            //item.transform.SetParent(gridPanel.transform); //�θ��ڽ� �����ϴ°͵� MakeSubItem�� ���ڷ� �־ ����߰��ϸ� �����Ͱ���.

            //UI_Inven_Item invenItem = Util.GetOrAddComponent<UI_Inven_Item>(item); //���� �������ڽĵ鿡�� UI_Inven_Item ������Ʈ�� �ٿ� ����� �ο� or �³׸� Get�ؼ� �ٸ��� ����Ҽ��ִ�.
            UI_Inven_Item invenItem = item.GetOrAddComponent<UI_Inven_Item>(); //GetOrAddComponent����� item ��, �׳� gameobject������ ���� �ҷ��ü��ְ� ������� extension���� Ȯ����״�.
            invenItem.SetInfo($"{i}��° �����");
        }

    }
}
