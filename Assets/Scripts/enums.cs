using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
   public enum RailDirection {
        Left,
        Forward,
        Right,
        LeftCircle,
        RightCircle
   }

   public enum SwipeDirection
   {
       Up,
       Down,
       Right,
       Left
   }

   public enum CircleRailConfig
   {
       Stop,
       Points,
       StopWithPoints,
       Clear
   }

   public enum AudioClipType
   {
        Swipe,
        StopHit,
        Coin,
        NewTrain,
        BoostStart,
        BoostEnd,

   }
}
