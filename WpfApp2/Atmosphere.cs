using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2
{
    internal class Atmosphere
    {
        Airplane airplane;
        double density;
        double gravity;

        public Atmosphere(Airplane airplane, double density, double gravity)
        {
            Airplane = airplane;
            Density = density;
            Gravity = gravity;
        }

        public double Density { get => density; set => density = value; }
        public double Gravity { get => gravity; set => gravity = value; }
        internal Airplane Airplane { get => airplane; set => airplane = value; }
    }
}
