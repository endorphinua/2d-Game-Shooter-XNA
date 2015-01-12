using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2DGame.Logic
{
    public class GameBlock : DrawableGameObject
    {
        public GameBlock(Game game)
            : base(game)
        {
            this.Initialize();
            this.LoadContent();
        }

        public GameBlock(Game game, String SpriteName, Rectangle Position)
            : this(game)
        {
            this.changeSprite(SpriteName);
            this.Position = Position;
            this.LoadContent();
        }
    }
}
