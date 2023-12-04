using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

//Вариант 23
//Написать упрошённую версию игры “Снег на голову”, используя графическую сцену:
//В случайном месте верхней части сцены появляется объект, в дальнейшем называемый “Снег”,
//который со случайной скоростью начинает падать вниз.
//В нижней части экрана расположен объект, в дальнейшем называемый“Игрок”,
//которым можно управлять клавишами мыши. В случае столкновения снега с игроком,
//выводится сообщение “Ну вооот” и приложение закрывается.
//В случае, если снег достиг нижней границы сцены, он перемещается в случайное место сверху экрана.
//При желании, можно добавить подсчёт очков. 

namespace VP_pr5_fur_Elina
{
    public abstract class CustomControl : Control
    {
        protected static string imgDirectory = Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).ToString()).ToString() + "\\img\\";
        public CustomControl() { }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
        }
    }
    public class Igrok : CustomControl
    {

        public static string imgLink = imgDirectory + "\\igrok.png";
        public Igrok()
        {
        }

        public Image GetImage()
        {
            Image igrok_image = new Image();
            igrok_image.Width = 150;
            igrok_image.Height = 120;
            igrok_image.Source = new BitmapImage(new Uri("img/igrok.png", UriKind.Relative));
            return igrok_image;
        }
    }

    public class Sneg : CustomControl
    {

        public static string imgLink = imgDirectory + "\\sneg.png";
        public Sneg()
        {
        }
        public Image GetImage()
        {
            Image sneg_image = new Image();
            sneg_image.Width = 90;
            sneg_image.Height = 90;
            sneg_image.Source = new BitmapImage(new Uri("img/sneg.png", UriKind.Relative));
            return sneg_image;
        }

    }
    public partial class MainWindow : Window
    {
        Igrok igrok;
        Sneg sneg;
        Image ig;
        Image sn;
        Random rnd = new Random();
        System.Windows.Threading.DispatcherTimer collisionTimer = new System.Windows.Threading.DispatcherTimer();
        System.Windows.Threading.DispatcherTimer snegTimer = new System.Windows.Threading.DispatcherTimer();
        int score = 0;
        bool isChanged = false;

        public MainWindow()
        {
            InitializeComponent();

            igrok = new Igrok();
            sneg = new Sneg();

            ig = igrok.GetImage();
            ig.Name = "igrok";
            int x = 325;
            int y = 430;
            MainCanvas.Children.Add(ig);
            Canvas.SetTop(ig, y);
            Canvas.SetLeft(ig, x);

            sn = sneg.GetImage();
            sn.Name = "sneg";
            sn.PreviewMouseDown += Change_Kalmuflyage;
            x = rnd.Next(0, 510);
            y = 0;
            Panel.SetZIndex(sn, 1);
            MainCanvas.Children.Add(sn);
            Canvas.SetTop(sn, y);
            Canvas.SetLeft(sn, x);

            PreviewKeyDown += (s, e) => { if (e.Key == Key.Escape) Close(); };
            PreviewMouseLeftButtonDown += Move_Left;
            PreviewMouseRightButtonDown += Move_Right;

            collisionTimer.Tick += new EventHandler(Check_Collision);
            snegTimer.Tick += new EventHandler(Sneg_Fall);
            collisionTimer.Interval = new TimeSpan(100);
            snegTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            collisionTimer.Start();
            snegTimer.Start();
        }


        private void Reset_Sneg()
        {
            int x = rnd.Next(0, (int)MainCanvas.ActualWidth - 90), y = 0;
            Canvas.SetTop(sn, y);
            Canvas.SetLeft(sn, x);
            score++;
            Score.Content = score.ToString();
        }

        private void Sneg_Fall(object sender, EventArgs e)
        {
            double s_y;
            s_y = (double)sn.GetValue(Canvas.TopProperty);
            if (s_y >= (int)MainCanvas.ActualHeight - 90)
            {
                Reset_Sneg();
                return;
            }
            
            Canvas.SetTop(sn, s_y + 30);
        }

        private void Change_Kalmuflyage(object sender, EventArgs e)
        {
            if(!isChanged)
                ((Image)sender).Source = new BitmapImage(new Uri("img/sneg_kal.png", UriKind.Relative));
            else
                ((Image)sender).Source = new BitmapImage(new Uri("img/sneg.png", UriKind.Relative));
            isChanged = !isChanged;
        }

        private void Move_Left(object sender, MouseButtonEventArgs e)
        {
            double i_x;
            i_x = (double)ig.GetValue(Canvas.LeftProperty);
            if (i_x <= 20)
                return;
            Canvas.SetLeft(ig, i_x - 30);
        }
        private void Move_Right(object sender, MouseButtonEventArgs e) 
        {
            double i_x;
            i_x = (double)ig.GetValue(Canvas.LeftProperty);
            if (i_x >= MainCanvas.ActualWidth - 160)
                return;
            Canvas.SetLeft(ig, i_x + 30);
        }
        
        private void Check_Collision(object sender, EventArgs args)
        { 
            double i_x = (double)ig.GetValue(Canvas.LeftProperty);
            double i_y = (double)ig.GetValue(Canvas.TopProperty);

            double s_x = (double)sn.GetValue(Canvas.LeftProperty);
            double s_y = (double)sn.GetValue(Canvas.TopProperty);

            double x_dif;
            bool x_collide = false;

            x_dif = i_x - s_x;
            if (x_dif > 0)
            {
                x_dif += 15;
                if (x_dif < 90)
                    x_collide = true;
            }
            if (x_dif < 0 && x_dif > -135)
                x_collide = true;

            if ( x_collide && Math.Abs(i_y - s_y) < 90)
            {
                collisionTimer.Stop();
                snegTimer.Stop();
                MessageBox.Show("Ну вооот", "Сообщение");
                Thread.Sleep(1500);
                this.Close();
            }

        }
   
    }
}
