using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using PathCreation.Examples;
using System.IO;
using System.Threading.Tasks;
using SpotifyApi;
using System.Linq;


// Section => the general shape of the track
// Segments => samples of 10 seconds (maybe how many sections there are)
    // build the track parts off of the segments
    // location of notes horizontal
    // maybe use timbre or pitch
// beats => location of notes vertically

public class PathGenerator : MonoBehaviour
{
    [SerializeField]
    PathCreator pathCreator;

    AudioAnalysis audioAnalysis;

    const float zMultiplier = 200f;
    const float xMultiplier = 200f;
    const float yMultiplier = 200f;

    // Start is called before the first frame update
    async Task Start()
    {
        await ReadJson();
        CreatePath();
        CreateRoadMesh();
        SendMessage("BuildBoxes");
    }

    private void CreatePath()
    {
        Vector3[] points = CreatePoints();
        pathCreator.bezierPath = new BezierPath(points, false, PathSpace.xyz);
        pathCreator.bezierPath.GlobalNormalsAngle = 90f;
        pathCreator.TriggerPathUpdate();
    }

    private Vector3[] CreatePoints()
    {
        //var trackPoints = new List<Vector3>
        //{
        //    new Vector3(0, 10f, 0),
        //    new Vector3(2 * xMultiplier, 10f, 4 * zMultiplier),
        //    new Vector3(4 * xMultiplier, 10f, 0 * zMultiplier),
        //    new Vector3(6 * xMultiplier, 100f, 2 * zMultiplier),
        //    new Vector3(6 * xMultiplier, -200f, 6 * zMultiplier),
        //    new Vector3(8 * xMultiplier, -200f, 4 * zMultiplier),
        //    new Vector3(6 * xMultiplier, 200f, 0 * zMultiplier),
        //    new Vector3(4 * xMultiplier, 0, 2 * zMultiplier),
        //    new Vector3(-2 * xMultiplier, 100, 0 * zMultiplier),
        //    new Vector3(-8 * xMultiplier, -200, 5 * zMultiplier),
        //    new Vector3(0 * xMultiplier, 0, 2 * zMultiplier),

        //};


        var trackPoints = new List<Vector3>
        {
            new Vector3(0,0,0)
        };

        var lastSectionX = 0f;
        var lastSectionY = 0f;
        var lastSectionZ = 0f;

        for (int sectionNumber = 1; sectionNumber < audioAnalysis.Sections.Count; sectionNumber++)
        {
            var x = 0f;
            var y = 0f;
            var z = 0f;

            var currentSection = audioAnalysis.Sections[sectionNumber];
            var previousSection = audioAnalysis.Sections[sectionNumber - 1];

            if (currentSection.Loudness > previousSection.Loudness)
            {
                // increase y from previous section point
                y++;
            }
            else
            {
                // decrease y from previous section point
                y--;
            }

            // Get the segments for this section
            var sectionDuration = currentSection.Duration;
            List<Segment> segmentsInSection = GetSegmentsInSection(ref sectionDuration);

            for (int segmentNumber = 0; segmentNumber < segmentsInSection.Count - 1; segmentNumber++)
            {
                var nextSegment = segmentsInSection[segmentNumber + 1];
                var currentSegment = segmentsInSection[segmentNumber];

                var distance = CalculateDistance(
                    currentSegment.Timbre,
                    nextSegment.Timbre);
                if (distance > 50f)
                {
                    // Increase z proportionally from previouss point
                    var trackPoint = new Vector3(x * xMultiplier, y * yMultiplier, z * zMultiplier);
                    trackPoints.Add(trackPoint);
                    z++;
                }
            }
        }
        //trackPoints.ForEach(point => print(point));
        return trackPoints.ToArray();
    }

    private List<Segment> GetSegmentsInSection(ref float sectionDuration)
    {
        var segmentsInSection = new List<Segment>();
        foreach (var segment in audioAnalysis.Segments)
        {
            if (sectionDuration - segment.Duration > -.2f)
            {
                sectionDuration -= segment.Duration;
                segmentsInSection.Add(segment);

            }
        }

        return segmentsInSection;
    }

    private float CalculateDistance(List<float> vector1, List<float> vector2)
    {
        var total = 0f;
        for (int i = 0; i < vector1.Count; i++)
        {
            total += Mathf.Pow(vector1[i] - vector2[i], 2);
        }

        var distance = Mathf.Sqrt(total);
        return distance ;
    }

    // TODO: Determine if this is necessary
    private void CalculateBumps(float bps, List<Vector3> trackPoints)
    {
        var distance = 21f;
        var numberBeats = distance / bps;
        var direction = 1;
        for (int i = 0; i < Mathf.FloorToInt(numberBeats); i++)
        {
            direction *= -1;
            var newPoint = new Vector3(0, direction * .5f, i * 30);
            trackPoints.Add(newPoint);
            print(newPoint);
        }
    }

    private float CalcuateX(Section currentSongSection, Section previousSongSection)
    {
        return (currentSongSection.Key - previousSongSection.Key) * xMultiplier;
    }

    private float CalculateY(Section currentSongSection, Section previousSongSection)
    {
        return currentSongSection.Loudness;
    }

    private float CalcuateZ(Section currentSongSection, Section previousSongSection)
    {
        return (currentSongSection.Start + currentSongSection.Duration) * zMultiplier;
    }

    private void CreateRoadMesh()
    {
        var roadMesh = pathCreator.GetComponent<RoadMeshCreator>();
        roadMesh.roadWidth = 25f;
        roadMesh.TriggerUpdate();
    }

    private async Task ReadJson()
    {
        string path = "Assets/Resources/audio_output.json";

        //Read the text from directly from the test.txt file
        try
        {
            StreamReader reader = new StreamReader(path);
            var json = await reader.ReadToEndAsync();
            this.audioAnalysis = SpotifyApi.AudioAnalysis.FromJson(json);
        }
        catch (System.Exception ex)
        {
            print(ex.Message);
            print(ex);
        }        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
