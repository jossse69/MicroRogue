using System;
using System.Collections.Generic;
using MicroRogue.Entities;
using MicroRogue.Tiles;
using MicroRogue.UI;
using RogueSharp;
using SFML.Graphics;
using SFML.System;
using SFML.Window;


namespace MicroRogue
{
    internal class Game
    {
        // colors
        public static Color COLOR_LIGHT = new Color(251, 255, 250);
        public static Color COLOR_DARK = new Color(7, 41, 0);

        // player
        public static int Coins = 0;

        // font
        public static Font FONT;

        // map
        public static MicroRogue.Tiles.Map GameMap;

        // tiles
        public static Tile WALL;

        // entities
        public static List<Entity> Entities = new List<Entity>();
        public static Entity Player;

        public void Start()
        {

            RenderWindow window = new RenderWindow(new VideoMode(512, 512), "Micro Rogue");
            bool isRunning = true;
            window.Closed += (sender, args) =>
            {
                isRunning = false;
            };
            window.KeyPressed += HandleKeys;

            FONT = new Font("assets/silkscreen.ttf");

            Assetloader.LoadTextures();

            WALL = new Tile(Assetloader.BuildSprite("walltile", new IntRect(0, 0, 16, 16)), false, false);

            GameMap = new MicroRogue.Tiles.Map(32, 27); // 27 hight for the map for space for the gui 

            Player = new Entity("Player",new Vector2i(1, 1), Assetloader.BuildSprite("player", new IntRect(0, 0, 16, 16)), 2);
            Player.Type = EntityType.Player;
            Player.Stats = new Mobs.CombatStats(Player ,30, 3, 3);
            Entities.Add(Player);

            GenerateMap();
            PlayerFOV();

            while (isRunning)
            {
                window.DispatchEvents();

                DeleteTheDead();

                window.Clear(COLOR_LIGHT);

                GameMap.Draw(window);

                List<Entity> orderedentities =
                [
                    .. Entities,
                ];

                // order entities via render layer
                orderedentities.Sort((a, b) => a.RenderLayer.CompareTo(b.RenderLayer));

                foreach (Entity entity in orderedentities)
                {
                    if (GameMap.PlayerFOV[entity.Position.X, entity.Position.Y] == false && entity.IgnoresFOV == false)
                        continue;

                    if (entity is TextEntity text)
                    {
                        text.UpdateTextEntity();
                    }

                    entity.Draw(window);
                }

                GUI.Draw(window);

                window.Display();
            }

            window.Dispose();

        }

        private void HandleKeys(object? sender, KeyEventArgs e)
        {
            var keycode = e.Code;
            bool moved = false;

            switch (keycode)
            {
                case Keyboard.Key.Right:
                    Player.TryMove(new Vector2i(Player.Position.X + 1, Player.Position.Y), GameMap.imap);
                    moved = true;
                    break;
                case Keyboard.Key.Left:
                    Player.TryMove(new Vector2i(Player.Position.X - 1, Player.Position.Y), GameMap.imap);
                    moved = true;
                    break;
                case Keyboard.Key.Up:
                    Player.TryMove(new Vector2i(Player.Position.X, Player.Position.Y -1), GameMap.imap);
                    moved = true;
                    break;
                case Keyboard.Key.Down:
                    Player.TryMove(new Vector2i(Player.Position.X, Player.Position.Y + 1), GameMap.imap);
                    moved = true;
                    break;
            }

            if (moved)
            {
                PlayerFOV();
                EnemyTurn();
            }
        }

        private static void GenerateMap()
        {
            Point startpos = new Point(0, 0);
            List<Rectangle> rooms = new List<Rectangle>();

            // frist, set all tiles to walls
            for (int x = 0; x < GameMap.Width; x++)
            {
                for (int y = 0; y < GameMap.Height; y++)
                {
                    GameMap.SetTile(x, y, WALL);
                }
            }

            // generate rooms
            for (int i = 0; i < 5; i++)
            {
                int tryies = 100;
                while (tryies > 0)
                {
                    int w = Random.Shared.Next(3, 5);
                    int h = Random.Shared.Next(3, 5);
                    int x = Random.Shared.Next(0, GameMap.Width - w);
                    int y = Random.Shared.Next(0, GameMap.Height - h);
                    Rectangle r = new Rectangle(x, y, w, h);

                    bool Intersects = false;
                    foreach (Rectangle room2 in rooms)
                    {
                        if (room2.Intersects(r))
                        {
                            Intersects = true;
                        }
                            
                    }

                    if (!Intersects)
                    {
                        rooms.Add(r);
                        break;
                    }

                    tryies--;
                }

            }

            startpos = rooms[0].Center;
            Player.Position = new Vector2i(startpos.X, startpos.Y);

            // generate hallways
            foreach (Rectangle room in rooms)
            {
                foreach (Rectangle target in rooms)
                {
                    Point brushpos = room.Center;
                    Point targetpos = target.Center;

                    while (brushpos != targetpos)
                    {
                        if (brushpos.X < targetpos.X)
                        {
                            brushpos.X++;
                        }
                        else if (brushpos.X > targetpos.X)
                        {
                            brushpos.X--;
                        }
                        else if (brushpos.Y < targetpos.Y)
                        {
                            brushpos.Y++;
                        }
                        else if (brushpos.Y > targetpos.Y)
                        {
                            brushpos.Y--;
                        }

                        GameMap.SetTile(brushpos.X, brushpos.Y, new Tile(null));
                    }
                }
            }

            foreach (Rectangle room in rooms)
            {

                // set tiles
                for (int y = 0; y < room.Height; y++)
                {
                    for (int x = 0; x < room.Width; x++)
                    {
                        GameMap.SetTile(room.X + x, room.Y + y, new Tile(null));
                    }
                }

            }


            // add border
            for (int x = 0; x < GameMap.Width; x++)
            {
                for (int y = 0; y < GameMap.Height; y++)
                {
                     // check if the wall is on the border
                    if (x == 0 || x == GameMap.Width - 1 || y == 0 || y == GameMap.Height - 1)
                    {
                        GameMap.SetTile(x, y, WALL);
                    }
                }
            }

            Sprite SekeletonSprite = Assetloader.BuildSprite("skeleton", new IntRect(0, 0, 16, 16)); // reuse texture and sprite for performance

            // add a few sekeletons mobs to the map
            for (int i = 0; i < 10; i++)
            {
                int x = 0;
                int y = 0;

                while(!GameMap.GetTile(x, y).IsWalkable)
                {
                    x = Random.Shared.Next(0, GameMap.Width);
                    y = Random.Shared.Next(0, GameMap.Height);
                }

                Entity mob = new Entity("Skeleton", new Vector2i(x, y), SekeletonSprite, 1);
                mob.Type = EntityType.Monster;
                mob.Stats = new Mobs.CombatStats(mob,15, 2, 1);
                mob.MobAI = new Mobs.MobAI(mob);
                Entities.Add(mob);

            }
        }

        public void EnemyTurn()
        {
            for (int i = 0; i < Entities.Count; i++)
            {
                Entity entity = Entities[i];
                if (entity.MobAI != null && GameMap.IsOnPlayerFOV(entity.Position))
                {
                    entity.MobAI.DoTurn(GameMap);
                }

            }
        }

        private static void PlayerFOV()
        {
            GameMap.ClearPlayerFOV();

            var fov = GameMap.CalculateFOV(Player.Position, 5);
            
            foreach (var cell in fov)
            {
                GameMap.PlayerFOV[cell.X, cell.Y] = true;
            }
        }

        private void DeleteTheDead()
        {
            List<Entity> ToDelete = new List<Entity>();

            foreach (Entity entity in Entities)
            {
                if (entity.DeleteFlag)
                    ToDelete.Add(entity);
            }

            foreach (Entity entity in ToDelete)
            {
                // chance to spawn a item if its a enemy and was killed by the player
                if (entity.Type == EntityType.Monster && entity.Stats.LastHit.Type == EntityType.Player)
                {
                    if (Random.Shared.Next(0, 10) >= 3)
                    {
                        SpawnRandomItem(entity.Position);
                    }
                }

                Entities.Remove(entity);
            }

        }

        public void SpawnRandomItem(Vector2i pos)
        {

            int type = Random.Shared.Next(10);

            if (type <= 4) // coin
            {
                Sprite sprite = Assetloader.BuildSprite("coin", new IntRect(0, 0, 16, 16));
                Entity entity = new Entity("Coin", pos, sprite, 0);
                entity.Type = EntityType.PickUp;
                entity.Value = 1;
                Entities.Add(entity);
                return;
            }
            else if (type >= 5) // healing potion
            {
                Sprite sprite = Assetloader.BuildSprite("healingpotion", new IntRect(0, 0, 16, 16));
                Entity entity = new Entity("Healing Potion", pos, sprite, 0);
                entity.Type = EntityType.PickUp;
                entity.HealAmount = 8;
                Entities.Add(entity);
                return;
            }
        }
    }
}
