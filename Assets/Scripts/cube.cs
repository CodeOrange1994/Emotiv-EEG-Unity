using UnityEngine;
using System.Collections;

public class cube : MonoBehaviour {

    public static int action = 0;
    public static float speed = 7.0f / 8.0f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if (action == 0)
        {
            if (transform.localPosition.z > 0)
            {
                transform.Translate(0, 0, -speed * Time.deltaTime * 10);
            }
            else
            {
                transform.localPosition = new Vector3(0, 0, 0);
            }
        }
        else
        {
            if (transform.localPosition.z < 7)
            {
                transform.Translate(0, 0, speed * Time.deltaTime);
            }
            else
            {
                transform.localPosition = new Vector3(0, 0, 7);
            }
        }
	}
}
