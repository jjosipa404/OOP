using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OTTER
{
    class Gem : Collectables
    {
        public Gem(string img, int _x, int _y) : base(img, _x, _y)
        {
            this.Points = 10;
        }
        
        public void GotoXY()
        {
            Random g = new Random();
            this.X = g.Next(0, GameOptions.RightEdge - this.Width);
            this.Y = g.Next(-(GameOptions.DownEdge - this.Heigth),0);
        }

    }

    class ExtraGem : Collectables
    {
        private bool trigger;
        public bool Trigger { get => trigger; set => trigger = value; }

        public ExtraGem(string img, int _x, int _y) : base(img, _x, _y)
        {
            this.Points = 500;
        }


    }

    class FalseGem : Collectables
    {
        private bool trigger;
        public bool Trigger { get => trigger; set => trigger = value; }

        public FalseGem(string img, int _x, int _y) : base(img, _x, _y)
        {
            this.Points = -1000;
        }

    }


}
       


        
