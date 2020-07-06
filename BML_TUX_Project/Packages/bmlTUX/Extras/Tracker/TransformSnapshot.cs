using System;
using UnityEngine;

[Serializable]
public struct TransformSnapshot {
    
    public int frame;
    public float timeStamp;
    
    public Vector3 position;
    public Quaternion rotation;

    public Vector3 localPosition;
    public Quaternion localRotation;
    public Vector3 localScale;

    public TransformSnapshot(Transform transform, int frame, float timeStamp) {
        position = transform.position;
        rotation = transform.rotation;
        localScale = transform.localScale;
        localPosition = transform.localPosition;
        localRotation = transform.localRotation;
        this.frame = frame;
        this.timeStamp = timeStamp;
    }

    public static TransformSnapshot FromCSVLine(string csvLine) {
        string[] split = csvLine.Split(',');
        // Debug.Log($"Line: {split.Length}");
        // foreach (string s in split) {
        //     Debug.Log(s);
        // }
        TransformSnapshot newSnap = new TransformSnapshot();
        newSnap.position.x = float.Parse(split[0]);
        newSnap.position.y = float.Parse(split[1]);
        newSnap.position.z = float.Parse(split[2]);
        newSnap.rotation.x = float.Parse(split[3]);
        newSnap.rotation.y = float.Parse(split[4]);
        newSnap.rotation.z = float.Parse(split[5]);
        newSnap.rotation.w = float.Parse(split[6]);
        newSnap.localScale.x = float.Parse(split[7]);
        newSnap.localScale.y = float.Parse(split[8]);
        newSnap.localScale.z = float.Parse(split[9]);
        newSnap.localPosition.x = float.Parse(split[10]);
        newSnap.localPosition.y = float.Parse(split[11]);
        newSnap.localPosition.z = float.Parse(split[12]);
        newSnap.localRotation.x = float.Parse(split[13]);
        newSnap.localRotation.y = float.Parse(split[14]);
        newSnap.localRotation.z = float.Parse(split[15]);
        newSnap.localRotation.w = float.Parse(split[16]);
        newSnap.frame = int.Parse(split[17]);
        newSnap.timeStamp = float.Parse(split[18]);
        return newSnap;
    }

    public string ToCsvRow =>
        $"{position.x}," +
        $"{position.y}," +
        $"{position.z}," +
        $"{rotation.x}," +
        $"{rotation.y}," +
        $"{rotation.z}," +
        $"{rotation.w}," +
        $"{localScale.x}," +
        $"{localScale.y}," +
        $"{localScale.z}," +
        $"{localPosition.x}," +
        $"{localPosition.y}," +
        $"{localPosition.z}," +
        $"{localRotation.x}," +
        $"{localRotation.y}," +
        $"{localRotation.z}," +
        $"{localRotation.w}," +
        $"{frame}," +
        $"{timeStamp}," +
        "";

    public static string CsvHeader => $"position.x," +
                                      $"position.y," +
                                      $"position.z," +
                                      $"rotation.x," +
                                      $"rotation.y," +
                                      $"rotation.z," +
                                      $"rotation.w," +
                                      $"localScale.x," +
                                      $"localScale.y," +
                                      $"localScale.z," +
                                      $"localPosition.x," +
                                      $"localPosition.y," +
                                      $"localPosition.z," +
                                      $"localRotation.x," +
                                      $"localRotation.y," +
                                      $"localRotation.z," +
                                      $"localRotation.w," +
                                      $"frame," +
                                      $"timeStamp," +
                                      "";

    
    public void CopyValuesToTransform(Transform transform) {
        transform.localScale = localScale;
        transform.localPosition = localPosition;
        transform.localRotation = localRotation;
    }
}