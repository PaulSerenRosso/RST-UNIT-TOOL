using System;
using System.Collections.Generic;

[Serializable]
public class Player
{
   public PlayerName Name ;
   public List<Squad> AllSquads; 
   public List<PlayerName> AlliedPlayers;
   public List<PlayerName> EnemyPlayers;
}
