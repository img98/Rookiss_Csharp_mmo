using System;
using System.Collections.Generic;
using System.Text;

namespace TRPG2
{
    public enum PlayerType
    {
        None = 0,
        Knight =1,
        Archer =2,
        Mage =3

    }
    class Player : Creature
    {
        protected PlayerType type = PlayerType.None; // 기본값으론 None
        
        protected Player(PlayerType type) : base(CreatureType.Player)
        {
            this.type = type;
        }

        public PlayerType GetPlayerType() { return type; }
    }

    class Knight : Player
    {
        public Knight() : base(PlayerType.Knight)
        {
            SetInfo(100, 10);
        }

    }
    class Archer : Player
    {
        public Archer() : base(PlayerType.Archer)
        {
            SetInfo(75, 12);
        }

    }
    class Mage : Player
    {
        public Mage () : base(PlayerType.Mage)
        {
            SetInfo(50, 15);

        }

    }

}
