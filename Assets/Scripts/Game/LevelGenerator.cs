using System;
using System.Collections;
using System.Collections.Generic;
using Nakama.TinyJson;
using UnityEngine;
using UnityEngine.U2D;

public class LevelGenerator : MonoBehaviour {

    public int LevelLength = 200; //x ekseninde uzunluk
    public int PointFreq = 5;  //Hangi sıklıkla nokta oluşturulsun
    public float Smoothness = 5f; //Yükseltiler arasındaki geçişin yumuşaklığı
    public float Amplitude = 15f; //Çukurların ve tepelerin derinlik ve yüksekliği
    public float Randomness = 100f; //Üretilen zeminler birbirinden ne kadar farklı olsun

    public GameObject[] ObstaclePrefabs;
    public SpriteShapeController SpriteShapeController;

    private Spline SplinePoints;
    public bool Ready = false;

    void Awake() {
        SplinePoints = SpriteShapeController.spline;
        //CreateGround(); //zemin üretimini test etmek için
        StartCoroutine(Initialize());
    }

    private IEnumerator Initialize() {
        yield return new WaitUntil(() => GlobalModel.MyCharacter != -1);
        //bölümü civciv üretsin
        if (GlobalModel.MyCharacter == GlobalModel.CHARACTER_CHICK) {
            CreateGround();
            GameController.SendState(GameController.ACTION_LEVEL_GENERATED, GlobalModel.GeneratedSplinePoints.ToJson());
        }
        else {
            yield return new WaitUntil(() => GlobalModel.GeneratedSplinePoints != null);
            SetReceivedGround(GlobalModel.GeneratedSplinePoints);
        }
        Ready = true;
        GlobalModel.GeneratedSplinePoints = null;
    }

    //civciv
    private void CreateGround() {
        GlobalModel.GeneratedSplinePoints = new List<(float, float)>();
        Vector3 pointPosition = new Vector3();
        int i;
        float xpos = 0, ypos;
        float seed = (float)new System.Random().NextDouble() * Randomness;
        for (i = 4; i < LevelLength / PointFreq; i++) {
            xpos = i * PointFreq;
            ypos = Mathf.PerlinNoise((seed + i) / Smoothness, 0f) * Amplitude - Amplitude / 5;
            pointPosition.Set(xpos, ypos, 0);
            Debug.Log("point" + SplinePoints.GetPointCount() + ": " + xpos + ", " + ypos);
            SplinePoints.InsertPointAt(i, pointPosition);
            SplinePoints.SetTangentMode(i, ShapeTangentMode.Continuous);
            GlobalModel.GeneratedSplinePoints.Add((xpos, ypos));
        }
        SplinePoints.InsertPointAt(i, new Vector3(xpos, -6));
        GlobalModel.GeneratedSplinePoints.Add((xpos, -2));
        SpriteShapeController.BakeCollider();
    }

    //kedi
    private void SetReceivedGround(List<(float, float)> points) {
        Vector3 pointPosition = new Vector3();
        for (int i = 0; i < points.Count; i++) {
            pointPosition.Set(points[i].Item1, points[i].Item2, 0);
            SplinePoints.InsertPointAt(i + 4, pointPosition);
            SplinePoints.SetTangentMode(i, ShapeTangentMode.Continuous);
        }
        SpriteShapeController.BakeCollider();
    }
}
