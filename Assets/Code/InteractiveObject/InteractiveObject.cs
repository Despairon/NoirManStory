using System;
using UnityEngine;

public partial class InteractiveObject : MonoBehaviour, IEventReceiver
{
	#region private_members

	private bool isPlayerInRange;

	#endregion

	#region public_members

	#endregion

	#region unity_members

	public void Start()
	{
		isPlayerInRange = false;
	}

	public void OnTriggerEnter(Collider trigger)
	{
		if (trigger.CompareTag("Player") == true) 
		{
			isPlayerInRange = true;
		}
	}

	public void OnTriggerExit(Collider trigger)
	{
		if (trigger.CompareTag("Player") == true) 
		{
			isPlayerInRange = false;
		}
	}

	#endregion


}

