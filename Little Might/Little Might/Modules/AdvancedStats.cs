using System;
using System.Collections.Generic;
using System.Text;

namespace Little_Might.Modules
{
    class AdvancedStats : Stats
    {
        public enum ILLNESSES
        {
            NONE = 0,
            CHOLERA,
            MOUNTAINFEVER,
            SCURVY,
            PNEUMONIA
        }

        private int _hunger;
        private int _hydration;
        private ILLNESSES _illness;

        public int Hunger
        {
            get { return _hunger; }
            set { _hunger = value; }
        }

        public int Hydration
        {
            get { return _hydration; }
            set { _hydration = value; }
        }

        public ILLNESSES Illness
        {
            get { return _illness; }
            set { _illness = value; }
        }

        public AdvancedStats(int hp, int sp, int mp, float spd, int intel, int str, int wis, int dex, int charisma, float crit)
        {
            HP = hp;
            Stamina = sp;
            Mana = mp;
            Speed = spd;

            INT = intel;
            STR = str;
            WIS = wis;
            DEX = dex;
            CHARISMA = charisma;
            CRIT = crit;

            _hunger = 100;
            _hydration = 100;
            _illness = ILLNESSES.NONE;
        }
    }
}
