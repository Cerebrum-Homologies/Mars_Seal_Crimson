using System.Collections.Generic;

namespace Mars_Seal_Crimson
{
    public class InventoryData
    {
        private List<InventoryItem> inventoryStore;
        private Dictionary<string, string> inventoryNameMap = new Dictionary<string, string>();

        public void AddItem(string itemName, InventoryItem inventoryItem) {
            if (inventoryItem != null) {
                inventoryNameMap.Add(itemName, inventoryItem.name);
                inventoryStore.Add(inventoryItem);
            }
        }
    }
}