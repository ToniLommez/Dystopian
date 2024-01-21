using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;

public class NeuralNetwork
{
    public int[] layers;
    public float[][] neurons;
    public float[][][] weights;

    public NeuralNetwork(int[] layers)
    {
        this.layers = new int[layers.Length];
        for (int i = 0; i < layers.Length; i++)
            this.layers[i] = layers[i];

        InitNeurons();
        InitWeights();
    }

    private void InitNeurons()
    {
        List<float[]> neuronsList = new List<float[]>();
        for (int i = 0; i < layers.Length; i++)
        {
            neuronsList.Add(new float[layers[i]]);
        }
        neurons = neuronsList.ToArray();
    }

    private void InitWeights()
    {
        List<float[][]> weightsList = new List<float[][]>();
        for (int i = 1; i < layers.Length; i++)
        {
            List<float[]> layerWeightsList = new List<float[]>();
            int neuronsInPreviousLayer = layers[i - 1];

            for (int j = 0; j < layers[i]; j++)
            {
                float[] neuronWeights = new float[neuronsInPreviousLayer];

                for (int k = 0; k < neuronsInPreviousLayer; k++)
                    neuronWeights[k] = UnityEngine.Random.Range(-0.5f, 0.5f);

                layerWeightsList.Add(neuronWeights);
            }
            weightsList.Add(layerWeightsList.ToArray());
        }
        weights = weightsList.ToArray();
    }

    public float[] FeedForward(float[] inputs)
    {
        for (int i = 0; i < inputs.Length; i++)
            neurons[0][i] = inputs[i];

        for (int i = 1; i < layers.Length; i++)
        {
            int layer = i - 1;
            for (int j = 0; j < layers[i]; j++)
            {
                float value = 0f;
                for (int k = 0; k < layers[layer]; k++)
                    value += weights[i - 1][j][k] * neurons[layer][k];

                neurons[i][j] = (float)Math.Tanh(value);
            }
        }

        return neurons[neurons.Length - 1];
    }

    public void Mutate(float mutationRate, float mutationStrength)
    {
        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    if (UnityEngine.Random.Range(0f, 1f) < mutationRate)
                    {
                        weights[i][j][k] += UnityEngine.Random.Range(-0.5f, 0.5f) * mutationStrength;
                    }
                }
            }
        }
    }

    public NeuralNetwork Crossover(NeuralNetwork partner)
    {
        // Assumindo que ambas as redes têm a mesma estrutura
        NeuralNetwork child = new NeuralNetwork(this.layers);

        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    child.weights[i][j][k] = UnityEngine.Random.Range(0f, 1f) < 0.5 ? this.weights[i][j][k] : partner.weights[i][j][k];
                }
            }
        }

        return child;
    }

    public void PrintNetwork()
    {
        Debug.Log("Neural Network Printout:");
        for (int i = 0; i < weights.Length; i++)
        {
            Debug.Log($"Layer {i}:");
            for (int j = 0; j < weights[i].Length; j++)
            {
                string weightsStr = "";
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    weightsStr += weights[i][j][k].ToString("F3") + (k == weights[i][j].Length - 1 ? "" : ", ");
                }
                Debug.Log($"  Neuron {j} Weights: [{weightsStr}]");
            }
        }
    }

    public string Serialize()
    {
        return JsonConvert.SerializeObject(this);
    }

    public void Save(string filePath)
    {
        string data = Serialize();
        File.WriteAllText(filePath, data);
    }

    public static NeuralNetwork Deserialize(string data)
    {
        return JsonConvert.DeserializeObject<NeuralNetwork>(data);
    }

    public static NeuralNetwork Load(string filePath)
    {
        if (File.Exists(filePath))
        {
            string data = File.ReadAllText(filePath);
            return Deserialize(data);
        }
        return null;
    }
}