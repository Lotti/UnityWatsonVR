using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class ClassifyResponse {
    public ImageData[] images;
    public int images_processed;
    public int custom_classes;
    public override string ToString() {
        List<string> array = new List<string>();
        int i = 0;
        foreach (ImageData img in images) {
            foreach (ClassifierData cla in img.classifiers) {
                foreach (ClassData c in cla.classes) {
                    array.Add((i+1).ToString("00") + ") " + cla.name + " - " + c.@class+ ": " + c.score);
                    i++;
                }
            }
        }

        return String.Join("\n", array);
    }
}

[System.Serializable]
public class ImageData
{
    public ClassifierData[] classifiers;
    public string image;
}

[System.Serializable]
public class ClassifierData
{
    public string classifier_id;
    public string name;
    public ClassData[] classes;
}

[System.Serializable]
public class ClassData {
    public string @class;
    public double score;
    public string type_hierarchy;
}
