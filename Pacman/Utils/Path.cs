using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace PacmanGame.Utils
{
    class Path
    {
        Vector2 p1;
        Vector2 p2;

        float angle;
        int distance;
        float currDist;
        float excess;

        public Path()
        {
            p1 = new Vector2();
            p2 = new Vector2();
        }

        public void update(float speed)
        {
            if (isDone())
                return;

            currDist += speed;

            if (currDist >= distance)
            {
                excess = currDist - distance;
                
                currDist = distance;
            }
        }

        public void set(float p1x, float p1y, float p2x, float p2y)
        {
            p1.X = p1x;
            p1.Y = p1y;
            p2.X = p2x;
            p2.Y = p2y;

            distance = (int)Math.Sqrt((p2.X - p1.X) * (p2.X - p1.X) + (p2.Y - p1.Y) * (p2.Y - p1.Y));
            currDist = excess;
            excess = 0;

            float dx = p2.X - p1.X;
            float dy = p2.Y - p1.Y;

            angle = (float) Math.Atan2(dy, dx);
        }

        public Vector2 setOnPath(Vector2 pos)
        {
            pos.X = p1.X + (float) (Math.Cos(angle) * currDist); 
            pos.Y = p1.Y + (float) (Math.Sin(angle) * currDist);

            return pos;
        }

        public void setCurrDistance(int dist)
        {
            currDist = dist;
            if (currDist > distance)
                currDist = distance;
        }

        public bool isDone()
        {
            return currDist >= distance;
        }
    }
}
