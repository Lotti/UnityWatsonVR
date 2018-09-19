using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using IBM.Watson.DeveloperCloud.Services.VisualRecognition.v3;
using IBM.Watson.DeveloperCloud.Connection;

#if !NETFX_CORE
using System.Net;
using System.Net.Security;
#endif

public class Quiz : MonoBehaviour {
    public Text questionText;
    public Text scoreText;
    public Button buttonAnswer;

    private CloudantDoc doc;
    private readonly List<Question> questions = new List<Question>();
    private int score = 0;
    private int questionPointer = 0;

    // Use this for initialization
    private void Start () {
        buttonAnswer.interactable = false;
        StartCoroutine(getQuestionsFromCloudant());
    }

    private void Update() {
        scoreText.text = score.ToString();
    }

    private string base64(string input) {
        byte[] bytesToEncode = Encoding.UTF8.GetBytes(input);
        return Convert.ToBase64String(bytesToEncode);

    }

    private IEnumerator getQuestionsFromCloudant() {
        string url = Secrets.CLOUDANT_URL + "/" + Secrets.CLOUDANT_DBNAME + "/quiz";
        string auth = "Basic " + base64(Secrets.CLOUDANT_USERNAME + ":" + Secrets.CLOUDANT_PASSWORD);

        /*
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Authorization", auth);

        WWW req = new WWW(url, null, headers);
        while (!req.isDone)
        {
            yield return null;
        }
        */


        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SetRequestHeader("Authorization", auth);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError) {
            Modal.ShowModal("Can't reach the Internet!\n\n" + www.error);
            Main.EndQuiz();
        } else {
            // Show results as text
            doc = JsonUtility.FromJson<CloudantDoc>(www.downloadHandler.text);
            StartQuiz();
        }
    }

    private void StartQuiz() {
        score = 0;
        questions.Clear();
        questions.AddRange(doc.questions);
        questions.Shuffle();
        NextQuestion();
    }

    private void NextQuestion() {
        if (questionPointer < questions.Count) {
            Question q = questions[questionPointer];
            questionText.text = q.question;
            buttonAnswer.interactable = true;
        } else {
            Modal.ShowModal("The quiz ended!\nYou scored " + score + " points!");
            Main.EndQuiz();
        }
    }

    public void AnswerQuestion() {
        Question q = questions[questionPointer];
        buttonAnswer.interactable = false;
        if (!Main.AnswerQuiz((ClassifiedImages response, Dictionary<string, object> customData) => {
            Debug.Log(customData["json"]);
            ClassifyResponse r = JsonUtility.FromJson<ClassifyResponse>(customData["json"].ToString());

            string @class = r.images[0].classifiers[0].classes[0].@class;
            bool correct = false;
            foreach (string a in q.answers) {
                Debug.Log(a + " - " + @class + " = " + a.Equals(@class, System.StringComparison.CurrentCultureIgnoreCase));
                if (a.Equals(@class, System.StringComparison.CurrentCultureIgnoreCase)) {
                    correct = true;
                    break;
                }
            }

            if (correct) {
                score += q.score;
                Modal.ShowModal("Right answer!\n\n" + r);
            } else {
                Modal.ShowModal("Wrong answer!\n\n" + r);
            }
            questionPointer++;
            NextQuestion();

        }, (RESTConnector.Error error, Dictionary<string, object> customData) => {
            Modal.ShowModal("error while querying watson: " + error.ErrorMessage);
            buttonAnswer.interactable = true;
        })) {
            Modal.ShowModal("Classify image failed!");
            buttonAnswer.interactable = true;
        } 
    }
}
