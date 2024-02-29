using MicroRogue.Mobs;
using RogueSharp;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroRogue.Entities
{
    public enum EntityType
    {
        Undefined,
        Player,
        Monster,
        Door,
        PickUp
    }
    internal class Entity
    {
        public string Name;
        public EntityType Type;
        public Vector2i Position;
        public Vector2f SpriteOffset; // offset of the sprite relative to the screen position of this entity.
        public Sprite? Sprite;
        public int RenderLayer; // the render layer of this entity, which determines the order of rendering the sprite
        public bool DeleteFlag = false; // whether to delete this entity
        public bool IgnoresFOV = false; // whether to ignore the field of view hiding of this entity
        public int Value; // the value of this entity in coins for if its selled or if its a coin, colected coins to add
        public int HealAmount; // the amount of health to add to the entity that colected this entity

        // extra mods
        public CombatStats? Stats; // the stats of this entity for combat related purposes
        public MobAI? MobAI; // the AI of this entity, used in mobs

        public Entity(string name,Vector2i position, Sprite? sprite, int renderLayer)
        {
            Position = position;
            SpriteOffset = new Vector2f(0, 0);
            Sprite = sprite;
            RenderLayer = renderLayer;
            Name = name;
        }

        public virtual void TryMove(Vector2i pos, IMap map)
        {
            List<Entity> entities = Game.Entities;

            // flip sprite if moving horizontally
            if (pos.X != Position.X && Sprite != null)
            {
                if (pos.X < Position.X)
                {
                    Sprite.Scale = new Vector2f(-1, 1);
                    Sprite.Origin = new Vector2f(16, 0);
                }
                else
                {
                    Sprite.Scale = new Vector2f(1, 1);
                    Sprite.Origin = new Vector2f(0, 0);
                }
            }

            if (map.GetCell(pos.X, pos.Y).IsWalkable)
            {
                foreach(Entity entity in entities)
                {
                    if (pos == entity.Position)
                    {
                        if (entity.Stats != null && Stats != null)
                        {
                            if (entity.Type != Type)
                            {
                                Stats.Attack(entity);
                                return;
                            }
                            else
                            {
                                return;
                            }
                            
                        }

                        if (Type == EntityType.Player && entity.Type == EntityType.PickUp)
                        {
                            Game.Coins += entity.Value;
                            Stats.Health = Math.Min(Stats.MaxHealth, Stats.Health + entity.HealAmount);
                            entity.DeleteFlag = true;
                        }
                    }
                }

                Position = pos;
            }
        }

        public virtual void Draw(RenderWindow window)
        {
            if (Sprite != null)
            {
                Vector2i spritepos = new Vector2i();
                spritepos.X = Position.X * 16;
                spritepos.Y = Position.Y * 16;
                spritepos.X += (int)SpriteOffset.X;
                spritepos.Y += (int)SpriteOffset.Y;
                Sprite.Position = new Vector2f((float)spritepos.X, (float)spritepos.Y);
                window.Draw(Sprite);
            }
            
        }
    }
}
