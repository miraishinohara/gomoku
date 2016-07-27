using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Gomokunarabe
{
    class Gomoku
    {
        enum TileState
        {
            Nothing,
            White,
            Black,
        }

        enum Direction
        {
            Up,
            Down,
            Left,
            Right,
            UpperLeft,
            UpperRight,
            LowerLeft,
            LowerRight
        }

        private const int MAP_SIZE = 8;
        private TileState[,] mapState;			
        private int[] stonePos = new int[2];		
        private int[] mystone = new int[2];			

        private const int LEFT = 3;		
        private const int TOP = 1;

        
        public Gomoku()
        {
           setMap();
        }

        int time = 1;
        
        public void Start()
        {
            int[] enemystone = new int[2];
            TileState winner = TileState.Nothing;
            
            while (winner == TileState.Nothing)
            {
                displayMap();
                
                PlayerInput();
                
                winner = judgeGame(mystone);
                if (winner != TileState.Nothing)
                    break;
                Thread.Sleep(time);
                
                enemystone = computerAI(TileState.White);
                putStone(enemystone[0], enemystone[1], TileState.White);
                
                winner = judgeGame(enemystone);
            }
 
        }

        
        private void displayWinner(TileState winner)
        {
            string winnerName;
            if (winner == TileState.Black)
            {
                winnerName = "白";
            }
            else
            {
                winnerName = "黒";
            }

            Console.WriteLine("{0}の勝ちです　　　", winnerName);
            Console.WriteLine("Endを押すと終了します");
        }

        private void PlayerInput()
        {
            while(true)
            {
                Console.WriteLine("");
                Console.WriteLine("縦座標を入力してください");
                int Player_x = int.Parse(Console.ReadLine());

                Console.WriteLine("横座標を入力してください");
                int Player_y = int.Parse(Console.ReadLine());

                if (mapState[Player_x, Player_y] == TileState.Nothing)
                {
                    mapState[Player_x, Player_y] = TileState.Black;
                    break;
                }
                else
                    Console.WriteLine("置かれています。もう一度選択してください");
            }
           

        }

        
        private bool waitInputEnd()
        {
            ConsoleKeyInfo key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.End:
                    return false;
                default:
                    return true;
            }
        }

        
        private bool putStone(int x, int y, TileState tileState)
        {
            if (mapState[x, y] == TileState.Nothing)
            {
                mapState[x, y] = tileState;
                drawStone(x, y);
                return true;
            }
            return false;
        }

        
        private void drawStone(int x, int y)
        {
            int[] drawgoisi = new int[2];
            Console.SetCursorPosition(drawgoisi[0], drawgoisi[1]);
            goisi(x, y);
            Console.SetCursorPosition(stonePos[0], stonePos[1]);
        }

        
        private void setMap()
        {
            mapState = new TileState[MAP_SIZE, MAP_SIZE];
        }

        
        private void displayMap()
        {
            
            for (int y = -1; y < MAP_SIZE; y++)
            {

                for (int x = -1; x < MAP_SIZE; x++)
                {
                    
                    if (y == -1)
                    {
                        if (x == -1)
                        {
                            Console.Write("　　");
                        }
                        else
                        {
                            Console.Write("{0,2}", x + 1 );
                        }
                    }
                    
                    else if (x == -1)
                    {
                        Console.Write("{0,2} ", y );
                    }
                    else
                    {
                        goisi(x, y);
                    }
                    
                    if (x == MAP_SIZE - 1)
                    {
                        Console.WriteLine();
                    }
                }
            }
        }

        
        private void goisi(int x, int y)
        {
            //黒
            if (mapState[x, y] == TileState.White)
            {
                Console.Write("○");
            }
            //白
            else if (mapState[x, y] == TileState.Black)
            {
                Console.Write("●");
            }
            //なにもなし
            else
            {
                Console.Write("┼");
            }
        }

        
        private TileState judgeGame(int[] pos)
        {
            TileState stoneColor = mapState[pos[0], pos[1]];
            
            int horizontalNum = 1;
            horizontalNum += kati(pos, stoneColor, Direction.Right, -1);
            horizontalNum += kati(pos, stoneColor, Direction.Left, -1);
            if (horizontalNum >= 5)
                return stoneColor;
            
            int verticalNum = 1;
            verticalNum += kati(pos, stoneColor, Direction.Up, -1);
            verticalNum += kati(pos, stoneColor, Direction.Down, -1);
            if (verticalNum >= 5)
                return stoneColor;
            
            int diagonallyRightNum = 1;
            diagonallyRightNum += kati(pos, stoneColor, Direction.UpperRight, -1);
            diagonallyRightNum += kati(pos, stoneColor, Direction.LowerLeft, -1);
            if (diagonallyRightNum >= 5)
                return stoneColor;
            
            int diagonallyLeftNum = 1;
            diagonallyLeftNum += kati(pos, stoneColor, Direction.UpperLeft, -1);
            diagonallyLeftNum += kati(pos, stoneColor, Direction.LowerRight, -1);
            if (diagonallyLeftNum >= 5)
                return stoneColor;

            return TileState.Nothing;
        }

        //盤面の座標から同じ色の碁石が何個並んでいるかを調べるメソッド
        private int kati(int[] pos, TileState color, Direction dir, int stoneNum)
        {
            //盤面の端の場合、再起呼び出し終了
            if (pos[0] < 0 || MAP_SIZE - 1 < pos[0] || pos[1] < 0 || MAP_SIZE - 1 < pos[1])
                return stoneNum;
            //受け取った座標が違う碁石の場合、再起呼び出し終了
            if (mapState[pos[0], pos[1]] != color)
            {
                return stoneNum;
            }

            stoneNum++;
            //座標を移動する
            int[] nextPos = new int[2];
            for (int i = 0; i < 2; i++)
            {
                nextPos[i] = pos[i];
            }
            
            switch (dir)
            {
                case Direction.Up:
                    nextPos[1]--;
                    break;
                case Direction.Down:
                    nextPos[1]++;
                    break;
                case Direction.Left:
                    nextPos[0]--;
                    break;
                case Direction.Right:
                    nextPos[0]++;
                    break;
                case Direction.UpperLeft:
                    nextPos[1]--;
                    nextPos[0]--;
                    break;
                case Direction.UpperRight:
                    nextPos[1]--;
                    nextPos[0]++;
                    break;
                case Direction.LowerLeft:
                    nextPos[1]++;
                    nextPos[0]--;
                    break;
                case Direction.LowerRight:
                    nextPos[1]++;
                    nextPos[0]++;
                    break;
            }
          
            stoneNum = kati(nextPos, color, dir, stoneNum);
            return stoneNum;
        }


        private int[] computerAI(TileState myColor)
        {
            int[] pos = new int[2];
            int[,] mapPriority = new int[MAP_SIZE, MAP_SIZE];
            
            for (int y = 0; y < MAP_SIZE; y++)
            {
                for (int x = 0; x < MAP_SIZE; x++)
                {
                    mapPriority[x, y] = checkAround(x, y, myColor);
                }
            }

            int maxPriority = 0;
            
            Random random = new Random();
            for (int y = 0; y < MAP_SIZE; y++)
            {
                for (int x = 0; x < MAP_SIZE; x++)
                {
                    if (maxPriority < mapPriority[x, y])
                    {
                        maxPriority = mapPriority[x, y];
                        pos[0] = x;
                        pos[1] = y;
                    }
                    
                    else if (maxPriority == mapPriority[x, y])
                    {

                        int num;
                        if ((num = random.Next(2)) == 0)
                        {
                            maxPriority = mapPriority[x, y];
                            pos[0] = x;
                            pos[1] = y;

                        }
                    }
                }
            }
            return pos;
        }

         private int checkAround(int x, int y, TileState myColor)
        {
            int priorityNum = 0;
            int[] pos = new int[2];
            pos[0] = x;
            pos[1] = y;
            
            if (mapState[pos[0], pos[1]] != TileState.Nothing)
            {
                return -1;
            }
            
            pos[0]--;
            priorityNum += getPriorityNum(pos, myColor, Direction.Left);

            pos[1]--;
            priorityNum += getPriorityNum(pos, myColor, Direction.UpperLeft);

            
            pos[0]++;
            priorityNum += getPriorityNum(pos, myColor, Direction.Up);

            //右上チェック
            pos[0]++;
            priorityNum += getPriorityNum(pos, myColor, Direction.UpperRight);

            //右チェック
            pos[1]++;
            priorityNum += getPriorityNum(pos, myColor, Direction.Right);

            //右下チェック
            pos[1]++;
            priorityNum += getPriorityNum(pos, myColor, Direction.LowerRight);

            //下チェック
            pos[0]--;
            priorityNum += getPriorityNum(pos, myColor, Direction.Down);

            //左下チェック
            pos[0]--;
            priorityNum += getPriorityNum(pos, myColor, Direction.LowerLeft);

            //元の位置に戻す
            pos[0]++;
            pos[1]--;

            return priorityNum;
        }

        //優先度を返すメソッド
        private int getPriorityNum(int[] pos, TileState myColor, Direction direction)
        {
            int priorityNum = 0;
            //はみ出しチェック
            if (pos[0] < 0 || MAP_SIZE - 1 < pos[0] || pos[1] < 0 || MAP_SIZE - 1 < pos[1])
                return -1;
            if (mapState[pos[0], pos[1]] == TileState.Nothing)
            {
                return priorityNum;
            }
            //個数をチェックする
            int stoneNum = 1;
            stoneNum += kati(pos, (TileState)(mapState[pos[0], pos[1]]), direction, -1);
            priorityNum += getPriorityPoint(stoneNum, pos, myColor, direction);

            return priorityNum;
        }

        
        private int getPriorityPoint(int stoneNum, int[] pos, TileState myColor, Direction direction)
        {
            
            TileState oppositionStone = checkOppositionStone(stoneNum, pos, direction);
           
            if (mapState[pos[0], pos[1]] != myColor)
            {
                switch (stoneNum)
                {
                    case 0:
                        return 0;
                    case 1:
                        return 1;
                    case 2:
                        return 3;
                    case 3:
                        if (oppositionStone == myColor)
                        {
                            return 5;
                        }
                        else if (oppositionStone == TileState.Nothing)
                        {
                            return 20;
                        }
                        else
                        {
                            return 4;
                        }
                    case 4:
                        if (oppositionStone == myColor)
                        {
                            return 30;
                        }
                        else if (oppositionStone == TileState.Nothing)
                        {
                            return 0;
                        }
                        else
                        {
                            return 30;
                        }
                    default:
                        return -1;
                }
            }
            
            else
            {
                switch (stoneNum)
                {
                    case 0:
                        return 0;
                    case 1:
                        return 1;
                    case 2:
                        return 2;
                    case 3:
                        if (oppositionStone == TileState.Nothing)
                        {
                            return 10;
                        }
                        else if (oppositionStone != myColor)
                        {
                            return 4;
                        }
                        else
                        {
                            return 3;
                        }
                    case 4:
                        return 100;
                    default:
                        return -1;
                }
            }
        }

        
        private TileState checkOppositionStone(int stoneNum, int[] pos, Direction direction)
        {
            int[] nextPos = new int[2];
            for (int i = 0; i < 2; i++)
            {
                nextPos[i] = pos[i];
            }
            
            switch (direction)
            {
                case Direction.Up:
                    nextPos[1] -= stoneNum;
                    break;
                case Direction.Down:
                    nextPos[1] += stoneNum;
                    break;
                case Direction.Left:
                    nextPos[0] -= stoneNum;
                    break;
                case Direction.Right:
                    nextPos[0] += stoneNum;
                    break;
                case Direction.UpperLeft:
                    nextPos[1] -= stoneNum;
                    nextPos[0] -= stoneNum;
                    break;
                case Direction.UpperRight:
                    nextPos[1] -= stoneNum;
                    nextPos[0] += stoneNum;
                    break;
                case Direction.LowerLeft:
                    nextPos[1] += stoneNum;
                    nextPos[0] -= stoneNum;
                    break;
                case Direction.LowerRight:
                    nextPos[1] += stoneNum;
                    nextPos[0] += stoneNum;
                    break;
            }
            
            return mapState[nextPos[0], nextPos[1]];
        }

    }
