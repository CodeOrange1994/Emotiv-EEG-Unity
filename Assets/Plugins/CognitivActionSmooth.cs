using UnityEngine;
using System.Collections;

public class CognitivActionEx
{
    public static float threshold;
    public float power;
    public float[] powerSamples;
    public CognitivActionEx()
    {
        power = 0;
        powerSamples = new float[6];
        for (int i = 0; i < 6; i++)
        {
            powerSamples[i] = 0;
        }
    }
    public void ClearSamples()
    {
        for (int i = 0; i < 6; i++)
        {
            powerSamples[i] = 0;
        }
    }
    public float Average(float newcome)
    {
        float total = 0.0f;
        for (int k = 0; k < 5; k++)
        {
            powerSamples[k] = powerSamples[k + 1];
        }

        powerSamples[5] = newcome;

        for (int k = 0; k < 6; k++)
        {
            total += powerSamples[k];
        }
        return total / 6;
    }
    public void UpdatePower(float iPower)
    {
        if (iPower >= threshold)
        {
            power = Average(iPower);
        }
        else
        {
            power = Average(0);
        }

    }
}

public class CognitivActionSmooth : MonoBehaviour
{
    public static CognitivActionEx pull;
    public float prePull;
    public static CognitivActionEx lift;
    public float preLift;
    private float timeLift;
    private

    void Start()
    {
        pull = new CognitivActionEx();
        lift = new CognitivActionEx();

    }

    void Update()
    {
		if (pull != null) {
			pull.UpdatePower(EmoMentalCommand.MentalCommandActionPower[2]);
			lift.UpdatePower(EmoMentalCommand.MentalCommandActionPower[3]);
		} else {
			Debug.Log ("pull or lift is null");
		}
    }
}

