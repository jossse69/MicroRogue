using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroRogue.Entities
{
    internal class TextEntity : Entity
    {
        public Text String { get; set; }
        public int Lifetime { get; set; }
        public Vector2f Speed { get; set; }

        public TextEntity(Vector2i position, Text @string, int lifetime, Vector2f speed) : base("Text Entity", position, null, 4)
        {
            String = @string;
            Lifetime = lifetime;
            Speed = speed;
            IgnoresFOV = true;
        }

        public override void Draw(RenderWindow window)
        {
            Vector2i spritepos = new Vector2i();
            spritepos.X = Position.X * 16;
            spritepos.Y = Position.Y * 16;
            spritepos.X += (int)SpriteOffset.X;
            spritepos.Y += (int)SpriteOffset.Y;
            String.Position = new Vector2f(((float)spritepos.X), (float)spritepos.Y);
            window.Draw(String);
        }

        public void UpdateTextEntity()
        {
            SpriteOffset += Speed;
            Lifetime--;
            if (Lifetime == 0)
            {
                DeleteFlag = true;
            }
        }
    }
}
