using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : Stat //�÷��̾����׸� Ư���� ������ stat���� ���� ��ũ��Ʈ, �Ϲ� Stat�� ��ӹ޾Ƽ� level, hp������ �Ƚ��൵�ȴ�.
{
    [SerializeField]
    protected int _exp;
    [SerializeField]
    protected int _gold;

    public int Exp 
    { 
        get { return _exp; } 
        set //���⼭ �ٷ� ������ üũ�� �ϸ�, �ٸ����� �ڵ带 �ش��ڵ带 �� ���� �������Ŵ�.
        { 
            _exp = value;
            //������ üũ
            //int level = Level; //üũ���ӽ� level , �� ���� Level
            int level = 1; //üũ level�� 1���� �����ϸ� ���� ����ġ�� ���̴� ��츦 �־����� ���� �ٿ ����� �������̴� ,��� ���� ����ġ�� ���缭 �Ʒ� while���� ������ �����̴�.
            while (true)
            {
                Data.Stat stat;
                if (Managers.Data.StatDict.TryGetValue(level + 1, out stat) == false)
                    break; //���� ������ ���ٸ�, �׳� ������
                if (_exp < stat.totalExp)
                    break;
                level++;
            }
            if(level !=Level) //���ڵ带 ���� �ӽ÷θ��纯��level�� �� Level�� �ٸ��ٸ�, �������� �Ͼ�ٴ� ���̴�, ���� Level�� ������Ʈ
            {
                Debug.Log("Level Up!");
                Level = level;
                SetStat(Level);
            }
        }
    }
    public int Gold { get { return _gold; } set { _gold = value; } }

    private void Start()
    {
        
        _level = 1;
        _exp = 0;
        _defense = 5; //����json�� ���� �߰������ֱ��ߴµ� ���� ����� �������� �׳� ����� �־�����.
        _moveSpeed = 5.0f;
        _gold = 0;

        SetStat(_level);
    }

    public void SetStat(int level)
    {
        Dictionary<int, Data.Stat> dict = Managers.Data.StatDict;
        Data.Stat stat = dict[level];

        _hp = stat.maxHp;
        _maxHp = stat.maxHp;
        _attack = stat.attack;
        //_exp = stat.totalExp; //��� exp�� ���� �����ϴٺ��� �ڿ��� �˼��ְ�, exp�� �� �ʰ����ִ� ���¿��� �ҷ����� ������ �������� ������ ����������� �Ƚ��ִ°� �´�.
    }

    protected override void OnDead(Stat attacker)
    {
        Debug.Log("Player Dead");
    }
}
