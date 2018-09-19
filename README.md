# UnityWatsonVR

This is a sample app builded to show Watson Visual Recognition capabilities inside Unity.

It makes use of [Watson SDK for Unity](https://github.com/watson-developer-cloud/unity-sdk).

## Setup

0) Register/Login to IBM Cloud from https://console.bluemix.net
1) Instantiate a Visual Recognition service. Create credentials and copy-paste the API_KEY in Assets/Scripts/Secrets file.
2) *optional* Instantiate a Watson Studio service if you want to create custom visual recognition model to play with Tangram mode and Quiz mode.
3) *optional* Inside Watson Studio, create a custom model named "tangram" with provided images. [More info here](https://www.youtube.com/watch?v=EIJdQtKYS1g)
4) *optional* Inside Watson Studio, create a custom model named "waste" with provided images. [More info here](https://www.youtube.com/watch?v=EIJdQtKYS1g)
5) *optional* Instantiate a Cloudant service if you want to try the "quiz game". Create credentials and copy-paste URL and DB_NAME (you have to create one by hand, inside the service interface) them in Assets/Scripts/Secrets file.
6) *optional* Create a document inside the Cloudant db with the structure showed on the sample file ["cloudant-doc.json"](cloudant-doc.json)
7) run the app in Unity or deploy it on a phone (only Android platform was tested, but it could run also on iOS)
8) you're ready to play with Watson Visual Recognition Face detector, the default classifier, the tangram custom classifier and the quiz!

---
This sample was made for IBM PartyCloud 2018 in Milan, Italy.
