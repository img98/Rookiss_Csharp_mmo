using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : Stat //�÷��̾����׸� Ư���� ������ stat���� ���� ��ũ��Ʈ, �Ϲ� Stat�� ��ӹ޾Ƽ� level, hp������ �Ƚ��൵�ȴ�.
{
    protected int _exp;
    protected int _gold;

    public int Exp { get { return _exp; } set { _exp = value; } }
    public int Gold { get { return _gold; } set { _gold = value; } }

    private void Start()
    {//������ ���߿� ���������� �����ؼ� ���� �־�����ϴµ�, ������ �־����ϱ� �������� �־�����.
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
