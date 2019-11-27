using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

public class EmoFacialExpression : MonoBehaviour
{
    //----------------------------------------
    EmoEngine engine = EmoEngine.Instance;
    public static Boolean isBlink     = false;
    public static Boolean isLeftWink  = false;
    public static Boolean isRightWink = false;
    public static Boolean isEyesOpen  = false;
    public static Boolean isLookingUp = false;
    public static Boolean isLookingDown = false;
    public static int isLookingLeft   = 0;
    public static int isLookingRight  = 0;
    public static float eyelidStateLeft = 0.0f;
    public static float eyelidStateRight = 0.0f;
    public static float eyeLocationX  = 0.0f;
    public static float eyeLocationY  = 0.0f;
    public static float eyebrowExtent = 0.0f;
    public static float smileExtent   = 0.0f;
    public static float clenchExtent  = 0.0f;
    public static EdkDll.IEE_FacialExpressionAlgo_t upperFaceAction;
    public static EdkDll.IEE_FacialExpressionAlgo_t lowerFaceAction;
    public static float upperFacePower = 0.0f;
    public static float lowerFacePower = 0.0f;
    public static EdkDll.IEE_FacialExpressionAlgo_t[] expAlgoList = 
	{ 
        EdkDll.IEE_FacialExpressionAlgo_t.FE_NEUTRAL,
		EdkDll.IEE_FacialExpressionAlgo_t.FE_BLINK, 
		EdkDll.IEE_FacialExpressionAlgo_t.FE_WINK_LEFT, 
		EdkDll.IEE_FacialExpressionAlgo_t.FE_WINK_RIGHT,
		EdkDll.IEE_FacialExpressionAlgo_t.FE_HORIEYE,  
        EdkDll.IEE_FacialExpressionAlgo_t.FE_SUPRISE,
		EdkDll.IEE_FacialExpressionAlgo_t.FE_FROWN, 		
		EdkDll.IEE_FacialExpressionAlgo_t.FE_CLENCH, 
		EdkDll.IEE_FacialExpressionAlgo_t.FE_LAUGH, 
		EdkDll.IEE_FacialExpressionAlgo_t.FE_SMIRK_LEFT, 
		EdkDll.IEE_FacialExpressionAlgo_t.FE_SMIRK_RIGHT			
    };
	
    public static Boolean[] isExpActiveList = new Boolean[expAlgoList.Length];
    public static bool IsStarted = false;
    //----------------------------------------

    void Update()
    {
    }
	
    void Start()
    {
        if (!IsStarted)
        {
            engine.FacialExpressionEmoStateUpdated +=
                new EmoEngine.FacialExpressionEmoStateUpdatedEventHandler(engine_FacialExpressionEmoStateUpdated);
            engine.FacialExpressionTrainingStarted +=
                new EmoEngine.FacialExpressionTrainingStartedEventEventHandler(engine_FacialExpressionTrainingStarted);
            engine.FacialExpressionTrainingSucceeded +=
                new EmoEngine.FacialExpressionTrainingSucceededEventHandler(engine_FacialExpressionTrainingSucceeded);
            engine.FacialExpressionTrainingCompleted +=
                new EmoEngine.FacialExpressionTrainingCompletedEventHandler(engine_FacialExpressionTrainingCompleted);
            engine.FacialExpressionTrainingRejected +=
                new EmoEngine.FacialExpressionTrainingRejectedEventHandler(engine_FacialExpressionTrainingRejected); 
			engine.EmoStateUpdated += 
				new EmoEngine.EmoStateUpdatedEventHandler (engine_EmoStateUpdated);
            IsStarted = true;
        }
    }
	
	static void engine_EmoStateUpdated(object sender, EmoStateUpdatedEventArgs e){
		EmoState es = e.emoState;
		isBlink     = es.FacialExpressionIsBlink();
		isLeftWink  = es.FacialExpressionIsLeftWink();
		isRightWink = es.FacialExpressionIsRightWink();
		isEyesOpen  = es.FacialExpressionIsEyesOpen();
		isLookingUp = es.FacialExpressionIsLookingUp();
		isLookingDown = es.FacialExpressionIsLookingDown();
		isLookingLeft = es.FacialExpressionIsLookingLeft();
		isLookingRight  = es.FacialExpressionIsLookingRight();
		es.FacialExpressionGetEyelidState(out eyelidStateLeft, out eyelidStateRight);
		es.FacialExpressionGetEyeLocation(out eyeLocationX, out eyeLocationY);
		eyebrowExtent = es.FacialExpressionGetEyebrowExtent();
		smileExtent   = es.FacialExpressionGetSmileExtent();
		clenchExtent  = es.FacialExpressionGetClenchExtent();
		upperFaceAction = es.FacialExpressionGetUpperFaceAction();
		upperFacePower  = es.FacialExpressionGetUpperFaceActionPower();
		lowerFaceAction = es.FacialExpressionGetLowerFaceAction();
		lowerFacePower  = es.FacialExpressionGetLowerFaceActionPower();
		for (int i = 0; i < expAlgoList.Length; ++i)
		{
				isExpActiveList[i] = es.FacialExpressionIsActive(expAlgoList[i]);
		}
	}

    static void engine_FacialExpressionEmoStateUpdated(object sender, EmoStateUpdatedEventArgs e)
    {
    }
	
    static void engine_FacialExpressionTrainingStarted(object sender, EmoEngineEventArgs e)
    {
    }

    static void engine_FacialExpressionTrainingSucceeded(object sender, EmoEngineEventArgs e)
    {
        EmoEngine.Instance.FacialExpressionSetTrainingControl(0, EdkDll.IEE_FacialExpressionTrainingControl_t.FE_ACCEPT);
    }

    static void engine_FacialExpressionTrainingCompleted(object sender, EmoEngineEventArgs e)
    {        
    }

    static void engine_FacialExpressionTrainingRejected(object sender, EmoEngineEventArgs e)
    {        
    }

    public void StartTrainFacialExpression(EdkDll.IEE_FacialExpressionAlgo_t FacialExpressionAlg)
    {
        engine.FacialExpressionSetTrainingAction((uint)EmoUserManagement.currentUser, FacialExpressionAlg);
        engine.FacialExpressionSetTrainingControl((uint)EmoUserManagement.currentUser, EdkDll.IEE_FacialExpressionTrainingControl_t.FE_START);
    } 
}