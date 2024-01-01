using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public static class ArrayUtils  
{
    public static T[] SubArray<T>(this T[] data, int index, int length)
    {
        T[] result = new T[length];
        Array.Copy(data, index, result, 0, length);
        return result;
    }
}
public class MLPNetwork : MonoBehaviour
{
    public int numInputPerceptrons = 24;
    public int numHiddenPerceptrons = 30;
    public int numOutputPerceptrons = 4;

    private float discountFactor = 0.9f;

    private List<Perceptron> inputPerceptrons;
    private List<Perceptron> hiddenPerceptrons;
    private List<Perceptron> outputPerceptrons;

    public void InitializeNetwork(float[] initialWeights = null)
    {
        inputPerceptrons = new List<Perceptron>();
        hiddenPerceptrons = new List<Perceptron>();
        outputPerceptrons = new List<Perceptron>();

        for (int i = 0; i < numInputPerceptrons; i++)
        {
            inputPerceptrons.Add(new Perceptron());
        }

        for (int i = 0; i < numHiddenPerceptrons; i++)
        {
            Perceptron hidden = new Perceptron();
            hidden.InitializeInputs(inputPerceptrons);
            hiddenPerceptrons.Add(hidden);
        }

        for (int i = 0; i < numOutputPerceptrons; i++)
        {
            Perceptron output = new Perceptron();
            output.InitializeInputs(hiddenPerceptrons);
            outputPerceptrons.Add(output);
        }

        if (initialWeights != null)
        {
            SetWeights(initialWeights);
        }
    }


    void Awake()
    {
        
    }



    public void GenerateOutPut(List<float> inputList)
    {
        for (int i = 0; i < inputPerceptrons.Count; i++)
        {
            inputPerceptrons[i].SetState(inputList[i]);
        }

        foreach (Perceptron perceptron in hiddenPerceptrons)
            perceptron.FeedForward();

        foreach (Perceptron perceptron in outputPerceptrons)
            perceptron.FeedForward();

    }

    public float[] Predict(float[] inputs)
    {
        float[] originalStates = GetStates();

        for (int i = 0; i < numInputPerceptrons; i++)
            inputPerceptrons[i].SetState(inputs[i]);

        foreach (var perceptron in hiddenPerceptrons)
            perceptron.FeedForward();
        foreach (var perceptron in outputPerceptrons)
            perceptron.FeedForward();
        float[] outputStates = outputPerceptrons.Select(p => p.GetState()).ToArray();
       
        SetStates(originalStates);

        return outputStates;
    }

    public void QLearningUpdate(float[] currentState, float[] newState, float reward, int actionTaken)
    {
        float[] currentQValues = Predict(currentState);
        float[] futureQValues = Predict(newState);
        float maxFutureQ = futureQValues.Max();
        float tdError = (reward + discountFactor * maxFutureQ) - currentQValues[actionTaken];

        outputPerceptrons[actionTaken].AdjustWeight(tdError);

        for (int i = 0; i < hiddenPerceptrons.Count; i++)
        {
            Perceptron hiddenPerceptron = hiddenPerceptrons[i];
            float errorContribution = hiddenPerceptron.GetIncomingWeight(outputPerceptrons[actionTaken]) * tdError;
            hiddenPerceptron.AdjustWeight(errorContribution);
        }
    }




    public List<float> GetOutputs()
    {
        List<float> output = new List<float>();

        foreach (Perceptron perceptron in outputPerceptrons)
            output.Add(perceptron.GetState());

        return output;
    }
    public float[] GetWeights()
    {
        List<float> weights = new List<float>();

        foreach (var perceptron in inputPerceptrons)
        {
            weights.AddRange(perceptron.GetWeights());
        }

        foreach (var perceptron in hiddenPerceptrons)
        {
            weights.AddRange(perceptron.GetWeights());
        }


        foreach (var perceptron in outputPerceptrons)
        {
            weights.AddRange(perceptron.GetWeights());
        }

        return weights.ToArray();
    }

    public float[] GetStates()
    {
        List<float> states = new List<float>();

        foreach (var perceptron in inputPerceptrons)
        {
            states.Add(perceptron.GetState());
        }

        foreach (var perceptron in hiddenPerceptrons)
        {
            states.Add(perceptron.GetState());
        }

        foreach (var perceptron in outputPerceptrons)
        {
            states.Add(perceptron.GetState());
        }

        return states.ToArray();
    }



    public void SetWeights(float[] newWeights)
    {
        int currentIndex = 0;

        foreach (var perceptron in inputPerceptrons)
        {
            int count = perceptron.GetWeights().Length;
            perceptron.SetWeights(newWeights.SubArray(currentIndex, count));
            currentIndex += count;
        }

        foreach (var perceptron in hiddenPerceptrons)
        {
            int count = perceptron.GetWeights().Length;
            perceptron.SetWeights(newWeights.SubArray(currentIndex, count));
            currentIndex += count;
        }

        foreach (var perceptron in outputPerceptrons)
        {
            int count = perceptron.GetWeights().Length;
            perceptron.SetWeights(newWeights.SubArray(currentIndex, count));
            currentIndex += count;
        }
    }

    public void SetStates(float[] states)
    {
        int index = 0;
  
        foreach (var perceptron in inputPerceptrons)
        {
            perceptron.SetState(states[index++]);
        }

        foreach (var perceptron in hiddenPerceptrons)
        {
            perceptron.SetState(states[index++]);
        }

        foreach (var perceptron in outputPerceptrons)
        {
            perceptron.SetState(states[index++]);
        }
    }
}
