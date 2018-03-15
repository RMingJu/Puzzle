using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
namespace Puzzle1._1
{
    public partial class Form1 : Form
    {
        Graphics gg;
        bool IsOver = false;

        Size size = new Size(420, 360);      //拼圖原圖大小
        Point StarPoint = new Point(20, 20); //拼圖起始位置

        Size Ref_size = new Size(140, 120);   //參考圖大小
        Point Ref_Point = new Point(20, 400); //參考圖位置

        
        Bitmap bmp;                          //原圖
        Bitmap new_bmp;                      //畫線拿去切割
        Bitmap Ref_bmp;                      //小參考圖
        

        //放置圖片的Label
        List<Label> lbs = new List<Label>();
        List<Point> lb_Point = new List<Point>();
        Label Control_Label;
        Label ll;                            //參考圖

        //紀錄步數
        /*
         * 上下左右 = 1234
        */
        List<int> GameStar = new List<int>();
        List<int> User = new List<int>();
        List<int> way;
        //作弊
        bool cheat = false;

        public Form1()
        {
            InitializeComponent();
            //預設圖片
            bmp = new Bitmap(Puzzle1._1.Properties.Resources.郭雪芙, size);

            //預設遊戲
            button1_Click(button1,null);
            label1.Focus();

        }

        private void _drawLine(int LineCount, Point StarPoint)
        {
            //注意!注意!注意!注意!注意!注意!注意!注意!注意!
            //錯誤寫法(是參考型別，所以拿到的會是記憶體位置!)
            /*
            new_bmp = bmp;
            */

            //new = 原圖(為了保留原圖)
            new_bmp = new Bitmap(bmp, bmp.Size);
            gg = Graphics.FromImage(new_bmp);

            Pen pen = new Pen(Color.White, 5);
            int Height = (size.Height / LineCount);
            int Width = (size.Width / LineCount);

            
            


            for (int i = 0; i <= LineCount; i++)
            {
                //橫線
                gg.DrawLine(pen,
                    StarPoint.X, StarPoint.Y + i * Height,
                    (size.Width + StarPoint.X), StarPoint.Y + i * Height);
                //直線
                gg.DrawLine(pen,
                    StarPoint.X + i * Width, StarPoint.Y,
                    StarPoint.X + i * Width, (size.Height + StarPoint.Y));

            }
        }
        
        //基礎設定
        private List<Bitmap> BasicSet(int LineCount, int ScreenWidth, int ScreenHeight )
        {
            //尚未結束
            IsOver = false;

            //去除步數紀錄
            GameStar.Clear();
            User.Clear();


            //去除之前的參考圖
            this.Controls.Remove(ll);
            //參考圖
            Ref_bmp = new Bitmap(bmp, Ref_size);
            ll = new Label();
            ll.Location = Ref_Point;
            ll.Text = "";
            ll.AutoSize = false;
            ll.Size = Ref_bmp.Size;
            ll.Image = Ref_bmp;
            this.Controls.Add(ll);


            //原圖先畫線
            _drawLine(LineCount, new Point(0, 0));
            //切割new_bmp(要先加入參考)
            Statistics.Screen sc = new Statistics.Screen(new_bmp, size.Width, size.Height, ScreenWidth, ScreenHeight);


            label1.Focus();

            //回傳切割後的圖片
            return sc.Img;


            
        }

        //用Label裝圖片，輸出
        private void _LabelImage(List<Bitmap> img, int Width_Count, int Height_Count)
        {
            //清除之前的Label
            foreach (var x in lbs)
            {
                this.Controls.Remove(x);
            }
            lbs.Clear();
            lb_Point.Clear();

            







            // Label的位置
            //Y軸
            for (int i = 0; i < Height_Count; i++)
            {
                //X軸
                for (int j = 0; j < Width_Count; j++)
                {
                    Point pt = new Point(StarPoint.X + img[0].Width * j, StarPoint.Y + img[0].Height * i);
                    lb_Point.Add(pt);
                }
            }

            //準備Label
            for (int i = 0; i < Width_Count * Height_Count; i++)
            {
                Label lb = new Label();
                lb.Name = "圖" + i;
                //lb.Text = lb.Name;
                lb.Tag = i;         //偷偷記錄是圖幾
                lb.AutoSize = false;
                lb.Size = img[i].Size;
                lb.Image = img[i];
                lb.Location = lb_Point[i];

                //加入集合
                lbs.Add(lb);
            }

            Random rr = new Random();
            int random = rr.Next(lbs.Count);

           

            //將label加到視窗內
            for (int i = 0; i < lbs.Count; i++)
            {
                //選擇一塊圖片不加入圖片,並放入Control_Label
                if (i == random)
                {

                    lbs[i].Image = null;
                    Control_Label = lbs[i];
                }


                Controls.Add(lbs[i]);
            }


        }




        private void button1_Click(object sender, EventArgs e)
        {
            
            //畫線3*3 切割寬3份 高3份 (回傳一份畫線後切割完畢的圖片)
            List<Bitmap> img = BasicSet(3,3,3);

            //將傳回的圖片加到Label上顯示
            _LabelImage(img, 3, 3);

            //洗牌
            Shuffle();


        }

        //洗牌(不給的話基本30)
        private void Shuffle(int times = 30)
        {
            Random rr = new Random();
            for (int i = 1; i <= 30; i++)
            {
                if (GameStar.Count > 0)
                {
                    //檢查上一次跟這次的路如果無意義(例如:先上後下，先左後右)跳過
                    int a = GameStar.Last();
                    int b = rr.Next(4) + 1;

                    switch (a)
                    {
                        case 1:
                            if (b == 2)
                                continue;
                            else
                                Game_Star(b);
                            break;
                        case 2:
                            if (b == 1)
                                continue;
                            else
                                Game_Star(b);
                            break;
                        case 3:
                            if (b == 4)
                                continue;
                            else
                                Game_Star(b);
                            break;
                        case 4:
                            if (b == 3)
                                continue;
                            else
                                Game_Star(b);
                            break;


                    }
                }
                else { Game_Star(rr.Next(4) + 1); }

            }
        }



        private void Game_Surrender(int run)
        {
            Point temp;  //佔存

            int width = lbs[0].Width;  //X軸一格大小
            int height = lbs[0].Height; //Y軸一格大小

            Point x = Control_Label.Location;

            switch (run)
            {
                case 2:
                    x = new Point(x.X, x.Y - height);
                    foreach (var lb in lbs)
                    {
                        if (lb.Location == x)
                        {
                            temp = lb.Location;
                            lb.Location = Control_Label.Location;
                            Control_Label.Location = temp;
                            
                            break;
                        }
                    }

                    break;

                case 1:
                    x = new Point(x.X, x.Y + height);
                    foreach (var lb in lbs)
                    {
                        if (lb.Location == x)
                        {
                            temp = lb.Location;
                            lb.Location = Control_Label.Location;
                            Control_Label.Location = temp;
                            
                            break;
                        }
                    }

                    break;
                case 4:
                    x = new Point(x.X - width, x.Y);
                    foreach (var lb in lbs)
                    {
                        if (lb.Location == x)
                        {
                            temp = lb.Location;
                            lb.Location = Control_Label.Location;
                            Control_Label.Location = temp;
                           
                            break;
                        }
                    }

                    break;
                case 3:
                    x = new Point(x.X + width, x.Y);
                    foreach (var lb in lbs)
                    {
                        if (lb.Location == x)
                        {
                            temp = lb.Location;
                            lb.Location = Control_Label.Location;
                            Control_Label.Location = temp;
                            
                            break;
                        }
                    }

                    break;
            }
        }


        private void Game_Star(int run)
        {
            Point temp;  //佔存

            int width = lbs[0].Width;  //X軸一格大小
            int height = lbs[0].Height; //Y軸一格大小

            Point x = Control_Label.Location;

            switch (run)
            {
                case 1:
                    x = new Point(x.X, x.Y - height);
                    foreach (var lb in lbs)
                    {
                        if (lb.Location == x)
                        {
                            temp = lb.Location;
                            lb.Location = Control_Label.Location;
                            Control_Label.Location = temp;
                            //紀錄腳步
                            GameStar.Add(1);
                            break;
                        }
                    }

                    break;

                case 2:
                    x = new Point(x.X, x.Y + height);
                    foreach (var lb in lbs)
                    {
                        if (lb.Location == x)
                        {
                            temp = lb.Location;
                            lb.Location = Control_Label.Location;
                            Control_Label.Location = temp;
                            //紀錄腳步
                            GameStar.Add(2);
                            break;
                        }
                    }

                    break;
                case 3:
                    x = new Point(x.X - width, x.Y);
                    foreach (var lb in lbs)
                    {
                        if (lb.Location == x)
                        {
                            temp = lb.Location;
                            lb.Location = Control_Label.Location;
                            Control_Label.Location = temp;
                            //紀錄腳步
                            GameStar.Add(3);
                            break;
                        }
                    }

                    break;
                case 4:
                    x = new Point(x.X + width, x.Y);
                    foreach (var lb in lbs)
                    {
                        if (lb.Location == x)
                        {
                            temp = lb.Location;
                            lb.Location = Control_Label.Location;
                            Control_Label.Location = temp;
                            //紀錄腳步
                            GameStar.Add(4);
                            break;
                        }
                    }

                    break;
            }
        }




        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            
            
                Point temp;  //佔存

                int width = lbs[0].Width;  //X軸一格大小
                int height = lbs[0].Height; //Y軸一格大小

                Point x = Control_Label.Location;

            //紀錄步數
            /*
             * 上下左右 = 1234
            */

            if (!IsOver)
            {

                //按鍵
                switch (e.KeyCode)
                {
                    case Keys.W:
                        x = new Point(x.X, x.Y - height);
                        foreach (var lb in lbs)
                        {
                            if (lb.Location == x)
                            {
                                temp = lb.Location;
                                lb.Location = Control_Label.Location;
                                Control_Label.Location = temp;
                                //紀錄腳步
                                User.Add(1);
                                break;
                            }
                        }

                        break;

                    case Keys.S:
                        x = new Point(x.X, x.Y + height);
                        foreach (var lb in lbs)
                        {
                            if (lb.Location == x)
                            {
                                temp = lb.Location;
                                lb.Location = Control_Label.Location;
                                Control_Label.Location = temp;
                                //紀錄腳步
                                User.Add(2);
                                break;
                            }
                        }

                        break;
                    case Keys.A:
                        x = new Point(x.X - width, x.Y);
                        foreach (var lb in lbs)
                        {
                            if (lb.Location == x)
                            {
                                temp = lb.Location;
                                lb.Location = Control_Label.Location;
                                Control_Label.Location = temp;
                                //紀錄腳步
                                User.Add(3);
                                break;
                            }
                        }

                        break;
                    case Keys.D:
                        x = new Point(x.X + width, x.Y);
                        foreach (var lb in lbs)
                        {
                            if (lb.Location == x)
                            {
                                temp = lb.Location;
                                lb.Location = Control_Label.Location;
                                Control_Label.Location = temp;
                                //紀錄腳步
                                User.Add(4);
                                break;
                            }
                        }

                        break;
                }
            }
            //作弊
            if (e.Control && e.Alt && e.KeyCode == Keys.H)
            {

                cheat = (!cheat);

            }

            if (cheat)
            {

                StringBuilder sb = new StringBuilder();

                foreach (var lb in lbs)
                {
                    lb.Text = lb.Name;
                }

                foreach (var zz in User)
                {
                    sb.Append(zz);
                }
                label1.Text = sb.ToString();
            }
            else
            {
                foreach (var lb in lbs)
                {
                    lb.Text = "";
                }

                label1.Text = "";
            }

            //檢查遊戲是否結束
            CheckWin();


        }

        private void button2_Click(object sender, EventArgs e)
        {
            //畫線3*3 切割寬3份 高3份 (回傳一份畫線後切割完畢的圖片)
            List<Bitmap> img = BasicSet(6, 6, 6);

            //將傳回的圖片加到Label上顯示
            _LabelImage(img, 6, 6);

            //洗牌
            Shuffle(100);


        }

        private void button3_Click(object sender, EventArgs e)
        {
            OFD.Filter = "(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|All files (*.*)|*.* ";
            String Path = "";
            

            if (OFD.ShowDialog() == DialogResult.OK)
            {
                Path = OFD.FileName;
                Image ii = Image.FromFile(Path);
                this.bmp = new Bitmap(ii, size);

                //預設遊戲
                button1_Click(button1, null);
            }
            else { label1.Focus(); }


        }

        private void button4_Click(object sender, EventArgs e)
        {
            
            List<Bitmap> img = BasicSet(4, 4, 4);

            //將傳回的圖片加到Label上顯示
            _LabelImage(img, 4, 4);

            //洗牌
            Shuffle();


        }

        private void button5_Click(object sender, EventArgs e)
        {
            
            List<Bitmap> img = BasicSet(5, 5, 5);

            //將傳回的圖片加到Label上顯示
            _LabelImage(img, 5, 5);

            //洗牌
            Shuffle();


        }

        private  void  button6_Click(object sender, EventArgs e)
        {
            if (!IsOver)
            {
                way = new List<int>();
                for (int i = User.Count - 1; i >= 0; i--)
                {
                    way.Add(User[i]);
                }

                for (int i = GameStar.Count - 1; i >= 0; i--)
                {
                    way.Add(GameStar[i]);
                }
                /*
                foreach (var x in way)
                {
                    Game_Surrender(x);
                }
                */

                ThreadStart ts = new ThreadStart(_Move);
                Thread th = new Thread(ts);
                th.Start();



                CheckWin();
            }
        }
        void _Move()
        {
            foreach(var x in way)
            {
                Real_move(x);
            }

        }

        delegate void move(int way);
        void Real_move(int way)
        {
            if (label1.InvokeRequired)
            {
                move m = new move(Real_move);
                this.Invoke(m, way);
            }
            else
            {
                if (!IsOver)
                {
                    Game_Surrender(way);
                    CheckWin();
                }
                
            }
            Thread.Sleep(150);
        }














        //檢查遊戲是否結束
        private void CheckWin()
        {
            int Count = 0;

            //檢查所有的label都在正確位置上
            for (int i = 0; i < lbs.Count; i++)
            {
                if (lbs[i].Location == lb_Point[i])
                {
                    Count++;
                }
            }

            if (Count == lbs.Count)
            {
               
                IsOver = true;
                MessageBox.Show("Game Over! 請重新開始");
            }
            
        }

        


    }
    
}
