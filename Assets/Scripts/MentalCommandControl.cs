using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class MentalCommandControl : MonoBehaviour {

	// Use this for initialization
    EmoEngine engine;
    public Button btnNeutral, btnPush;
    public Slider slider;
    public Text status;
    uint userId;
    bool training = false;
    float trainingInterval = 0.0625f; //duration 8s
	void Start () {
        engine = EmoEngine.Instance;
        bindEvents();
        StartCoroutine(updateSlider());
	}

    IEnumerator updateSlider(){
        yield return new WaitForSeconds(0.5f);
        if (training)
        {
            slider.value += trainingInterval;
        }
        StartCoroutine(updateSlider());
    }
    void bindEvents(){
        engine.UserAdded += onUserAdded;
        engine.UserRemoved += onUserRemoved;
        engine.MentalCommandTrainingStarted += onTrainingStarted;
        engine.MentalCommandTrainingSucceeded += onTrainingSuccessed;
        engine.MentalCommandTrainingCompleted += onTrainingCompleted;
        engine.MentalCommandEmoStateUpdated += onMentalCommandEmoStateUpdated;
    }


    void onMentalCommandEmoStateUpdated(object sender, EmoStateUpdatedEventArgs args){
        if (training)
        {
            return;
        }
        EdkDll.IEE_MentalCommandAction_t action = args.emoState.MentalCommandGetCurrentAction();
        Debug.Log("current action: " + action.ToString());
        switch (action)
        {
            case EdkDll.IEE_MentalCommandAction_t.MC_NEUTRAL:
                cube.action = 0;
                status.text = "CurrentAction: Neutral";
                break;
            case EdkDll.IEE_MentalCommandAction_t.MC_PUSH:
                cube.action = 1;
                status.text = "CurrentAction: Pushing";
                break;
        }
    }
    private void onUserAdded(object sender, EmoEngineEventArgs args){
        userId = args.userId;
    }
    private void onUserRemoved(object sender, EmoEngineEventArgs args){
        
    }

    void onTrainingStarted(object sender, EmoEngineEventArgs args){
        status.text = "Training started";
        btnNeutral.gameObject.SetActive(false);
        btnPush.gameObject.SetActive(false);
        slider.value = 0;
        training = true;
    }

    void onTrainingSuccessed(object sender, EmoEngineEventArgs args){
        status.text = "Training successed";
        //should confirm user that they accept that training, in this case auto accept
        engine.MentalCommandSetTrainingControl(userId, EdkDll.IEE_MentalCommandTrainingControl_t.MC_ACCEPT);
        slider.value = 1;
    }

    void onTrainingCompleted(object sender, EmoEngineEventArgs args){
        status.text = "Training completed";
        btnNeutral.gameObject.SetActive(true);
        btnPush.gameObject.SetActive(true);
        slider.value = 0;
        training = false;
    }
	// Update is called once per frame
	void Update () {
	
	}

    public void startTrainingNeutral(){
        training = true;
        status.text = "Start training neutral";
        //no need to call MentalCommandSetActiveActions with neutral action
        engine.MentalCommandSetTrainingAction(userId, EdkDll.IEE_MentalCommandAction_t.MC_NEUTRAL);
        engine.MentalCommandSetTrainingControl(userId, EdkDll.IEE_MentalCommandTrainingControl_t.MC_START);
    }
    public void startTrainingPush(){
        training = true;
        status.text = "Start training push";
        cube.action = 1;
        engine.MentalCommandSetActiveActions(userId, (uint)EdkDll.IEE_MentalCommandAction_t.MC_PUSH);
        engine.MentalCommandSetTrainingAction(userId, EdkDll.IEE_MentalCommandAction_t.MC_PUSH);
        engine.MentalCommandSetTrainingControl(userId, EdkDll.IEE_MentalCommandTrainingControl_t.MC_START);
    }

}
