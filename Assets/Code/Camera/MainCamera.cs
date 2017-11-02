using UnityEngine;

public partial class MainCamera : MonoBehaviour, IEventReceiver
{
    #region private_members

    private Player  player;
    private Vector3 offsetFromPlayer;

    private void followPlayer()
    {
        transform.position = new Vector3(player.transform.position.x + offsetFromPlayer.x,
                                         offsetFromPlayer.y,
                                         player.transform.position.z + offsetFromPlayer.z);
    }
    
    #endregion

    #region public_members

    #endregion

    #region unity_defined_methods

    void Start ()
    {
        var playerObj = GameObject.FindWithTag("Player");

        if (playerObj != null)
            player = playerObj.GetComponent<Player>();

        offsetFromPlayer = transform.position - player.transform.position;

        if (player != null) followPlayer();
    }

	void Update ()
    {
		if (player != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;

                var ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit, float.PositiveInfinity, LayerMask.GetMask("InteractableObject")))
                {
                    var evtData = new InteractableObjectClickData(hit.collider.gameObject, hit.point);
                    EventsManager.instance.sendEventToObject(player.name, EventID.INTERACTABLE_OBJECT_CLICKED, evtData);
                }             
            }

            followPlayer();
        }
	}

    #endregion

}
