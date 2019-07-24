using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class BlockCreator : MonoBehaviour
{

    [SerializeField]
    PathCreator pathCreator;

    [SerializeField]
    GameObject positiveBoxPrefab;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BuildBoxes()
    {
        var path = pathCreator.path;
        for (int i = 0; i < 100; i++)
        {
            var time = i / 100f;
            var point = path.GetPointAtTime(time);
            var position = new Vector3(point.x, point.y + 6f, point.z);
            var box = Instantiate(positiveBoxPrefab, position, Quaternion.identity);
            box.transform.parent = gameObject.transform;
        }
    }
}
