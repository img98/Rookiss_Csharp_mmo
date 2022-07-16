using System;
using System.Collections.Generic;
using System.Text;

namespace TRPG2
{
    public enum CreatureType
    {
        None,
        Player = 1,
        Monster = 2
    }
    class Creature
    {
        CreatureType type;

        protected Creature(CreatureType type)
        {
            this.type = type;
        }

        protected int hp = 0;
        protected int attack = 0;

        public void SetInfo(int hp, int attack)
        {
            this.hp = hp;
            this.attack = attack;
        }

        public int GetHp() { return hp; }
        public int GetAttack() { return attack; }
    }
}
