using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class UI_Base : MonoBehaviour
{
    protected Dictionary<Type, UnityEngine.Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>(); //Ŭ������ Type��, ����Ƽ ������Ʈ���� �迭�� �������ּ��� ����ǹ�.

    public abstract void Init(); //��¼�� UI_base ��ü�� ������ ���⿡, virutal ��� abstract�� �������. abstract = �̸����޾Ƴ��� ������ ������, �������� ��ӹ��� �ֵ鿡�� ������ �����ض�.
 
    private void Start() //������ü���� Start�� ��������, �Ʒ���ü���� Start�� �Ƚᵵ�ǰ�, �׳� Init()�� �������ָ�ȴ�.
    {
        Init();
    }

    protected void Bind<T>(Type type) where T : UnityEngine.Object//������ ���ִµ� enum�� �ִ� ������� ���� �˾ƾߵǴ�, ��Ÿ�Ӱ� �Բ� Ȯ���� ������ reflection������ �������.
    {
        string[] names = Enum.GetNames(type);

        UnityEngine.Object[] objects = new UnityEngine.Object[names.Length]; //���ڷθ��� str�迭ũ����, ����Ƽ���� ������Ʈ Ÿ���� �迭�� �����.
        _objects.Add(typeof(T), objects);
        //���� ����ؾ��� object���� ã������ �ϳ��ϳ� ���� �������ָ�ȴ�.
        for (int i = 0; i < names.Length; i++)
        {
            if (typeof(T) == typeof(GameObject)) //gameObject�� ã���� ���� FindChild�� �ȸ�����, �б⸦ ���� �ٸ� find�� ��ߵ� 
            {
                objects[i] = Util.FindChild(gameObject, names[i], true);
            }
            else
                objects[i] = Util.FindChild<T>(gameObject, names[i], true);

            if (objects[i] == null)
                Debug.Log($"Failed to Bind ({names[i]})");
        }
    }

    protected T Get<T>(int idx) where T : UnityEngine.Object
    {
        UnityEngine.Object[] objects = null; //�����ֵ��� ��Ƴ��� �ӽ� �迭
        if (_objects.TryGetValue(typeof(T), out objects) == false) // ���� �������� ���ٸ�(����) null
            return null;

        return objects[idx] as T; //�����ߴٸ� TŸ������ ����
    }
    protected TextMeshProUGUI GetText(int idx) //������ Get������, Ÿ�Ը��� �ҷ��ö� Get<Ÿ��> ġ�� �������� �׳� ���������� �Լ��� ��������
    { return Get<TextMeshProUGUI>(idx); }
    protected Button GetButton(int idx) { return Get<Button>(idx); }
    protected Image GetImage(int idx) { return Get<Image>(idx); }
    protected GameObject GetObject(int idx) { return Get<GameObject>(idx); }


    public static void BindEvent(GameObject go, Action<PointerEventData> action, Define.UIEvent type = Define.UIEvent.Click) //���ڴ� (���, �ൿ, ������ ���ϴ���)
    {
        /* UI_EventHandler evt = go.GetComponent<UI_EventHandler>(); //������ ã�� go���� UI_EventHandler��� ������Ʈ�� evt�� ������
        if (evt == null) // ���� ������Ʈ�� UI_EventHandler������Ʈ�� �ȴ޷��ִٸ�, ���� �޾��ش�.
            evt = go.AddComponent<UI_EventHandler>(); */ //�� ������ GetOrAddComponent�� �����̴�.
        UI_EventHandler evt=Util.GetOrAddComponent<UI_EventHandler>(go);

        switch(type)
        {
            case Define.UIEvent.Click:
                evt.OnClickHandler -= action;
                evt.OnClickHandler += action;
                break;

            case Define.UIEvent.Drag:
                evt.OnDragHandler -= action;
                evt.OnDragHandler += action;
                break;
        }
    }

}

