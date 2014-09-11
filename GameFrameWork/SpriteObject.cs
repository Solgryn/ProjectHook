using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectHook;
using ProjectHook.GameFrameWork;

namespace GrappleRace.GameFrameWork
{
    public class SpriteObject : GameObjectBase
    {

        //-------------------------------------------------------------------------------------
        // Class constructors

        public SpriteObject(GameHost game)
            : base(game)
        {
            // Set the default scale and color
            ScaleX = 1;
            ScaleY = 1;
            SpriteColor = Color.White;
            spriteEffects = SpriteEffects.None;
        }

        public SpriteObject(GameHost game, Vector2 position)
            : this(game)
        {
            // Store the provided position
            Position = position;
        }

        public SpriteObject(GameHost game, Vector2 position, Texture2D texture)
            : this(game, position)
        {
            // Store the provided texture
            SpriteTexture = texture;
        }

        //-------------------------------------------------------------------------------------
        // Properties

        /// <summary>
        /// A reference to the default texture used by this sprite
        /// </summary>
        public virtual Texture2D SpriteTexture { get; set; }

        /// <summary>
        /// The sprite's X coordinate
        /// </summary>
        public virtual float PositionX { get; set; }
        /// <summary>
        /// The sprite's Y coordinate
        /// </summary>
        public virtual float PositionY { get; set; }
        
        /// <summary>
        /// The sprite's origin X coordinate
        /// </summary>
        public virtual float OriginX { get; set; }
        /// <summary>
        /// The sprite's origin Y coordinate
        /// </summary>
        public virtual float OriginY { get; set; }
        
        /// <summary>
        /// The sprite's rotation angle (in radians)
        /// </summary>
        public virtual float Angle { get; set; }

        /// <summary>
        /// The sprite's X scale
        /// </summary>
        public virtual float ScaleX { get; set; }
        /// <summary>
        /// The sprite's Y scale
        /// </summary>
        public virtual float ScaleY { get; set; }

        /// <summary>
        /// An optional source rectangle to read from the sprite texture
        /// </summary>
        public virtual Rectangle SourceRect { get; set; }

        /// <summary>
        /// The sprite's color
        /// </summary>
        public virtual Color SpriteColor { get; set; }

        /// <summary>
        /// The sprite's layer depth
        /// </summary>
        public virtual float LayerDepth { get; set; }

        /// <summary>
        /// The sprite's position represented as a Vector2 structure
        /// </summary>
        public Vector2 Position
        {
            get
            {
                return new Vector2(PositionX, PositionY);
            }
            set
            {
                PositionX = value.X;
                PositionY = value.Y;
            }
        }

        /// <summary>
        /// The sprite's origin represented as a Vector2 structure
        /// </summary>
        public Vector2 Origin
        {
            get
            {
                return new Vector2(OriginX, OriginY);
            }
            set
            {
                OriginX = value.X;
                OriginY = value.Y;
            }
        }

        /// <summary>
        /// The sprite's scale represented as a Vector2 structure
        /// </summary>
        public Vector2 Scale
        {
            get
            {
                return new Vector2(ScaleX, ScaleY);
            }
            set
            {
                ScaleX = value.X;
                ScaleY = value.Y;
            }
        }

        //Change sprite effects
        public SpriteEffects spriteEffects { get; set; }


        /// <summary>
        /// Calculate a simple bounding box for the sprite
        /// </summary>
        /// <remarks>Note that this doesn't currently take rotation into account so that
        /// the box size remains constant when rotating.</remarks>
        public virtual Rectangle BoundingBox
        {
            get
            {
                Rectangle result;
                Vector2 spritesize;
                var spriteoffset = new Vector2(0, 0);

                if (_boundingBox.IsEmpty)
                {
                    if (SourceRect.IsEmpty)
                    {
                        // The size is that of the whole texture
                        spritesize = new Vector2(SpriteTexture.Width, SpriteTexture.Height);
                    }
                    else
                    {
                        // The size is that of the rectangle
                        spritesize = new Vector2(SourceRect.Width, SourceRect.Height);
                    }
                }
                else
                {
                    spriteoffset = new Vector2(_boundingBox.X, _boundingBox.Y);
                    spritesize = new Vector2(_boundingBox.Width, _boundingBox.Height);
                }
                

                // Build a rectangle whose position and size matches that of the sprite
                // (taking scaling into account for the size)
                result = new Rectangle((int)PositionX, (int)PositionY, (int)(spritesize.X * ScaleX), (int)(spritesize.Y * ScaleY));

                // Offset the sprite by the origin
                result.Offset((int)(-OriginX * ScaleX), (int)(-OriginY * ScaleY));


                result.Offset((int)(spriteoffset.X), (int)(spriteoffset.Y));

                // Return the finished rectangle
                return result;
            }
            set { _boundingBox = value; }
        }

        private Rectangle _boundingBox;

        //-------------------------------------------------------------------------------------
        // Game functions

        /// <summary>
        /// Draw the sprite using its default settings
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="spriteBatch"></param>
        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Do we have a texture? If not then there is nothing to draw...
            if (SpriteTexture != null)
            {
                //Round off position to reduce blurryness (CUSTOM)
                var roundedPosition = new Vector2((float)Math.Round(Position.X), (float)Math.Round(Position.Y))
                    + new Vector2((float)Math.Round(-Camera.Position.X), (float)Math.Round(-Camera.Position.Y)) ;
                //+ new Vector2(Camera.Width/2f, Camera.Height/2f)

                // Has a source rectangle been set?
                if (SourceRect.IsEmpty)
                {
                    // No, so draw the entire sprite texture
                    spriteBatch.Draw(SpriteTexture, roundedPosition * Camera.Scale, null, SpriteColor, Angle, Origin, Scale * Camera.Scale, spriteEffects, LayerDepth);
                }
                else
                {
                    // Yes, so just draw the specified SourceRect
                    spriteBatch.Draw(SpriteTexture, roundedPosition * Camera.Scale, SourceRect, SpriteColor, Angle, Origin, Scale * Camera.Scale, spriteEffects, LayerDepth);
                }
            }
        }

        //If object overlaps another
        public virtual bool Overlaps(SpriteObject other)
        {
            return BoundingBox.Intersects(other.BoundingBox);
        }

        //Check if the object is overlapping a tile from a layer with a specific layer name
        public bool OverlapsTileLayer(Rectangle hitbox, string layerName)
        {
            foreach (var tile in Collections.Tiles)
            {
                if (hitbox.Intersects(tile.BoundingBox) && tile.LayerName == layerName)
                    return true;
            }
            return false;
        }

        //Get -1 if facing left, 1 is facing right
        public int GetDirection()
        {
            if (spriteEffects == SpriteEffects.FlipHorizontally) return -1;
            if (spriteEffects == SpriteEffects.None) return 1;
            return 0;
        }

        //Remove the object from the game
        public virtual void Destroy()
        {
            Game.GameObjects.Remove(this);
        }

        //Check if object is out of the visible screen
        public bool IsOutOfFrame(float offset = 0)
        {
            return (PositionX + offset < Camera.Position.X ||
                    PositionX - offset > Camera.Position.X + Camera.Width ||
                    PositionY + offset < Camera.Position.Y ||
                    PositionY - offset > Camera.Position.Y + Camera.Height);
        }
    }
}
