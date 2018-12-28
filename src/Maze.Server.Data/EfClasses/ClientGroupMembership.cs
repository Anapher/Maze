namespace Maze.Server.Data.EfClasses
{
    public class ClientGroupMembership
    {
        public int ClientId { get; set; }
        public int ClientGroupId { get; set; }

        public Client Client { get; set; }
        public ClientGroup ClientGroup { get; set; }
    }
}
