/*
  An IStats is the business logic behind RPG elements of an in-game character.
*/
using System.Collections.Generic;

public interface IStats {
  bool HasStat(string stat);
  List<string> GetStatList();
  int GetStat(string stat);
  void SetStat(string stat, int val);
  string GetFact(string fact);
  void SetFact(string fact, string val);
  bool StatCheck(string stat, int difficulty);
  bool ConsumeStat(string stat, int amount); 
  void ReceiveDamage(Damage damage);
  void RestoreCondition();
  void Update(float delta);
}