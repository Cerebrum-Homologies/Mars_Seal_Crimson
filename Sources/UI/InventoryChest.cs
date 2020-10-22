using Godot;
using System.Collections.Generic;
using System;

namespace Mars_Seal_Crimson
{
	public class InventoryChest : Control
	{
		private Godot.Panel chestPanel;
		private Godot.Timer eventsTimer;
		private Godot.PanelContainer itemSlotsPanel;
		private Godot.PanelContainer inventoryActionsPanel;
		private Dictionary<string, Godot.Sprite> inventoryItemsDisplayMap = new Dictionary<string, Godot.Sprite>();
		private Dictionary<string, string> inventorySpriteResourceMap = new Dictionary<string, string>();
		private Dictionary<string, int> inventorySlotIndexMap = new Dictionary<string, int>();
		private InventoryData inventoryStoreCollection = new InventoryData();
		private int maxItemSlots = 8;
		private int eventsTimerCounter = 0;
		private Vector2 itemBoxDimension; //The dimension for the inventory slot area
		public override void _Ready()
		{
			GD.Print("Ready on InventoryChest");
			eventsTimer = this.GetNodeOrNull<Godot.Timer>("Timer-Events");
			if (eventsTimer != null)
			{
				eventsTimer.Connect("timeout", this, nameof(_on_EventsTimer_timeout));
				eventsTimer.Start();
			}
			//buttonGameIntroStart = this.GetNodeOrNull<TextureButton>("Button-Intro-Start");
			//Diagnostics.PrintNullValueMessage(buttonGameIntroStart, "buttonGameIntroStart");
			chestPanel = this.GetNodeOrNull<Godot.Panel>("Panel");
			if (chestPanel != null) {
				itemSlotsPanel = chestPanel.GetNodeOrNull<Godot.PanelContainer>("Panel-Item-Slots");
				inventoryActionsPanel = chestPanel.GetNodeOrNull<Godot.PanelContainer>("Panel-Inventory-Actions");
			}
		}

		public void SetInventoryItemResource(string inventoryItem, string resource) {
			inventorySpriteResourceMap.Add(inventoryItem, resource);
		}

		public string GetInventoryItemResource(string inventoryItem) {
			string res = "";
			return res;            
		}

		public Sprite FindSpriteForItem(string inventoryItem) {
			Sprite res = null;
			if (inventoryItem != null) {
				if (inventoryItemsDisplayMap.ContainsKey(inventoryItem))
					res = inventoryItemsDisplayMap[inventoryItem];
			}
			return res;
		}

		public void AddInventoryItem(string itemName, InventoryItem item) {
			inventoryStoreCollection.AddItem(itemName, item);
		}

		// slots with 0 - index
		private void PlaceItem(string itemName, int slot) {
			if (slot >= 0) {
				if (inventorySlotIndexMap.ContainsKey(itemName)) {

				}
				inventorySlotIndexMap[itemName] = slot;
			}
		}

		private void BuildItemSlots() {
			
		}

		public void _on_EventsTimer_timeout()
		{
			eventsTimerCounter += 1;
		}
	}
}
