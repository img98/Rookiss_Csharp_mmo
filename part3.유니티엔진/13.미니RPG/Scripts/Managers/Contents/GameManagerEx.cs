using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerEx
{
    //Dictionary<int, GameObject> _players = new Dictionary<int, GameObject>();
    GameObject _player;//지금은 플레이어가 한명뿐이니 굳이 딕셔너리 안써도 된다.
    HashSet<GameObject> _monsters = new HashSet<GameObject>(); //해쉬셋 = 키 없는 Dictionary (아직 서버연동은 안했으니, 키ID가 없다. 그냥 해쉬셋으로도 충분)

    public GameObject Spawn(Define.WorldObject type, string path, Transform parent = null)
    {
        GameObject go = Managers.Resource.Instantiate(path, parent);

        switch(type)
        {
            case Define.WorldObject.Monster:
                _monsters.Add(go);
                break;
            case Define.WorldObject.Player:
                _player = go;
                break;
        }

        return go;
    }

    public Define.WorldObject GetWorldObjectType(GameObject go)
    {
        BaseController bc = go.GetComponent<BaseController>();
        if (bc == null)
            return Define.WorldObject.Unknown;

        return bc.WorldObjectType;
    }

    public void Despawn(GameObject go)
    {
        Define.WorldObject type = GetWorldObjectType(go);

        switch(type)
        {
            case Define.WorldObject.Monster:
                {
                    if (_monsters.Contains(go))
                        _monsters.Remove(go);
                }
                break;
            case Define.WorldObject.Player:
                {
                    if (_player == go)
                        _player = null;
                }
                break;
        }

        Managers.Resource.Destroy(go);

    }
}
