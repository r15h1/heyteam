namespace HeyTeam.Core.Entities
{
    public class Player
    {
        public enum Foot 
        {
            RIGHT,
            LEFT
        }

        public long? Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Nationality { get;set; }
        public string SquadNumber {get; set; }
        public Foot DominantFoot { get; set; } = Foot.RIGHT;
    }
}