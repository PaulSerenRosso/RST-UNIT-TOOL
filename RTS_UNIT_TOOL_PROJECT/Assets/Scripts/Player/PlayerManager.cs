using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
 [SerializeField]
 private List<Player> _allPlayers;

 public Dictionary<PlayerName, Player> AllPlayers;
public List<string> AllTypesPlayer;


void Start()
{
 for (int i = 0; i < _allPlayers.Count; i++)
  AllPlayers.Add(_allPlayers[i].Name, _allPlayers[i]);
}
}
