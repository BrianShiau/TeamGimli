using UnityEngine;
using System.Collections;

public class CollisionController : MonoBehaviour {
  public float radius=0.5f;
  public float x,y;
  public float distanceFactor=1f;
  public float maxX,maxY,minX,minY;
  
  private Hero thisHero;
  private Hero otherHero;
  
	// Use this for initialization
	void Start () {
    minY=-14.0f; 
    maxY=4.4f;
    
	}
	
	// Update is called once per frame
	void FixedUpdate () {
    GameObject[] objs = GameObject.FindObjectsOfType(typeof(CollisionController)) as GameObject[];
    thisHero = gameObject.GetComponent<Hero>();
    
    foreach (GameObject obj in objs){
      if(obj != gameObject)//if not this object
      {
        Vector2 thisVelocity = thisHero.velocity; //this collision controller
        float thisX = gameObject.transform.position.x;
        float thisY = gameObject.transform.position.y;
        
        otherHero = obj.GetComponent<Hero>(); //the other collision controller
        Vector2 otherVelocity = otherHero.velocity;
        float otherX = obj.transform.position.x;
        float otherY = obj.transform.position.y;
        
        Vector3 linePoint1 = new Vector3(thisX,thisY,0); //input for line line intersection
        Vector3 lineVec1 = thisVelocity;
        Vector3 linePoint2 = new Vector3(otherX,otherY,0);
        Vector3 lineVec2 = otherVelocity;
        Vector3 intersection;
        if(LineLineIntersection(out intersection,linePoint1,lineVec1,linePoint2,lineVec2)){ //collision detected
          //reverse directions
          thisHero.velocity = new Vector2(-thisVelocity.x,-thisVelocity.y);
        }
        //if within walls
        //detect collision
      }
    }
    
	}
  
  //Calculate the intersection point of two lines. Returns true if lines intersect, otherwise false.
	//Note that in 3d, two lines do not intersect most of the time. So if the two lines are not in the 
	//same plane, use ClosestPointsOnTwoLines() instead.
	public static bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2){
 
		intersection = Vector3.zero;
 
		Vector3 lineVec3 = linePoint2 - linePoint1;
		Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
		Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);
 
		float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);
 
		//Lines are not coplanar. Take into account rounding errors.
		if((planarFactor >= 0.00001f) || (planarFactor <= -0.00001f)){
 
			return false;
		}
 
		//Note: sqrMagnitude does x*x+y*y+z*z on the input vector.
		float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
 
		if((s >= 0.0f) && (s <= 1.0f)){
 
			intersection = linePoint1 + (lineVec1 * s);
			return true;
		}
 
		else{
			return false;       
		}
	}

}
