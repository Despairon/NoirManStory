using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

#region private_members

    private bool _isMoving = false;

    private delegate void PlayerInteraction(Player player, GameObject obj);

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

    private List<InteractionTableItem> playerInteractionTable = new List<InteractionTableItem>()
    {
        new InteractionTableItem(InteractableObjectType.FLOOR, new PlayerInteraction(lookAt)),
        new InteractionTableItem(InteractableObjectType.FLOOR, new PlayerInteraction(moveTo))
        // add player interactions here!
    };

#endregion

#region public members

    public static void moveTo(Player player, GameObject obj)
    {
        // TODO: implement
    }

    public static void lookAt(Player player, GameObject obj)
    {
        // TODO: implement

        /*Vector3 targetDir = obj.transform.position - player.transform.position;
        Vector3 newDir = Vector3.RotateTowards(player.transform.forward, obj.transform.position, Mathf.PI, 0.0F);
        Debug.DrawRay(player.transform.position, targetDir, Color.red);
        player.transform.rotation = Quaternion.LookRotation(newDir);*/
    }

    public bool isMoving
    {
        get { return _isMoving; }
        private set { _isMoving = value; }
    }


    public void interact(GameObject obj)
    {
        if (InteractableObjectsManager.isObjectInteractable(obj))
        {
            var interactionType      = InteractableObjectsManager.getInteractionType(obj);
            var interactionTableItem = playerInteractionTable.Find(interaction => interaction.interactionType == interactionType);

            if (interactionTableItem != null)
                interactionTableItem.interact(this, obj);
        }
    }

    public void onInteractableObjectClick(GameObject obj)
    {
        interact(obj);
    }

#endregion

#region unity_defined_methods

    void Start ()
    {
		
	}
	
	void Update ()
    {

    }

#endregion

}
