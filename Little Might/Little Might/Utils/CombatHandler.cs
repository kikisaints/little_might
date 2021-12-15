using System;
using System.Collections.Generic;
using System.Text;

namespace Little_Might.Utils
{
    class CombatHandler
    {
        private static Random rand = new Random();

        //Returns true if monster is dead
        public static bool AttackMonster(Modules.Character character, ref Modules.Monster monster, string attackType, Modules.InventoryItem weapon = null)
        {
            int damage = 0;
            switch (attackType.ToLower())
            {
                case "punch":
                    damage = (int)((character.Stats.STR / 2) - monster.Stats.DEFENSE);
                    break;
                case "kick":
                    damage = (int)(character.Stats.STR - monster.Stats.DEFENSE);                    
                    break;
                case "slash":
                    if (weapon != null)
                        damage = (int)((character.Stats.STR + weapon.Damage) - monster.Stats.DEFENSE);
                    break;
                case "stab":
                    if (weapon != null)
                        damage = (int)((character.Stats.STR / 2) + weapon.Damage);
                    break;
                default:
                    break;
            }

            if (damage >= 0)
                monster.Stats.HP -= damage;

            if (monster.Stats.HP <= 0)
                return true;

            return false;
        }

        public static string GetAttack(Modules.Monster.MONSTERTYPE monsterType)
        {
            switch (monsterType)
            {
                case Modules.Monster.MONSTERTYPE.SLIME:
                    int type = Utils.MathHandler.GetRandomNumber(0, 1);
                    if (type == 0)
                        return "poison";
                    else
                        return "spit";
            }

            return "";
        }

        public static string[] SetCombatOptions(string weaponName)
        {
            string name = weaponName.ToLower();

            if (name == "stonesword" || name == "steelsword")
                return new string[] { "Slash -", "Stab" };

            return new string[] { "Punch -", "Kick", "Yell At" };
        }

        public static int AttackCharacter(ref Modules.Character character, Modules.Monster monster, string attackType)
        {
            switch (attackType.ToLower())
            {
                case "spit":
                    return (int)((monster.Stats.DEX + monster.Stats.Stamina) - character.Stats.DEFENSE);
                case "poison":
                    character.Stats.Illness = Modules.AdvancedStats.ILLNESSES.MOUNTAINFEVER;
                    break;
            }

            return 0;
        }
    }
}
