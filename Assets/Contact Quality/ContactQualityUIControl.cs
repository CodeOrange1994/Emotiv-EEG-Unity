using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public enum HeadsetType
{
    Unknown = 0,
    Epoc,
    Insight
}

//listen emostateupdate event to display contact quality
public class ContactQualityUIControl : MonoBehaviour {

    GameObject insight, epoc;
    Image[] iChilds, eChilds;
    public Text txtDevice;
	private HeadsetType headsetType = HeadsetType.Epoc;
	private long _currentHeadsetVersion = 0;

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

    private void onEmoStateUpdated(object sender, EmoStateUpdatedEventArgs args)
	{
		DetectHeadset(args.userId);

        EdkDll.IEE_EEG_ContactQuality_t[] cq = getContactQuality(args.emoState);
        setColor(cq);
    }

    EdkDll.IEE_EEG_ContactQuality_t[] getContactQuality(EmoState state){
        EdkDll.IEE_EEG_ContactQuality_t[] cq;
        if (headsetType == HeadsetType.Epoc)
        {

            //epoc contact quality
            cq = new EdkDll.IEE_EEG_ContactQuality_t[16];

            cq[0]  = state.GetContactQuality((int)EdkDll.IEE_InputChannels_t.IEE_CHAN_AF3);
            cq[1]  = state.GetContactQuality((int)EdkDll.IEE_InputChannels_t.IEE_CHAN_AF4);
            cq[2]  = state.GetContactQuality((int)EdkDll.IEE_InputChannels_t.IEE_CHAN_F3);
            cq[3]  = state.GetContactQuality((int)EdkDll.IEE_InputChannels_t.IEE_CHAN_F4);
            cq[4]  = state.GetContactQuality((int)EdkDll.IEE_InputChannels_t.IEE_CHAN_F7);
            cq[5]  = state.GetContactQuality((int)EdkDll.IEE_InputChannels_t.IEE_CHAN_F8);
            cq[6]  = state.GetContactQuality((int)EdkDll.IEE_InputChannels_t.IEE_CHAN_FC5);
            cq[7]  = state.GetContactQuality((int)EdkDll.IEE_InputChannels_t.IEE_CHAN_FC6);
            cq[8]  = state.GetContactQuality((int)EdkDll.IEE_InputChannels_t.IEE_CHAN_O1);
            cq[9]  = state.GetContactQuality((int)EdkDll.IEE_InputChannels_t.IEE_CHAN_O2);
            cq[10] = state.GetContactQuality((int)EdkDll.IEE_InputChannels_t.IEE_CHAN_P7);
            cq[11] = state.GetContactQuality((int)EdkDll.IEE_InputChannels_t.IEE_CHAN_P8);
            cq[12] = state.GetContactQuality((int)EdkDll.IEE_InputChannels_t.IEE_CHAN_T7);
            cq[13] = state.GetContactQuality((int)EdkDll.IEE_InputChannels_t.IEE_CHAN_T8);
            cq[14] = state.GetContactQuality((int)EdkDll.IEE_InputChannels_t.IEE_CHAN_CMS);
            cq[15] = state.GetContactQuality((int)EdkDll.IEE_InputChannels_t.IEE_CHAN_DRL);
            return cq;
        }

        //insight contact quality
        cq = new EdkDll.IEE_EEG_ContactQuality_t[6];
        cq[0] = EdkDll.IEE_EEG_ContactQuality_t.IEEG_CQ_VERY_BAD;
        cq[1] = state.GetContactQuality((int)EdkDll.IEE_InputChannels_t.IEE_CHAN_AF3);
        cq[2] = state.GetContactQuality((int)EdkDll.IEE_InputChannels_t.IEE_CHAN_T7);
		cq[3] = state.GetContactQuality((int)EdkDll.IEE_InputChannels_t.IEE_CHAN_O1);
        cq[4] = state.GetContactQuality((int)EdkDll.IEE_InputChannels_t.IEE_CHAN_T8);
        cq[5] = state.GetContactQuality((int)EdkDll.IEE_InputChannels_t.IEE_CHAN_AF4);
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
		uint version;
		EdkDll.IEE_HardwareGetVersion(userId, out version);


		long headsetVersion = (version & 0xffff0000) >> 16;
		long dongleVersion  = (version & 0x0000ffff);

		//0x30XX/0x70XX - EPOC+ Consumer,   0x6XX/0x7XX - EPOC+ Premium
		//0x50XX/0x90XX - Insight Consumer, 0x8XX/0x9XX - Insight Premium
		//0x1000/0x1E00 - EPOC Consumer,    0x0565      - EPOC Premium
		//0x17B0        - Insight Consumer, 0x0170      - Insight Premium on Prototype mode

		if (headsetVersion == 0x0565 || headsetVersion == 0x1000 || headsetVersion == 0x1E00) {
			
			if (_currentHeadsetVersion != headsetVersion) {
				_currentHeadsetVersion = headsetVersion;
				headsetType = HeadsetType.Epoc;
				ShowHeadset();
			}
			
			return;
		}			

		headsetVersion = headsetVersion & 0xff00;

		if (headsetVersion == 0x3000 || headsetVersion == 0x7000 
			|| headsetVersion == 0x600 || headsetVersion == 0x700) {

			if (_currentHeadsetVersion != headsetVersion){
				_currentHeadsetVersion = headsetVersion;
				headsetType = HeadsetType.Epoc;
				ShowHeadset();
			}

			return;
		}

		if (headsetVersion == 0x5000 || headsetVersion == 0x9000
			|| headsetVersion == 0x800 || headsetVersion == 0x900) {

			if (_currentHeadsetVersion != headsetVersion){
				_currentHeadsetVersion = headsetVersion;
				headsetType = HeadsetType.Insight;
				ShowHeadset();
			}

			return;
		}			
    }

    private void onUserAdded(object sender, EmoEngineEventArgs args){
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
			UnityEngine.Debug.Log ("EPOC");
		}
		else
		{
			epoc.SetActive(false);
			insight.SetActive(true);
			UnityEngine.Debug.Log ("INSIGHT");
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
                returnButt = new Color(Color.black.r, Color.black.g, Color.black.b, 1f);
                break;
            case EdkDll.IEE_EEG_ContactQuality_t.IEEG_CQ_VERY_BAD:
                returnButt = new Color(Color.red.r, Color.red.g, Color.red.b, 1f);
                break;
            case EdkDll.IEE_EEG_ContactQuality_t.IEEG_CQ_POOR:
                returnButt = new Color(1.0F, 0.5F, 0.0F, 1f);
                break;
            case EdkDll.IEE_EEG_ContactQuality_t.IEEG_CQ_FAIR:
                returnButt = new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, 1f);
                break;
            case EdkDll.IEE_EEG_ContactQuality_t.IEEG_CQ_GOOD:
                returnButt = new Color(Color.green.r, Color.green.g, Color.green.b, 1f);
                break;
            default:
                returnButt = new Color(Color.black.r, Color.black.g, Color.black.b, 1f);
                break;
        }
        return returnButt;
    }
}
