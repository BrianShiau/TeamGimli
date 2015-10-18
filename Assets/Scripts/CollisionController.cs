using UnityEngine;
using System;
using System.Collections;

public class CollisionController : MonoBehaviour {
	private float radius;
	private float x,y;
	private float maxX, minX, maxY, minY;
	public float wallDampeningFactor = 0.5f;

	private Hero thisHero;
	private Hero otherHero;
	private String type;
	private Vector2 border;
  
	// Use this for initialization
	void Start () {
	  type = this.gameObject.tag;
	  if(type == "Pickup") {
	  	radius = 1.7f;
	  } else if(type == "Player") {
	    radius = 1.3f;
	  } else {
	  	border = new Vector2(0, 0);
	  }


	  maxX = 28.0f;
	  minX = -28.0f;
	  maxY = 12.5f;
	  minY = -12.5f;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		// See if out of bounds
		if(type == "Player") {
			this.thisHero = gameObject.GetComponent<Hero>();

			DetectEdge();

		    var objs = GameObject.FindObjectsOfType<CollisionController>() as CollisionController[];
		    
		    foreach (CollisionController cont in objs){
		      GameObject obj = cont.gameObject;
		      if(obj != this.gameObject)//if not this object
		      {
		        if(collision(obj)) {
	            // These objects collided
	                this.thisHero.Hit(obj);
		        }	        
		      }
		    }
		}
	       
	}
  	
 	bool collision(GameObject obj) {
        float startX = gameObject.transform.position.x;
        float startY = gameObject.transform.position.y;

        if(obj.tag == "Player" || obj.tag == "Pickup") {
	        float centerX = obj.transform.position.x;
	        float centerY = obj.transform.position.y;

	        CollisionController theirCol = obj.GetComponent<CollisionController>();
	    	float theirRad = theirCol.getRadius();

	        if(Math.Pow(centerX - startX, 2) + Math.Pow(centerY - startY, 2) <= Math.Pow(radius + theirRad , 2)) {
	        	return true;
	        }

	        return false;
        } else if (obj.tag == "Wall") {
        	Debug.Log("Bounce mother fucker!");
        	return false;
        }

        return false;
        
 	}

	void DetectEdge() {
		float thisX = gameObject.transform.position.x;
	    float thisY = gameObject.transform.position.y;
	    Vector2 thisVelocity = thisHero.velocity;

        // Do not dampen if buffer active
		if(thisX >= this.maxX || thisX <= this.minX)
		{
            
			thisX += (thisX >= this.maxX ? -1 : 1);
			Vector3 pos = new Vector3(thisX, thisY, 0);
			gameObject.transform.position = pos;
			this.thisHero.velocity = new Vector2(-thisVelocity.x, thisVelocity.y);
            if (!thisHero.GetComponent<ShieldBuff>().enabled)
            {
                this.thisHero.accelerateByScalar(wallDampeningFactor);
            }
		}

		if(thisY >= this.maxY || thisY <= this.minY)
		{
			thisY += (thisY >= this.maxY ? -1 : 1);
			Vector3 pos = new Vector3(thisX, thisY, 0);
			gameObject.transform.position = pos;
			this.thisHero.velocity = new Vector2(thisVelocity.x, -thisVelocity.y);
            if (!thisHero.GetComponent<ShieldBuff>().enabled)
            {
                this.thisHero.accelerateByScalar(wallDampeningFactor);
            }
		}
	}

	void OnGUI() {
		bool debug = true;

		if(debug){
			if(this.type == "Player") {
				Vector3 pos = gameObject.transform.position;
				Vector2 velocity = this.thisHero.velocity;
				Drawing.DrawLine (Camera.main, pos, new Vector3(pos.x + velocity.x, pos.y + velocity.y, 0), Color.red, 2f);
				Drawing.DrawCircle(Camera.main, pos, this.radius, Color.red, 2f, Vector2.one);
			} else if(this.type == "Pickup") {
				Vector3 pos = gameObject.transform.position;
				Drawing.DrawCircle(Camera.main, pos, this.radius, Color.red, 2f, Vector2.one);
			}
		}
	}

	public float getRadius() {
		return this.radius;
	}

}
