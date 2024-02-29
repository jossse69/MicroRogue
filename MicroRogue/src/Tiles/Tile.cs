using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroRogue.Tiles
{
    internal class Tile
    {
        public Sprite? Sprite;
        public bool IsWalkable;
        public bool IsTransparent;

        public Tile(Sprite? sprite, bool iswalkble = true, bool isTransparent = true)
        {
            Sprite = sprite;
            IsWalkable = iswalkble;
            IsTransparent = isTransparent;
        }
    }
}
