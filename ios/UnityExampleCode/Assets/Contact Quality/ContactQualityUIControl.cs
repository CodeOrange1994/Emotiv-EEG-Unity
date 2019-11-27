using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.UI;
public enum HeadsetType
{
    Unknown = 0,
    Epoc,
    Insight
}
public enum IEE_HeadsetVersion_t  {
    IEE_EPOC_EEG = 0x0565,
    IEE_EPOC_NOEEG1 = 0x1000,
    IEE_EPOC_NOEEG2 = 0x1E00,
    IEE_INSIGHT_EEG_OLD = 0x0170,
    IEE_INSIGHT_NOEEG1_OLD = 0x17B0,
    IEE_INSIGHT_NOEEG2_OLD = 0x0810,
    IEE_INSIGHT_EEG1_NEW = 0x08,
    IEE_INSIGHT_EEG2_NEW = 0x09,
    IEE_INSIGHT_NOEEG1_NEW = 0x50,
    IEE_INSIGHT_NOEEG2_NEW = 0x90,
    IEE_EPOC_PLUS_EEG1 = 0x06,
    IEE_EPOC_PLUS_EEG2 = 0x07,
    IEE_EPOC_PLUS_NOEEG1 = 0x30,
    IEE_EPOC_PLUS_NOEEG2 = 0x70
};
//listen emostateupdate event to display contact quality
public class ContactQualityUIControl : MonoBehaviour {

    GameObject insight, epoc;
    Image[] iChilds, eChilds;
    public Text txtDevice;
    private HeadsetType headsetType = HeadsetType.Unknown;

	private static ContactQualityUIControl _instance;
	public static ContactQualityUIControl Instance{
		get{
			return _instance;
		}
	}
	public Image[] epocChannel, insightChannel;
    public int type;
	// Use this for initialization

	void Awake(){
		_instance = this;
	}

    bool allowCheckHeadset = true;
    EmoEngine engine;


	void Start () {
        initHeadset();
        engine = EmoEngine.Instance;
        bindEvents();
	}

    void bindEvents(){
        engine.EmoStateUpdated += onEmoStateUpdated;
        engine.UserAdded += onUserAdded;
        engine.UserRemoved += onUserRemoved;
    }

    private void onEmoStateUpdated(object sender, EmoStateUpdatedEventArgs args){
        if (allowCheckHeadset)
        {
            allowCheckHeadset = false;
            DetectHeadset(args.userId);
            ShowHeadset();

        }
        EdkDll.IEE_EEG_ContactQuality_t[] cq = getContactQuality(args.emoState);
        setColor(cq);
    }

    EdkDll.IEE_EEG_ContactQuality_t[] getContactQuality(EmoState state){
        EdkDll.IEE_EEG_ContactQuality_t[] cq;
        if (headsetType == HeadsetType.Epoc)
        {

            //epoc contact quality
            cq = new EdkDll.IEE_EEG_ContactQuality_t[16];

            cq [0] = state.GetContactQuality((int)EdkDll.IEE_InputChannels_t.IEE_CHAN_AF3);
            cq[1]= state.GetContactQuality((int)EdkDll.IEE_InputChannels_t.IEE_CHAN_AF4);
            cq[2]= state.GetContactQuality((int)EdkDll.IEE_InputChannels_t.IEE_CHAN_F3);
            cq[3]= state.GetContactQuality((int)EdkDll.IEE_InputChannels_t.IEE_CHAN_F4);
            cq[4]= state.GetContactQuality((int)EdkDll.IEE_InputChannels_t.IEE_CHAN_F7);
            cq[5]= state.GetContactQuality((int)EdkDll.IEE_InputChannels_t.IEE_CHAN_F8);
            cq[6]= state.GetContactQuality((int)EdkDll.IEE_InputChannels_t.IEE_CHAN_FC5);
            cq[7]= state.GetContactQuality((int)EdkDll.IEE_InputChannels_t.IEE_CHAN_FC6);
            cq[8]= state.GetContactQuality((int)EdkDll.IEE_InputChannels_t.IEE_CHAN_O1);
            cq[9]= state.GetContactQuality((int)EdkDll.IEE_InputChannels_t.IEE_CHAN_O2);
            cq[10]= state.GetContactQuality((int)EdkDll.IEE_InputChannels_t.IEE_CHAN_P7);
            cq[11]= state.GetContactQuality((int)EdkDll.IEE_InputChannels_t.IEE_CHAN_P8);
            cq[12]= state.GetContactQuality((int)EdkDll.IEE_InputChannels_t.IEE_CHAN_T7);
            cq[13]= state.GetContactQuality((int)EdkDll.IEE_InputChannels_t.IEE_CHAN_T8);
            cq[14]= state.GetContactQuality((int)EdkDll.IEE_InputChannels_t.IEE_CHAN_CMS);
            cq[15]= state.GetContactQuality((int)EdkDll.IEE_InputChannels_t.IEE_CHAN_DRL);
            return cq;
        }

        //insight contact quality
        cq = new EdkDll.IEE_EEG_ContactQuality_t[6];
        cq [0] = EdkDll.IEE_EEG_ContactQuality_t.IEEG_CQ_VERY_BAD;
        cq[1]= state.GetContactQuality((int)EdkDll.IEE_InputChannels_t.IEE_CHAN_AF3);
        cq[2]= state.GetContactQuality((int)EdkDll.IEE_InputChannels_t.IEE_CHAN_T7);
        cq[3]= state.GetContactQuality((int)EdkDll.IEE_InputChannels_t.IEE_CHAN_Pz);
        cq[4]= state.GetContactQuality((int)EdkDll.IEE_InputChannels_t.IEE_CHAN_T8);
        cq[5]= state.GetContactQuality((int)EdkDll.IEE_InputChannels_t.IEE_CHAN_AF4);
        for (int i = 1; i < cq.Length; i++)
        {
            if (cq[i] > EdkDll.IEE_EEG_ContactQuality_t.IEEG_CQ_VERY_BAD)
            {
                cq[0] = EdkDll.IEE_EEG_ContactQuality_t.IEEG_CQ_GOOD;
                break;
            }
        }
        return cq;
    }
    void DetectHeadset(uint userId){
        //uint v = 0;
        ////auto detect headset type
        //EdkDll.Plugin_IEE_HardwareGetVersion((UInt32)userId, out v);
        //uint a = v >> 24;
        //UnityEngine.Debug.Log ("detect hardware: " + a.ToString() + ":" + v.ToString ());
        //if (a != (uint)IEE_HeadsetVersion_t.IEE_INSIGHT_EEG1_NEW &&
        //    a != (uint)IEE_HeadsetVersion_t.IEE_INSIGHT_EEG2_NEW &&
        //    a != (uint)IEE_HeadsetVersion_t.IEE_INSIGHT_NOEEG1_NEW &&
        //    a != (uint)IEE_HeadsetVersion_t.IEE_INSIGHT_NOEEG2_NEW &&
        //    a != (uint)IEE_HeadsetVersion_t.IEE_INSIGHT_NOEEG1_OLD &&
        //    a != (uint)IEE_HeadsetVersion_t.IEE_INSIGHT_NOEEG1_OLD)
        //{
        //    txtDevice.text = "Epoc";
        //    headsetType = HeadsetType.Epoc;
        //}
        //else
        //{
        //    txtDevice.text = "Insight";
        //    headsetType = HeadsetType.Insight;
        //}
        headsetType = HeadsetType.Insight;
    }

    private void onUserAdded(object sender, EmoEngineEventArgs args){
        allowCheckHeadset = true;
        txtDevice.text = "User added!";
    }
    private void onUserRemoved(object sender, EmoEngineEventArgs args){
        Debug.Log("user removed");
        txtDevice.text = "User removed!";
    }
    void initHeadset(){
        foreach (Transform t in transform)
        {
            if (t.gameObject.name == "insight")
            {
                insight = t.gameObject;
                iChilds = new Image[6];
                foreach (Transform t1 in insight.transform)
                {
                    if (t1.gameObject.name == "bg")
                    {
                        continue;
                    }
                    int index;

                    int.TryParse(t1.gameObject.name, out index);
                    iChilds[index] = t1.gameObject.GetComponent<Image>();
                }
            }
            else if (t.gameObject.name == "epoc")
            {
                epoc = t.gameObject;
                eChilds = new Image[17];
                foreach (Transform t1 in epoc.transform)
                {
                    if (t1.gameObject.name == "bg")
                    {
                        continue;
                    }
                    int index;
                    int.TryParse(t1.gameObject.name, out index);
                    eChilds[index] = t1.gameObject.GetComponent<Image>();
                }
            }
        }
        setColor (null);
    }

    public void ShowHeadset(){
        if (headsetType == HeadsetType.Epoc)
		{
			epoc.SetActive(true);
			insight.SetActive(false);
		}
		else
		{
			epoc.SetActive(false);
			insight.SetActive(true);
		}
	}
    //insight
    public void setColor(EdkDll.IEE_EEG_ContactQuality_t[] contacts)
    {
		if (contacts == null) {
			for (var i = 0; i < epocChannel.Length;i++){
				epocChannel[i].color = getColor(EdkDll.IEE_EEG_ContactQuality_t.IEEG_CQ_NO_SIGNAL);
			}
			insightChannel[0].color = getColor (EdkDll.IEE_EEG_ContactQuality_t.IEEG_CQ_NO_SIGNAL);
			insightChannel[1].color = getColor (EdkDll.IEE_EEG_ContactQuality_t.IEEG_CQ_NO_SIGNAL);
			insightChannel[2].color = getColor (EdkDll.IEE_EEG_ContactQuality_t.IEEG_CQ_NO_SIGNAL);
			insightChannel[3].color = getColor (EdkDll.IEE_EEG_ContactQuality_t.IEEG_CQ_NO_SIGNAL);
			insightChannel[4].color = getColor (EdkDll.IEE_EEG_ContactQuality_t.IEEG_CQ_NO_SIGNAL);
			insightChannel[5].color = getColor (EdkDll.IEE_EEG_ContactQuality_t.IEEG_CQ_NO_SIGNAL);
			return;
		}
        if (headsetType == HeadsetType.Epoc)
        {
			for (var i = 0; i < epocChannel.Length;i++){
				epocChannel[i].color = getColor(contacts[i]);
			}
        }
        else
        {
            for (var i = 0; i < insightChannel.Length;i++){
                insightChannel[i].color = getColor(contacts[i]);
            }
//			
//			insightChannel[1].color = getColor (contacts [(int)DataProcessing.Channels.AF3]);
//			insightChannel[2].color = getColor (contacts [(int)DataProcessing.Channels.T7]);
//			insightChannel[3].color = getColor (contacts [(int)DataProcessing.Channels.O1]);
//			insightChannel[4].color = getColor (contacts [(int)DataProcessing.Channels.T8]);
//			insightChannel[5].color = getColor (contacts [(int)DataProcessing.Channels.AF4]);
        }
    }
    int current = 0;
    public void test()
    {
        EdkDll.IEE_EEG_ContactQuality_t[] contacts = new EdkDll.IEE_EEG_ContactQuality_t[17];
        for (int i = 0; i < 17; i++)
        {
            contacts[i] = (EdkDll.IEE_EEG_ContactQuality_t)((current + i) % 5);
        }
        current++;
        setColor(contacts);
    }

    private Color getColor(EdkDll.IEE_EEG_ContactQuality_t node)
    {
        Color returnButt;
        switch (node)
        {
            case EdkDll.IEE_EEG_ContactQuality_t.IEEG_CQ_NO_SIGNAL:
                //returnButt = Color.black;
                returnButt = new Color(Color.black.r, Color.black.g, Color.black.b, 1f);
                break;
            case EdkDll.IEE_EEG_ContactQuality_t.IEEG_CQ_VERY_BAD:
                //returnButt = Color.red;
                returnButt = new Color(Color.red.r, Color.red.g, Color.red.b, 1f);
                break;
            case EdkDll.IEE_EEG_ContactQuality_t.IEEG_CQ_POOR:
                returnButt = new Color(1.0F, 0.5F, 0.0F, 1f);
                //returnButt = new Color(Color.black.r, Color.black.g, Color.black.b, 0.3f);
                break;
            case EdkDll.IEE_EEG_ContactQuality_t.IEEG_CQ_FAIR:
                //returnButt = Color.yellow;
                returnButt = new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, 1f);
                break;
            case EdkDll.IEE_EEG_ContactQuality_t.IEEG_CQ_GOOD:
                //returnButt = Color.green;
                returnButt = new Color(Color.green.r, Color.green.g, Color.green.b, 1f);
                break;
            default:
                //returnButt = Color.black;
                returnButt = new Color(Color.black.r, Color.black.g, Color.black.b, 1f);
                break;
        }
        return returnButt;
    }
}
