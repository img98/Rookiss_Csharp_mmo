using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data{ //���߿� Stat�̶�� stat�������� ��ũ��Ʈ�� ����ǵ�, �׷��� �Ʒ� class Stat�� �̸��� ���ļ� namespace���༭ �긦 �ٲ���ߵȴ�. ���߿� ���Ÿ� Data.Stat, Data.StatData �̷������� namespcae. �� ����ߵ�
    #region Stat
    [Serializable] //Serializable = �޸𸮿��� ����ִ°�, ���Ϸ� ��ȯ�Ҽ��ִ�.�� �ǹ�. �׳� �����Ϳ��� �ٿ��ߵȴٰ� �ϱ�����
    public class Stat
    {
        public int level;
        public int maxHp;
        public int attack;
        public int totalExp;
    }

    [Serializable]
    public class StatData : ILoader<int, Stat>
    {
        public List<Stat> stats = new List<Stat>();

        public Dictionary<int, Stat> MakeDict() //�о�� ������������ dictinonary���·� ����� �Լ�
        {
            Dictionary<int, Stat> dict = new Dictionary<int, Stat>();
            foreach (Stat stat in stats)
                dict.Add(stat.level, stat); //���� ��ųʸ��� Ű������ ID�� �����°� ������, ������ �ǽ��̶� ������ �׳� level�� ������
            return dict;
        }
    }
    #endregion
}