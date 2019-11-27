using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour {
    EmoEngine engine;
    public float processInterval = 0.1f;
	bool locked = false;
	void Start () {
        engine = EmoEngine.Instance;
        engine.Connect();
        engine.UserAdded += onUserAdded;
        engine.UserRemoved += onUserRemoved;
        StartCoroutine(process());
	}

    private void onUserAdded(object sender, EmoEngineEventArgs args){
        Debug.Log("user added");
    }
    private void onUserRemoved(object sender, EmoEngineEventArgs args){
        Debug.Log("user removed");
		locked = false;
    }

    IEnumerator process(){
        yield return new WaitForSeconds(processInterval);
		connectDevice();
        engine.ProcessEvents();
        StartCoroutine(process());
    }

	void connectDevice(){
		if (locked){
			return;
		}
		int nDevices = EdkDll.Plugin_IEE_GetInsightDeviceCount();
		if (nDevices> 0){
			locked = true;
			EdkDll.Plugin_IEE_ConnectInsightDevice(0);
		}
	}
}
