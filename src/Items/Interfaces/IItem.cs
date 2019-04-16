interface IItem {
    
    public void Use(Item.ItemInputs input);
    public void Equip(object wielder);
    public void Unequip();
    
    // Simple Getters
    public object GetWielder();
    public int GetId();
    public Node GetNode();

    // Setters
    public void SetCollision(bool val);

    // For serialization
    public ItemFactory.Items GetItemEnum();
    public void LoadJson(string json);
    public string GetJson();

    // provide a delegate factory
    public List<ItemFactory.Items> GetSupportedItems();
    public IItem Factory(ItemFactory.Items item);
}