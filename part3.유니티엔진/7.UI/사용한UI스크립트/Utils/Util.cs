using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util //�׳� ��ɼ� �Լ����� ���� �־���� ��ũ��Ʈ
{
    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component // ������Ʈ���� ������Ʈ�� �޾ƿ��ų� ������ �߰����ּ���
    {
        T component = go.GetComponent<T>(); 
        if (component == null)
            component = go.AddComponent<T>();
        return component;
    }

    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        Transform transform = FindChild<Transform>(go, name, recursive); // ��� go�� transform�� ������, ���� �ڵ�©�ʿ� ����, �Ʒ� FindChild<T>�ڵ忡 �ʿ��� ������Ʈ<T>�� Transform���� ����ȴ�.

        if (transform != null)
            return transform.gameObject;
        return null;
    }

    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
        //�ֻ��� �θ� �Է��ϸ�, ���� �ڽĵ��� �� �Ⱦ�鼭 �̸�(name)�� ���� �ڽ��� return�ϰų�,�̸��Է��� ���ٸ� �׳� Ÿ���� ������ �������ִ��Լ�
        //resursive�� ���������, �ڽ��� �ڽĵ� ã������ �ɼ�
    {
        if (go == null)
            return null;
        if(recursive==false)
        {
            for(int i =0;i<go.transform.childCount;i++)
            {
                Transform transform = go.transform.GetChild(i); //�� �ڱ� �ڽĸ� �������� ���̺귯��
                if (string.IsNullOrEmpty(name) || transform.name == name)
                {//�̸� ������ ��������� ������Ʈ�� ����ִ��� Ȯ������ߵ�
                    T component = transform.GetComponent<T>();
                    if (component != null)
                        return component;
                }
            }
            
        }
        else
        {
            foreach(T component in go.GetComponentsInChildren<T>()) //�ڽĵ�������Ʈ �������� ���̺귯���� ���� �ϳ��� component�� ����
            {
                if (string.IsNullOrEmpty(name) || component.name == name) //�̸��� ����ְų�, ���� ã���Ÿ� �װ� �����ֱ�.
                    return component;
            }

        }
        return null;
    }
}
