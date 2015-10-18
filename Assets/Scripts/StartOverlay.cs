using UnityEngine;
using System.Collections;

public class StartOverlay : MonoBehaviour {

    private GUIStyle guiStyle = new GUIStyle();
    private float x_displacement = (-1) * (Screen.width/3.5f);
    private float x_increment = Screen.width/4;
    private float y_displacement = (-1) * (Screen.height/2);
    private public int start_num = 3;
    public float time_per_num = 1.2f;
    private int cur_num;
    private float endTime;

	// Use this for initialization
	void Start () {
        cur_num = start_num;
        Time.timeScale = 0;
        guiStyle.fontSize = Screen.width/2;
        guiStyle.alignment = TextAnchor.UpperCenter;
        endTime = Time.realtimeSinceStartup + time_per_num;
	}
	
    void OnGUI()
    {
        if(cur_num == 0)
            GUI.Label(new Rect(Screen.width/2.2f, Screen.height/2 + y_displacement,100,100), "GO", guiStyle);
        else
            GUI.Label(new Rect(Screen.width/2 + x_displacement, Screen.height/2 + y_displacement,100,100), cur_num + "", guiStyle);
    }

	// Update is called once per frame
	void Update () {
        if(Time.realtimeSinceStartup > endTime)
        {
            endTime += time_per_num;
            cur_num--;
            x_displacement += x_increment;
        }
        if(cur_num == -1) {
            Time.timeScale = 1;
            Destroy(gameObject);
        }
	}
}