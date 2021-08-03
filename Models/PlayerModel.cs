using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Task6.Models
{
    public class PlayerModel 
    {
        public string Id { get; set; }

        public string GameId { get; set; }
        public PlayerModel(string playerId)
        {
            Id = playerId;
        }

        public static bool operator ==(PlayerModel p1, PlayerModel p2)
        {
            if((Object)p1 == null && (Object)p2 == null)
                return true;

            if((Object)p1 == null || (Object)p2 == null)
                return false;
                
            return p1.Id == p2.Id;
        }

        public static bool operator !=(PlayerModel p1, PlayerModel p2)
        {
            if(p1 == null && p2 ==  null)
                return false;
            if(p1 == null || p2 == null)
                return true;

            return p1.Id != p2.Id;
        }

        public override bool Equals(object obj)
        {
            PlayerModel other = obj as PlayerModel;

            if (other == null)
            {
                return false;
            }

            return this.Id.Equals(other.Id);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}