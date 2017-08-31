using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{

#region private_members

    private Player player;
    private Vector3 initialPosition;

#endregion


#region public_members

    public void followPlayer()
    {
        transform.position = new Vector3(initialPosition.x + player.transform.position.x,
                                 initialPosition.y,
                                 initialPosition.z + player.transform.position.z);
    }

#endregion

#region unity_defined_methods

    void Start ()
    {
        var playerObj = GameObject.FindWithTag("Player");

        if (playerObj != null)
            player = playerObj.GetComponent<Player>();

        initialPosition = transform.position;

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

            if (player.isMoving)
                followPlayer();

            //TODO: ...
        }
	}

#endregion

}
