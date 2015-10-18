using UnityEngine;
using System;
using System.Collections;
using Jolly;

public class Hero : MonoBehaviour
{
	public float ScaleAdjustment;
	public int ScaleIterations;
	public Vector2 HUDPosition
	{
		get
		{
			switch (this.PlayerIndex)
			{
			case 1: return new Vector2 (15, 35);
			case 2: return new Vector2 (495, 35);
			case 3: return new Vector2 (975, 35);
			case 4: return new Vector2 (1455, 35);
			}
			return Vector2.zero;
		}
	}
	public float SpawnMagnitude;
	public GameObject GroundDetector;
	public GameObject ProjectileEmitLocator;
	public GameObject ChannelLocator;
	public GameObject CounterLocator;
	public GameObject Projectile;
	public GameObject ProjectileExplosion;
	public GameObject StunVisual;
	public GameObject ChannelVisual;
	public GameObject MaxGrowthVisual;
	public GameObject body;
	public bool EnableDoubleJump;
    public bool AboveThreshold = false;
    private float Threshold = 300.0f;
	public float ChannelTime;
	public float RespawnTime;
	public float RespawnTimeIncreasePerDeath;
	public float StunTime;
	public float JumpForgivenessTimeAmount;
	public int PlayerIndex
	{
		get
		{
			return 1+this.HeroController.PlayerNumber;
		}
	}
	public GUIText HUDText;
	public float TimeAtMaxSize;

    // Types of powerups
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
	public bool hasPowerup = false;
    public string powerupName = "StopGun";

	private HeroController HeroController;

	public float ProjectileLaunchVelocity;
	public float ProjectileDelay;
	private float TimeUntilNextProjectile = 0.0f;

	private bool FacingRight = true;

	private float RespawnTimeCalculated = 0.0f;
	private float RespawnTimeLeft = 0.0f;
	private float TimeLeftStunned = 0.0f;
	private float TimeSpentChanneling = 0.0f;
	private bool IsChanneling = false;
	private GameObject ChannelVisualInstance;
	private GameObject MaxVisualInstance;
	private GameObject StunVisualInstance;
	private bool CanDoubleJump;
	private bool GroundedLastFrame;
	private float JumpForgivenessTimeLeft;
	private GameObject MaxSizeSound;
	private int NumDeaths;

	public Sprite[] BodySprites;
	public Sprite[] ProjectileSprites;
	public Sprite[] ProjectileExplosions;
	public Sprite ProjectileSprite;
	public Sprite ProjectileExplosionSprite;

	void Start ()
	{
		this.scalarAccelerationModifier = defaultScalarAccelerationModifier;
		this.HeroController = this.GetComponent<HeroController>();
		this.GetComponentInChildren<SpriteRenderer>().sprite = this.BodySprites[this.HeroController.PlayerNumber];
		this.ProjectileSprite = this.ProjectileSprites[this.HeroController.PlayerNumber];
		this.ProjectileExplosionSprite = this.ProjectileExplosions[this.HeroController.PlayerNumber];
		this.RespawnTimeCalculated = this.RespawnTime;

		this.groundMask = LayerMask.NameToLayer ("Ground");
	}

	private float scale
	{
		set
		{
			float minYOld = this.GetComponent<Collider2D>().bounds.min.y;
			this.transform.localScale = new Vector3((this.FacingRight ? 1.0f : -1.0f) * value, value, 1.0f);
			float minYNew = this.GetComponent<Collider2D>().bounds.min.y;
			Vector3 v = this.transform.position;
			this.transform.position = new Vector3(v.x, v.y + minYOld - minYNew, v.z);
		}
		get
		{
			return this.transform.localScale.y;
		}
	}

	void SetDoubleJumpAllowed()
	{
		if (this.EnableDoubleJump)
		{
			this.CanDoubleJump = true;
		}
	}

	void Update ()
	{
        if (this.HeroController.Shooting && /*this.hasPowerup &&*/
                this.powerupName == "StopGun")
        {
			this.TimeUntilNextProjectile = this.ProjectileDelay;
			GameObject projectile = (GameObject)GameObject.Instantiate(this.Projectile, this.ProjectileEmitLocator.transform.position, Quaternion.identity);
			projectile.GetComponent<SpriteRenderer>().sprite = this.ProjectileSprite;
			projectile.GetComponent<Projectile>().OwnerHero = this;
			projectile.transform.localScale = this.transform.localScale;
			float launchVelocity = (this.FacingRight ? 1.0f : -1.0f) * this.ProjectileLaunchVelocity;
			projectile.GetComponent<Projectile>().Velocity = new Vector2(launchVelocity, 0.0f);
			SoundFX.Instance.OnHeroFire(this);
			Physics2D.IgnoreCollision(projectile.GetComponent<Collider2D>(), this.GetComponent<Collider2D>());
            // Debug.Log("Shooting Stop Gun");
        }

        else if (this.HeroController.Shooting)
        {
			this.TimeUntilNextProjectile = this.ProjectileDelay;
			GameObject projectile = (GameObject)GameObject.Instantiate(this.Projectile, this.ProjectileEmitLocator.transform.position, Quaternion.identity);
			float launchVelocity = (this.FacingRight ? 1.0f : -1.0f) * this.ProjectileLaunchVelocity;
			projectile.GetComponent<Projectile>().Velocity = this.velocity * this.ProjectileLaunchVelocity; 
			SoundFX.Instance.OnHeroFire(this);
			Physics2D.IgnoreCollision(projectile.GetComponent<Collider2D>(), this.GetComponent<Collider2D>());

        }

        else if (this.HeroController.Jump)
        {
            // Add "force field" to Hero for a temporary amount of time to not be slowed by hitting wall
        }

		if (this.HeroController.GetResetGame)
		{
			GameObject scoreKeeper = GameObject.Find("ScoreKeeper");
			scoreKeeper.GetComponent<ScoreKeeper>().ResetGame();
		}

		// check powerups
		if (this.hasPowerup)
		{
			if (this.TimeTillNotAccelerated != 0)
			{
				if (Time.time > this.TimeTillNotAccelerated)
				{
					this.scalarAccelerationModifier = defaultScalarAccelerationModifier;
					this.TimeTillNotAccelerated = 0;
					this.hasPowerup = false;
				}
			}
			// if(!this.hasPowerup)
			// 	// Debug.Log(this.name + "'s powerup ended");
		}

		float newX = this.velocity.x + (this.HeroController.HorizontalMovementAxis * this.scalarAccelerationModifier);
		float newY = this.velocity.y + (this.HeroController.VerticalMovementAxis * this.scalarAccelerationModifier);

		float newRotation = 0;
		if (newY != 0) {
			if(newY>0 && newX>0){
				if(Math.Abs(newY)>Math.Abs(newX))
					newRotation = 180 / (float)Math.PI * (float)Math.Sin ((float)newX / (float)newY);
				else
					newRotation = 90 - 180 / (float)Math.PI * (float)Math.Sin ((float)newY / (float)newX);
			}
			if(newY<0 && newX>0){
				if(Math.Abs(newY)<Math.Abs(newX))
					newRotation = 90 - 180 / (float)Math.PI * (float)Math.Sin ((float)newY / (float)newX);
				else
					newRotation = 180 + 180 / (float)Math.PI * (float)Math.Sin ((float)newX / (float)newY);
			}
			if(newY<0 && newX<0){
				if(Math.Abs(newY)>Math.Abs(newX))
					newRotation = 180 + 180 / (float)Math.PI * (float)Math.Sin ((float)newX / (float)newY);
				else
					newRotation = 270 - 180 / (float)Math.PI * (float)Math.Sin ((float)newY / (float)newX);
			}
			if(newY>0 && newX<0){
				if(Math.Abs(newY)<Math.Abs(newX))
					newRotation = 270 - 180 / (float)Math.PI * (float)Math.Sin ((float)newY / (float)newX);
				else
					newRotation = 360 + 180 / (float)Math.PI * (float)Math.Sin ((float)newX / (float)newY);
			}
			if(newX==0){
				if(newY>0)
					newRotation = 0;
				else
					newRotation = 180;
			}
		}
		else{
			newRotation = newX > 0 ? 90 : 270;
			if (newX==0)
				newRotation = 0;
		}
		body.transform.localRotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, -newRotation));
		
		this.velocity = new Vector2 (newX, newY);

        // Sets threshold to true if at a velocity that kills another player
        this.AboveThreshold = this.velocity.magnitude >= this.Threshold;
		if (this.AboveThreshold) {
			body.gameObject.GetComponent<Renderer> ().material.color = Color.yellow;
			body.GetComponent<TrailRenderer>().enabled = true;
		} else {
			body.gameObject.GetComponent<Renderer> ().material.color = Color.white;
			body.GetComponent<TrailRenderer>().enabled = false;
		}
	}

	void accelerateByVector(float x_component, float y_component)
	{
		this.velocity = new Vector2 (this.velocity.x + x_component, this.velocity.y + y_component);
	}

	public void accelerateByScalar(float scalar)
	{
		this.velocity = new Vector2 (this.velocity.x * scalar, this.velocity.y * scalar);
	}

	public float defaultScalarAccelerationModifier = 0.4f;
	public float scalarAccelerationModifier; // will be set in start, changing in gui will do nothing
	public float StaticMargin = 0.4f;
	public float FallingMargin = 0.5f;
	public float MaxNewSpeed = 50.0f;

	public float TimeTillNotAccelerated = 0;

	private Rect box;
	public Vector2 velocity = Vector2.zero;
	private int groundMask;

	void FixedUpdate ()
	{

		this.TimeUntilNextProjectile -= Time.fixedDeltaTime;

		this.transform.Translate (this.velocity * Time.fixedDeltaTime);
    }

	void LateUpdate ()
	{
	}

	public void Hit (Hero attackingHero)
	{
		if (this == attackingHero)
		{
			return;
		}

        // This hero is fast enough to kill the hero that ran into it
        if (this.AboveThreshold && !attackingHero.AboveThreshold)
        {
            GameObject projectileExplosion = (GameObject)GameObject.Instantiate(attackingHero.ProjectileExplosion, this.transform.position, Quaternion.identity);
            projectileExplosion.GetComponent<SpriteRenderer>().sprite = attackingHero.ProjectileExplosionSprite;
            this.velocity -= attackingHero.velocity;
            attackingHero.Die(null);
        }

        // Hero that rammed this hero is fast enough to kill
        else if (!this.AboveThreshold && attackingHero.AboveThreshold)
        {
            GameObject projectileExplosion = (GameObject)GameObject.Instantiate(attackingHero.ProjectileExplosion, this.transform.position, Quaternion.identity);
            projectileExplosion.GetComponent<SpriteRenderer>().sprite = attackingHero.ProjectileExplosionSprite;
            attackingHero.velocity -= this.velocity;
            this.Die(null);
        }

        else if (this.AboveThreshold && attackingHero.AboveThreshold)
        {
            if (this.velocity.magnitude > attackingHero.velocity.magnitude)
            {
                this.velocity -= attackingHero.velocity;
                attackingHero.Die(null);
            }
            else
            {
                attackingHero.velocity -= this.velocity;
                this.Die(null);
            }
        }
        // Neither of the colliding heroes are fast enough to kill, so they reverse their direction
        else
        {
            // this.accelerateByScalar(-1.0f);
            // attackingHero.accelerateByScalar(-1.0f);
            Vector2 ourVelo = this.velocity;
            Vector2 theirVelo = attackingHero.velocity;

            Vector2 ourNewVelo = new Vector2(theirVelo.x - ourVelo.x, theirVelo.y - ourVelo.y);
            Vector2 theirNewVelo = ourNewVelo * -1f;

            this.velocity = ourNewVelo;
            attackingHero.velocity = theirNewVelo;


        	Vector2 ourPos = gameObject.transform.position;
        	Vector2 theirPos = attackingHero.gameObject.transform.position;
        	CollisionController ourCol = gameObject.GetComponent<CollisionController>();
        	CollisionController theirCol = attackingHero.gameObject.GetComponent<CollisionController>();
        	float ourRad = ourCol.getRadius();
        	float theirRad = theirCol.getRadius();

        	float distance = (float) Math.Sqrt(Math.Pow(ourPos.x - theirPos.x, 2) + Math.Pow(ourPos.y - theirPos.y, 2));
        	float minDistance = 1.5f * (ourRad + theirRad);

        	if(distance < minDistance) {
        		this.transform.Translate (this.velocity * Time.fixedDeltaTime);
        		attackingHero.transform.Translate (attackingHero.velocity * Time.fixedDeltaTime);
        	}
        }
	}

    public void SetPowerup (Pickup pickup)
    {
        // Debug.Log(pickup.PickupType.ToString());
        this.powerupName = pickup.PickupType.ToString();
    }


    public void Shot (Hero attackingHero)
    {
        // Logic here to kill this.Hero via projectile
    }


	void Die(Hero attackingHero)
	{
		AudioSourceExt.StopClipOnObject(this.MaxSizeSound);
		Destroy(this.MaxSizeSound);

		SoundFX.Instance.OnHeroDies(this);

		Destroy(gameObject);
	}
}
