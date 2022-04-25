using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Player
{     
   public PlayerName Name ;
   public List<Squad> AllSquads; 
   public List<PlayerName> AlliedPlayers;
   public List<PlayerName> EnemyPlayers;
   public Material Material;
}
