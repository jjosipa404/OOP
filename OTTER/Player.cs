using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OTTER
{
    class Player: Sprite
    {
       
        private int score;
        public int Score { get => score; set => score = value; }

        private bool trigger;
        public bool Trigger { get => trigger; set => trigger = value; }
         
        private int floorCount;
        public int FloorCount { get => floorCount; set => floorCount = value; }

        public Player(string img,int _x,int _y) :base(img,_x,_y)
        {
            this.Score = 0;
            this.FloorCount = 0;
            this.Trigger = false;
        }


        public override int Y
        {
            get
            {
                
                return y;
            }
            set
            {
                if (value >= 700 && this.Trigger == false)
                {//dok se jos player nije popeo na y=225 - tada je trigger = false,ogranicimo Y na 700
                    //da player bude na flooru
                    //downedge(800)-floorHeight(50)-playerHeight(50)=700
                    this.y = 700;
                }
                
                else
                    this.y = value;
           
            }
        }
        public override int X
        {
            get
            {

                return x;
            }
            set
            {//ogranicimo da player ne izlazi iz lijevog i desnog ruba ekrana
                if (value >= GameOptions.RightEdge - this.Width)
                {
                    this.x = GameOptions.RightEdge - this.Width;
                }
                else if (value <= 0)
                {
                    this.x = 0;
                }
                else
                {
                    this.x = value;

                }
            }
        }


    }
}
