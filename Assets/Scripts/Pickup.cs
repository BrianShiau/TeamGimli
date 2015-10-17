using UnityEngine;
using System.Collections;
using Jolly;

public class Pickup : MonoBehaviour
{
	public enum Type
	{
		Shield,
		MassiveAccel,
		MassiveDecel,
		StickyPad,
		GrapplingHook,
		StopGun,
		AlterThreshold
	}

	public Type PickupType;

	public float MassiveAccelerationValue = 0.6f;
	public float MassiveDecelerationValue = 0.2f;

	public float ExpirationTime;
	public float StartBlinkTime;

	void Update ()
	{
		if (Time.time > this.ExpirationTime)
		{
			GameObject.Destroy (this.gameObject);
			return;
		}

		float dt = Time.time - this.StartBlinkTime;
		if (dt >= 0.0f)
		{
			this.GetComponent<SpriteRenderer>().enabled = (Mathf.FloorToInt (dt / 0.35f) % 2 == 0);
		}
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		var hero = other.gameObject.GetComponent<Hero>();
		if (hero == null)
			return;

		Debug.Log(this.PickupType.ToString());

		switch(this.PickupType)
		{
			case Type.Shield:
			{
				ShieldBuff.AddToHero (hero);	// TODO: Change the shieldbuff class
				break;
			}
			case Type.MassiveAccel:
			{
				hero.scalarAccelerationModifier = MassiveAccelerationValue;
				break;
			}
			case Type.MassiveDecel:
			{
				hero.scalarAccelerationModifier = MassiveDecelerationValue;
				break;
			} 
			case Type.StickyPad:
			{
				break;
			} 
			case Type.GrapplingHook:
			{
				break;
			} 
			case Type.StopGun:
			{
				break;
			} 
			case Type.AlterThreshold:
			{
				break;
			} 
		}

		GameObject.Destroy (this.gameObject);
	}
}
