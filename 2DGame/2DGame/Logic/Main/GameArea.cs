using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2DGame.Logic
{
    public class GameArea : GameObject
    {
        public GameArea(Game game)
            : base(game)
        { }

        public GameArea(Game game, Rectangle Position, double SpeedX, double SpeedY)
            : this(game)
        {
            
            this.Position = Position;
            this.LocalPosition = Position;
            this.SpeedX = SpeedX;
            this.SpeedY = SpeedY;
        }
    }
}
