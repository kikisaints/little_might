using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Little_Might.Utils
{
    class MonsterManager
    {
        private Utils.GraphicsManager _graphicsManager;
        private ContentManager _content;
        private List<Modules.Monster> _allMonsters;

        private double _timer = 0;

        public MonsterManager(ContentManager content, Utils.GraphicsManager gManager, Vector2 playerPos)
        {
            _allMonsters = new List<Modules.Monster>();
            _graphicsManager = gManager;
            _content = content;

            Modules.Monster testMonster = new Modules.Monster("monster_slime", "slime_interaction_img", playerPos + new Vector2(18, 9), _content, Utils.GameColors.MonsterSlimeColor, Modules.Monster.MONSTERTYPE.SLIME);
            _graphicsManager.AddCharacterObject(testMonster);

            _allMonsters.Add(testMonster);
        }

        public void UpdateMonsters(GameTime time, bool overworldActive = true)
        {
            if (overworldActive)
            {
                _timer += time.ElapsedGameTime.TotalSeconds;

                foreach (Modules.Monster monster in _allMonsters)
                {
                    if (_timer >= monster.MoveTime)
                    {
                        _timer = 0;
                        int dir = Utils.MathHandler.GetRandomNumber(0, 4);

                        if (dir == 0)
                        {
                            monster.Position = new Vector2(monster.Position.X + Utils.WorldMap.UNITSIZE, monster.Position.Y);
                        }
                        else if (dir == 1)
                        {
                            monster.Position = new Vector2(monster.Position.X, monster.Position.Y + Utils.WorldMap.UNITSIZE);
                        }
                        else if (dir == 2)
                        {
                            monster.Position = new Vector2(monster.Position.X, monster.Position.Y - Utils.WorldMap.UNITSIZE);
                        }
                        else
                        {
                            monster.Position = new Vector2(monster.Position.X - Utils.WorldMap.UNITSIZE, monster.Position.Y);
                        }
                    }
                }
            }
        }
    }
}
