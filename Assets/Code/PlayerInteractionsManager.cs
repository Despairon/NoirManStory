using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerInteractionParams
{
    public readonly GameObject obj;
    public readonly Vector3    interactionPoint;

    public PlayerInteractionParams(GameObject obj, Vector3 interactionPoint)
    {
        this.obj              = obj;
        this.interactionPoint = interactionPoint;
    }
}

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

    public delegate void PlayerInteraction();

    public PlayerInteractionsManager(Player player)
    {
        this.player = player;

        playerInteractionTable = new List<InteractionTableItem>();
    }

    public void addInteraction(InteractableObjectType interactionType, PlayerInteraction interact)
    {
        playerInteractionTable.Add(new InteractionTableItem(interactionType, interact));
    }

    
     public void interactWith(PlayerInteractionParams interactionParams)
     {
        if (InteractableObjectsManager.isObjectInteractable(interactionParams.obj))
        {
            var interactionType = InteractableObjectsManager.getInteractionType(interactionParams.obj);

            foreach (var interactionTableItem in playerInteractionTable.FindAll(interaction => interaction.interactionType == interactionType))
                if (interactionTableItem.interact != null)
                    interactionTableItem.interact();
        }
    }
     

    #endregion
}
