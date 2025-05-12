using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Windows.Threading;

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool inAir = false;
        const double arrowScale =  150;
        double deltaTime = 0.016;
        int level = 0;
        Airplane airplane;
        Atmosphere atmosphere;
        DispatcherTimer flightTimer;
        Point planeCenter;


        public MainWindow()
        {
            InitializeComponent();
            airplane = new Airplane(int.Parse(MassTextBox.Text), double.Parse(VelocityTextBox.Text), int.Parse(WingAreaTextBox.Text));
            atmosphere = new Atmosphere(airplane, double.Parse(AirDensityTextBox.Text), double.Parse(GravityTextBox.Text));

            flightTimer = new DispatcherTimer();
            flightTimer.Interval = TimeSpan.FromSeconds(deltaTime);
            flightTimer.Tick += FlightTimer_Tick;

            airplane.Pos_x = 20;
            airplane.Pos_y = 200;
            updateGround();
        }
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            airplane.Mass = int.Parse(MassTextBox.Text);
            airplane.Velocity_x = double.Parse(VelocityTextBox.Text);
            airplane.Velocity_y = 0;
            airplane.WingArea = int.Parse(WingAreaTextBox.Text);
            airplane.AngleDeg = 0;
            airplane.Thrust = airplane.Velocity_x *  150;
            airplane.PitchAngle = 5;

            atmosphere.Density = double.Parse(AirDensityTextBox.Text);
            atmosphere.Gravity = double.Parse(GravityTextBox.Text);

            Canvas.SetLeft(AirplaneImage, 20);
            Canvas.SetTop(AirplaneImage, 200);
            planeCenter = new Point(20,
                                    200);

            airplane.Pos_x = 20;
            airplane.Pos_y = 200;

            inAir = true;
            LineCanvas.Children.Clear();
            updateGround();
            flightTimer.Start();
        }
        private void FlightTimer_Tick(object sender, EventArgs e)
        {
            if (inAir)
                CalculateFly();
        }

        private void CalculateFly()
        {

            updatePitch();
            airplane.CalculateFly(atmosphere, deltaTime);
            updateMotor();
            updateUI();
        }

        private void updatePitch()
        {
            int pitchRate = 0;

            if (Keyboard.IsKeyDown(Key.W))
                pitchRate = 30;
            if (Keyboard.IsKeyDown(Key.S))
                pitchRate = -30;

            airplane.PitchAngle += pitchRate * deltaTime;
        }

        public void updateUI()
        {
            updatePlanePos();
            checkBorders();
            updatePanel();
            updateGround();
            drawArrows(Canvas.GetLeft(AirplaneImage) + AirplaneImage.Width / 2,
                       Canvas.GetTop(AirplaneImage) + AirplaneImage.Height / 2);
        }

        private void updatePlanePos()
        {
            double tmp_x = Canvas.GetLeft(AirplaneImage);
            double tmp_y = Canvas.GetTop(AirplaneImage);

            double pos_x = tmp_x + airplane.Velocity_x * deltaTime;
            double pos_y = tmp_y + airplane.Velocity_y * deltaTime;

            Canvas.SetLeft(AirplaneImage, pos_x);
            Canvas.SetTop(AirplaneImage, pos_y);
            airplane.Pos_x += airplane.Velocity_x * deltaTime;
            airplane.Pos_y += airplane.Velocity_y * deltaTime;
            AirplaneImage.RenderTransform = new RotateTransform(airplane.AngleDeg,
                AirplaneImage.Width / 2, AirplaneImage.Height / 2);

            drawTrail();
        }

        private void drawTrail()
        {
            double angleDeg = airplane.AngleDeg * Math.PI / 180;

            double tailOffset_x = -(AirplaneImage.Width / 2) * Math.Cos(angleDeg);
            double tailOffset_y = -(AirplaneImage.Width / 2) * Math.Sin(angleDeg);

            double new_x = Canvas.GetLeft(AirplaneImage);
            double new_y = Canvas.GetTop(AirplaneImage);
            double newTail_x = new_x + AirplaneImage.Width / 2 + tailOffset_x;
            double newTail_y = new_y + AirplaneImage.Height / 2 + tailOffset_y;

            double oldTail_x = planeCenter.X + AirplaneImage.Width / 2 + tailOffset_x;
            double oldTail_y = planeCenter.Y + AirplaneImage.Height / 2 + tailOffset_y;

            planeCenter.X = new_x;
            planeCenter.Y = new_y;

            drawCanvasLine(oldTail_x, oldTail_y, 
                           newTail_x, newTail_y,
                           LineCanvas, Brushes.Black);
        }

        private void checkBorders()
        {
            double pos_x = Canvas.GetLeft(AirplaneImage);
            double pos_y = Canvas.GetTop(AirplaneImage);

            if (pos_x >= Canvas.Width - AirplaneImage.Width/2)
            {
                pos_x = 0;
                planeCenter.X = 0;
                LineCanvas.Children.Clear();
                
            }
            else if (pos_x <= 0)
            {
                pos_x = Canvas.Width - AirplaneImage.Width/2 -20;
                planeCenter.X = Canvas.Width - 10;
                LineCanvas.Children.Clear();
            }
            else if (pos_y <= 0)
            {
                pos_y = Canvas.Height - 10;
                planeCenter.Y = Canvas.Height - 10;
                LineCanvas.Children.Clear();
                level++;
            }
            else if (pos_y >= Canvas.Height && airplane.Pos_y >= 0)
            {
                pos_y = 0;
                planeCenter.Y = 0;
                LineCanvas.Children.Clear();
                level--;
            }
            else if (airplane.Pos_y >= Canvas.Height - AirplaneImage.Width/2)
            {
                inAir = false;
            }

            Canvas.SetLeft(AirplaneImage, pos_x);
            Canvas.SetTop(AirplaneImage, pos_y);
        }

        private void updatePanel()
        {
            ThrustStat.Content = $"Meghajtás: {airplane.Thrust:0.####}";
            SpeedStat.Content = $"Sebesség: {airplane.Speed:0.####}";
            DragStat.Content = $"Ellenállás: {airplane.Drag:0.####}";
            GravStat.Content = $"Gravitáció: {airplane.ForceGravity:0.####}";
            LiftStat.Content = $"Felhajtóerő: {airplane.Lift:0.##}";
            LiftCoeff.Content = $"FelEggy: {airplane.LiftCoeff:0.####}";
            Aoa.Content = $"Támadási szög: {airplane.Aoa:0.####}";
            //DragCoeff.Content = $"EllEggy: {airplane.DragCoeff:0.####}";
            //XVelocityStat.Content = $"X sebesség: {airplane.Velocity_x:0.####}";
            //YVelocityStat.Content = $"Y sebesség: {-airplane.Velocity_y:0.####}";
            //XAccelStat.Content = $"X gyorsulás: {airplane.Accel_x:0.####}";
            //YAccelStat.Content = $"Y gyorsulás: {airplane.Accel_y:0.####}";
            //XPosStat.Content = $"X pozíció: {Canvas.GetLeft(AirplaneImage):0.####}";
            //YPosStat.Content = $"Y pozíció: {Canvas.GetTop(AirplaneImage):0.####}";
            //XActual.Content = $"X koordináta: {airplane.Pos_x:0.####}";
            //YActual.Content = $"Y koordináta: {airplane.Pos_y:0.####}";
            //PlaneAngle.Content = $"Bezárt szög: {-airplane.AngleDeg:0.####}";
        }

        private void drawCanvasLine(double x1, double y1, double x2, double y2, Canvas canvas, SolidColorBrush color)
        {
            canvas.Children.Add(new Line
            {
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2,
                Stroke = color,
                StrokeThickness = 2
            });
        }
        private void drawArrows(double start_x, double start_y)
        {
            ArrowCanvas.Children.Clear();
            drawCanvasLine(start_x, start_y,
                start_x + airplane.ForceThrust_x / arrowScale,
                start_y + airplane.ForceThrust_y / arrowScale,
                ArrowCanvas, Brushes.Red);
            drawCanvasLine(start_x, start_y,
                start_x + airplane.ForceDrag_x / arrowScale,
                start_y + airplane.ForceDrag_y / arrowScale,
                ArrowCanvas, Brushes.Blue);
            drawCanvasLine(start_x, start_y,
                start_x + airplane.ForceLift_x / arrowScale,
                start_y + airplane.ForceLift_y / arrowScale,
                ArrowCanvas, Brushes.Green);
            drawCanvasLine(start_x, start_y,
                start_x,
                start_y + airplane.ForceGravity / arrowScale,
                ArrowCanvas, Brushes.HotPink);
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            inAir = false;
        }

        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            inAir = true;
            flightTimer.Start();
        }

        static Rectangle ground = new Rectangle
        {
            Width = 800,
            Height = 30,
            Stroke = Brushes.Green,
            StrokeThickness = 2,
            Fill = Brushes.Green
        };

        private void updateGround()
        {
            if (!LineCanvas.Children.Contains(ground) && level == 0)
            {
                LineCanvas.Children.Add(ground);
                Canvas.SetTop(ground, Canvas.Height);
            }
            else if (LineCanvas.Children.Contains(ground) && level != 0)
                LineCanvas.Children.Remove(ground);
        }

        bool haveMotor = false;
        bool motorWorking = false;
        private void Motor_Click_1(object sender, RoutedEventArgs e)
        {
            haveMotor = !haveMotor;
        }

        private void updateMotor()
        {
            if (haveMotor && !motorWorking)
                airplane.Thrust = (airplane.Thrust > 0.0001 ? airplane.Thrust * 0.999 : 0);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.P && haveMotor)
            {
                airplane.Thrust +=  150;
                motorWorking = true;
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.P && haveMotor)
            {
                motorWorking = false;
            }
        }

        private void DeltaTime_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            deltaTime = e.NewValue;
        }


    }
}
