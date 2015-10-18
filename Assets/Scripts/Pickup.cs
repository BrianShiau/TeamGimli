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
		AlterThreshold,
		SpeedPad
	}

	public Type PickupType;
	public int secTillResetPowerup = 5; // in seconds
	public float MassiveAccelerationValue = 0.6f;
	public float MassiveDecelerationValue = 0.02f;
	public float AlteredThresholdModifier = 0.5f;
	public float ExpirationTime;
	public float StartBlinkTime;

	void Update ()
	{
		if (Time.time > this.ExpirationTime) {
			GameObject.Destroy (this.gameObject);
			return;
		}

		float dt = Time.time - this.StartBlinkTime;
		if (dt >= 0.0f) {
			this.GetComponent<SpriteRenderer> ().enabled = (Mathf.FloorToInt (dt / 0.35f) % 2 == 0);
		}
		if (this.PickupType == Type.SpeedPad) {
			if(this.transform.position.x > 1 && this.transform.position.y > 1)
				this.transform.localRotation = Quaternion.Euler (new Vector3 (0.0f, 0.0f, 225.0f));
			if(this.transform.position.x > 1 && this.transform.position.y <= 1)
				this.transform.localRotation = Quaternion.Euler (new Vector3 (0.0f, 0.0f, 135.0f));
			if(this.transform.position.x <= 1 && this.transform.position.y <= 1)
				this.transform.localRotation = Quaternion.Euler (new Vector3 (0.0f, 0.0f, 45.0f));
			if(this.transform.position.x <= 1 && this.transform.position.y > 1)
				this.transform.localRotation = Quaternion.Euler (new Vector3 (0.0f, 0.0f, 315.0f));
			
		}
	}

	public void HandleCollision (Hero hero)
	{
		if (hero == null)
			return;
		else if (hero.hasPowerup && this.PickupType!=Type.SpeedPad) {
			Debug.Log (hero.name + " could not pick up: " + this.PickupType.ToString ());
			return;
		} else {
			Debug.Log (hero.name + " picked up: " + this.PickupType.ToString ());
			hero.hasPowerup = true;
			hero.SetPowerup (this);
		}

		switch (this.PickupType) {
		case Type.Shield:
			{
				ShieldBuff.AddToHero (hero);	// TODO: Change the shieldbuff class
				break;
			}
		case Type.MassiveAccel:
			{
				hero.TimeTillNotPowered = Time.time + secTillResetPowerup;
				hero.scalarAccelerationModifier = MassiveAccelerationValue;
				break;
			}
		case Type.MassiveDecel:
			{
				hero.TimeTillNotPowered = Time.time + secTillResetPowerup;
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
				hero.TimeTillNotPowered = Time.time + secTillResetPowerup;
				hero.ThresholdModifier = this.AlteredThresholdModifier;
				break;
			} 
		case Type.SpeedPad:
			{
				if(this.transform.position.x > 1)
					hero.velocity.x -= 120;
				if(this.transform.position.y > 1)
					hero.velocity.y -= 80;
				hero.velocity.x += 40;
				hero.velocity.y += 40;
				this.ExpirationTime = 0.0f;
				break;
			}
		}

		GameObject.Destroy (this.gameObject);
	}
}
