using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

public class EmoMentalCommand : MonoBehaviour
{
    //----------------------------------------
    EmoEngine engine = EmoEngine.Instance;
    public static EdkDll.IEE_MentalCommandAction_t[] MentalCommandActionList = {EdkDll.IEE_MentalCommandAction_t.MC_NEUTRAL,
                                                                    EdkDll.IEE_MentalCommandAction_t.MC_PUSH,
                                                                    EdkDll.IEE_MentalCommandAction_t.MC_PULL,
                                                                    EdkDll.IEE_MentalCommandAction_t.MC_LIFT,
                                                                    EdkDll.IEE_MentalCommandAction_t.MC_DROP,
                                                                    EdkDll.IEE_MentalCommandAction_t.MC_LEFT,
                                                                    EdkDll.IEE_MentalCommandAction_t.MC_RIGHT,
                                                                    EdkDll.IEE_MentalCommandAction_t.MC_ROTATE_LEFT,
                                                                    EdkDll.IEE_MentalCommandAction_t.MC_ROTATE_RIGHT,
                                                                    EdkDll.IEE_MentalCommandAction_t.MC_ROTATE_CLOCKWISE,
                                                                    EdkDll.IEE_MentalCommandAction_t.MC_ROTATE_COUNTER_CLOCKWISE,
                                                                    EdkDll.IEE_MentalCommandAction_t.MC_ROTATE_FORWARDS,
                                                                    EdkDll.IEE_MentalCommandAction_t.MC_ROTATE_REVERSE,
                                                                    EdkDll.IEE_MentalCommandAction_t.MC_DISAPPEAR
                                                                    };
    public static Boolean[] MentalCommandActionsEnabled = new Boolean[MentalCommandActionList.Length];
    public static float[] MentalCommandActionPower = new float[MentalCommandActionList.Length];
    public static bool IsStarted = false;
    //----------------------------------------  
    void Start()
    {
        if (!IsStarted)
        {
            MentalCommandActionsEnabled[0] = true;
            for (int i = 1; i < MentalCommandActionList.Length; i++)
            {
                MentalCommandActionsEnabled[i] = false;
            }
            engine.MentalCommandEmoStateUpdated +=
                new EmoEngine.MentalCommandEmoStateUpdatedEventHandler(engine_MentalCommandEmoStateUpdated);
            engine.MentalCommandTrainingStarted +=
                new EmoEngine.MentalCommandTrainingStartedEventEventHandler(engine_MentalCommandTrainingStarted);
            engine.MentalCommandTrainingSucceeded +=
                new EmoEngine.MentalCommandTrainingSucceededEventHandler(engine_MentalCommandTrainingSucceeded);
            engine.MentalCommandTrainingCompleted +=
                new EmoEngine.MentalCommandTrainingCompletedEventHandler(engine_MentalCommandTrainingCompleted);
            engine.MentalCommandTrainingRejected +=
                new EmoEngine.MentalCommandTrainingRejectedEventHandler(engine_MentalCommandTrainingRejected);
            IsStarted = true;
        }
    }
    float timeVariable;
    void Update()
    {
        //if in 2s , there's no updata of MentalCommandStatei
        //set the state down to 0
        timeVariable += Time.deltaTime;
        if (timeVariable < 0.3)
        {
            isNotResponding = true;
        }

        if (timeVariable > 1.0f)
        {
            timeVariable = 0.0f;
            if (isNotResponding)
            {
                //call smooth state
                ResetMentalCommandPower(2);
                ResetMentalCommandPower(3);
            }
        }
    }

    public static bool isNotResponding = true;
    static void engine_MentalCommandEmoStateUpdated(object sender, EmoStateUpdatedEventArgs e)
    {
        isNotResponding = false;
        EmoState es = e.emoState;
        EdkDll.IEE_MentalCommandAction_t cogAction = es.MentalCommandGetCurrentAction();
		
        float power = (float)es.MentalCommandGetCurrentActionPower();
        
        for (int i = 1; i < MentalCommandActionList.Length; i++)
        {
            if (cogAction == MentalCommandActionList[i])
            {
                MentalCommandActionPower[i] = power;
            }
            else MentalCommandActionPower[i] = 0;
        }
    }
    static void engine_MentalCommandTrainingStarted(object sender, EmoEngineEventArgs e)
    {
        Debug.Log("MentalCommand Training Started");
    }

    static void engine_MentalCommandTrainingSucceeded(object sender, EmoEngineEventArgs e)
    {
        EmoEngine.Instance.MentalCommandSetTrainingControl(0, EdkDll.IEE_MentalCommandTrainingControl_t.MC_ACCEPT);
        Debug.Log("MentalCommand Training Succeeded");
    }

    static void engine_MentalCommandTrainingCompleted(object sender, EmoEngineEventArgs e)
    {
        Debug.Log("MentalCommand Training Completed");
    }

    static void engine_MentalCommandTrainingRejected(object sender, EmoEngineEventArgs e)
    {
        Debug.Log("MentalCommand Training Rejected");
    }
    /// <summary>
    /// Start traning MentalCommand action
    /// </summary>
    /// <param name="MentalCommandAction">MentalCommand Action</param>
    public static void StartTrainingMentalCommand(EdkDll.IEE_MentalCommandAction_t MentalCommandAction)
    {
        if (MentalCommandAction == EdkDll.IEE_MentalCommandAction_t.MC_NEUTRAL)
        {
            EmoEngine.Instance.MentalCommandSetTrainingAction((uint)EmoUserManagement.currentUser, MentalCommandAction);
            EmoEngine.Instance.MentalCommandSetTrainingControl((uint)EmoUserManagement.currentUser, EdkDll.IEE_MentalCommandTrainingControl_t.MC_START);
        }
        else
            for (int i = 1; i < MentalCommandActionList.Length; i++)
            {
                if (MentalCommandAction == MentalCommandActionList[i])
                {
                    Debug.Log("Action compare");
                    if (MentalCommandActionsEnabled[i])
                    {
                        Debug.Log("Action is enabled");
                        EmoEngine.Instance.MentalCommandSetTrainingAction((uint)EmoUserManagement.currentUser, MentalCommandAction);
                        EmoEngine.Instance.MentalCommandSetTrainingControl((uint)EmoUserManagement.currentUser, EdkDll.IEE_MentalCommandTrainingControl_t.MC_START);
                    }
                    else Debug.Log("Action is not enabled");
                }
            }

    }
    /// <summary>
    /// Enable MentalCommand action in arraylist
    /// </summary>
    /// <param name="MentalCommandAction">MentalCommand Action</param>
    /// <param name="iBool">True = Enable/False = Disable</param>
    public static void EnableMentalCommandAction(EdkDll.IEE_MentalCommandAction_t MentalCommandAction, Boolean iBool)
    {
        for (int i = 1; i < MentalCommandActionList.Length; i++)
        {
            if (MentalCommandAction == MentalCommandActionList[i])
            {
                MentalCommandActionsEnabled[i] = iBool;
                Debug.Log("MentalCommandEnabledList has changed");
            }
        }
    }
   
    public static void EnableMentalCommandActionsList()
    {
        Debug.Log("Set MentalCommandList Enable");
        uint MentalCommandActions = 0x0000;
		
        for (int i = 1; i < MentalCommandActionList.Length; i++)
        {
            if (MentalCommandActionsEnabled[i])
            {
                MentalCommandActions = MentalCommandActions | ((uint)MentalCommandActionList[i]);
            }
        }

		if (MentalCommandActions != 0x0000) {
			EmoEngine.Instance.MentalCommandSetActiveActions ((uint)EmoUserManagement.currentUser, (uint)MentalCommandActions);
		}
    }
	
    /// <summary>
    /// Get MentalCommand action power in an array of float
    /// </summary>
    /// <returns></returns>
    public static float[] GetMentalCommandActionPower()
    {
        return MentalCommandActionPower;
    }
	
    public static void ResetAllMentalCommandPower()
    {
        for (int i = 0; i < MentalCommandActionList.Length; i++)
        {
            MentalCommandActionPower[i] = 0;
        }
    }
	
    public static void ResetMentalCommandList()
    {
        for (int i = 1; i < MentalCommandActionsEnabled.Length; i++)
        {
            MentalCommandActionsEnabled[i] = false;
        }
    }
	
    public static void ResetMentalCommandPower(int MentalCommandAction)
    {
        MentalCommandActionPower[MentalCommandAction] = 0;
    }
}