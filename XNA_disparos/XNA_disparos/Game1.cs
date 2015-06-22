using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace XNA_disparos
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        
        // spriteNave
        Texture2D spriteNaveTextura;
        Rectangle spriteNaveRectangulo;
        
        // centro de la imagen
        Vector2 spriteOrigen;
        Vector2 spritePosicion;
        Vector2 spritePosicion2;
        
        float rotacion;
        Vector2 spriteVelocidad;
        const float velocidadTangencial = 5f;
        float friccion = 0.1f;
        //Rockets
        List<Rocket> rockets = new List<Rocket>();


        //Enemigo:
        Texture2D texturaEnemigo;
        Rectangle rectanguloEnemigo;

        //Pantalla:
        int screenHeight;
        int screenWidth;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            // Cambia el tamaño de la pantalla
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = 1200;
            graphics.PreferredBackBufferHeight = 900;
        }
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
            spriteNaveTextura = Content.Load<Texture2D>("nave5");
            spritePosicion = new Vector2(250, 200);

            //Load parámetros pantalla.
            screenHeight = GraphicsDevice.Viewport.Height;
            screenWidth = GraphicsDevice.Viewport.Width;


            //Load enemigo
            Random rand = new Random();
            texturaEnemigo = Content.Load<Texture2D>("enemy_g");
            rectanguloEnemigo = new Rectangle(rand.Next(800), rand.Next(600), 84, 60);
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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back ==
            ButtonState.Pressed)
                this.Exit();

            spriteNaveRectangulo = 
                new Rectangle(
                    (int)spritePosicion.X, 
                    (int)spritePosicion.Y, 
                    spriteNaveTextura.Width, 
                    spriteNaveTextura.Height);
            spritePosicion = spriteVelocidad + spritePosicion;
            spritePosicion2 = spritePosicion * 2;
            spriteOrigen = new Vector2(
                spriteNaveRectangulo.Width / 2,
                spriteNaveRectangulo.Height / 2);

            if (Keyboard.GetState().IsKeyDown(Keys.Right)) rotacion += 0.2f;
            if (Keyboard.GetState().IsKeyDown(Keys.Left)) rotacion -= 0.2f;
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                spriteVelocidad.X = (float)Math.Cos(rotacion) *
                velocidadTangencial;
                spriteVelocidad.Y = (float)Math.Sin(rotacion) *
                velocidadTangencial;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                spriteVelocidad.X = spriteVelocidad.X*0;
                spriteVelocidad.Y = spriteVelocidad.Y*0;
            }
            else if (spriteVelocidad != Vector2.Zero)
            {
                float i = spriteVelocidad.X;
                float j = spriteVelocidad.Y;
                spriteVelocidad.X = i -= friccion * i;
                spriteVelocidad.Y = j -= friccion * j;
            }
            

            //Disparos
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
                Disparo();
                UpdateRockets();
                //Límites pantalla para nave.
                if (spritePosicion.X <= 0)  {
                    spritePosicion.X = 0;
                }

                if (spritePosicion.X >= screenWidth){
                    spritePosicion.X = screenWidth;
                }

                if (spritePosicion.Y <= 0){
                    spritePosicion.Y = 0;
                }

                if (spritePosicion.Y >= screenHeight){
                    spritePosicion.Y = screenHeight;
                }

            base.Update(gameTime);
        }

        public void UpdateRockets()
        {
            foreach (Rocket rocket in rockets)
            {
                rocket.posicion += rocket.velocidad*3;
                if (Vector2.Distance(rocket.posicion, spritePosicion) > 600)
                    rocket.isVisible = false;
                //Rebotes de rockets en paredes del juego.
                if (rocket.posicion.X <= 0)
                    rocket.velocidad.X = -rocket.velocidad.X;
                if (rocket.posicion.X + rocket.textura.Width >= screenWidth)
                    rocket.velocidad.X = -rocket.velocidad.X;
                if (rocket.posicion.Y <= 0)
                    rocket.velocidad.Y = -rocket.velocidad.Y;
                if (rocket.posicion.Y + rocket.textura.Height >= screenHeight)
                    rocket.velocidad.Y = -rocket.velocidad.Y;
            }

            for (int i = 0; i < rockets.Count; i++)
            {
                if (!rockets[i].isVisible)
                {
                    rockets.RemoveAt(i);
                    i--;
                }
            } 
        }

        public void Disparo()
        {
            Rocket nuevoRocket = new Rocket(Content.Load<Texture2D>("fire2"));
            nuevoRocket.velocidad = new Vector2((float)Math.Cos(rotacion),
            (float)Math.Sin(rotacion)) * 5f + spriteVelocidad;
            nuevoRocket.posicion = spritePosicion + nuevoRocket.velocidad;
            //nuevoRocket.posicion.X = spritePosicion.X +100;
            //nuevoRocket.posicion.Y = spritePosicion.Y - 20;
            nuevoRocket.isVisible = true;
            if (rockets.Count() < 1)
                rockets.Add(nuevoRocket);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            spriteBatch.Draw(spriteNaveTextura, spritePosicion, null,
            Color.White, rotacion, spriteOrigen, 1f, SpriteEffects.None, 0);
            foreach (Rocket rocket in rockets)
                rocket.Draw(spriteBatch);

            spriteBatch.Draw(texturaEnemigo, rectanguloEnemigo, Color.White);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }

    class Rocket
    {
        public Texture2D textura;
        public Vector2 posicion, velocidad, origen;
        public bool isVisible;
        public Rocket(Texture2D nuevaTextura)
        {
            textura = nuevaTextura;
            isVisible = false;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(textura, posicion, null,
            Color.White, 0f, origen, 1f, SpriteEffects.None, 0);
        }
  
    }


}
