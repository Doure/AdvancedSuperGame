using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace ConsoleApplication1
{
    public class Paint : GameLoop
    {
        //Ширина окна
        public const uint ScreenWidth = 800;
        //Высота окна
        public const uint ScreenHeight = 600;

        //Максимальная ширина листа
        private int AreaSizeX = 1000;

        //Максимальная высота листа
        private int AreaSizeY = 1000;

        //Где мы сейчас
        private COORD current = new COORD();

        //Горизонтальное кол-во клеточек
        private uint HorizontalNumCells;

        //Вертикальное кол-во клеточек
        private uint VerticalNumCells;

        //Двухмерный массив, где мы храним информацию о клеточках
        private Cell[,] Cells;

        //Размер клетки
        private Vector2f CellsSize = new Vector2f(18, 18);
        private Vector2f CellsDefSize = new Vector2f(18, 18);

        //Отступ от клетки до клетки 
        private float CellsSpace = 2;
        private float CellsDefSpace = 2;
        private float CurrentZoom = 0f;

        private RectangleShape RSWhite;
        private RectangleShape RSBlue;

        //Позиция мыши
        private Vector2i MousePos;
        
        //Захваченная клеточка
        private Cell CurrentCell;

        public Paint() : base(ScreenWidth, ScreenHeight, "My Game", Color.Black) { }

        public override void Install()
        {
            HorizontalNumCells = ScreenWidth / (uint)(CellsSize.X + CellsSpace);
            VerticalNumCells = ScreenHeight / (uint)(CellsSize.Y + CellsSpace);

            current.X = AreaSizeX / 2;
            current.Y = AreaSizeY / 2;

            RSWhite = new RectangleShape();
            RSBlue = new RectangleShape();

            RSBlue.Size = RSWhite.Size = CellsSize;

            RSWhite.FillColor = Color.White;
            RSBlue.FillColor = Color.Blue;

            Cells = new Cell[AreaSizeX, AreaSizeY];

            for (int i = 0; i < AreaSizeY; i++)
            {
                for (int j = 0; j < AreaSizeX; j++)
                    Cells[i, j] = new Cell(new COORD(i, j));
            }
        }

        public override void LoadData()
        {
            Utility.LoadData();
        }

        public override void Update(GameTime gt)
        {
            Logic();

            HandleInput();
        }

        public override void Draw(GameTime gt)
        {
            Draw();
        }

        private void Draw()
        {
            for (int i = 0; i < HorizontalNumCells; i++)
            {
                if (i + current.X < Cells.GetLength(0))
                {
                    for (int j = 0; j < VerticalNumCells; j++)
                    {
                        if (j + current.Y < Cells.GetLength(1))
                        {
                            if (Cells[i + current.X, j + current.Y].isDedicated)
                            {
                                RSBlue.Position = new Vector2f(i * (CellsSize.X + CellsSpace) + 1, j * (CellsSize.Y + CellsSpace) + 1);
                                Window.Draw(RSBlue);
                            }
                            else
                            {
                                RSWhite.Position = new Vector2f(i * (CellsSize.X + CellsSpace) + 1, j * (CellsSize.Y + CellsSpace) + 1);
                                Window.Draw(RSWhite);
                            }
                        }
                    }
                }
            }
        }

        private void Logic()
        {
            if (Keyboard.IsKeyPressed(Keyboard.Key.Space))
            {
                foreach (Cell cell in Cells)
                {
                    cell.DrawDesign();

                    SetCellsChecked(cell.Coord.X, cell.Coord.Y);
                }
            }
        }
        private void HandleInput()
        {
            MousePos = Mouse.GetPosition(Window);
            var X = Utility.Clip(0, AreaSizeX - 1, (int)(MousePos.X / (CellsSize.X + CellsSpace) + current.X));
            var Y = Utility.Clip(0, AreaSizeY - 1, (int)(MousePos.Y / (CellsSize.Y + CellsSpace) + current.Y));

            CurrentCell = Cells[X,Y];

            //Стиреть всё
            if (Keyboard.IsKeyPressed(Keyboard.Key.R))
            {
                foreach (Cell cell in Cells)
                {
                    cell.isDedicated = false;
                    cell.NeighborsCount = 0;
                }
            }

            //Отрисовать при нажатии, если не нарисован
            if (Mouse.IsButtonPressed(Mouse.Button.Left))
            {
                if (!CurrentCell.isDedicated)
                {
                    CurrentCell.isDedicated = true;
                    SetCellsChecked(CurrentCell.Coord.X, CurrentCell.Coord.Y);
                }
            }

            //Убрать рисовку при нажатии, если нарисован
            if (Mouse.IsButtonPressed(Mouse.Button.Right))
            {
                if (CurrentCell.isDedicated)
                {
                    CurrentCell.isDedicated = false;
                    SetCellsUnChecked(CurrentCell.Coord.X, CurrentCell.Coord.Y);
                }
            }

            Zooming();

            //Движение по карте
            if (current.Y > 0 && Keyboard.IsKeyPressed(Keyboard.Key.Up))
                --current.Y;
            else if (current.Y < (AreaSizeY - VerticalNumCells) && Keyboard.IsKeyPressed(Keyboard.Key.Down))
                ++current.Y;
            else if (current.X > 0 && Keyboard.IsKeyPressed(Keyboard.Key.Left))
                --current.X;
            else if (current.X < (AreaSizeX - HorizontalNumCells) && Keyboard.IsKeyPressed(Keyboard.Key.Right))
                ++current.X;

         
        }
        private void SetCellsChecked(int X, int Y)
        {
            if (Cells[X, Y].isDedicated)
            {
                if (X > 0)
                {
                    if (Y > 0)
                        ++Cells[X - 1, Y - 1].NeighborsCount;

                    ++Cells[X - 1, Y + 0].NeighborsCount;

                    if (Y < Cells.GetLength(1) - 1)
                        ++Cells[X - 1, Y + 1].NeighborsCount;
                }

                if (Y > 0)
                    ++Cells[X, Y - 1].NeighborsCount;

                if (Y < Cells.GetLength(1) - 1)
                    ++Cells[X, Y + 1].NeighborsCount;

                if (X < Cells.GetLength(0) - 1)
                {
                    if (Y > 0)
                        ++Cells[X + 1, Y - 1].NeighborsCount;

                    ++Cells[X + 1, Y + 0].NeighborsCount;

                    if (Y < Cells.GetLength(1) - 1)
                        ++Cells[X + 1, Y + 1].NeighborsCount;
                }
            }
        }
        private void SetCellsUnChecked(int X, int Y)
        {
            if (Cells[X, Y].isDedicated)
            {
                if (X > 0)
                {
                    if (Y > 0)
                        --Cells[X - 1, Y - 1].NeighborsCount;

                    --Cells[X - 1, Y + 0].NeighborsCount;

                    if (Y < Cells.GetLength(1) - 1)
                        --Cells[X - 1, Y + 1].NeighborsCount;
                }

                if (Y > 0)
                    --Cells[X, Y - 1].NeighborsCount;

                if (Y < Cells.GetLength(1) - 1)
                    --Cells[X, Y + 1].NeighborsCount;

                if (X < Cells.GetLength(0) - 1)
                {
                    if (Y > 0)
                        --Cells[X + 1, Y - 1].NeighborsCount;

                    --Cells[X + 1, Y + 0].NeighborsCount;

                    if (Y < Cells.GetLength(1) - 1)
                        --Cells[X + 1, Y + 1].NeighborsCount;
                }
            }
        }

        private void Zooming()
        {
            if (Keyboard.IsKeyPressed(Keyboard.Key.S))
            {
                CurrentZoom += -0.05f;
                CurrentZoom = Utility.Clip(0.25f, 2.3f, CurrentZoom);

                CellsSize = CellsDefSize * CurrentZoom;
                CellsSpace = CellsDefSpace * CurrentZoom;

                RSWhite.Size = RSWhite.Size = CellsSize;

                HorizontalNumCells = ScreenWidth / (uint)(CellsSize.X + CellsSpace);
                VerticalNumCells = ScreenHeight / (uint)(CellsSize.Y + CellsSpace);
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.W))
            {
                CurrentZoom += 0.05f;
                CurrentZoom = Utility.Clip(0.25f, 2.3f, CurrentZoom);

                CellsSize = CellsDefSize * CurrentZoom;
                CellsSpace = CellsDefSpace * CurrentZoom;

                RSWhite.Size = RSWhite.Size = CellsSize;

                HorizontalNumCells = ScreenWidth / (uint)(CellsSize.X + CellsSpace);
                VerticalNumCells = ScreenHeight / (uint)(CellsSize.Y + CellsSpace);
            }
        }



    }
}
