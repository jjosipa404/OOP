using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OTTER
{
    class Brick:Sprite
    {
        public Brick(string img, int _x, int _y) : base(img, _x, _y)
        {

        }

        public void BrickChange(string[] images,ref List<Brick> bricksHopOn)
        {
            //kada platforma izadje iz ekrana
            //promijeni random x i random sliku,postavi sprite iznad ekrana
            Random g = new Random();
            string randI = images[g.Next(0, 3)];
            this.ChangeImage(randI);

            this.X = g.Next(0, 600);
            this.Y = -50;

            //izbaci tu platformu iz liste na koje je vec skoceno
            if (bricksHopOn.Count != 0)
            {
                for (int j = 0; j < bricksHopOn.Count; j++)
                {
                    if (bricksHopOn[j] == this)
                    {
                        bricksHopOn.RemoveAt(j);
                    }
                }
            }
        }
      
    }
}
