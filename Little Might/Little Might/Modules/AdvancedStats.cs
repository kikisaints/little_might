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

        private int _maxHunger;
        private int _maxHydration;

        private int _baseHP;

        public int Hunger
        {
            get { return _hunger; }
            set { 
                _hunger = value;

                if (_hunger > _maxHunger)
                    _hunger = _maxHunger;
            }
        }

        public int Hydration
        {
            get { return _hydration; }
            set { 
                _hydration = value;

                if (_hydration > _maxHydration)
                    _hydration = _maxHydration;
            }
        }

        public ILLNESSES Illness
        {
            get { return _illness; }
            set { _illness = value; }
        }

        public int BaseHP
        {
            get { return _baseHP; }
        }

        public AdvancedStats(int hp, int sp, int mp, float spd, int intel, int str, int wis, int dex, int charisma, float def, float crit)
        {
            _baseHP = hp;
            HP = _baseHP;

            Stamina = sp;
            Mana = mp;
            Speed = spd;

            INT = intel;
            STR = str;
            WIS = wis;
            DEX = dex;
            CHARISMA = charisma;
            DEFENSE = def;
            CRIT = crit;

            _hunger = 100;
            _hydration = 100;
            _illness = ILLNESSES.NONE;

            _maxHunger = _hunger;
            _maxHydration = _hydration;
        }
    }
}
