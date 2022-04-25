using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
public List<Player> AllPlayersList;

 public Dictionary<PlayerName, Player> AllPlayers = new Dictionary<PlayerName, Player>();
public List<string> AllTypesPlayer;
public static PlayerManager Instance { get; private set; }
 
void Awake()
{
 if (Instance != null && Instance != this)
  Destroy(gameObject);    // Suppression d'une instance précédente (sécurité...sécurité...)
 
 Instance = this;
}

void Start()
{
 for (int i = 0; i < AllPlayersList.Count; i++)
  AllPlayers.Add(AllPlayersList[i].Name, AllPlayersList[i]);
}
}
