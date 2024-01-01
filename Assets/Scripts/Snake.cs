using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public enum Direction
{
    UP = 0, 
    DOWN =  1, 
    LEFT = 2,
    RIGHT = 3,

}

[Serializable]
public struct SnakeObservation
{
    public float[] FoodDistance;  
    public float[] BodyDistance; 
    public float[] WallDistance;  
}


public class Snake : MonoBehaviour
{
    public Color color;
    public int score = 0;

    public Vector2 moveDir;
    public List<Vector2> bodyPos;

    [SerializeField]
    private GameObject foodPrefab;

    [SerializeField]
    private GameObject snakeBitPrefab;

    private Food food;
    private float foodDistance;


    private List<GameObject> body;
    private GameObject head;

    private MLPNetwork brain;
    
    [SerializeField]
    public SnakeObservation currentObservation;


    public bool dead = false;
    public int distanceTraveled = 0;
    public bool movedCloserToFood = false;
    public Direction direction;
    public float output;

    private float initialDistanceToFood;
    private float finalDistanceToFood;
    private int timeSinceLastFruit = 1;
    private bool firstIteration = true;




    void Start()
    {


    }
    public void Initialize(float[] initialWeights)
    {

        brain = gameObject.AddComponent<MLPNetwork>(); 
        brain.InitializeNetwork(initialWeights);
        moveDir = new Vector2(1, 0);
        body = new List<GameObject>();
        bodyPos = new List<Vector2>();
        color = new Color(Random.Range(0F, 1F), Random.Range(0, 1F), Random.Range(0, 1F));
        head = gameObject.transform.GetChild(0).gameObject;
        head.GetComponent<Renderer>().material.color = color;
        for (int i = 0; i < gameObject.transform.GetChild(1).childCount; i++)
        {
            GameObject snakeBit = gameObject.transform.GetChild(1).GetChild(i).gameObject;
            snakeBit.GetComponent<Renderer>().material.color = color;
            body.Add(snakeBit);
            bodyPos.Add(new Vector2(snakeBit.transform.position.x, snakeBit.transform.position.y));
        }
        PlaceRandomly();

        GameObject foodInstance = Instantiate(foodPrefab);
        food = foodInstance.GetComponent<Food>();
        foodInstance.GetComponent<Renderer>().material.color = color;
        food.PlaceRandomly(this);
        initialDistanceToFood = Vector2.Distance(transform.position, food.transform.position);

    }

        public void PlaceRandomly()
    {
        int x = (int)Random.Range(0, GameManager.gridSize.x);
        int y = (int)Random.Range(0, -GameManager.gridSize.y);

        transform.position = new Vector3(x, y, 1);
    }

    public void ChangeDirection(Vector2 _direction)
    {
        moveDir = _direction;
        float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        Quaternion newRotation = Quaternion.Euler(0, 0, angle);
        head.transform.rotation = newRotation;
    }

    public void Grow()
    {
        score++;
        timeSinceLastFruit = 0;
        GameObject newPart = Instantiate(snakeBitPrefab);
        newPart.transform.SetParent(gameObject.transform.GetChild(1));
        newPart.GetComponent<Renderer>().material.color = color;

        body.Add(newPart);
        bodyPos.Add(new Vector2(newPart.transform.position.x, newPart.transform.position.y));
        Show();
    }

    void Die()
    {
        dead = true;
        Hide();
    }
    public void Hide()
    {

        head.GetComponent<Renderer>().enabled = false;
        head.transform.GetChild(0).GetChild(0).GetComponent<Renderer>().enabled = false;
        head.transform.GetChild(0).GetComponent<Renderer>().enabled = false;
        head.transform.GetChild(1).GetChild(0).GetComponent<Renderer>().enabled = false;
        head.transform.GetChild(1).GetComponent<Renderer>().enabled = false;

        for (int i = 0; i < gameObject.transform.GetChild(1).childCount; i++)
        {
            GameObject snakeBit = gameObject.transform.GetChild(1).GetChild(i).gameObject;
            snakeBit.GetComponent<Renderer>().enabled = false;
        }

        food.GetComponent<Renderer>().enabled = false;
    }

    public void Show()
    {

        head.GetComponent<Renderer>().enabled = true;
        head.transform.GetChild(0).GetChild(0).GetComponent<Renderer>().enabled = true;
        head.transform.GetChild(0).GetComponent<Renderer>().enabled = true;
        head.transform.GetChild(1).GetChild(0).GetComponent<Renderer>().enabled = true;
        head.transform.GetChild(1).GetComponent<Renderer>().enabled = true;

        for (int i = 0; i < gameObject.transform.GetChild(1).childCount; i++)
        {
            GameObject snakeBit = gameObject.transform.GetChild(1).GetChild(i).gameObject;
            snakeBit.GetComponent<Renderer>().enabled = true;
        }

        food.GetComponent<Renderer>().enabled = true;
    }
    public void OnDestroy()
    {
        Destroy(gameObject);
        if(food)
            Destroy(food.gameObject);
    }
    public void Move()
    {
        
        distanceTraveled++;
        timeSinceLastFruit++;
        for (int i = body.Count - 1; i > 0; i--)
        {
            body[i].transform.position = body[i - 1].transform.position;
            bodyPos[i] = new Vector2(body[i - 1].transform.position.x, body[i - 1].transform.position.y);
        }
        body[0].transform.position = head.transform.position;
        bodyPos[0] = new Vector2(head.transform.position.x, head.transform.position.y);

        Vector3 newHeadPos = head.transform.position + new Vector3(moveDir.x, moveDir.y, 0);
        if (newHeadPos.x < 0 || newHeadPos.x > GameManager.gridSize.x ||
       newHeadPos.y > 0 || newHeadPos.y <= -GameManager.gridSize.y)
        {
            Die();
            return;
        }
        for (int i = 1; i < body.Count; i++)
        {
            if (newHeadPos == body[i].transform.position)
            {
                Die();
                return;
            }
        }

        head.transform.position = newHeadPos;
        if (new Vector2(food.transform.position.x, food.transform.position.y) == new Vector2(head.transform.position.x, head.transform.position.y))
        {
            food.PlaceRandomly(this);
            Grow();
        }
        else
        {
            float newFoodDistance = Vector2.Distance(new Vector2(food.transform.position.x, food.transform.position.y), new Vector2(head.transform.position.x, head.transform.position.y));
            if (newFoodDistance > foodDistance)
                movedCloserToFood = false;
            else
                movedCloserToFood = true;

            foodDistance = newFoodDistance;
        }
        finalDistanceToFood = Vector2.Distance(transform.position, food.transform.position);
    }

    public float CalculateFitness()
    {
        float distanceChange = initialDistanceToFood - finalDistanceToFood;

        float fitness;

            fitness = Mathf.Floor(distanceTraveled * distanceTraveled) * Mathf.Pow(2, score);



        return fitness;
}

    private bool IsFacingFood()
    {
        
        Vector2 directionToFood = food.transform.position - head.transform.position;
        directionToFood.Normalize(); 
        Vector2 currentDirection = moveDir;    
        float dotProduct = Vector2.Dot(currentDirection, directionToFood);
        return dotProduct > 0.9; 
    }

    private SnakeObservation CalculateObservation()
    {
        Vector2[] directions = new Vector2[]
        {
        Vector2.up, Vector2.down, Vector2.left, Vector2.right, 
        new Vector2(1, 1), new Vector2(1, -1), new Vector2(-1, 1), new Vector2(-1, -1) 
        };

        SnakeObservation observation = new SnakeObservation
        {
            FoodDistance = new float[directions.Length],
            BodyDistance = new float[directions.Length],
            WallDistance = new float[directions.Length],
        };

        for (int i = 0; i < directions.Length; i++)
        {
            Vector2 direction = directions[i];
            // Initialize max distances
            observation.FoodDistance[i] = float.MaxValue;
            observation.BodyDistance[i] = float.MaxValue;
            observation.WallDistance[i] = float.MaxValue;

            Vector2 wallIntersection = FindWallIntersection(direction);
            observation.WallDistance[i] = Vector2.Distance(head.transform.position, wallIntersection);

            foreach (var bodyPart in bodyPos)
            {
                if (IsInDirection(head.transform.position, bodyPart, direction))
                {
                    float distance = Vector2.Distance(head.transform.position, bodyPart);
                    if (distance < observation.BodyDistance[i])
                    {
                        observation.BodyDistance[i] = distance;
                    }
                }
            }

            if (IsInDirection(head.transform.position, food.transform.position, direction))
            {
                observation.FoodDistance[i] = Vector2.Distance(head.transform.position, food.transform.position);
            }
        }

        return observation;
    }

    private bool IsInDirection(Vector2 origin, Vector2 target, Vector2 direction)
    {
        Vector2 toTarget = (target - origin).normalized;
        return Vector2.Dot(toTarget, direction) > 0.9; 
    }

    private Vector2 FindWallIntersection(Vector2 direction)
    {
        
        if (direction.x != 0)
        {
            float x = (direction.x > 0) ? GameManager.gridSize.x : 0;
            return new Vector2(x, head.transform.position.y);
        }
        else 
        {
            float y = (direction.y > 0) ? 0 : -GameManager.gridSize.y;
            return new Vector2(head.transform.position.x, y);
        }
    }



    private List<float> ConvertObservationToInputs(SnakeObservation observation)
    {
        List<float> inputs = new List<float>();


        inputs.AddRange(observation.FoodDistance);
        inputs.AddRange(observation.BodyDistance);
        inputs.AddRange(observation.WallDistance);
        return inputs;
    }

 
    private int InterpretDecision(List<float> decision)
    {
        int actionIndex = Array.IndexOf(decision.ToArray(), decision.Max());
        switch (actionIndex)
        {
            case 0:
                ChangeDirection(Vector2.up);
                break;
            case 1:
                ChangeDirection(Vector2.down);
                break;
            case 2:
                ChangeDirection(Vector2.left);
                break;
            case 3:
                ChangeDirection(Vector2.right);
                break;
        }
        return actionIndex;
    }

    private void DecideNextMove()
    {
        List<float> inputs = ConvertObservationToInputs(currentObservation);
        brain.GenerateOutPut(inputs);

        var decision = brain.GetOutputs();

        InterpretDecision(decision);

    }

    public void Iterate()
    {
        if (dead) return;

        if (firstIteration)
        {
            firstIteration = false;
            Move();

            currentObservation = CalculateObservation();

            DecideNextMove();
            return;
        }
        
        List<float> currentState = ConvertObservationToInputs(currentObservation);

        brain.GenerateOutPut(currentState);
        var decision = brain.GetOutputs();
        int actiontaken = InterpretDecision(decision); 
        Move();
        currentObservation = CalculateObservation();

      
         List<float> newState = ConvertObservationToInputs(currentObservation);
        float reward = CalculateReward(); 


        brain.QLearningUpdate(currentState.ToArray(), newState.ToArray(), reward, actiontaken);
    }

    private float CalculateReward()
    {
        
        const float PENALTY_FOR_DEATH = -10.00f;
        const float PENALTY_FOR_DISTANCING = -0.50f;
        const float PENALTY_FOR_STALLING = -0.005f; 
        const float REWARD_FOR_CLOSER = 0.50f;
        const float REWARD_FOR_SURVIVING = 0.0005f;
        const float REWARD_FOR_FACING_FOOD = 1.0f; 
        const float REWARD_FOR_FOOD = 5.0f; 

        float reward = 0;


        reward += IsFacingFood() ? REWARD_FOR_FACING_FOOD : (REWARD_FOR_FACING_FOOD * -1);

        reward += movedCloserToFood ? REWARD_FOR_CLOSER : PENALTY_FOR_DISTANCING;


        if (timeSinceLastFruit == 0)
        {
            reward += REWARD_FOR_FOOD;
        }

       
        else if (timeSinceLastFruit > score * 2)
        {
            reward += (timeSinceLastFruit * PENALTY_FOR_STALLING);
        }
        reward += distanceTraveled * REWARD_FOR_SURVIVING;

        if (dead)
        {
            reward += PENALTY_FOR_DEATH;
        }

        return reward;
    }

    public float[] GetWeights()
    {
        return brain.GetWeights();
    }
}