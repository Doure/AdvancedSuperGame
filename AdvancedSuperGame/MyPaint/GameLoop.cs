using System;
using SFML.Graphics;
using SFML.Window;
using SFML.System;

namespace ConsoleApplication1
{
    public abstract class GameLoop
    {
        public const int FPS = 60;
        public const float UPDATE_TIME = 1f / FPS;

        public RenderWindow Window { get; protected set; }
        public GameTime GT { get; protected set; }
        public Color WindowColor { get; protected set; }
        private Clock clock = new Clock();

        public abstract void LoadData();
        public abstract void Install();
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(GameTime gameTime);

        public GameLoop(uint Width, uint Height, string Title, Color Color)
        {
            GT = new GameTime();

            WindowColor = Color;
            Window = new RenderWindow(new VideoMode(Width, Height), Title, Styles.Close);
            Window.Closed += WindowClosed;
        }

        private void WindowClosed(object sender, EventArgs e)
        {
            Window.Close();
        }

        public void Run()
        {
            LoadData();

            Install();

            float update_time = 0;
            float last_time = 0;
            float dt = 0;
            float other_time = 0;

            while (Window.IsOpen)
            {
                Window.DispatchEvents();

                other_time = clock.ElapsedTime.AsSeconds();

                dt = other_time - last_time;

                last_time = other_time;

                update_time += dt;

                if (update_time >= UPDATE_TIME)
                {
                    GT.Update(update_time, clock.ElapsedTime.AsSeconds());

                    update_time = 0;

                    Update(GT);

                    Window.Clear(WindowColor);

                    Draw(GT);

                    Window.Display();
                }
            }
        }
    }


}
