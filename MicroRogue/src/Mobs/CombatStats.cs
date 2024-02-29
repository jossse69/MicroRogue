using MicroRogue.Entities;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroRogue.Mobs
{
    internal class CombatStats
    {
        public Entity Parent { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int Strength { get; set; }
        public int Defense { get; set; }
        public Entity? LastHit { get; set; }
        public CombatStats(Entity parent,int health = 10, int strength = 3, int defense = 3)
        {
            Parent = parent;
            Health = MaxHealth = health;
            Strength = strength;
            Defense = defense;
        }

        public void Attack(Entity target)
        {
            if (target == null || target.Stats == null)
                return;

            int dmg = (Strength * 2) - target.Stats.Defense;
            Text t = new Text(dmg.ToString(), Game.FONT, 16);
            TextEntity text = new TextEntity(target.Position,t, 60, new SFML.System.Vector2f(0, -0.25f));
            text.String.FillColor = Game.COLOR_DARK;
            Game.Entities.Add(text);

            target.Stats.Health = Math.Max(target.Stats.Health - dmg, 0);
            target.Stats.LastHit = Parent;
            if (target.Stats.Health == 0)
                target.DeleteFlag = true;
        }
    }
}
