using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers s_instance;
    public static Managers Instance { get { Init(); return s_instance; } } // 유일한 매니저를 갖고온다.

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
            if (go == null) //만약 @Managers가 없어 null이 들어온다면 새로 @managers를 만들어주자
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }
            DontDestroyOnLoad(go); // 중요한거니 왠만하면 삭제하지 말라는 문법
            s_instance = go.GetComponent<Managers>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
