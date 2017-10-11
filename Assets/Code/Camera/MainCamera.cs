using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
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
                    player.onInteractableObjectClick(hit.collider.gameObject, hit.point);             
            }

            if (player.state != Player.State.IDLE)
                followPlayer();
        }
	}

    #endregion

}
