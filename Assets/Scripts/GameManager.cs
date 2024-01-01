using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    List<Snake>snakes = new List<Snake>();

    /*[SerializeField]
    //for testing snake
    Snake playerTest;*/
    [SerializeField]
    int populationSize = 20;

    [SerializeField]
    GameObject snakePrefab ;

    [Serializable]
    public class SnakeSaveData
    {
        public float[] bestWeights;
        public int highestScore;
        public int generation;
    }

    [SerializeField]
    TMP_Text highestScoreText;
    [SerializeField]
    TMP_Text generationText;

    public int PreviousHighestScore = 0;
    public int HighestScore = 0;
    public int Generation = 1;
    public float snakeSpeed = 1f;
    public float timeLimitInSeconds = 1f;

    float timer = 0f;
    float limitTimer = 0f;
    public static Vector2 gridSize = new Vector2(30,30);
    private float mutationRate = 0.01f;
    private float mutationAmount = 0.01f;
    private List<float[]> newWeights;

    // Start is called before the first frame update
    void Start()
    {
        LoadPopulationData();
    }
    void InitializePopulation()
    {
        for (int i = 0; i < populationSize; i++)
        {
            GameObject newSnakeGameObject = Instantiate(snakePrefab);
            Snake newSnake = newSnakeGameObject.GetComponent<Snake>(); 
            newSnake.Initialize(null); // start with random weights
            snakes.Add(newSnake);
        }
    }

    // Update is called once per frame
    void Update()
    {
        limitTimer += Time.deltaTime;
        timer += Time.deltaTime;
        //getInput();//for testing snake
        if (timer < 1/snakeSpeed) return;
        timer = 0f;
        if (AllSnakesDead() || limitTimer > timeLimitInSeconds)
        {
            /*if(HighestScore>= PreviousHighestScore)
            {*/
                SavePopulationData(snakes, HighestScore, Generation);
                EvolvePopulation();
                PreviousHighestScore = HighestScore;

            /*}
            else 
            {
                ResetSnakes();
            }*/
            limitTimer = 0f;
            HighestScore = 0;
            Generation++;

        }
        else
        {
            foreach (var snake in snakes)
            {
                snake.Iterate();
                if (snake.score < 5)
                    snake.Hide();

                if (snake.score>HighestScore) 
                    HighestScore = snake.score;
            }
        }

        //playerTest.Move();//for testing snake
        UpdateUI();
    }


    void UpdateUI()
    {
        
        highestScoreText.text = "High Score of this Gen: " + HighestScore;
        generationText.text = "Generation: " + Generation;
    }
    void EvolvePopulation()
    {

        int numberToSelect = Mathf.Max(2, snakes.Count / 5);//best 20%

        List<Snake> selectedForBreeding = SelectBestSnakes(snakes, numberToSelect);


        newWeights = SelectAndBreed(selectedForBreeding);

        ResetSnakes();


    }
    private void ResetSnakes()
    {
        for (int i = 0; i < snakes.Count; i++)
        {
            Destroy(snakes[i]);
            GameObject newSnakeGameObject = Instantiate(snakePrefab);
            Snake newSnake = newSnakeGameObject.GetComponent<Snake>();
            newSnake.Initialize(newWeights[i % newWeights.Count]);
            snakes[i] = newSnake;
        }
    }
    List<Snake> SelectBestSnakes(List<Snake> allSnakes, int numberToSelect)
    {
       
        List<Snake> sortedSnakes = allSnakes.OrderByDescending(snake => snake.CalculateFitness()).ToList();
        return sortedSnakes.Take(numberToSelect).ToList();
    }

    List<float[]> SelectAndBreed(List<Snake> selectedForBreeding)
    {
        List<float[]> newWeights = new List<float[]>();

        
        for (int i = 0; i < selectedForBreeding.Count - 1; i += 2)
        {
            Snake parent1 = selectedForBreeding[i];
            Snake parent2 = selectedForBreeding[i + 1];

            
            float[] parent1Weights = parent1.GetWeights();
            float[] parent2Weights = parent2.GetWeights();

            // Crossover: Create a child by combining parts of each parent's weights
            float[] childWeights = new float[parent1Weights.Length];
            int crossoverPoint = UnityEngine.Random.Range(0, childWeights.Length); 
            for (int j = 0; j < childWeights.Length; j++)
            {
                childWeights[j] = j < crossoverPoint ? parent1Weights[j] : parent2Weights[j];
            }

            // Mutation: Slightly randomize the child's weights to introduce variation
            for (int j = 0; j < childWeights.Length; j++)
            {
                if (UnityEngine.Random.Range(0f, 1f) < mutationRate)
                {
                    childWeights[j] += UnityEngine.Random.Range(-mutationAmount, mutationAmount);
                }
            }

            newWeights.Add(childWeights);
        }

        return newWeights;
    }

    private bool AllSnakesDead()
    {
        foreach (var snake in snakes)
        {
            if(!snake.dead) return false;
        }
        return true;
    }

    public void SavePopulationData(List<Snake> snakes, int highestScore, int generation)
    {

        Snake bestSnake = SelectBestSnakes(snakes, 1).First();
        SnakeSaveData saveData = new SnakeSaveData
        {
            bestWeights =  bestSnake.GetWeights() ,
            highestScore = bestSnake.score,  
            generation = Generation
        };


        string json = JsonUtility.ToJson(saveData);
        try
        {
            string path = Application.dataPath + "/Save/savefile.json";
            System.IO.File.WriteAllText(path, json);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to write save file: " + e.Message);
        }

    }
    void InitializePopulationFromSaveData(SnakeSaveData saveData)
    {

        for (int i = 0; i < populationSize; i++)
        {

            GameObject snakeObj = Instantiate(snakePrefab);  
            Snake newSnake = snakeObj.GetComponent<Snake>();
            newSnake.Initialize(saveData.bestWeights);
            snakes.Add(newSnake);
        }
    }

    public void LoadPopulationData()
    {
        string path = Application.dataPath + "/Save/savefile.json";
        if (System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            SnakeSaveData saveData = JsonUtility.FromJson<SnakeSaveData>(json);

            InitializePopulationFromSaveData(saveData);
            HighestScore = saveData.highestScore;
            Generation = saveData.generation;
        }
        else
        {
            
            InitializePopulation();
        }
    }

    /*void getInput()
    {
        if ( UnityEngine.Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (playerTest.moveDir != new Vector2(0, -1)) 
                playerTest.ChangeDirection(new Vector2(0, 1)); 
        }
        else if (UnityEngine.Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (playerTest.moveDir != new Vector2(-1, 0))
                playerTest.ChangeDirection(new Vector2(1, 0));
        }
       
        else if (UnityEngine.Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (playerTest.moveDir != new Vector2(1, 0))
                playerTest.ChangeDirection(new Vector2(-1, 0));
        }
        else if (UnityEngine.Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (playerTest.moveDir != new Vector2(0, 1))
                playerTest.ChangeDirection(new Vector2(0, -1));
        }
    }*/
}

