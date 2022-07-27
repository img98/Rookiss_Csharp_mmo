using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    public T Load<T>(string path) where T: Object // 단,T는 Object형식 이라는 뜻
    {
        return Resources.Load<T>(path);
    }

    public GameObject Instantiate(string path, Transform parent = null) // prefab을 넣을 부모(상위)메쉬를 설정가능, 안쓸거면 null
    {
        GameObject prefab = Load<GameObject>($"Prefabs/{path}"); // 여기서의 Load는 위 T Load를 만들어서 쓸수있는것. 원래라면 Resources.Load를 써야지
        if (prefab == null)
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }
        return Object.Instantiate(prefab, parent); // 프리팹을 반환하고, 옵션으로 생성한 프리팹을 붙일 부모까지 설정가능
        //object.Instantiate를 한이유? 간단하다. object. 을 안붙이면 재귀함수처럼 자기자신을 호출할것임. 그게 아니라고 명시하기위해서
    }

    public void Destroy(GameObject go)
    {
        if (go == null)
            return ;
        Object.Destroy(go);
    }


}
