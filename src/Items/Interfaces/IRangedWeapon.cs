public interface IRangedWeapon {
  float GetEffectiveRange();
  int GetAmmo();
  Damage GetDamage();
  float AttackDelay();
}