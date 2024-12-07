using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;

namespace RobbieWagnerGames.TurnBasedCombat
{
    public static class Inventory
    {
        public static Dictionary<GameItem, int> inventory = new Dictionary<GameItem, int>();

        public static void AddItemsToInventory(List<GameItem> inv)
        {
            foreach (GameItem item in inv)
                TryAddItemToInventory(item);
        }

        public static bool TryAddItemToInventory(GameItem item)
        {
            if(inventory.TryGetValue(item, out int count))
            {
                if(count >= 99)
                    return false;
                inventory[item]++;
                return true;
            }
            inventory.Add(item, 1);
            return true;
        }

        public static bool TryRemoveItemFromInventory(GameItem item, int amount = 1)
        {
            if(inventory.TryGetValue(item, out int count))
            {
                if(count < amount)
                    return false;
                if (!item.reusable)
                {
                    inventory[item] -= amount;
                    if (count == amount)
                        inventory.Remove(item);
                }
                return true;
            }
            return false;
        }

        public static List<CombatItem> GetCombatItems()
        {
            List<CombatItem> result = new List<CombatItem>();

            foreach(GameItem item in inventory.Keys)
            {
                if (item.GetType() == typeof(CombatItem))
                    result.Add((CombatItem)item);
            }

            return result;
        }
    }
}