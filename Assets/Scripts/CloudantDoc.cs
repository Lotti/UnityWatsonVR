[System.Serializable]
class CloudantDoc
{
    public string _id;
    public string _rev;
    public Question[] questions;
}

[System.Serializable]
class Question
{
    public string question;
    public string[] answers;
    public int score;
}


