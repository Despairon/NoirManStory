using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum InputAction
{
    NONE,
    SINGLE_TAP,
    DOUBLE_TAP
}

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
        public InteractionTableItem(InteractableObjectType interactionType, PlayerInteraction interact, InputAction inputAction)
        {
            this.interactionType = interactionType;
            this.interact        = interact;
            this.inputAction     = inputAction;
        }

        public readonly InteractableObjectType interactionType;
        public readonly PlayerInteraction      interact;
        public readonly InputAction            inputAction;
    }

    private List<InteractionTableItem> playerInteractionTable;

    #endregion

    #region public_members

    public readonly Player player;

    public delegate void PlayerInteraction(PlayerInteractionParams interactionParams);

    public PlayerInteractionsManager(Player player)
    {
        this.player = player;

        playerInteractionTable = new List<InteractionTableItem>();
    }

    public void addInteraction(InteractableObjectType interactionType, PlayerInteraction interact, InputAction inputAction)
    {
        playerInteractionTable.Add(new InteractionTableItem(interactionType, interact, inputAction));
    }

    
     public void interactWith(PlayerInteractionParams interactionParams, InputAction inputAction)
     {
        if (InteractableObjectsManager.isObjectInteractable(interactionParams.obj))
        {
            var interactionType = InteractableObjectsManager.getInteractionType(interactionParams.obj);

            var interactionTableItems = playerInteractionTable.FindAll(interaction => (interaction.interactionType == interactionType)
                                                                                   && ((interaction.inputAction    == inputAction)
                                                                                   || (interaction.inputAction     == InputAction.NONE)));

            foreach (var interactionTableItem in interactionTableItems)
                if (interactionTableItem.interact != null)
                    interactionTableItem.interact(interactionParams);
        }
    }
     

    #endregion
}
