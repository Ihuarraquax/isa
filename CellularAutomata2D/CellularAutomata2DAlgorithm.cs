using System;
using System.Linq;

namespace CellularAutomata2D
{
    public class CellularAutomata2DAlgorithm
    {
        public int X { get; set; } = 20;
        public int Y { get; set; } = 20;
        public double R { get; set; } = 0.1;

        public int StepIndex { get; set; } = 1;
        
        public bool IsFinished { get; set; }
        
        public bool[,] OldArea { get; set; }
        public bool[,] Area { get; set; }
        public string Mode { get; set; }
        public int StepMax { get; set; }

        public void Init()
        {
            var random = new Random();
            // init 
            Area = new bool[X, Y];
            OldArea = new bool[X, Y];
            for (int x = 0; x < X; x++)
            {
                for (int y = 0; y < Y; y++)
                {
                    var r = random.NextDouble() < R;
                    Area[x, y] = r;
                    OldArea[x, y] = r;
                }
            }
        }

        public void Step()
        {
            if (StepIndex >= StepMax || IsFinished)
            {
                return;
            }
            StepIndex++;
            var newArea = new bool[X, Y];
            // calculate every single cell
            IsFinished = true;
            for (int x = 0; x < X; x++)
            {
                for (int y = 0; y < Y; y++)
                {
                    
                    
                    var somsiads = CalcSomsiads(x, y);
                    if (somsiads == 3)
                    {
                        newArea[x, y] = true;
                    }
                    else
                    {
                        if (somsiads == 2 && Area[x,y])
                        {
                            newArea[x, y] = true;
                        }
                        else
                        {
                            newArea[x, y] = false;
                        }
                    }
                   
                    if (IsFinished && newArea[x, y] != OldArea[x, y])
                    {
                        IsFinished = false;
                    }
                    OldArea[x, y] = Area[x, y];
                }
            }
            Area = newArea;
        }

        private int CalcSomsiads(int x, int y)
        {
            var count = 0;

            if (Mode == "Moore")
            {
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (!(i == 0 && j == 0))
                        {
                            int offsetX = (x + i);
                            int offsetY = (y + j);

                            if (offsetX == -1)
                                offsetX = X - 1;
                            else if (offsetX == X)
                                offsetX = 0;

                            if (offsetY == -1)
                                offsetY = Y - 1;
                            else if (offsetY == Y)
                                offsetY = 0;

                            if (Area[offsetX, offsetY])
                                count++;
                        }
                    }
                }
            }
            else
            {
                for(int i = -1; i <= 1; i++)
                {
                    for(int j = -1; j <= 1; j++)
                    {
                        if(!(i == 0 && j == 0))
                        {
                            if(!((i==-1 && j==-1) || (i==1 && j==1) || (i == -1 && j == 1) || (i == 1 && j == -1)))
                            {
                                int offsetX = (x + i);
                                int offsetY = (y + j);

                                if(offsetX == -1)
                                    offsetX = X - 1;
                                else if(offsetX == X)
                                    offsetX = 0;

                                if(offsetY == -1)
                                    offsetY = Y - 1;
                                else if(offsetY == Y)
                                    offsetY = 0;

                                if(Area[offsetX , offsetY])
                                    count++;
                            }
                        }
                    }
                }
            }
            
            return count;
        }
    }
}