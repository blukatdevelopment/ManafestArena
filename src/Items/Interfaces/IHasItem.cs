using System.Collections.Generic;

interface IHasItem{

  bool HasItem(string item);
  string ItemInfo();
  bool ReceiveItem(Item item);
  Item PrimaryItem();
  int ItemCount();
  List<ItemData> GetAllItems();
}
