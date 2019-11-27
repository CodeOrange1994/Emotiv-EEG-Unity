using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EEGDataReader : MonoBehaviour
{
    EmoEngine engine;
    uint userId;

    double[] alpha = new double[1];
    double[] low_beta = new double[1];
    double[] high_beta = new double[1];
    double[] gamma = new double[1];
    double[] theta = new double[1];

    EdkDll.IEE_DataChannel_t[] channelList;
    public static float[] thetaValues;
    public static float[] alphaValues;
    public static float[] low_betaValues;
    public static float[] high_betaValues;
    public static float[] gammaValues;

    public static float[][] processedValues;
    public static float[][] currentValues;
    public float currentValueRatio = 0.8f;

    public static int channelCount = 14;

    // Start is called before the first frame update
    void Start()
    {
        engine = EmoEngine.Instance;
        bindEvents();

        channelList = new EdkDll.IEE_DataChannel_t[14] {EdkDll.IEE_DataChannel_t.IED_F3,
                                                        EdkDll.IEE_DataChannel_t.IED_F4,
                                                        EdkDll.IEE_DataChannel_t.IED_AF3,
                                                        EdkDll.IEE_DataChannel_t.IED_AF4,
                                                        EdkDll.IEE_DataChannel_t.IED_F7,
                                                        EdkDll.IEE_DataChannel_t.IED_F8,
                                                        EdkDll.IEE_DataChannel_t.IED_FC5,
                                                        EdkDll.IEE_DataChannel_t.IED_FC6,
                                                        EdkDll.IEE_DataChannel_t.IED_T7,
                                                        EdkDll.IEE_DataChannel_t.IED_T8,
                                                        EdkDll.IEE_DataChannel_t.IED_P7,
                                                        EdkDll.IEE_DataChannel_t.IED_P8,
                                                        EdkDll.IEE_DataChannel_t.IED_O1,
                                                        EdkDll.IEE_DataChannel_t.IED_O2 };

        thetaValues = new float[channelCount];
        alphaValues = new float[channelCount];
        low_betaValues = new float[channelCount];
        high_betaValues = new float[channelCount];
        gammaValues = new float[channelCount];

        currentValues = new float[5][];
        processedValues = new float[5][];
        for(int i = 0; i < 5; i++)
        {
            processedValues[i] = new float[channelCount];
        }

        currentValues[0] = alphaValues;
        currentValues[1] = low_betaValues;
        currentValues[2] = high_betaValues;
        currentValues[3] = thetaValues;
        currentValues[4] = gammaValues;
    }

    // Update is called once per frame
    void Update()
    {
        //engine.ProcessEvents(10);
        
        for (int i = 0; i < channelCount; i++)
        {
            int result = engine.IEE_GetAverageBandPowers((uint)userId, channelList[i], theta, alpha, low_beta, high_beta, gamma);
            if(result == EdkDll.EDK_OK)
            {
                if (i == 0)
                {
                    Debug.Log("Chanel" + i + " alpha: " + alpha[0]);
                }
                
                thetaValues[i] = (float)theta[0];
                alphaValues[i] = (float)alpha[0];
                low_betaValues[i] = (float)low_beta[0];
                high_betaValues[i] = (float)high_beta[0];
                gammaValues[i] = (float)gamma[0];
            }
        }

        
        ProcessValues();
    }



    private void ProcessValues()
    {
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < channelCount; j++)
            {
                processedValues[i][j] = currentValues[i][j]*currentValueRatio + processedValues[i][j]*(1-currentValueRatio);
            }
        }
    }

    void bindEvents()
    {
        engine.UserAdded += onUserAdded;
        engine.UserRemoved += onUserRemoved;
    }

    private void onUserAdded(object sender, EmoEngineEventArgs args)
    {
        userId = args.userId;
    }

    private void onUserRemoved(object sender, EmoEngineEventArgs args)
    {

    }

    

}
