// Interface for anything that can be damaged.
using System;

public interface IReceiveDamage {

	void ReceiveDamage(Damage damage);
    int GetHealth();
}