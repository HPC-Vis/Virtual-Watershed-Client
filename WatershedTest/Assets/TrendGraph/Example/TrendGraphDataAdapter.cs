using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using VTL;
using VTL.TrendGraph;

public class TrendGraphDataAdapter : MonoBehaviour
{
    SumOfSinesDisturbance dist;
    TrendGraphController trendGraph;

    // Use this for initialization
    void Start()
    {
        trendGraph = GetComponent<TrendGraphController>();

        List<SumOfSinesDisturbance.WaveformComponent> waveforms = new List<SumOfSinesDisturbance.WaveformComponent>();
        waveforms.Add(new SumOfSinesDisturbance.WaveformComponent(10.0f, 0.005f, Random.Range(0f, 1f) * Mathf.PI * 2f));
        waveforms.Add(new SumOfSinesDisturbance.WaveformComponent(5.0f, 0.007f, Random.Range(0f, 1f) * Mathf.PI * 2f));
        waveforms.Add(new SumOfSinesDisturbance.WaveformComponent(3.0f, 0.011f, Random.Range(0f, 1f) * Mathf.PI * 2f));
        waveforms.Add(new SumOfSinesDisturbance.WaveformComponent(2.0f, 0.023f, Random.Range(0f, 1f) * Mathf.PI * 2f));
        dist = new SumOfSinesDisturbance(waveforms, stdev: 0.1f, bias: 100.0f);

        InvokeRepeating("UpdateTrendGraph", 0, 0.5f);
    }

    void UpdateTrendGraph()
    {
        trendGraph.Add(System.DateTime.Now, dist.Sample(Time.time));
    }
}
