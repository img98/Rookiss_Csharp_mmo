using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    public T Load<T>(string path) where T: Object // ��,T�� Object���� �̶�� ��
    {
        //�ε带 �ϱ����� Ǯ���Ǿ��ִ� �༮���� Ȯ�κ����غ���.
        if(typeof(T)==typeof(GameObject)) //Ÿ���� gameObject�� �������� ���ɼ��� ������, �̰�츦 ��������
        {
            string name = path;
            int index = name.LastIndexOf('/'); //�Ʒ� Instantiate�� �����ֵ�, Prefabs/{path} �̷��� /�� ����. �׷��Ƿ� ������ /�ڿ� �����°� �ʿ��� name�̴�.
            if (index >= 0) //�߰��ߴٸ�
                name = name.Substring(index + 1); // '/' ��ĭ���� �ؽ�Ʈ�� �����°� �̸�.

            GameObject go = Managers.Pool.GetOriginal(name); //Ǯ���Ŵ����� ���� original�� �ҷ��ͼ�
            if (go != null) //ã�Ƽ� �־��ٸ� �װ� return
                return go as T; 
        }

        return Resources.Load<T>(path); //Resources.Load�� �̹� �����ϴ� ���̺귯��
    }

    public GameObject Instantiate(string path, Transform parent = null) // prefab�� ���� �θ�(����)������Ʈ�� ��������, �Ⱦ��Ÿ� null
    {
        GameObject original = Load<GameObject>($"Prefabs/{path}"); // ���⼭�� Load�� �� T Load�� ���� �����ִ°�. ������� Resources.Load�� �����
        if (original == null)
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }

        // Ȥ�� Ǯ���� �ְ� �ִٸ� �·� ����
        if (original.GetComponent<Poolable>() != null)
            return Managers.Pool.Pop(original, parent).gameObject;

        GameObject go = Object.Instantiate(original, parent);
        go.name = original.name; //���纻 �̸��� �׳� ���������̶� �����Ѵ�.(Ŭ�� �ȶ߰�)
        return go; 
    }

    public void Destroy(GameObject go)
    {
        if (go == null)
            return ;
        //�ٷ� ������������, ���� Ǯ���� �ʿ��ϴٸ�->Ǯ�� �Ŵ������� ��Ź. �ʿ��ϸ� ���� �������ְ�
        Poolable poolable = go.GetComponent<Poolable>();
        if(poolable!=null)
        {
            Managers.Pool.Push(poolable);
            return;
        }

        Object.Destroy(go);
    }


}
