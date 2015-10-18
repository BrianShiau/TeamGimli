using UnityEngine;
using System.Collections;
using Jolly;

public class ShieldBuff : MonoBehaviour {

    public float BufferCooldown = 3.0f;
	public float BufferLifetime = 4.0f;
	public GameObject EffectRenderer;
	private GameObject effectInstance;

	void Start ()
	{
		this.enabled = false;

	}

    void Update ()
    {
        this.BufferLifetime -= Time.deltaTime;

        if (this.BufferLifetime < 0.0f)
        {
            Debug.Log("KILLING: "+this);
            /* this.OnDisable(); */
            this.enabled = false;
            /* Destroy (this); */
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
		var sb = hero.GetComponent <ShieldBuff> ();
		sb.enabled = true;
	}
}
