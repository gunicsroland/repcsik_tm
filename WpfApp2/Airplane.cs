using System.Windows.Controls;
using System.Windows.Media;
using System;

namespace WpfApp2
{
    internal class Airplane
    {
        int mass;
        double velocity_x;
        double velocity_y;
        int wingArea;
        double dragCoeef;
        double liftCoeff;
        double planeAngle;

        double pos_x;
        double pos_y;

        double pitchAngle;

        public int Mass { get => mass; set => mass = value; }
        public double Velocity_x { get => velocity_x; set => velocity_x = value; }
        public double Velocity_y { get => velocity_y; set => velocity_y = value; }
        public int WingArea { get => wingArea; set => wingArea = value; }
        public double DragCoeff { get => dragCoeef; set => dragCoeef = value; }
        public double LiftCoeff { get => liftCoeff; set => liftCoeff = value; }
        public double Pos_x { get => pos_x; set => pos_x = value; }
        public double Pos_y { get => pos_y; set => pos_y = value; }
        public double AngleDeg { get => planeAngle; set => planeAngle = value; }
        public double Thrust { get => thrust; set => thrust = value; }
        public double Speed { get => speed; set => speed = value; }
        public double Drag { get => drag; set => drag = value; }
        public double ForceGravity { get => forceGravity; set => forceGravity = value; }
        public double Lift { get => lift; set => lift = value; }
        public double Accel_x { get => accel_x; set => accel_x = value; }
        public double Accel_y { get => accel_y; set => accel_y = value; }
        public double ForceThrust_x { get => forceThrust_x; set => forceThrust_x = value; }
        public double ForceThrust_y { get => forceThrust_y; set => forceThrust_y = value; }
        public double ForceDrag_x { get => forceDrag_x; set => forceDrag_x = value; }
        public double ForceDrag_y { get => forceDrag_y; set => forceDrag_y = value; }
        public double ForceLift_x { get => forceLift_x; set => forceLift_x = value; }
        public double ForceLift_y { get => forceLift_y; set => forceLift_y = value; }
        public double PitchAngle { get => pitchAngle; set => pitchAngle = value; }
        public double Aoa { get => aoa; set => aoa = value; }

        public Airplane(int mass, double velocity, int wingArea)
        {
            this.mass = mass;
            this.velocity_x = velocity;
            this.wingArea = wingArea;
            PitchAngle = 5;
        }

        double angleRad;
        double thrust;
        double speed;
        double drag;
        double forceGravity;
        double lift;
        double forceLift_x;
        double forceLift_y;
        double forceDrag_x;
        double forceDrag_y;
        double forceThrust_x;
        double forceThrust_y;
        double force_x;
        double force_y;
        double accel_x;
        double accel_y;
        double aoa;

        public void CalculateFly(Atmosphere atmosphere, double deltaTime)
        {
            Speed = Math.Sqrt(Velocity_x * Velocity_x + Velocity_y * Velocity_y);
            angleRad = Math.Atan2(Velocity_y, Velocity_x);
            AngleDeg = angleRad * (180 / Math.PI);

            double PitchAngleRad = pitchAngle * (Math.PI / 180);
            double angleOfAttackRad = angleRad - PitchAngleRad;
            while (angleOfAttackRad > Math.PI) angleOfAttackRad -= 2 * Math.PI;
            while (angleOfAttackRad < -Math.PI) angleOfAttackRad += 2 * Math.PI;

            aoa = angleOfAttackRad * (180 / Math.PI);

            ForceGravity = Mass * atmosphere.Gravity;

            updateCoeffs(angleOfAttackRad);

            Drag = 0.5 * atmosphere.Density * Speed * Speed * WingArea * DragCoeff;
            Lift = 0.5 * atmosphere.Density * Speed * Speed * WingArea * LiftCoeff;

            double dragLocal_x = -Drag;
            double liftLocal_y = -Lift;
            double thrustLocal_x = Thrust;

            ForceLift_x = -Math.Sin(angleRad) * liftLocal_y;
            ForceLift_y = Math.Cos(angleRad) * liftLocal_y;

            ForceDrag_x = Math.Cos(angleRad) * dragLocal_x;
            ForceDrag_y = Math.Sin(angleRad) * dragLocal_x;

            ForceThrust_x = Math.Cos(angleRad) * thrustLocal_x;
            ForceThrust_y = Math.Sin(angleRad) * thrustLocal_x;

            force_x = ForceThrust_x + ForceDrag_x + ForceLift_x;
            force_y = ForceGravity + ForceDrag_y + ForceLift_y;

            Accel_x = force_x / Mass;
            Accel_y = force_y / Mass;

            Velocity_x += Accel_x * deltaTime;
            Velocity_y += Accel_y * deltaTime;
        }

        private void updateCoeffs(double angleOfAttackRad)
        {
            LiftCoeff = 1.2 * Math.Sin(2 * angleOfAttackRad);
            DragCoeff = 0.05 + 0.1 * angleOfAttackRad * angleOfAttackRad;

            if (Math.Abs(angleOfAttackRad * (180 / Math.PI)) > 20)
            {
                LiftCoeff *= 0.7;
                DragCoeff *= 1.5;
            }
        }


    }
}