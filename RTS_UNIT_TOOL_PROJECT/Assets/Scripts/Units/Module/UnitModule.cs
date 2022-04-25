using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class UnitModule : MonoBehaviour
{
 public UnitScript Unit;

 public virtual void OnValidate()
 {
  if (Unit == null)
  TryGetComponent(out Unit);
 }

 public abstract void OnStart();
 public abstract void AskUpdate();
 public abstract void OnUpdate();
}
