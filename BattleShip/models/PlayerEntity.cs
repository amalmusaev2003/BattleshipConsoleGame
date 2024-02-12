using System.ComponentModel.DataAnnotations;

namespace BattleShip.models
{
    public class PlayerEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<MoveEntity> Moves { get; set; }
        
    }
}
