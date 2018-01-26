using UnityEngine;


public class StraightAstroidSpawner : AstroidSpawner
{
    private float timeSample;
    private Vector3 instantiatePos;


    public override void StartSpawn()
    {
        base.StartSpawn();
        if(isSpawning)return;
        timeSample = Time.deltaTime;
        instantiatePos = GetRandomPosInRange();

    }

    protected override bool Spawn()
    {
        if (!base.Spawn()) return false;
        if (Time.time - timeSample > GetRandomDelayBetweenSecondsInRange())
        {
            timeSample = Time.time;
            var astroid = GetReadyAstroid();
            astroid.Instance.transform.position = instantiatePos;
            astroid.Instance.SetActive(true);
            astroid.InitlialInitliaze(Vector3.left, GetRandomSpeedInRange()); //todo get random Direction

        }
        return true;
    }

    protected override void Start()
    {
        base.Start();
        instantiatePos = GetRandomPosInRange();
    }

    protected override void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.X))
            StartSpawn();
    }


}
