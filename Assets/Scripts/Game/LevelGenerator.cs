using System;
using System.Collections;
using System.Collections.Generic;
using Nakama.TinyJson;
using UnityEngine;
using UnityEngine.U2D;

public class LevelGenerator : MonoBehaviour {

    public bool isTest = false;
    public int LevelLength = 400; //x ekseninde uzunluk
    public int PointFreq = 5;  //Hangi sıklıkla nokta oluşturulsun
    public float Smoothness = 5f; //Yükseltiler arasındaki geçişin yumuşaklığı
    public float Amplitude = 15f; //Çukurların ve tepelerin derinlik ve yüksekliği
    public float Randomness = 100f; //Üretilen zeminler birbirinden ne kadar farklı olsun

    public int ObstacleCount = 15; //Engel sayısı
    public GameObject[] ObstaclePrefabs;
    public SpriteShapeController SpriteShapeController;

    private Spline SplinePoints;
    public bool Ready = false;

    private static readonly System.Random Random = new System.Random();

    void Awake() {
        SplinePoints = SpriteShapeController.spline;
        if (isTest)
            TestLevelGeneration();
        else
            StartCoroutine(Initialize());
    }

    private void TestLevelGeneration() {
        CreateGround();
        CreateObstacles();
    }

    private IEnumerator Initialize() {
        yield return new WaitUntil(() => GlobalModel.MyCharacter != -1);
        //bölümü civciv üretsin
        if (GlobalModel.MyCharacter == GlobalModel.CHARACTER_CHICK) {
            CreateGround();
            GameController.SendState(GameController.ACTION_GROUND_GENERATED, GlobalModel.GeneratedSplinePoints.ToJson());
            CreateObstacles();
            GameController.SendState(GameController.ACTION_OBSTACLES_GENERATED, GlobalModel.GeneratedObstaclePositions.ToJson());
        }
        else {
            yield return new WaitUntil(() => GlobalModel.GeneratedSplinePoints != null);
            SetReceivedGround(GlobalModel.GeneratedSplinePoints);
            yield return new WaitUntil(() => GlobalModel.GeneratedObstaclePositions != null);
            SetReceivedObstacles(GlobalModel.GeneratedObstaclePositions);
        }
        Ready = true;
    }

    #region civciv
    private void CreateGround() {
        GlobalModel.GeneratedSplinePoints = new List<(float, float)>();
        Vector2 pointPosition = new Vector2();
        int i;
        float xpos = 0, ypos;
        float seed = (float)Random.NextDouble() * Randomness;
        for (i = 4; i < LevelLength / PointFreq; i++) {
            xpos = i * PointFreq;
            ypos = Mathf.PerlinNoise((seed + i) / Smoothness, 0f) * Amplitude - Amplitude / 5;
            pointPosition.Set(xpos, ypos);
            if (isTest)
                Debug.Log("point" + SplinePoints.GetPointCount() + ": " + xpos + ", " + ypos);
            SplinePoints.InsertPointAt(i, pointPosition);
            SplinePoints.SetTangentMode(i, ShapeTangentMode.Continuous);
            GlobalModel.GeneratedSplinePoints.Add((xpos, ypos));
        }
        SplinePoints.InsertPointAt(i++, new Vector2(xpos, 30));
        SplinePoints.InsertPointAt(i++, new Vector2(xpos + 30, 30));
        SplinePoints.InsertPointAt(i, new Vector2(xpos + 30, -10));
        GlobalModel.GeneratedSplinePoints.Add((xpos, 30));
        GlobalModel.GeneratedSplinePoints.Add((xpos + 30, 30));
        GlobalModel.GeneratedSplinePoints.Add((xpos + 30, -10));
        SpriteShapeController.BakeCollider();
    }

    private void CreateObstacles() {
        Quaternion rotation = new Quaternion(0, 0, 0, 0);
        Vector2 position = new Vector2();
        GlobalModel.GeneratedObstaclePositions = new List<(float, int)>();
        for (int i = 2; i < ObstacleCount; i++) {
            float xpos = (float)(Random.NextDouble() * 10 + (LevelLength / ObstacleCount * i - 10));
            int obsType = Random.Next(ObstaclePrefabs.Length);
            position.Set(xpos, 10);
            Instantiate(ObstaclePrefabs[obsType], position, rotation);
            GlobalModel.GeneratedObstaclePositions.Add((xpos, obsType));
        }
    }
    #endregion

    #region kedi
    private void SetReceivedGround(List<(float, float)> points) {
        Vector2 pointPosition = new Vector2();
        for (int i = 0; i < points.Count; i++) {
            pointPosition.Set(points[i].Item1, points[i].Item2);
            SplinePoints.InsertPointAt(i + 4, pointPosition);
            SplinePoints.SetTangentMode(i + 4, ShapeTangentMode.Continuous);
        }
        SpriteShapeController.BakeCollider();
    }

    private void SetReceivedObstacles(List<(float, int)> obstacles) {
        Quaternion rotation = new Quaternion(0, 0, 0, 0);
        Vector2 position = new Vector2();
        for (int i = 0; i < obstacles.Count; i++) {
            float xpos = obstacles[i].Item1;
            int type = obstacles[i].Item2;
            position.Set(xpos, 10);
            Instantiate(ObstaclePrefabs[type], position, rotation);
        }
    }
    #endregion
}
