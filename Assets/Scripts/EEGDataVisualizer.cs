using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum WaveBand { ALPHA, LOW_BETA, HIGH_BETA, THETA, GAMMA}
public class EEGDataVisualizer : MonoBehaviour
{
    public static WaveBand band = WaveBand.THETA;

    float[] values;

    Text readValueDisplay;

    public float valueScale = 300000;
    // Start is called before the first frame update
    void Start()
    {
        values = new float[EEGDataReader.channelCount];
        readValueDisplay = GameObject.Find("Read Value").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        ReadData();
        for(int i = 0; i < EEGDataReader.channelCount; i++)
        {
            transform.GetChild(i).localScale = new Vector3(1, values[i] / valueScale, 1);
        }

        readValueDisplay.text = string.Concat(values[0] + "\r\n"+
            values[1] + "\r\n" +
            values[2] + "\r\n" +
            values[3] + "\r\n" +
            values[4] + "\r\n" +
            values[5] + "\r\n" +
            values[6] + "\r\n" +
            values[7] + "\r\n" +
            values[8] + "\r\n" +
            values[9] + "\r\n" +
            values[10] + "\r\n" +
            values[11] + "\r\n" +
            values[12] + "\r\n" +
            values[13] + "\r\n");
    }

    void ReadData()
    {
        switch (band)
        {
            case WaveBand.ALPHA:
                /*
                for(int i = 0; i < EEGDataReader.channelCount; i++)
                {
                    //values[i] = EEGDataReader.alphaValues[i];
                    values[i] = EEGDataReader.processedValues[0][i];
                }
                */
                values = EEGDataReader.processedValues[0];
                break;
            case WaveBand.LOW_BETA:
                /*
                for (int i = 0; i < EEGDataReader.channelCount; i++)
                {
                    //values[i] = EEGDataReader.low_betaValues[i];
                    values[i] = EEGDataReader.processedValues[1][i];
                }
                */
                values = EEGDataReader.processedValues[1];
                break;
            case WaveBand.HIGH_BETA:
                /*
                for (int i = 0; i < EEGDataReader.channelCount; i++)
                {
                    //values[i] = EEGDataReader.high_betaValues[i];
                    values[i] = EEGDataReader.processedValues[2][i];
                }
                */
                values = EEGDataReader.processedValues[2];
                break;
            case WaveBand.THETA:
                /*
                for (int i = 0; i < EEGDataReader.channelCount; i++)
                {
                    //values[i] = EEGDataReader.thetaValues[i];
                    values[i] = EEGDataReader.processedValues[3][i];
                }
                */
                values = EEGDataReader.processedValues[3];
                break;
            case WaveBand.GAMMA:
                /*
                for (int i = 0; i < EEGDataReader.channelCount; i++)
                {
                    //values[i] = EEGDataReader.gammaValues[i];
                    values[i] = EEGDataReader.processedValues[4][i];
                }
                */
                values = EEGDataReader.processedValues[4];
                break;
        }
            
    }

    public void SwitchToAlpha()
    {
        band = WaveBand.ALPHA;
    }

    public void SwitchToLow_Beta()
    {
        band = WaveBand.LOW_BETA;
    }

    public void SwitchToHigh_Beta()
    {
        band = WaveBand.HIGH_BETA;
    }

    public void SwitchToTheta()
    {
        band = WaveBand.THETA;
    }

    public void SwitchToGamma()
    {
        band = WaveBand.GAMMA;
    }
}
