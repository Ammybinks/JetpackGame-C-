using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using SpriteLibrary;
using CameraLibrary;

namespace New_Program
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Random rand = new Random();

        LinkedList<Sprite> activeSprites = new LinkedList<Sprite>();
        LinkedList<Sprite> activeProjectiles = new LinkedList<Sprite>();
        LinkedList<Sprite> floors = new LinkedList<Sprite>();
        LinkedList<Sprite> walls = new LinkedList<Sprite>();
        LinkedList<Sprite> temp = new LinkedList<Sprite>();

        KeyboardState currentKeyState;
        KeyboardState oldKeyState;

        MouseState currentMouseState;
        MouseState oldMouseState;

        Vector2 screenSize;

        SpriteFont Calibri;

        bool fired;
        bool firing;
        bool walking;
        bool jumped;
        bool jumping;
        bool floored;
        float jumpSpeed;
        float verticalCompensation = 0;
        float verticalAddition = 0;
        float horizontalCompensation = 0;
        float horizontalAddition = 0;
        int mana = 100;

        Sprite playerSprite;
        Texture2D playerTexture;

        Sprite projectileSprite;
        Texture2D projectileTexture;

        Sprite floorSprite;
        Texture2D floorTexture;
        Sprite smallFloorSprite;
        Texture2D smallFloorTexture;
        Sprite floor1;
        Texture2D floor1Texture;
        Sprite floor2;
        Texture2D floor2Texture;

        Camera camera;

        int projectileCount = 0;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            IsMouseVisible = true;
            screenSize = new Vector2(1200, 800);
            graphics.PreferredBackBufferWidth = (int)screenSize.X;
            graphics.PreferredBackBufferHeight = (int)screenSize.Y;
            graphics.ApplyChanges();

            Calibri = Content.Load<SpriteFont>("Fonts\\Calibri");
            playerTexture = Content.Load<Texture2D>("Textures\\Just a Box");
            floorTexture = Content.Load<Texture2D>("Textures\\Just a Floor");
            smallFloorTexture = Content.Load<Texture2D>("Textures\\Just a Small Floor");
            floor1Texture = Content.Load<Texture2D>("Textures\\Floor1");
            floor2Texture = Content.Load<Texture2D>("Textures\\Floor2");
            projectileTexture = Content.Load<Texture2D>("Textures\\Flamey Thingy");


            playerSprite = new Sprite();
            playerSprite.SetTexture(playerTexture);
            playerSprite.UpperLeft = new Vector2(450, 0);
            playerSprite.MaxSpeed = 10;
            activeSprites.AddLast(playerSprite);


            floorSprite = new Sprite();
            floorSprite.SetTexture(floorTexture);
            floorSprite.UpperLeft = new Vector2(260, 680);
            activeSprites.AddLast(floorSprite);
            walls.AddLast(floorSprite);

            smallFloorSprite = new Sprite();
            smallFloorSprite.SetTexture(smallFloorTexture);
            smallFloorSprite.UpperLeft = new Vector2(260, 380);
            activeSprites.AddLast(smallFloorSprite);
            walls.AddLast(smallFloorSprite);
            smallFloorSprite = new Sprite();
            smallFloorSprite.SetTexture(smallFloorTexture);
            smallFloorSprite.UpperLeft = new Vector2(1360, 380);
            activeSprites.AddLast(smallFloorSprite);
            walls.AddLast(smallFloorSprite);

            floor1 = new Sprite();
            floor1.SetTexture(floor1Texture);
            floor1.UpperLeft = new Vector2(260, 680);
            activeSprites.AddLast(floor1);
            floors.AddLast(floor1);

            floor2 = new Sprite();
            floor2.SetTexture(floor2Texture);
            floor2.UpperLeft = new Vector2(260, 380);
            activeSprites.AddLast(floor2);
            floors.AddLast(floor2);
            floor2 = new Sprite();
            floor2.SetTexture(floor2Texture);
            floor2.UpperLeft = new Vector2(1360, 380);
            activeSprites.AddLast(floor2);
            floors.AddLast(floor2);

            camera = new Camera();
            camera.WorldHeight = 1080;
            camera.WorldWidth = 1920;
            camera.ViewHeight = (int)screenSize.Y;
            camera.ViewWidth = (int)screenSize.X;
            camera.UpperLeft = new Vector2((playerSprite.UpperLeft.X - playerSprite.GetWidth() / 2) - (camera.ViewWidth / 2),
                                                            (playerSprite.UpperLeft.Y + playerSprite.GetHeight() / 2) - (camera.ViewHeight / 2));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            currentKeyState = Keyboard.GetState();
            currentMouseState = Mouse.GetState();

            if (oldKeyState == null)
            {
                oldKeyState = currentKeyState;
            }
            if (oldMouseState == null)
            {
                oldMouseState = currentMouseState;
            }

            foreach (Sprite floor in floors)
            {
                if (playerSprite.IsCollided(floor))
                {
                    if (playerSprite.GetVelocity().Y > 0)
                    {
                        if (playerSprite.AccelerateY(0.1f, 0 * -1) < 0 == false)
                        {
                            playerSprite.UpperLeft = new Vector2(playerSprite.UpperLeft.X, floor.UpperLeft.Y - playerSprite.GetHeight() + 1);
                        }
                        playerSprite.SetVelocity(playerSprite.GetVelocity().X, 0);

                        jumped = false;
                        jumping = false;
                        jumpSpeed = -1f;

                        if (!firing)
                        {
                            fired = false;
                            verticalCompensation = 0;
                        }
                    }

                    floored = true;
                }
            }
            
            walking = false;

            if (currentKeyState.IsKeyDown(Keys.A))
            {
                playerSprite.Accelerate(-0.2f, 0);
                walking = true;
                if (playerSprite.GetVelocity().X > 0)
                {
                    playerSprite.Accelerate(-playerSprite.windResistance, 0);
                }
            }

            if (currentKeyState.IsKeyDown(Keys.D))
            {
                playerSprite.Accelerate(0.2f, 0);
                walking = true;
                if (playerSprite.GetVelocity().X < 0)
                {
                    playerSprite.Accelerate(playerSprite.windResistance, 0);
                }
            }

            if ((currentMouseState.LeftButton == ButtonState.Pressed) && (mana > 0))
            {
                double direction = Sprite.CalculateDirectionAngle(new Vector2(currentMouseState.X - ((playerSprite.UpperLeft.X - camera.UpperLeft.X) + playerSprite.GetWidth() / 2),
                                                                             (currentMouseState.Y - ((playerSprite.UpperLeft.Y - camera.UpperLeft.Y) - playerSprite.GetHeight() / 2))));

                int deviation = rand.Next(0, 15);

                direction -= 7.5;
                direction += deviation;

                projectileSprite = new Sprite();
                projectileSprite.SetTexture(projectileTexture);
                projectileSprite.Scale = new Vector2(4, 4);
                projectileSprite.RotationAngle = direction;
                projectileSprite.SetSpeedAndDirection(8, direction);
                projectileSprite.UpperLeft = new Vector2(playerSprite.UpperLeft.X + playerSprite.GetWidth() / 2,
                                                         playerSprite.UpperLeft.Y + playerSprite.GetHeight() / 2);
                projectileSprite.SetCollisions();
                activeProjectiles.AddFirst(projectileSprite);
                activeSprites.AddFirst(projectileSprite);
                
                //projectileSprite = new Sprite();
                //projectileSprite.SetTexture(projectileTexture);
                //projectileSprite.Scale = new Vector2(4, 4);
                //projectileSprite.RotationAngle = direction;
                //projectileSprite.SetSpeedAndDirection(8, direction);
                //projectileSprite.UpperLeft = new Vector2(playerSprite.UpperLeft.X + playerSprite.GetWidth() / 2,
                //                                         playerSprite.UpperLeft.Y + playerSprite.GetHeight() / 2);
                //activeProjectiles.AddFirst(projectileSprite);
                //activeSprites.AddFirst(projectileSprite);

                //deviation = rand.Next(0, 15);

                //direction -= 7.5;
                //direction += deviation;

                projectileSprite = new Sprite();
                projectileSprite.SetTexture(projectileTexture);
                projectileSprite.Scale = new Vector2(4, 4);
                projectileSprite.RotationAngle = direction;
                projectileSprite.SetSpeedAndDirection(8, direction);
                projectileSprite.UpperLeft = new Vector2(playerSprite.UpperLeft.X + playerSprite.GetWidth() / 2,
                                                         playerSprite.UpperLeft.Y + playerSprite.GetHeight() / 2);
                projectileSprite.SetCollisions();
                activeProjectiles.AddFirst(projectileSprite);
                activeSprites.AddFirst(projectileSprite);

                deviation = rand.Next(0, 30);

                direction -= 15;
                direction += deviation;

                projectileSprite = new Sprite();
                projectileSprite.SetTexture(projectileTexture);
                projectileSprite.Scale = new Vector2(4, 4);
                projectileSprite.RotationAngle = direction;
                projectileSprite.SetSpeedAndDirection(8, direction);
                projectileSprite.UpperLeft = new Vector2(playerSprite.UpperLeft.X + playerSprite.GetWidth() / 2,
                                                         playerSprite.UpperLeft.Y + playerSprite.GetHeight() / 2);
                projectileSprite.SetCollisions();
                activeProjectiles.AddFirst(projectileSprite);
                activeSprites.AddFirst(projectileSprite);

                direction = Sprite.CalculateDirectionAngle(new Vector2(((playerSprite.UpperLeft.X - camera.UpperLeft.X) + playerSprite.GetWidth() / 2) - currentMouseState.X,
                                                                      (((playerSprite.UpperLeft.Y - camera.UpperLeft.Y) - playerSprite.GetHeight() / 2) - currentMouseState.Y) * -1));

                if (floored)
                {
                    playerSprite.AccelerateX(0.01f, direction * -1);
                    horizontalCompensation = 0;
                    horizontalAddition = 0.1f;
                }
                else
                {
                    playerSprite.AccelerateX(horizontalCompensation, direction * -1);
                    if (horizontalAddition < 0.2)
                    {
                        horizontalCompensation += horizontalAddition;
                        horizontalAddition += 0.01f;
                    }
                }
                if ((direction > 90) && (direction <= 270))
                {
                    if (playerSprite.GetVelocity().X > 0)
                    {
                        playerSprite.Accelerate(-playerSprite.windResistance, 0);
                    }
                }
                if (((direction > 270) && (direction <= 360)) || ((direction > 0) && (direction <= 90)))
                {
                    if (playerSprite.GetVelocity().X < 0)
                    {
                        playerSprite.Accelerate(playerSprite.windResistance, 0);
                    }
                }

                if (playerSprite.AccelerateY(0.1f, direction * -1) < 0)
                {
                    playerSprite.AccelerateY(verticalCompensation, direction * -1);
                    if (verticalAddition > 0)
                    {
                        verticalCompensation += verticalAddition;
                        verticalAddition -= 0.01f;
                    }
                }
                else
                {
                    verticalCompensation = 0;
                    verticalAddition = 0.1f;
                }

                mana -= 1;

                fired = true;
                firing = true;
            }
            else if ((currentMouseState.LeftButton == ButtonState.Released))
            {
                mana = 100;

                firing = false;

                verticalCompensation = 0;
                verticalAddition = 0.1f;
            }
            else
            {
                firing = false;

                verticalCompensation = 0;
                verticalAddition = 0.1f;
            }

            if (currentKeyState.IsKeyDown(Keys.W) && oldKeyState.IsKeyUp(Keys.W))
            {
                if (jumped == false)
                {
                    playerSprite.SetVelocity(playerSprite.GetVelocity().X, -10);
                    playerSprite.UpperLeft.Y -= 10;
                    jumped = true;
                    jumping = true;

                    jumpSpeed = -0.6f;
                }
            }
            else if (currentKeyState.IsKeyDown(Keys.W) && jumped == true && jumping == true)
            {
                if (jumpSpeed < 0)
                {
                    playerSprite.Accelerate(0, jumpSpeed);
                    jumpSpeed += 0.015f;
                }
            }
            else
            {
                jumping = false;
            }


            if (!floored)
            {
                playerSprite.Accelerate(0, playerSprite.gravity);
            }

            if ((!walking) && (!firing))
            {
                if (playerSprite.GetVelocity().X > 0)
                {
                    playerSprite.Accelerate(-playerSprite.windResistance, 0);
                }
                if (playerSprite.GetVelocity().X < 0)
                {
                    playerSprite.Accelerate(playerSprite.windResistance, 0);
                }
                if ((playerSprite.GetVelocity().X < 0.5) && (playerSprite.GetVelocity().X > -0.5) && (playerSprite.GetVelocity().X != 0))
                {
                    playerSprite.SetVelocity(0, playerSprite.GetVelocity().Y);
                }
            }

            temp.Clear();
            projectileCount = 0;

            foreach (Sprite projectile in activeProjectiles)
            {
                projectile.tempAccelerating = false;

                foreach (Sprite floor in floors)
                {
                    Rectangle rectangle = projectile.GetBoundingRectangle();

                    if ((((rectangle.X) + (rectangle.Width / 2)) > (floor.UpperLeft.X)) &&
                        (((rectangle.X) + (rectangle.Width / 2)) < (floor.UpperLeft.X + floor.GetWidth())) &&
                        (((rectangle.Y) + (rectangle.Height / 2)) > (floor.UpperLeft.Y)) &&
                        (((rectangle.Y) + (rectangle.Height / 2)) < (floor.UpperLeft.Y + floor.GetHeight())))
                    {
                        projectile.SetTempVelocity(projectile.GetVelocity().X, 0);

                        projectile.tempAccelerating = true;

                        break;
                    }
                }

                projectile.Scale = new Vector2(projectile.Scale.X - 0.05f, projectile.Scale.Y - 0.05f);
                projectile.AccelerateDirection(-0.1, projectile.GetDirectionAngle());

                projectile.Move();

                projectileCount += 1;

                temp.AddFirst(projectile);
            }

            foreach (Sprite sprite in temp)
            {
                double X = sprite.GetVelocity().X;
                double Y = sprite.GetVelocity().Y;

                if(X < 0)
                {
                    X *= -1;
                }
                if(Y < 0)
                {
                    Y *= -1;
                }
                if((X <= 0.1) && (Y <= 0.1))
                {
                    sprite.SetVelocity(0, 0);
                    sprite.Cull();
                    activeProjectiles.Remove(sprite);
                }

            }


            playerSprite.Move();

            camera.UpperLeft = new Vector2((playerSprite.UpperLeft.X - playerSprite.GetWidth() / 2) - (camera.ViewWidth / 2),
                                                            (playerSprite.UpperLeft.Y + playerSprite.GetHeight()) - ((camera.ViewHeight / 4) * 3));

            oldKeyState = currentKeyState;
            oldMouseState = currentMouseState;
            floored = false;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin();

            foreach(Sprite sprite in activeSprites)
            {
                sprite.Draw(spriteBatch, camera.UpperLeft);
            }

            spriteBatch.DrawString(Calibri, "Mana: ", new Vector2(10, 10), Color.Black);
            spriteBatch.DrawString(Calibri, mana.ToString(), new Vector2(60, 10), Color.Black);

            spriteBatch.DrawString(Calibri, projectileCount.ToString(), new Vector2(200, 10), Color.Black);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
