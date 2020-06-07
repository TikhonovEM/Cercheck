using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cercheck
{
    public class Move
    {
        public Point StartPoint { get; set; }
        public Point DestinationPoint { get; set; }
        public Move(Point startPoint, Point destinationPoint)
        {
            this.StartPoint = startPoint;
            this.DestinationPoint = destinationPoint;
        }
    }
}
