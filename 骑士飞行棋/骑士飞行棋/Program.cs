using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace 骑士飞行棋
{
    class Program
    {
        static int[] Map = new int[100]; // 用于存放地图
        static int[] playerPos = { 0 ,0 };// playerPos[0]存玩家A的下标，playerPos[1]存玩家B的下标
        static string[] names = new string[2];// names[0]存玩家A的姓名，names[1]存玩家B的姓名
        static bool[] isStop = { false, false };//默认初始化两位玩家均未处于暂停状态，当为false时玩家处于暂停，反之亦然

        static void Main(string[] args)
        {
           //在下面的数组存储我们游戏地图各各关卡
           //数组的下表为0的元素对应地图上的第1格  下标为2的元素对应第2格...下标为n的元素对应n+1格
           //在数组中用  1：表示幸运轮盘
           //            2：地雷
           //            3：暂停
           //            4：时空隧道
           //            0：普通关卡 □

            ShowUI(); // 显示游戏界面

            Console.WriteLine("请输入玩家A的姓名？");
            names[0] = Console.ReadLine();
            // 如果输入为空 提示玩家重新输入
            while (names[0] == "")
            {
                Console.WriteLine("玩家A的姓名不能为空，请重新输入！");
                names[0] = Console.ReadLine();
            }

            Console.WriteLine("请输入玩家B的姓名？");
            names[1] = Console.ReadLine();
            // 如果输入为空 提示玩家重新输入
            while (names[1] == "" || names[1] == names[0])
            {
                if(names[1] == "")
                {
                    Console.WriteLine("玩家B的姓名不能为空，请重新输入！");
                }
                if(names[1] == names[0])// A玩家和B玩家重名
                {
                    Console.WriteLine("该姓名已被玩家占用，请重新输入");
                }
                names[1] = Console.ReadLine();
            }  

            Console.Clear();// 清空游戏界面
            ShowUI(); // 重绘游戏界面

            Console.WriteLine("对战开始......");
            Console.WriteLine("玩家{0}用A来表示",names[0]);
            Console.WriteLine("玩家{0}用B来表示", names[1]);
            Console.WriteLine("如果A和B在同一位置，用<>来表示");

            InitialMap();// 初始化地图
            DrawMap(); // 绘制地图

            Console.WriteLine("开始游戏...");

            // 这个循环中让玩家A和玩家B轮流掷骰子  当玩家A或者玩家B的坐标>=99时，则结束循环。
            // 那循环条件就是
            while (playerPos[0]<99 && playerPos[1]<99)
            {
                //A玩家掷骰子
                if (isStop[0] == false)
                {
                    Action(0);
                }
                else
                {
                    //说明isStop == true
                    isStop[0] = false;
                }
                //判断玩家A是否获胜
                if (playerPos[0] >= 99)
                {
                    break;
                }
                //B玩家掷骰子
                if (isStop[1] == false)
                {
                    Action(1);
                }
                else
                {
                    isStop[1] = false;
                }
            }

            Console.Clear();
            ShowUI();

            // 判断谁胜利，谁失败
            if (playerPos[0] >= 99)
            {
                Console.WriteLine("恭喜{0}玩家胜利了(＾－＾)V，玩家{1}战败而归(╥╯^╰╥),望君下次再接再厉！",
                    names[0],names[1]);
            }
            else 
            {
                Console.WriteLine("恭喜{0}玩家胜利了(＾－＾)V，玩家{1}战败而归,望君下次再接再厉！", 
                    names[1], names[0]);
            }
            Console.WriteLine(@"亲爱的玩家您好( ^_^ )！
          ***************************请    按   ESC   键   退   出   游   戏*************************");
            ConsoleKeyInfo c = Console.ReadKey();//按esc键退出程序
            while (c.Key != ConsoleKey.Escape)
            {
                c = Console.ReadKey();//按esc键退出程序
            }
        }

        /// <summary>
        /// A或B掷骰子的方法
        /// </summary>
        /// <param name="playerNumber">A玩家掷骰子传0过来  B玩家掷骰子传1过来</param>
        static void Action(int playerNumber)
        {
            #region 掷骰子...
            //playerNumber变量中存储的就是当前玩家 姓名 坐标 是否暂停 这三个数组的下标
            //1-playerNumber就代表对方玩家 姓名 坐标 是否暂停 这三个数组的下标

            Random r = new Random();// r是产生随机数用的
            string msg = "";// 用于存储用户踩到某关卡说的话
            int step = 0; // 用于存放产生的随机数
            Console.WriteLine("{0}玩家按任意键开始掷骰子...", names[playerNumber]);

            #region 游戏后门代码部分
            ConsoleKeyInfo rec = Console.ReadKey(true);
            step = r.Next(1, 7);// 产生一个1-6之间的随机整数
            //判断玩家是否按了组合键：control，shift和Tab键进行作弊操作
            if (rec.Key == ConsoleKey.Tab && rec.Modifiers == (ConsoleModifiers.Control | ConsoleModifiers.Shift))
            {
                ConsoleKeyInfo cc = Console.ReadKey(true);
                if (cc.Key == ConsoleKey.F1)
                {
                    Console.WriteLine("恭喜玩家{0}进入游戏超级模式...请输入1-100内任何你想要掷出的点数？",names[playerNumber]);
                    step = ReadInt(1, 100);//通过后门修改step值，轻松赢得游戏
                }
            }
            #endregion

            Console.WriteLine("{0}掷出了：{1}", names[playerNumber], step);
            Console.WriteLine("按任意键开始行动...");
            Console.ReadKey(true); //参数为true时，控制台不显示玩家点击的按钮
            playerPos[playerNumber] = playerPos[playerNumber] + step;// 注意,一旦坐标发生改变，就要判断坐标值是否 >99 或者 <0
            CheckPos();// 检测坐标是否越界

            if (playerPos[0] == playerPos[1]) //判断两位玩家是否处于同一位置
            {
                playerPos[1- playerNumber] = 0; // 将被踩到的玩家踢回老家（出发点）
                msg = string.Format("玩家{0}踩到了玩家{1},玩家{1}(~~~~(>_<)~~~~)泪奔着退回原点", names[playerNumber], names[1- playerNumber]);
            }
            else
            {// 没踩到，要判断玩家A现在所在位置是否有其他关卡
                switch (Map[playerPos[playerNumber]])
                {
                    case 0:
                        // 普通，没有效果
                        msg = "";
                        break;
                    case 1:
                        // 走到了幸运转盘关卡
                        Console.Clear();
                        DrawMap();
                        Console.WriteLine("{0}玩家走到了幸运转盘，请选择运气？", names[playerNumber]);
                        Console.WriteLine("1---交换位置  2---轰炸对方");
                        int userSelect = ReadInt(1, 2);
                        if (userSelect == 1)
                        {// 要与对方交换位置
                            int temp = playerPos[0];
                            playerPos[0] = playerPos[1];
                            playerPos[1] = temp;
                            msg = string.Format("{0}玩家选择了与玩家{1}交换位置！", names[playerNumber],names[1- playerNumber]);
                        }
                        else
                        {
                            //轰炸对方
                            playerPos[1- playerNumber] = playerPos[1- playerNumber] - 6;
                            CheckPos();
                            msg = string.Format("{0}玩家选择轰炸玩家{1}，玩家{1} (o(≧口≦)o) 垂头丧气地退6格", names[playerNumber], names[1- playerNumber]);

                        }
                        break;
                    case 2:
                        // 踩到了地雷
                        playerPos[playerNumber] = playerPos[playerNumber] - 6;
                        CheckPos();
                        msg = string.Format("{0}玩家踩到了地雷，玩家{0} (〒▽〒) 很不情愿地退6格", names[playerNumber]);
                        break;
                    case 3:
                        // 暂停一次
                        isStop[playerNumber] = true; // 设置玩家暂停标志
                        msg = string.Format("{0}玩家走到红灯 ( ˇ-ˇ ) 郁闷地暂停一次！", names[playerNumber]);
                        break;
                    case 4:
                        // 时空隧道
                        playerPos[playerNumber] = playerPos[playerNumber] + 10;
                        CheckPos();
                        msg = string.Format("恭喜玩家{0}进入时空隧道，\\(^o^)/爽死啦！玩家{0}兴奋地前进10格！", names[playerNumber]);
                        break;
                    default:
                        Console.WriteLine("未知错误？ 爱莫能助┑(￣Д ￣)┍！！！！！");
                        break;
                }
            }
            #endregion
            // 清理界面重绘操作
            Console.Clear();
            DrawMap();

            //输出玩家进入特殊关卡后状态
            if (msg != "")
            {
                Console.WriteLine(msg);
            }
            else
            {
                Console.WriteLine("{0}玩家前进了{1}格,行动完成！", names[playerNumber], step);
            }
            Console.WriteLine("********************玩家A和玩家B的位置如下*******************");
            Console.WriteLine("{0}玩家的当前位置为{1}", names[0], playerPos[0] + 1);
            Console.WriteLine("{0}玩家的当前位置为{1}", names[1], playerPos[1] + 1);

        }

        /// <summary>
        /// 用于绘制飞行棋的名称
        /// </summary>
        static void ShowUI()
        {
            Console.WriteLine("************************************************************************");
            Console.WriteLine("*                                                                      *");
            Console.WriteLine("*                 骑        士        飞       行       棋             *");
            Console.WriteLine("*                                                                      *");
            Console.WriteLine("************************************************************************");
        }

        /// <summary>
        /// 绘制地图
        /// </summary>
        static void DrawMap()
        {
            Console.WriteLine("图例：普通关卡：□  幸运转盘：◎   地雷： ★   暂停： ▲   时空隧道： 卐");
            // 画第一行  绘制下标从0-29格的地图 
            for (int i = 0; i <= 29; i++)
            {
                Console.Write(GetMapString(i));
            }
            // 第一行绘制完毕
            Console.WriteLine();


            // 绘制第一列
            for (int i = 30; i <= 34; i++)
            {
                // 绘制29个双空格
                for (int j = 0; j < 29; j++)
                {
                    Console.Write("  ");
                }
                Console.WriteLine(GetMapString(i));
            }

            //绘制第二行
            for (int i = 64; i >= 35; i--)
            {
                Console.Write(GetMapString(i));
            }

            Console.WriteLine();//换行

            // 绘制第二列
            for (int i = 65; i <= 69; i++)
            {
                Console.WriteLine(GetMapString(i));
            }

            // 绘制最后一行
            for (int i = 70; i <= 99; i++)
            {
                Console.Write(GetMapString(i));
            }
            Console.WriteLine();//换行
            Console.ResetColor();// 重置控制台输出颜色为默认白色
        }

        /// <summary>
        /// 用于检查玩家是否过界
        /// </summary>
        static void CheckPos()
        {
            for(int i=0; i<2; i++)
            {
                if (playerPos[i] > 99)
                {
                    playerPos[i] = 99;
                }
                if (playerPos[i] < 0)
                {
                    playerPos[i] = 0;
                }
            }
       
        }

        /// <summary>
        /// 对地图中的关卡进行初始化
        /// </summary>
        static void InitialMap()
        {
            //用于存储在地图中为数组的下标   特殊关卡设置
            int[] luckyTurn = { 6, 23, 40, 55, 69, 83 };//幸运转盘◎ 1
            int[] landMine = { 5, 13, 17, 33, 38, 50, 64, 80, 94 };//地雷 ★ 2
            int[] pause = { 9, 27, 60, 93 }; //暂停 ▲ 3
            int[] timeTunnel = { 20, 25, 45, 63, 72, 88, 90 };//时空隧道 卐  4
            for (int i=0;i<luckyTurn.Length;i++)
            {
                Map[luckyTurn[i]] = 1;
            }
            for (int i = 0; i < landMine.Length; i++)
            {
                Map[landMine[i]] = 2;
            }
            for (int i = 0; i < pause.Length; i++)
            {
                Map[pause[i]] = 3;
            }
            for (int i = 0; i < timeTunnel.Length; i++)
            {
                Map[timeTunnel[i]] = 4;
            }
        }

        /// <summary>
        /// 获得第pos坐标上应该绘制的图案
        /// </summary>
        /// <param name="pos">当前要绘制的坐标</param>
        /// <returns></returns>
        static string GetMapString(int pos)
        {
            string result = "";

            //判断A和B玩家是否在当前要画的第i格上
            if (playerPos[0] == pos && playerPos[1] == pos)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                result = "<>";
            }
            else if (playerPos[0] == pos)// A在当前画的格上
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                result = "Ａ";
            }
            else if (playerPos[1] == pos)// B在当前画的格上
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                result = "Ｂ";
            }
            else
            {
                switch (Map[pos])
                {
                    case 0:
                        Console.ForegroundColor = ConsoleColor.White;
                        result = "□";
                        break;
                    case 1:
                        Console.ForegroundColor = ConsoleColor.Red;
                        result = "◎";
                        break;
                    case 2:
                        Console.ForegroundColor = ConsoleColor.Green;
                        result = "★";
                        break;
                    case 3:
                        Console.ForegroundColor = ConsoleColor.Blue;
                        result = "▲";
                        break;
                    case 4:
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        result = "卐";
                        break;
                    default:
                        break;
                }
            }
            return result;
        }

        /// <summary>
        /// 基类
        /// </summary>
        /// <returns></returns>
        static int ReadInt()
        {
            int i = ReadInt(int.MinValue, int.MaxValue);
            return i;
        }

        /// <summary>
        /// 重载于基类的方法
        /// </summary>
        /// <param name="min">允许输入最小值</param>
        /// <param name="max">允许输入最大值</param>
        /// <returns></returns>
        static int ReadInt(int min,int max)
        {
            while (true)
            {
                try
                {
                    int number = Convert.ToInt32(Console.ReadLine());
                    if (number < min || number > max)
                    {
                        Console.WriteLine("只能输入{0}-{1}之间的数字，请重新输入：",min,max);
                        continue;
                    }
                    return number;
                }
                catch
                {

                    Console.WriteLine("只能输入数字，请重新输入！");
                }
            }

        }
    }

}