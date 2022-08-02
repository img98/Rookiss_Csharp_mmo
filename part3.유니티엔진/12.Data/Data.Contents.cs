using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Stat
[Serializable] //Serializable = 메모리에서 들고있는걸, 파일로 변환할수있다.는 의미. 그냥 데이터에는 붙여야된다고 암기하자
public class Stat
{
    public int level;
    public int hp;
    public int attack;
}

[Serializable]
public class StatData : ILoader<int, Stat>
{
    public List<Stat> stats = new List<Stat>();

    public Dictionary<int, Stat> MakeDict() //읽어온 데이터파일을 dictinonary형태로 만드는 함수
    {
        Dictionary<int, Stat> dict = new Dictionary<int, Stat>();
        foreach (Stat stat in stats)
            dict.Add(stat.level, stat); //원랜 딕셔너리의 키값으로 ID를 물리는게 좋은데, 지금은 실습이라 없으니 그냥 level로 물리자
        return dict;
    }
}
#endregion