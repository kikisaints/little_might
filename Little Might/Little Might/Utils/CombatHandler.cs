using System;
using System.Collections.Generic;
using System.Text;

namespace Little_Might.Utils
{
    class CombatHandler
    {
        private static Random rand = new Random();

        //Returns true if monster is dead
        public static bool AttackMonster(Modules.Character character, ref Modules.Monster monster, string attackType)
        {
            int damage = 0;
            switch (attackType.ToLower())
            {
                case "punch":
                    damage = (int)((character.Stats.STR / 2) - monster.Stats.DEFENSE);
                    if (damage >= 0)
                        monster.Stats.HP -= damage;
                    break;
                case "kick":
                    damage = (int)(character.Stats.STR - monster.Stats.DEFENSE);
                    if (damage >= 0)
                        monster.Stats.HP -= damage;
                    break;
                default:
                    break;
            }

            if (monster.Stats.HP <= 0)
                return true;

            return false;
        }

        public static int AttackCharacter(ref Modules.Character character, Modules.Monster monster, string attackType)
        {
            switch (attackType.ToLower())
            {
                case "spit":
                    return (int)((monster.Stats.DEX + monster.Stats.Stamina) - character.Stats.DEFENSE);
            }

            return 0;
        }
    }
}
