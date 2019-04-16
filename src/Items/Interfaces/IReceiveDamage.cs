// Interface for anything that can be damaged.
using System;

interface IReceiveDamage {

	void ReceiveDamage(Damage damage);
    int GetHealth();
}