using System.ComponentModel.DataAnnotations;

namespace BattleShip.models 
{
    
    public class MoveEntity
    {
        public int Id { get; set; } 
        public int PlayerId { get; set; }
        [Range(1, 9)]
        public int Row { get; set; }
        [Range(1, 9)]
        public int Column { get; set; } 
        public bool Hit { get; set; } 
        public DateTime Time { get; set; } 

        public PlayerEntity Player { get; set; }
    }
}