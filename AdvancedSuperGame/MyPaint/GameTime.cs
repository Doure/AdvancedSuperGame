using System;

namespace ConsoleApplication1
{
    public class GameTime
    {
        private float timeScale = 1f;
        public float dt { get; private set; }
        public float timeElapsed { get; private set; }

        public float TimeScale
        {
            get { return timeScale; }
            set { timeScale = value; }
        }

        public float DeltaTime
        {
            get { return dt * timeScale; }
            private set { dt = value; }
        }

        public void Update(float deltaTime, float timeElapsed)
        {
            DateTime dt = DateTime.Now;
            var ts =  DateTime.Now - dt;
            //ts.TotalMilliseconds

            this.dt = deltaTime;
            this.timeElapsed = timeElapsed;
        }
    }
}
