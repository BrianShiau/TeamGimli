using UnityEngine;
using System.Collections;
using Jolly;

public class DestroyAfter : MonoBehaviour
{
	public float Duration = 5.0f;

    void Start()
    {
        this.GetComponent<SpriteRenderer>().sortingLayerName = "Default";
    }

	void Update()
	{

	}
}
