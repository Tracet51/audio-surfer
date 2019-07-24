using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositiveBox : MonoBehaviour
{
    [SerializeField]
    GameObject CollectPositiveFX;

    [SerializeField]
    Transform ParentGameObject;

    [SerializeField]
    int ScorePerHit = 100;

    ScoreBoard ScoreBoard;


    // Start is called before the first frame update
    private void Start()
    {
        AddBoxColider();
        ScoreBoard = FindObjectOfType<ScoreBoard>();
    }

    private void AddBoxColider()
    {
        Collider boxCollider = this.gameObject.AddComponent<BoxCollider>();
        boxCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider collider)
    {
        //var fx = Instantiate(CollectPositiveFX, transform.position, Quaternion.identity);
        //fx.transform.parent = ParentGameObject;
        Destroy(gameObject);

        ScoreBoard.ScoreHit(ScorePerHit);
    }

    // Update is called once per frame
    private void Update()
    {

    }
}
