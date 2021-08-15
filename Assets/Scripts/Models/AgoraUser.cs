using System;

namespace Assets.Scripts.Models
{
    public class AgoraUser
    {
        public int Id { get; set; }
        public uint Uid { get; set; }
        public bool IsLocal { get; set; }
        public bool Display { get; set; }
        public int DisplayId { get; set; }
        public DateTime DateJoined { get; set; }
        public bool LeftRoom { get; set; }
    }
}
