using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using VTL;
using VTL.TrendGraph;
using VTL.SimTimeControls;

public class TrendGraphDataAdapter : MonoBehaviour
{
    public TrendGraphController trendGraph;
    public TimeSlider timeSlider;

    SumOfSinesDisturbance dist;

    // Use this for initialization
    void Start()
    {

        List<SumOfSinesDisturbance.WaveformComponent> waveforms = new List<SumOfSinesDisturbance.WaveformComponent>();
        waveforms.Add(new SumOfSinesDisturbance.WaveformComponent(10.0f, 0.00005f, Random.Range(0f, 1f) * Mathf.PI * 2f));
        waveforms.Add(new SumOfSinesDisturbance.WaveformComponent(5.0f,  0.00007f, Random.Range(0f, 1f) * Mathf.PI * 2f));
        waveforms.Add(new SumOfSinesDisturbance.WaveformComponent(3.0f,  0.00011f, Random.Range(0f, 1f) * Mathf.PI * 2f));
        waveforms.Add(new SumOfSinesDisturbance.WaveformComponent(2.0f,  0.00023f, Random.Range(0f, 1f) * Mathf.PI * 2f));
        dist = new SumOfSinesDisturbance(waveforms, stdev: 0.1f, bias: 100.0f);

    }

    void Update()
    {
        var time = timeSlider.SimTime;
        float t = (float)(time - timeSlider.StartTime).TotalSeconds;
        trendGraph.Add(time, dist.Sample(t));
    }
}
