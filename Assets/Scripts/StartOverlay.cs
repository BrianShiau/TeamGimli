using UnityEngine;
using System.Collections;

public class StartOverlay : MonoBehaviour {

    private static int RoundCount = 0;
    private GUIStyle guiStyle = new GUIStyle();
    private GUIStyle guiStyleRoundCount = new GUIStyle();
    private float x_displacement = (-1) * (Screen.width/8);
    private float x_increment = Screen.width/8;
    private float y_displacement = (Screen.height/6);
    private int start_num = 3;
    public float time_per_num = 1.2f;
    private int cur_num;
    private float endTime;

	// Use this for initialization
	void Start () {
        RoundCount++;
        cur_num = start_num;
        Time.timeScale = 0;
        guiStyleRoundCount.fontSize = Screen.width/5;
        guiStyleRoundCount.alignment = TextAnchor.UpperCenter;
        guiStyle.fontSize = Screen.width/3;
        guiStyle.alignment = TextAnchor.MiddleCenter;
        endTime = Time.realtimeSinceStartup + time_per_num;
	}
	
    void OnGUI() {
        GUI.Label(new Rect(0, 0, Screen.width,Screen.height), "Round " + RoundCount, guiStyleRoundCount);
   
       // GUI.Label(new Rect(Screen.width/2.2f, Screen.height/2 + y_displacement,100,100), "Round " + RoundCount, guiStyleRoundCount);
        if(cur_num == 0)
            GUI.Label(new Rect(0, y_displacement, Screen.width,Screen.height), "GO", guiStyle);
        else
            GUI.Label(new Rect(x_displacement, y_displacement, Screen.width + x_displacement,Screen.height), cur_num + "", guiStyle);
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