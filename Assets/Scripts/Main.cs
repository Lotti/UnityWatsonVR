using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using IBM.Watson.DeveloperCloud.Services.VisualRecognition.v3;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.Connection;

public class Main : MonoBehaviour
{

    public GameObject Menu;
    public GameObject Quiz;
    public RawImage webcamImage;
    public ScrollContent scroll;
    public ScrollContentButton scrollButton;
    public AspectRatioFitter webcamImageARF;

    private static VisualRecognition _visualRecognitionTangram;
    private static VisualRecognition _visualRecognitionWaste;
    private WebCamTexture wct;

    private WebCamDevice cam;
    private int currentCam;
    private int camAmount;
    private static Main instance;

    private void Awake()
    {
        instance = this;

        InstantiateVR();

        Menu.SetActive(true);
        Quiz.SetActive(false);
    }

    // Use this for initialization
    private void Start()
    {
        camAmount = WebCamTexture.devices.Length;
        StartCamera(0);
    }

    // Update is called once per frame
    private void Update()
    {
        if (wct.width < 100)
        {
            Debug.Log("Still waiting another frame for correct info...");
            return;
        }

        // change as user rotates iPhone or Android:

        int cwNeeded = wct.videoRotationAngle;
        // Unity helpfully returns the _clockwise_ twist needed
        // guess nobody at Unity noticed their product works in counterclockwise:
        int ccwNeeded = -cwNeeded;

        // IF the image needs to be mirrored, it seems that it
        // ALSO needs to be spun. Strange: but true.
        if (wct.videoVerticallyMirrored) ccwNeeded += 180;

        // you'll be using a UI RawImage, so simply spin the RectTransform
        webcamImage.rectTransform.localEulerAngles = new Vector3(0f, 0f, ccwNeeded);

        float videoRatio = (float)wct.width / (float)wct.height;

        // you'll be using an AspectRatioFitter on the Image, so simply set it
        webcamImageARF.aspectRatio = videoRatio;

        // alert, the ONLY way to mirror a RAW image, is, the uvRect.
        // changing the scale is completely broken.
        if (wct.videoVerticallyMirrored)
        {
            // webcamImage.uvRect = new Rect(1, 0, -1, 1);  // means flip on vertical axis
            webcamImage.uvRect = new Rect(0, 1, 1, -1);  // means flip on vertical axis
        }
        else
        {
            webcamImage.uvRect = new Rect(0, 0, 1, 1);  // means no flip
        }
    }

    private void InstantiateVR()
    {
        TokenOptions iamTokenOptionsTangram = new TokenOptions()
        {
            IamApiKey = Secrets.APIKEY_TANGRAM
        };

        Credentials cTangram = new Credentials(iamTokenOptionsTangram);

        _visualRecognitionTangram = new VisualRecognition(cTangram)
        {
            VersionDate = "2018-03-19"
        };


        TokenOptions iamTokenOptionsWaste = new TokenOptions()
        {
            IamApiKey = Secrets.APIKEY_WASTE
        };

        Credentials cWaste = new Credentials(iamTokenOptionsWaste);

        _visualRecognitionWaste = new VisualRecognition(cWaste)
        {
            VersionDate = "2018-03-19"
        };
    }

    private void StartCamera(int i)
    {
        if (wct != null && wct.isPlaying)
        {
            wct.Stop();
        }

        WebCamDevice device = WebCamTexture.devices[i];
        wct = new WebCamTexture(device.name)
        {
            // Set camera filter modes for a smoother looking image
            filterMode = FilterMode.Trilinear
        };

        webcamImage.texture = wct;
        webcamImage.material.mainTexture = wct;
        wct.Play();
    }

    public void CycleCamera()
    {
        currentCam++;
        if (currentCam >= camAmount)
        {
            currentCam = 0;
        }

        StartCamera(currentCam);
    }

    private void TakePicture()
    {
        if (System.IO.File.Exists(GetFilePath())) {
            System.IO.File.Delete(GetFilePath());
        }

        scroll.AddContent(new string[] { "Taking Picture..." });
        scrollButton.ShowPanel();

        Texture2D snap = new Texture2D(wct.width, wct.height);
        snap.SetPixels32(wct.GetPixels32());
        snap.Apply();
        #if UNITY_ANDROID
        snap = rotateTexture(snap, true);
        #endif
        System.IO.File.WriteAllBytes(GetFilePath(), snap.EncodeToJPG());
        string filename = string.Format("{0}/screen_UNITY_{1}.png", Application.temporaryCachePath, System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
        System.IO.File.WriteAllBytes(filename, snap.EncodeToJPG());
        Debug.Log("saved to: "+filename);
    }

    private static string GetFilePath()
    {
        string fileName = "webcamImage.jpg";
        return System.IO.Path.Combine(Application.temporaryCachePath, fileName);
    }

    public void Check(string what)
    {
        scrollButton.HidePanel();
        wct.Pause();
        TakePicture();
        switch (what)
        {
            default:
            case "default":
                ClassifyImage();
                break;
            case "face":
                DetectFaces();
                break;
            case "tangram":
                DetectTangram();
                break;
        }
        wct.Play();
        scroll.AddContent(new string[] { "Sending picture to Watson..." });
        scrollButton.ShowPanel();
    }

    private void DetectFaces()
    {
        //  Detect Face using image path
        if (!_visualRecognitionTangram.DetectFaces((DetectedFaces response, Dictionary<string, object> customData) => {
            FaceResponse r = JsonUtility.FromJson<FaceResponse>(customData["json"].ToString());
            scroll.AddContent(r.ToString().Split('\n'));
            scrollButton.ShowPanel();
        }, OnFail, GetFilePath()))
        {
            Debug.Log("Detect faces failed!");
            scroll.AddContent(new string[] { "Detect faces failed!" });
            scrollButton.ShowPanel();
        }
    }

    private void ClassifyImage()
    {
        string[] classifiers = { "default" };

        //  Classify image using image path
        if (!_visualRecognitionTangram.Classify((ClassifiedImages response, Dictionary<string, object> customData) =>
        {
            Debug.Log(customData["json"]);
            ClassifyResponse r = JsonUtility.FromJson<ClassifyResponse>(customData["json"].ToString());
            scroll.AddContent(r.ToString().Split('\n'));
            scrollButton.ShowPanel();
        }, OnFail, GetFilePath(), null, classifiers, 0.5f))
        {
            Debug.Log("Classify image failed!");
            scroll.AddContent(new string[] { "Classify image failed!" });
            scrollButton.ShowPanel();
        }
    }

    private void DetectTangram()
    {
        string[] classifiers = { Secrets.TANGRAM_CUSTOM_CLASSIFICATOR };

        //  Classify image using image path
        if (!_visualRecognitionTangram.Classify((ClassifiedImages response, Dictionary<string, object> customData) =>
        {
            Debug.Log(customData["json"]);
            ClassifyResponse r = JsonUtility.FromJson<ClassifyResponse>(customData["json"].ToString());
            scroll.AddContent(r.ToString().Split('\n'));
            scrollButton.ShowPanel();
        }, OnFail, GetFilePath(), null, classifiers, 0.0f))
        {
            Debug.Log("Classify image failed!");
            scroll.AddContent(new string[] { "Classify image failed!" });
            scrollButton.ShowPanel();
        }
    }

    public static bool AnswerQuiz(VisualRecognition.SuccessCallback<ClassifiedImages> successCallback, VisualRecognition.FailCallback failCallback)
    {
        string[] classifiers = { Secrets.QUIZ_CUSTOM_CLASSIFICATOR };

        //  Classify image using image path
        return _visualRecognitionWaste.Classify(successCallback, failCallback, GetFilePath(), null, classifiers, 0.5f);
    }

    private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
    {
        Debug.Log("error while querying watson: " + error.ErrorMessage);
        scroll.AddContent(new string[] { error.ErrorMessage });
        scrollButton.ShowPanel();
    }

    public void StartQuiz()
    {
        Menu.SetActive(false);
        Quiz.SetActive(true);
    }

    public static void EndQuiz()
    {
        instance.Menu.SetActive(true);
        instance.Quiz.SetActive(false);
    }




    Texture2D rotateTexture(Texture2D originalTexture, bool clockwise)
    {
        Color32[] original = originalTexture.GetPixels32();
        Color32[] rotated = new Color32[original.Length];
        int w = originalTexture.width;
        int h = originalTexture.height;

        int iRotated, iOriginal;

        for (int j = 0; j < h; ++j)
        {
            for (int i = 0; i < w; ++i)
            {
                iRotated = (i + 1) * h - j - 1;
                iOriginal = clockwise ? original.Length - 1 - (j * w + i) : j * w + i;
                rotated[iRotated] = original[iOriginal];
            }
        }

        Texture2D rotatedTexture = new Texture2D(h, w);
        rotatedTexture.SetPixels32(rotated);
        rotatedTexture.Apply();
        return rotatedTexture;
    }
}
