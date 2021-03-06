﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Minifice.Enums;
using Microsoft.Xna.Framework.Content;
using Minifice.GameManagement.Movement;

namespace Minifice.GameManagement.Shooting
{
    class Gun : Missile
    {
        Texture2D bullet;
        int r = 25; // rozrzut nabojow przy strzelaniu

        public Gun(Vector2 start, Vector2 target, TimeSpan timeStart, Faction faction, GameTime gameTime)
        {
            power = 1;
            range = 450f;
            speed = 3.5f;
            position = new Vector2(start.X, start.Y);
            this.start = new Vector2(start.X, start.Y);

            this.target = new Vector2(target.X - start.X, target.Y - start.Y);

            if (this.target != Vector2.Zero)
            {
                this.target.Normalize();
                this.target *= range;
                this.target += this.start;
                // Jakaś mała randomizacja przy strzelaniu
                Random rand = new Random();
                this.target += new Vector2(rand.Next(-r, r), rand.Next(-r, r));
            }

            //double s = Math.Sqrt(Math.Pow(this.target.X - this.start.X, 2) + Math.Pow(this.target.Y - this.start.Y, 2));


            List<Vector2> points = new List<Vector2>();
            Vector2 v = new Vector2(target.X - start.X, target.Y - start.Y);
            points.Add(Vector2.Zero);
            if (v != Vector2.Zero)
            {
                v.Normalize();
                v *= 10;
                points.Add(new Vector2(v.X, v.Y));
                Vector2.Transform(v, Matrix.CreateRotationX(MathHelper.PiOver2));
                //points.Add(new Vector2(v.X, v.Y));
                Vector2.Transform(v, Matrix.CreateRotationX(MathHelper.Pi));
                //points.Add(new Vector2(v.X, v.Y));
                
            }
            boundaries = Boundaries.CreateFromPoints(points);
            this.faction = faction;
            timeStartMove = timeStart;
        }

        public override void Load(ContentManager content)
        {
            bullet = content.Load<Texture2D>(@"Game\bulletred");
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            float layerDepth = position.Y * 0.00001f + 0.00001f;
            spriteBatch.Draw(bullet, position, null, Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, layerDepth);
        }

        public override bool Update(GameTime gameTime, GameMap gameMap, List<Fighter> fighters, List<Enemy> enemies)
        {
            float s = speed * (gameTime.ElapsedGameTime.Ticks) * 0.00001f;

            Vector2 shift = new Vector2(target.X - position.X, target.Y - position.Y);

            if (shift != Vector2.Zero)
                shift.Normalize();
            shift *= s;

            if ((position + shift).Similar(target, s)) return true;

            position += shift;

            Object colide;

            if ((colide = Collision(gameMap,fighters,enemies)) != null)
            {
                if (colide is Enemy || colide is Fighter)
                {
                    if (((Unit)colide).IsAlive)
                        ((Unit)colide).Die(fighters);
                }
                return true;
            }

            return false;
        }
        
        
        
    }
}
