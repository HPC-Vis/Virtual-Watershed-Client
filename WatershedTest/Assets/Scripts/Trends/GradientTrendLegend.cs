using UnityEngine;
using System.Collections;

public class GradientTrendLegend : MonoBehaviour 
{

    Color[] colors = new Color[] 
    {
        new Color(241f/255f,  44f/255f,  100f/255f, 1f), // east
        new Color( 71f/255f,  198f/255f, 244f/255f, 1f), // west
        new Color(254f/255f,  205f/255f,  70f/255f, 1f), // north
        new Color( 91f/255f,  201f/255f,  70f/255f, 1f), // south
        new Color(159f/255f,  159f/255f, 159f/255f, 1f), // flat
    };

    private ParticleSystem.Particle[] points;
    private Vector3 origin;

    public float offset = 0.5f;

	// Use this for initialization
	void Start () {
        origin = GetComponent<ParticleSystem>().transform.position;

        points = new ParticleSystem.Particle[5];
        points[0].position = new Vector3(origin.x + offset, origin.y, origin.z); // east
        points[1].position = new Vector3(origin.x - offset, origin.y, origin.z); // west
        points[2].position = new Vector3(origin.x, origin.y + offset, origin.z); // north
        points[3].position = new Vector3(origin.x, origin.y - offset, origin.z); // south
        points[4].position = origin;

        for (int i = 0; i < 5; i++ )
        {
            points[i].color = colors[i];
            points[i].size = 0.4f;
        }
        GetComponent<ParticleSystem>().SetParticles(points, points.Length);

	}

}
