using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class Player
{
   public PlayerName Name ;
   public List<Squad> AllSquads; 
   public List<PlayerName> AlliedPlayers;
   public List<PlayerName> EnemyPlayers;
}
