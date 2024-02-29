using MicroRogue.Entities;
using RogueSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroRogue.Mobs
{
    internal class MobAI
    {
        public Entity Parent { get; set; }

        public MobAI(Entity parent)
        {
            Parent = parent;
        }
        public void DoTurn(Tiles.Map map)
        {
                Point target = new Point(Game.Player.Position.X, Game.Player.Position.Y);

                if (target.X > Parent.Position.X)
                {
                    Parent.TryMove(new SFML.System.Vector2i(Parent.Position.X + 1, Parent.Position.Y), map.imap);
                }
                else if (target.X < Parent.Position.X)
                {
                    Parent.TryMove(new SFML.System.Vector2i(Parent.Position.X - 1, Parent.Position.Y), map.imap);
                }
                else if (target.Y > Parent.Position.Y)
                {
                    Parent.TryMove(new SFML.System.Vector2i(Parent.Position.X, Parent.Position.Y + 1), map.imap);
                }
                else if (target.Y < Parent.Position.Y)
                {
                    Parent.TryMove(new SFML.System.Vector2i(Parent.Position.X, Parent.Position.Y - 1), map.imap);
                }
            
        }
    }
}
