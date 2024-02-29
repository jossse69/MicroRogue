using RogueSharp;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroRogue.Tiles
{
    internal class Map
    {


        public int Width;
        public int Height;
        public Tile[,] Tiles;
        public bool[,] PlayerFOV;
        public IMap imap;
        RogueSharp.PathFinder PathFinder;
        public Map(int width, int height)
        {
            Width = width;
            Height = height;
            Tiles = new Tile[Width, Height];
            PlayerFOV = new bool[Width, Height];
            imap = new RogueSharp.Map(width, height);
            PathFinder = new RogueSharp.PathFinder(imap);
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Tiles[x, y] = new Tile(null);
                    PlayerFOV[x, y] = false;
                }
            }
            UpdateIMap();
        }

        public void SetTile(int x, int y, Tile tile)
        {
            if (x < 0 || y < 0 || x > Width - 1 || y > Height - 1)
            {
                return;
            }
            Tiles[x, y] = tile;
            UpdateIMap();
        }

        public Tile? GetTile(int x, int y)
        {
            if (x < 0 || y < 0 || x > Width - 1 || y > Height - 1)
            {
                return null;
            }
            return Tiles[x, y];
        }

        public ReadOnlyCollection<ICell> CalculateFOV(Vector2i pos, int radius)
        {
            return imap.ComputeFov(pos.X, pos.Y, radius, true);
        }

        public void ClearPlayerFOV()
        {
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    PlayerFOV[x, y] = false;
        }
        public bool IsOnPlayerFOV(Vector2i point)
        {
            return PlayerFOV[point.X, point.Y];
        }
        public RogueSharp.Path? GetAStarPath(Vector2i start, Vector2i end)
        {
            RogueSharp.Path path = PathFinder.TryFindShortestPath(imap.GetCell(start.X, start.Y), imap.GetCell(end.X, end.Y));
            return path;
        }
        public void Draw(RenderWindow window)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (PlayerFOV[x, y] == false)
                    {
                        Vector2i spritepos = new Vector2i();
                        spritepos.X = x * 16;
                        spritepos.Y = y * 16;
                        RectangleShape rectangle = new RectangleShape(new Vector2f(16, 16));
                        rectangle.Position = new Vector2f((float)spritepos.X, (float)spritepos.Y);
                        rectangle.FillColor = Game.COLOR_DARK;
                        window.Draw(rectangle);
                    }
                    else if (Tiles[x, y].Sprite != null)
                    {
                        Vector2i spritepos = new Vector2i();
                        spritepos.X = x * 16;
                        spritepos.Y = y * 16;
                        Sprite sprite = Tiles[x, y].Sprite;
                        sprite.Position = new Vector2f((float)spritepos.X, (float)spritepos.Y);
                        window.Draw(sprite);
                    }
                }
            }
        }

        public void UpdateIMap()
        {
            imap.Clear();
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    imap.SetCellProperties(x, y, Tiles[x, y].IsTransparent, Tiles[x, y].IsWalkable);
                }
            }
        }
    }
}
