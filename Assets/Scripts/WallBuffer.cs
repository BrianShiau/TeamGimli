using UnityEngine;
using System.Collections;
using Jolly;

public class WallBuffer: MonoBehaviour {

    public float Duration = 1.0f;
	private float Lifetime = 1.0f;
	public GameObject EffectRenderer;
	private GameObject effectInstance;

	void Start ()
	{
		this.enabled = false;

	}

    void Update ()
    {
        this.Lifetime -= Time.deltaTime;

        if (this.Lifetime < 0.0f)
        {
            Debug.Log("KILLING: "+this);
            this.enabled = false;
            this.Lifetime = this.Duration;
        }
    }

	void OnEnable ()
	{
		this.EffectRenderer.GetComponent<SpriteRenderer>().enabled = true;
	}

	void OnDisable ()
	{
		this.EffectRenderer.GetComponent<SpriteRenderer>().enabled = false;
	}

	public static void AddToHero (Hero hero)
	{
		var sb = hero.GetComponent <WallBuffer> ();
		sb.enabled = true;
	}
}
