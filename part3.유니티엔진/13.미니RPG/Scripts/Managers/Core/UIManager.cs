using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    int _order = 10; // ��¼�� ���ο��� ����ϴ� ������ �����ϻ��̴� ���� 0���ʿ�� ����. Ȥ�� 1~9�� �ٸ��ɷ� �̸� �����صֵ� �ǰڴ�.

    Stack<UI_Popup> _popupStack = new Stack<UI_Popup>();//��� �˾��� ����� UI_Popup�̶�� ������Ʈ �Ͽ� �����Ѵ�. �׷��� ������ Ÿ���� ���� GameObject�� �ƴ� UI_Popup���� ������. 
    UI_Scene _sceneUI = null; //��� sceneUI�� ������ص� �Ǵµ�, Ȥ�� ���� ������ �𸣴ϱ� �ӽ������� ��� ������ ��������.

    public GameObject Root
    {
        get
        {
            GameObject root = GameObject.Find("@UI_Root");
            if (root == null)
                root = new GameObject { name = "@UI_Root" };
            return root;
        }
    }

    public void SetCanvas(GameObject go, bool sort = true) //�ܺο��� ui�� �˾���ų�� ������ ���� ĵ����order�� �����ش޶�� ��
    {
        // UI�� �����ɶ� �ʿ��� �ڵ��̹Ƿ�, UI_Popup�� UI_Scene�� �ʿ��ϰڴ�. 

        Canvas canvas = Util.GetOrAddComponent<Canvas>(go);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay; //�������� �׳� �̰� ���
        canvas.overrideSorting = true; // �θ�� ������ �ڽŸ��� sortOrder�� ���ڴ�.

        if (sort == true)
        {
            canvas.sortingOrder = _order;
;           _order++;
        }
        else
        {
            canvas.sortingOrder = 0;
        }
    }

    public T MakeWorldSpaceUI<T>(Transform parent = null, string name = null) where T : UI_Base
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"UI/WorldSpace/{name}");

        if (parent != null)
            go.transform.SetParent(parent);

        Canvas canvas = go.GetOrAddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;

        return Util.GetOrAddComponent<T>(go);
    }

    public T MakeSubItem<T>(Transform parent = null, string name = null) where T : UI_Base //�����տ� ���°� �������� ���̱����� �Լ�
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"UI/SubItem/{name}");

        if (parent != null)
            go.transform.SetParent(parent);

        return Util.GetOrAddComponent<T>(go);
    }

    public T ShowPopupUI<T>(string name = null) where T : UI_Popup //UI�� �˾� ��Ű��(����) ���
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name; //�̸��� ������ �ʾƵ�, ���� ������ type�� �����Ҽ�������, name���� <T>�� ����ϰڴٴ¸�(�츮�� �������̶� ����Ÿ���� �������ּ� ex.UI_Popup ������ �ڵ��.)

        GameObject go= Managers.Resource.Instantiate($"UI/Popup/{name}"); //�˾��� �ᱹ�� �����տ� �ִ°� �����ϴ°Ŵϱ�
        T popup = Util.GetOrAddComponent<T>(go); //������ ������ GetOrAddComponent
        _popupStack.Push(popup);

        go.transform.SetParent(Root.transform);
        
        return popup;
    }

    public T ShowSceneUI<T>(string name = null) where T : UI_Scene //UI�� ���� ���. �׷����� SceneUI�̱⿡ ���ÿ� ���� �ʰڴ�.
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name; //�̸��� ������ �ʾƵ�, ���� ������ type�� �����Ҽ�������, name���� <T>�� ����ϰڴٴ¸�(�츮�� �������̶� ����Ÿ���� �������ּ� ex.UI_Popup ������ �ڵ��.)

        GameObject go = Managers.Resource.Instantiate($"UI/Scene/{name}");
        T sceneUI= Util.GetOrAddComponent<T>(go); //������ ������ GetOrAddComponent
        _sceneUI = sceneUI;

        go.transform.SetParent(Root.transform); //Root�� ������Ƽ �������� ���� �ٷ� ��밡����.

        return sceneUI;
    }

    public void ClosePopupUI(UI_Popup popup) // �߸��Ȱ� ���������ϰ�, ���� �̸��� �Է��ϴ� ����
    {
        if (_popupStack.Count == 0)
            return;
        if (_popupStack.Peek() == popup)
        {
            Debug.Log($"Close Popuo Fail! : this time u need to close {_popupStack.Peek()}, but u requested {popup}"); // �����ؼ� �� ��Դø�
            return;
        }

        ClosePopupUI();
    }
        public void ClosePopupUI() // �˾���UI ����� ���
    {
        if (_popupStack.Count == 0)
            return;

        UI_Popup popup = _popupStack.Pop();
        Managers.Resource.Destroy(popup.gameObject); //popup.gameObject��, �ش������ UI_Popup�� �����ִ� ������Ʈ�� �ǹ��Ѵ�
        popup = null; //���������� popup�� �ٽðǵ帮�� ������. �ƿ�null�� ó���ع�����

        _order--;
    }

    public void CloseAllPopupUI()
    {
        while (_popupStack.Count > 0)
            ClosePopupUI();
    }

    public void Clear()
    {
        CloseAllPopupUI();
        _sceneUI = null;
    }


}
