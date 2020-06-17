using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OTTER
{
    public abstract class Collectables:Sprite
    {
        private int points;
        public int Points { get => points; set => points = value; }

        public Collectables(string img, int _x, int _y) : base(img, _x, _y)
        {

        }
        
    }

}
