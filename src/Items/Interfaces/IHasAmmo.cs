using System.Collections.Generic;

interface IHasAmmo {
    int CheckAmmo(string ammoType, int max);
    List<ItemData> RequestAmmo(string ammoType, int max);
    List<ItemData> StoreAmmo(List<ItemData> ammo);
    string[] AmmoTypes();
}
