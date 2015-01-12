using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2DGame.Logic
{
    public class GameBonus : DrawableGameObject
    {
        /// <summary>
        /// Тип бонуса
        /// </summary>
        public String Type
        { get; set; }

        /// <summary>
        /// Значение бонуса
        /// </summary>
        public double Value
        { get; set; }

        public GameBonus(Game game)
            : base(game)
        { 
            this.Initialize();
            this.LoadContent();
        }

        public GameBonus(Game game, Game1 Parent, String spriteName, Rectangle Position, String Type, double Value)
            : this(game)
        {
            this.changeSprite(spriteName);
            this.Position = Position;
            this.Parent = Parent;
            this.Type = Type;
            this.Value = Value;
        }
    }
}
