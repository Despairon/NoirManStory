using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

#region private_members

    private bool _isMoving = false;

    private delegate void PlayerInteraction(Player player, GameObject obj, Vector3 interactionPoint);

    private sealed class InteractionTableItem
    {
        public InteractionTableItem(InteractableObjectType interactionType, PlayerInteraction interact)
        {
            this.interactionType = interactionType;
            this.interact        = interact;
        }

        public readonly InteractableObjectType interactionType;
        public readonly PlayerInteraction      interact;
    }

    private static List<InteractionTableItem> playerInteractionTable = new List<InteractionTableItem>()
    {
        new InteractionTableItem(InteractableObjectType.WALL,  new PlayerInteraction(lookAt)),
        new InteractionTableItem(InteractableObjectType.FLOOR, new PlayerInteraction(lookAt)),
        new InteractionTableItem(InteractableObjectType.FLOOR, new PlayerInteraction(moveTo))
        // add player interactions here!
    };

#endregion

#region public members

    public static void moveTo(Player player, GameObject obj, Vector3 interactionPoint)
    {
        // TODO: implement
        lookAt(player, obj, interactionPoint);
    }

    public static void lookAt(Player player, GameObject obj, Vector3 interactionPoint)
    {
        // TODO: implement
        var playerToObject = interactionPoint - player.transform.position;
        playerToObject.y = 0f;

        Quaternion newRotation = Quaternion.LookRotation(playerToObject);
        player.GetComponent<Rigidbody>().rotation = newRotation;
    }

    public bool isMoving
    {
        get         { return _isMoving;  }
        private set { _isMoving = value; }
    }

    public void onInteractableObjectClick(GameObject obj, Vector3 interactionPoint)
    {
        if (InteractableObjectsManager.isObjectInteractable(obj))
        {
            var interactionType = InteractableObjectsManager.getInteractionType(obj);

            foreach (var interactionTableItem in playerInteractionTable.FindAll(interaction => interaction.interactionType == interactionType))
                if (interactionTableItem.interact != null)
                    interactionTableItem.interact(this, obj, interactionPoint);
        }
    }

#endregion

#region unity_defined_methods

    void Start ()
    {
		
	}
	
	void Update ()
    {

    }

    private void FixedUpdate()
    {

    }

    #endregion

}
