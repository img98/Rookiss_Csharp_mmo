using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : Stat //플레이어한테만 특별히 더있을 stat들을 위한 스크립트, 일반 Stat을 상속받아서 level, hp같은건 안써줘도된다.
{
    protected int _exp;
    protected int _gold;

    public int Exp { get { return _exp; } set { _exp = value; } }
    public int Gold { get { return _gold; } set { _gold = value; } }

    private void Start()
    {//스탯은 나중에 서버데이터 연동해서 값을 넣어줘야하는데, 지금은 멀었으니까 수동으로 넣어주자.
        _level = 1;
        _hp = 100;
        _maxHp = 100;
        _attack = 10;
        _defense = 5;
        _moveSpeed = 5.0f;
        _exp = 0;
        _gold = 0;
    }
}
