using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers s_instance;
    public static Managers Instance { get { Init(); return s_instance; } } // ������ �Ŵ����� ����´�.

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    static void Init()
    {
        if (s_instance == null)
        {
            GameObject go = GameObject.Find("@Managers");
            if (go == null) //���� @Managers�� ���� null�� ���´ٸ� ���� @managers�� ���������
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }
            DontDestroyOnLoad(go); // �߿��ѰŴ� �ظ��ϸ� �������� ����� ����
            s_instance = go.GetComponent<Managers>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
