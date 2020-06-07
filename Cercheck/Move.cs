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
        public double Weight { get; set; } = 0;
        public Move PreviousMove { get; set; }
        public Move(Point startPoint, Point destinationPoint)
        {
            this.StartPoint = startPoint;
            this.DestinationPoint = destinationPoint;
        }
        public Move(Point startPoint, Point destinationPoint, Move prevMove)
        {
            this.StartPoint = startPoint;
            this.DestinationPoint = destinationPoint;
            this.PreviousMove = prevMove;
        }
    }
}
