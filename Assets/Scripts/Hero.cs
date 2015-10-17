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
	public bool EnableDoubleJump;
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
	private float StartScale;
	private float StartWidth;
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
		this.scalarAccelerationModifier = 0.1f;
		this.HeroController = this.GetComponent<HeroController>();
		this.GetComponentInChildren<SpriteRenderer>().sprite = this.BodySprites[this.HeroController.PlayerNumber];
		this.ProjectileSprite = this.ProjectileSprites[this.HeroController.PlayerNumber];
		this.ProjectileExplosionSprite = this.ProjectileExplosions[this.HeroController.PlayerNumber];
		this.StartScale = this.scale;
		this.StartWidth = this.GetComponent<Collider2D>().bounds.size.x;
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

	void OnGUI()
	{
		this.DrawHUD(this.HUDPosition);
	}

	void SetDoubleJumpAllowed()
	{
		if (this.EnableDoubleJump)
		{
			this.CanDoubleJump = true;
		}
	}

	void DrawHUD(Vector2 position)
	{
		float iconSizeWidth = 50;
		float heartSizeWidth = 35;

		float xPosition = position.x;

		Texture badge = (Texture)Resources.Load(string.Format("p{0}_badge", this.PlayerIndex), typeof(Texture));
		GUI.DrawTexture(new Rect(xPosition / 1920.0f * Screen.width, (position.y - iconSizeWidth * 0.5f) / 1080.0f * Screen.height, iconSizeWidth / 1920.0f * Screen.width, iconSizeWidth / 1920.0f * Screen.width), badge);
		xPosition += (iconSizeWidth * 1.5f);

		bool drawHearts = false;
		if (drawHearts)
		{
			Texture heart = (Texture)Resources.Load("heart_full", typeof(Texture));
			GUI.DrawTexture(new Rect(xPosition / 1920.0f * Screen.width, (position.y - heartSizeWidth * 0.5f) / 1080.0f * Screen.height, heartSizeWidth / 1920.0f * Screen.width, heartSizeWidth / 1920.0f * Screen.width), heart);
			xPosition += (heartSizeWidth * 1.1f);

			GUI.DrawTexture(new Rect(xPosition / 1920.0f * Screen.width, (position.y - heartSizeWidth * 0.5f) / 1080.0f * Screen.height, heartSizeWidth / 1920.0f * Screen.width, heartSizeWidth / 1920.0f * Screen.width), heart);
			xPosition += (heartSizeWidth * 1.1f);

			GUI.DrawTexture(new Rect(xPosition / 1920.0f * Screen.width, (position.y - heartSizeWidth * 0.5f) / 1080.0f * Screen.height, heartSizeWidth / 1920.0f * Screen.width, heartSizeWidth / 1920.0f * Screen.width), heart);
			xPosition += (iconSizeWidth * 1.5f);
		}

		GUIStyle style = new GUIStyle("label");
		style.font = this.HUDText.font;
		style.fontSize = (int)(Screen.width * 0.027027f);
		style.alignment = TextAnchor.UpperLeft;

		string displayString = "Flawless!";
		if (this.RespawnTimeLeft > 0)
		{
			displayString = string.Format("Back in {0}s!", ((int)Math.Ceiling(this.RespawnTimeLeft)).ToString());
		}
		else if (this.NumDeaths == 1)
		{
 			displayString = string.Format("{0} Death", 1);
		}
		else if (this.NumDeaths > 0)
		{
			displayString = string.Format("{0} Deaths", this.NumDeaths);
		}

		this.DrawOutlineText(new Rect((position.x + iconSizeWidth * 1.25f) / 1920.0f * Screen.width, 0, Screen.width, Screen.height), displayString, style, Color.black, Color.white, 1);
	}

	void Update ()
	{
		if (this.RespawnTimeLeft > 0.0f)
		{
			this.transform.position = new Vector3(0.0f, -20.0f, 0.0f);

			this.RespawnTimeLeft -= Time.deltaTime;
			if (this.RespawnTimeLeft < 0.0)
			{
				this.Respawn ();
			}
		}

		if (this.HeroController.Shooting && this.TimeUntilNextProjectile < 0.0f)
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
		}

		if (this.HeroController.GetResetGame)
		{
			GameObject scoreKeeper = GameObject.Find("ScoreKeeper");
			scoreKeeper.GetComponent<ScoreKeeper>().ResetGame();
		}

		float newX = this.velocity.x + (this.HeroController.HorizontalMovementAxis * this.scalarAccelerationModifier);
		float newY = this.velocity.y + (this.HeroController.VerticalMovementAxis * this.scalarAccelerationModifier);
		Debug.Log(scalarAccelerationModifier);
		this.velocity = new Vector2 (newX, newY);
	}

	public float scalarAccelerationModifier; // MUST BE SET IN START
	public float StaticMargin = 0.4f;
	public float FallingMargin = 0.5f;
	public float MaxNewSpeed = 50.0f;

	private Rect box;
	private Vector2 velocity = Vector2.zero;
	private int groundMask;

	void FixedUpdate ()
	{
		var bounds = this.GetComponent<Collider2D>().bounds;
		this.box = Rect.MinMaxRect (bounds.min.x, bounds.min.y, bounds.max.x, bounds.max.y);

		bool hitSomething = false;
		RaycastHit2D raycastHit;
		Vector3 startPoint = new Vector3(this.box.xMin + this.StaticMargin, this.box.yMin + this.StaticMargin, this.transform.position.z);
		Vector3 endPoint   = new Vector3(this.box.xMax - this.StaticMargin, startPoint.y, startPoint.z);

        float distance = this.StaticMargin;
		int verticalRays = Mathf.Max (3, Mathf.CeilToInt ((endPoint.x - startPoint.x) / this.StartWidth));

		for (int i = 0; i < verticalRays; ++i)
		{
			Vector2 origin = Vector2.Lerp (startPoint, endPoint, (float)i / (float)(verticalRays - 1));

			for (int mask = 0; mask < 2; ++mask)
			{
				if (mask == 0)
				{
					int oldLayer = this.gameObject.layer;
					this.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
					raycastHit = Physics2D.Linecast(origin, origin - new Vector2(0.0f, distance), (1<< LayerMask.NameToLayer("Default")));
					this.gameObject.layer = oldLayer;
				}
				else
				{
					raycastHit = Physics2D.Linecast(origin, origin - new Vector2(0.0f, distance), (1 << this.groundMask));
				}
			}
		}

		if ((this.velocity.x > 0 && !this.FacingRight) || (this.velocity.x < 0 && this.FacingRight))
		{
			this.Flip();
		}

		this.TimeUntilNextProjectile -= Time.fixedDeltaTime;

		this.transform.Translate (this.velocity * Time.fixedDeltaTime);
    }


	void LateUpdate ()
	{
	}

	void Flip ()
	{
		this.FacingRight = !this.FacingRight;
		this.scale = this.scale;
	}

	public bool IsAlive()
	{
		return (this.RespawnTimeLeft <= 0.0f);
	}

	public void Hit (Hero attackingHero)
	{
		if (this == attackingHero || !this.IsAlive())
		{
			return;
		}

		GameObject projectileExplosion = (GameObject)GameObject.Instantiate(attackingHero.ProjectileExplosion, this.transform.position, Quaternion.identity);
		projectileExplosion.GetComponent<SpriteRenderer>().sprite = attackingHero.ProjectileExplosionSprite;
		projectileExplosion.transform.localScale *= attackingHero.scale / attackingHero.StartScale;

		if (this.GetComponent<ShieldBuff>().enabled)
		{
			this.GetComponent<ShieldBuff>().enabled = false;
		}
		else
		{
			this.Die(attackingHero);
		}
	}

	void Die(Hero attackingHero)
	{
		if (!this.IsAlive())
		{
			return;
		}

		AudioSourceExt.StopClipOnObject(this.MaxSizeSound);
		Destroy(this.MaxSizeSound);

		SoundFX.Instance.OnHeroDies(this);
		this.RespawnTimeLeft = this.RespawnTimeCalculated;
		this.RespawnTimeCalculated += this.RespawnTimeIncreasePerDeath;
		this.NumDeaths++;
	}

	public void Reset()
	{
		this.Die(null);
		this.Respawn();
		this.transform.localPosition = Vector3.zero;
		this.RespawnTimeCalculated = this.RespawnTime;
		this.NumDeaths = 0;
    }

	void Respawn()
	{
		this.transform.position = new Vector3(0,0,0);

		this.velocity = new Vector2(0.0f, 1.0f) * this.SpawnMagnitude;

		SoundFX.Instance.OnHeroRespawn(this);
		this.RespawnTimeLeft = -1.0f;
    }

	void OnTriggerEnter2D(Collider2D other)
	{
		this.gameObject.layer = LayerMask.NameToLayer ("IgnorePlatforms");
	}

	void OnTriggerExit2D(Collider2D other)
	{
		this.gameObject.layer = LayerMask.NameToLayer ("Default");
	}
}
