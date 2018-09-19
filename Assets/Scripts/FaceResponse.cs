using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class FaceResponse {
    public FaceImageData[] images;
    public int images_processed;
    public override string ToString() {
        List<string> array = new List<string>();
        int i = 0;
        foreach (FaceImageData img in images) {
            foreach (Faces f in img.faces) {
                array.Add((i+1).ToString("00") + ") age: " + f.age.min + "-" + f.age.max + " (" + f.age.score + "), gender: " + f.gender.gender + " ("+ f.gender.score +")");
                i++;
            }
        }

        return String.Join("\n", array);
    }
}

[System.Serializable]
public class FaceImageData {
    public Faces[] faces;
    public string image;
}

[System.Serializable]
public class Faces {
    public Age age;
    public Location face_location;
    public Gender gender;
}

[System.Serializable]
public class Age
{
    public int min;
    public int max;
    public double score;
}

[System.Serializable]
public class Location {
    public int height;
    public int width;
    public int left;
    public int top;
}

[System.Serializable]
public class Gender {
    public string gender;
    public double score;
}
