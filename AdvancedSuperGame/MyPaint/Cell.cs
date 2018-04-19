namespace ConsoleApplication1
{
    /// <summary>
    /// Класс точки 
    /// </summary>
    public struct COORD
    {
        public int X { get; set; }
        public int Y { get; set; }

        public COORD(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }
    }

    /// <summary>
    ///Класс клеточки 
    /// </summary>
    public class Cell
    {
        public int NeighborsCount = 0;
        public bool isDedicated = false;
        public COORD Coord { get; private set; }

        public Cell(COORD coord)
        {
            Coord = coord;
        }

        public bool DrawDesign()
        {
            if (NeighborsCount < 2 || NeighborsCount > 3)
                isDedicated = false;
            else if (NeighborsCount == 3)
                isDedicated = true;

            NeighborsCount = 0;

            return isDedicated;
        }
    }
}
