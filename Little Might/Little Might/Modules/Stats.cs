namespace Little_Might.Modules
{
    internal class Stats
    {
        private int _health;
        private int _stamina;
        private int _mana;
        private float _speed;

        private int _INT;
        private int _STR;
        private int _WIS;
        private int _DEX;
        private int _CHAR;

        private float _DEF;
        private float _CRIT;

        public int HP
        {
            get { return _health; }
            set { _health = value; }
        }

        public int Stamina
        {
            get { return _stamina; }
            set { _stamina = value; }
        }

        public int Mana
        {
            get { return _mana; }
            set { _mana = value; }
        }

        public float Speed
        {
            get { return _speed; }
            set { _speed = value; }
        }

        public int INT
        {
            get { return _INT; }
            set { _INT = value; }
        }

        public int STR
        {
            get { return _STR; }
            set { _STR = value; }
        }

        public int WIS
        {
            get { return _WIS; }
            set { _WIS = value; }
        }

        public int DEX
        {
            get { return _DEX; }
            set { _DEX = value; }
        }

        public int CHARISMA
        {
            get { return _CHAR; }
            set { _CHAR = value; }
        }

        public float DEFENSE
        {
            get { return _DEF; }
            set { _DEF = value; }
        }

        public float CRIT
        {
            get { return _CRIT; }
            set { _CRIT = value; }
        }

        public Stats()
        { }

        public Stats(int hp, int sp, int mp, float spd, int intel, int str, int wis, int dex, int charisma, float def, float crit)
        {
            _health = hp;
            _stamina = sp;
            _mana = mp;
            _speed = spd;

            _INT = intel;
            _STR = str;
            _WIS = wis;
            _DEX = dex;
            _CHAR = charisma;
            _DEF = def;
            _CRIT = crit;
        }
    }
}
