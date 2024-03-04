using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment1
{
    public class GameObject
    {
        public Point Position { get; set; }
        public bool IsAlive { get; set; } = true;

        public GameObject(Point position)
        {
            Position = position;
        }
    }
}
