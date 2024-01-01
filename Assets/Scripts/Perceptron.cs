using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Windows;

class Input
{
    public Perceptron inputPerceptron;
    public float weight;
}

public class Perceptron
{

    List<Input> inputs;

    public static float initialGain = 0.1f;
    float gain;
    float state { get; set; }
    public float bias;

    float error;
    public Perceptron()
    {
        inputs = new List<Input>();
        bias = UnityEngine.Random.Range(-0.1f, 0.1f);
        gain = initialGain;
    }
    public void FeedForward()
    {
        float sum = bias;
        foreach (Input input in inputs)
        {
            sum += input.inputPerceptron.GetState() * input.weight;
        }
        state = Threshold(sum);
    }


    public void InitializeInputs(List<Perceptron> inputPerceptrons)
    {
        foreach (Perceptron p in inputPerceptrons)
        {
            
            Input newInput = new Input();
            newInput.inputPerceptron = p;
            newInput.weight = UnityEngine.Random.Range(-0.1f, 0.1f);
            inputs.Add(newInput);
        }
    }

    public float GetIncomingWeight(Perceptron perceptron)
    {
        foreach (Input input in inputs)
        {
            if (input.inputPerceptron == perceptron)
                return input.weight;
        }
        return 0;
    }

    public float GetState()
    {
        return state;
    }
    public float GetError()
    {
        return error;
    }

    public float GetGain()
    {
        return gain;
    }


    public void SetState(float _state)
    {
        state = _state;
    }
    public void SetGain(float _gain)
    {
        gain = _gain;
    }



    public float[] GetWeights()
    {
        List<float> weights = new List<float>();
        foreach (Input input in inputs)
        {
            weights.Add(input.weight);  
        }
        return weights.ToArray();  
    }


    public void SetWeights(float[] newWeights)
    {
        if (newWeights.Length != inputs.Count)
        {
            throw new System.Exception("\n num of weights =/= num of inputs \n");
        }

        for (int i = 0; i < newWeights.Length; i++)
        {
            inputs[i].weight = newWeights[i];  
        }
    }

    public void AdjustWeight(float error)
    {
        // TD-Error is calculated as: reward + discountFactor * maxFutureQ - currentQ
        foreach (Input input in inputs)
        {
            float deltaWeight = gain * error * input.inputPerceptron.GetState();
            input.weight += deltaWeight;
        }
        bias += gain * error; // Adjust bias with TD-Error
    }

    float Threshold(float input)
    {
        //return 1 / (1 + (float)Math.Exp(-input));
        float result =(float)Math.Tanh(input);
        return result;
    }


}
