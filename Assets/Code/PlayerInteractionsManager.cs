using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractionsManager
{
#region private_members

    private sealed class InteractionTableItem
    {
        public InteractionTableItem(InteractableObjectType interactionType, PlayerInteraction interact)
        {
            this.interactionType = interactionType;
            this.interact = interact;
        }

        public readonly InteractableObjectType interactionType;
        public readonly PlayerInteraction interact;
    }

    private List<InteractionTableItem> playerInteractionTable;

#endregion

#region public_members

    public readonly Player player;

    public delegate void PlayerInteraction(GameObject obj, Vector3 interactionPoint);

    public PlayerInteractionsManager(Player player)
    {
        this.player = player;

        playerInteractionTable = new List<InteractionTableItem>();
    }

    public void addInteraction(InteractableObjectType interactionType, PlayerInteraction interact)
    {
        playerInteractionTable.Add(new InteractionTableItem(interactionType, interact));
    }

    
     public void interactWith(GameObject obj, Vector3 interactionPoint)
     {
        if (InteractableObjectsManager.isObjectInteractable(obj))
        {
            var interactionType = InteractableObjectsManager.getInteractionType(obj);

            foreach (var interactionTableItem in playerInteractionTable.FindAll(interaction => interaction.interactionType == interactionType))
                if (interactionTableItem.interact != null)
                    interactionTableItem.interact(obj, interactionPoint);
        }
    }
     

    #endregion
}
