using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    public T Load<T>(string path) where T: Object // ��,T�� Object���� �̶�� ��
    {
        return Resources.Load<T>(path);
    }

    public GameObject Instantiate(string path, Transform parent = null) // prefab�� ���� �θ�(����)�޽��� ��������, �Ⱦ��Ÿ� null
    {
        GameObject prefab = Load<GameObject>($"Prefabs/{path}"); // ���⼭�� Load�� �� T Load�� ���� �����ִ°�. ������� Resources.Load�� �����
        if (prefab == null)
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }
        return Object.Instantiate(prefab, parent); // �������� ��ȯ�ϰ�, �ɼ����� ������ �������� ���� �θ���� ��������
        //object.Instantiate�� ������? �����ϴ�. object. �� �Ⱥ��̸� ����Լ�ó�� �ڱ��ڽ��� ȣ���Ұ���. �װ� �ƴ϶�� ����ϱ����ؼ�
    }

    public void Destroy(GameObject go)
    {
        if (go == null)
            return ;
        Object.Destroy(go);
    }


}
