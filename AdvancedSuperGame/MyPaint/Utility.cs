using SFML.Graphics;

namespace ConsoleApplication1
{
    public static class Utility
    {
        public const string FONT_DIR = "./Fonts/arial.ttf";
        public static Font ConsoleFont;
        public static int Clip(int min, int max, int current)
        {
            if (current < min)
                current = min;

            if (current > max)
                current = max;

            return current;
        }
        public static float Clip(float min, float max, float current)
        {
            if (current < min)
                current = min;

            if (current > max)
                current = max;

            return current;
        }

        public static void LoadData()
        {
            ConsoleFont = new Font(FONT_DIR);
        }
    }
}
