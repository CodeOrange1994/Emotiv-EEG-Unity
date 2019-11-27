using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EEGDataPerformer : MonoBehaviour
{

    float[][] values;

    MeshRenderer[] meshRenderers;

    static int colorPropertyId = Shader.PropertyToID("_Color");
    static MaterialPropertyBlock sharedPropertyBlock;

    // Start is called before the first frame update
    void Start()
    {
        
        meshRenderers = new MeshRenderer[EEGDataReader.channelCount];
        for (int i = 0; i < EEGDataReader.channelCount; i++)
        {
            meshRenderers[i] = transform.GetChild(i).GetChild(0).GetComponent<MeshRenderer>();
            Debug.Log("Recorded Chanel " + i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(values == null)
        {
            values = EEGDataReader.processedValues;
        }


        for (int i = 0; i < EEGDataReader.channelCount; i++)
        {
            transform.GetChild(i).position = new Vector3(transform.GetChild(i).position.x, values[0][i] / 300000, transform.GetChild(i).position.z); // alpha controls altitude
            transform.GetChild(i).GetChild(0).localScale = new Vector3(values[1][i] / 1000000, values[1][i] / 1000000, values[1][i] / 1000000); // low-beta controls scale

            Color color = Color.HSVToRGB(values[2][i] / 2000000, values[3][i] / 2000000, 1);//high-beta controls hue, theta controls saturation
            if (sharedPropertyBlock == null)
            {
                sharedPropertyBlock = new MaterialPropertyBlock();
            }
            sharedPropertyBlock.SetColor(colorPropertyId, color);
            meshRenderers[i].SetPropertyBlock(sharedPropertyBlock);
        }
    }
}
