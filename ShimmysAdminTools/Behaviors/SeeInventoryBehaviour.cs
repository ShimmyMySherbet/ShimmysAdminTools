using SDG.Unturned;
using UnityEngine;

namespace ShimmysAdminTools.Behaviors
{
    public class SeeInventoryBehaviour : MonoBehaviour
    {
        public bool awake = false;

        public Player Player => GetComponent<Player>();
        public PlayerInventory Inventory => Player.inventory;

        public Player OpenedPlayer;
        public Items OpenedItems;
        public PlayerInventory OpenedInentory => OpenedPlayer.inventory;

        public void Awake()
        {
            awake = true;
            if (Player == null) Destroy(this);
        }

        public void SendOpenInventory(Player player, byte page)
        {
            OpenedPlayer = player;
            OpenedItems = player.inventory.items[page];

            Inventory.updateItems(7, OpenedItems);
            Inventory.sendStorage();

            OpenedItems.onItemAdded += onItemAdded;
            OpenedItems.onItemRemoved += onItemRemoved;

            OpenedItems.onItemUpdated += onItemUpdated;

            player.animator.onInventoryGesture += InventoryGestureUpdate;
        }

        public void InventoryGestureUpdate(bool inInv)
        {
            if (!inInv)
            {
                Destroy(this);
            }
        }

        public void onItemAdded(byte page, byte index, ItemJar jar)
        {
            if (!isActiveAndEnabled || !awake) return;
            Inventory.sendUpdateInvState(page, jar.x, jar.y, jar.item.state);
            OpenedInentory.sendUpdateInvState(page, jar.x, jar.y, jar.item.state);
        }

        public void onItemUpdated(byte page, byte index, ItemJar jar)
        {
            if (!isActiveAndEnabled || !awake) return;
            Inventory.sendUpdateInvState(page, jar.x, jar.y, jar.item.state);
            OpenedInentory.sendUpdateInvState(page, jar.x, jar.y, jar.item.state);
        }

        public void onItemRemoved(byte page, byte index, ItemJar jar)
        {
            if (!isActiveAndEnabled || !awake) return;
            Inventory.sendDropItem(page, jar.x, jar.y);
            OpenedInentory.sendDropItem(page, jar.x, jar.y);
        }

        public void OnDestroy()
        {
            OpenedItems.onItemAdded -= onItemAdded;
            OpenedItems.onItemRemoved -= onItemRemoved;
            OpenedItems.onItemUpdated -= onItemUpdated;
            awake = false;
        }
    }
}