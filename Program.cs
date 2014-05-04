using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Skylight;
using System.Threading;

namespace MazeCreator
{
    class Program
    {
        static Room r = new Room("PW8JgebeuXbUI");
        static Bot b = new Bot(r, Console.ReadLine(), Console.ReadLine());
        static int MazeWallId = BlockIds.Blocks.Basic.DARKGRAY;

        // MUST BE ODD
        static int laneWidth = 5;
        static void Main(string[] args)
        {
            b.LogIn();
            b.Join();

            // FillWithDots();

            BuildGrid();

            Console.Read();
            
            /*while (true)
            {
                PlaceRandomWall(laneWidth);
                Thread t = new Thread(new ThreadStart(BreakWalls));
                t.Start();
            }*/
        }

        static void FillWithDots()
        {
            for (int x = 1; x < r.Width; x++)
            {
                for (int y = 1; y < r.Width; y++)
                {
                    if (r.Map[x, y, 0].Id != BlockIds.Action.Gravity.ZERO)
                    {
                        b.Push.Build(new Block(BlockIds.Action.Gravity.ZERO, x, y));

                        if (x == r.Width / 2)
                        {
                            b.Push.Say("50% done");
                        }
                    }
                }
            }
        }

        static void BuildGrid()
        {
            for (int x = 0; x <= r.Width; x += laneWidth + 1)
            {
                for (int y = 0; y <= r.Height; y++)
                {
                    b.Push.Build(new Block(MazeWallId, x, y));
                }
            }

            for (int y = 0; y <= r.Height; y += laneWidth + 1)
            {
                for (int x = 0; x <= r.Height; x++)
                {
                    b.Push.Build(new Block(MazeWallId, x, y));
                }
            }

            BreakWalls();
        }

        enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }

        static void BreakWalls()
        {
            List<Block> unvisitedCells = new List<Block>();

            for (int x = (laneWidth + 1) / 2; x <= r.Width; x += laneWidth + 1)
            {
                for (int y = (laneWidth + 1) / 2; y <= r.Width; y += laneWidth + 1)
                {
                    unvisitedCells.Add(r.Map[x, y, 0]);
                }
            }

            int SelectedX = (laneWidth + 1) / 2, SelectedY = (laneWidth + 1) / 2;

            while (unvisitedCells.Count > 0)
            {
                unvisitedCells.Remove(r.Map[SelectedX, SelectedY, 0]);

                List<Direction> neighborsUnvisited = new List<Direction>();

                // right
                if (SelectedX != r.Width - (laneWidth + 1) / 2 && unvisitedCells.Contains(r.Map[SelectedX + laneWidth + 1, SelectedY, 0]))
                {

                    neighborsUnvisited.Add(Direction.Right);
                }

                // down
                if (SelectedY != r.Height - (laneWidth + 1) / 2 && unvisitedCells.Contains(r.Map[SelectedX, SelectedY + laneWidth + 1, 0]))
                {
                    neighborsUnvisited.Add(Direction.Down);
                }

                // Up
                if (SelectedY != (laneWidth + 1) / 2 && unvisitedCells.Contains(r.Map[SelectedX, SelectedY - laneWidth - 1, 0]))
                {
                    neighborsUnvisited.Add(Direction.Up);
                }

                // left
                if (SelectedX != (laneWidth + 1) / 2 && unvisitedCells.Contains(r.Map[SelectedX - laneWidth - 1, SelectedY, 0]))
                {
                    neighborsUnvisited.Add(Direction.Left);
                }

                if (neighborsUnvisited.Count > 0)
                {
                    int randomDirection = Tools.Ran.Next(0, neighborsUnvisited.Count - 1);
                    for (int i = 0; i < 3; i++)
                    {
                        switch (neighborsUnvisited[randomDirection])
                        {
                            case Direction.Up:
                                int yUpper = SelectedY - (laneWidth + 1) / 2;
                                for (int x = SelectedX - (laneWidth + 1) / 2 + 1; x < SelectedX + (laneWidth + 1) / 2; x++)
                                {
                                    b.Push.Build(new Block(BlockIds.Action.Gravity.ZERO, x, yUpper));
                                }


                                break;
                            case Direction.Down:
                                int yLower = SelectedY + (laneWidth + 1) / 2;
                                for (int x = SelectedX - (laneWidth + 1) / 2 + 1; x < SelectedX + (laneWidth + 1) / 2; x++)
                                {
                                    b.Push.Build(new Block(BlockIds.Action.Gravity.ZERO, x, yLower));
                                }


                                break;
                            case Direction.Left:
                                int xLeft = SelectedX - (laneWidth + 1) / 2;
                                for (int y = SelectedY - (laneWidth + 1) / 2 + 1; y < SelectedY + (laneWidth + 1) / 2; y++)
                                {
                                    b.Push.Build(new Block(BlockIds.Action.Gravity.ZERO, xLeft, y));
                                }

                                break;
                            case Direction.Right:
                                int xRight = SelectedX + (laneWidth + 1) / 2;
                                for (int y = SelectedY - (laneWidth + 1) / 2 + 1; y < SelectedY + (laneWidth + 1) / 2; y++)
                                {
                                    b.Push.Build(new Block(BlockIds.Action.Gravity.ZERO, xRight, y));
                                }

                                break;
                        }

                        randomDirection = Tools.Ran.Next(0, neighborsUnvisited.Count - 1);
                    }

                    switch (neighborsUnvisited[randomDirection])
                    {
                        case Direction.Up:
                            SelectedY -= (laneWidth + 1) / 2;
                            break;
                        case Direction.Down:
                            SelectedY += (laneWidth + 1) / 2;
                            break;
                        case Direction.Left:
                            SelectedX -= (laneWidth + 1) / 2;
                            break;
                        case Direction.Right:
                            SelectedX += (laneWidth + 1) / 2;
                            break;
                    }
                }
                else
                {
                    if (unvisitedCells.Count == 0)
                        return;

                    Block randomBlock = unvisitedCells[Tools.Ran.Next(0, unvisitedCells.Count - 1)];
                    SelectedX = randomBlock.X;
                    SelectedY = randomBlock.Y;
                }
            }
        }

        static void PlaceRandomWall(int laneWidth)
        {
            int x = Tools.Ran.Next(1, r.Width / laneWidth + 1) * (laneWidth + 1),
                y = Tools.Ran.Next(1, r.Height / laneWidth + 1) * (laneWidth + 1);

            List<Direction> dirs = new List<Direction>() {Direction.Up, Direction.Down, Direction.Left, Direction.Right};

            switch(dirs[Tools.Ran.Next(0, 4)])
            {
                case Direction.Up:
                    
                    break;
                case Direction.Down:
                    break;
                case Direction.Left:
                    break;
                case Direction.Right:
                    break;
            }



        }
        static void RemoveRandomWall(int laneWidth)
        { 
        }
    }
}
