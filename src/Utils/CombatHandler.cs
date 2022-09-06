using System;

namespace Little_Might.Utils
{
    internal class CombatHandler
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
                    int slimeATTK = Utils.MathHandler.GetRandomNumber(0, 3);
                    if (slimeATTK == 0)
                        return "poison";
                    else
                        return "spit";

                case Modules.Monster.MONSTERTYPE.GOBLIN:
                    int goblinATTK = Utils.MathHandler.GetRandomNumber(0, 3);
                    if (goblinATTK == 0)
                        return "strike";
                    else
                        return "poison";

                case Modules.Monster.MONSTERTYPE.MOLERAT:
                    int moleATTK = Utils.MathHandler.GetRandomNumber(0, 3);
                    if (moleATTK == 0)
                        return "bite";
                    else
                        return "scratch";

                case Modules.Monster.MONSTERTYPE.RABBIT:
                    return "runaway";

                case Modules.Monster.MONSTERTYPE.DEER:
                    return "runaway";

                case Modules.Monster.MONSTERTYPE.CELESTIALHORROR:
                    return "dark strand";
            }

            return "";
        }

        public static string[] SetCombatOptions(string weaponName)
        {
            string name = weaponName.ToLower();

            if (name == "stone sword" || name == "steel sword")
                return new string[] { "Slash -", "Stab" };

            if (name == "wood staff")
                return new string[] { "Hit -", "Firebolt", "Magic Missile" };

            return new string[] { "Punch -", "Kick", "Yell At" };
        }

        public static int AttackCharacter(ref Modules.Character character, Modules.Monster monster, string attackType)
        {
            switch (attackType.ToLower())
            {
                case "spit":
                    int total = (int)((monster.Stats.DEX + monster.Stats.Stamina) - character.Stats.DEFENSE);

                    if (total < 0)
                        total = 0;
                    return total;

                case "bite":
                    total = (int)((monster.Stats.Speed + monster.Stats.DEX) - character.Stats.DEFENSE);

                    if (total < 0)
                        total = 0;
                    return total;

                case "scratch":
                    total = (int)((monster.Stats.Speed + monster.Stats.DEX) - character.Stats.DEFENSE);

                    if (total < 0)
                        total = 0;
                    return total;

                case "strike":
                    total = (int)((monster.Stats.STR + monster.Stats.Speed) - character.Stats.DEFENSE);

                    if (total < 0)
                        total = 0;
                    return total;

                case "poison":
                    character.Stats.Illness = Modules.AdvancedStats.ILLNESSES.MOUNTAINFEVER;
                    break;

                case "dark strand":
                    total = (int)((monster.Stats.INT + monster.Stats.Speed) - character.Stats.DEFENSE);

                    if (total < 0)
                        total = 0;
                    return total;                    
            }

            return 0;
        }
    }
}
