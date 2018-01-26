using UnityEngine;

public class StraightAstroidSpawner: AstroidSpawner
{
    private float timeSample;

    public override void StartSpawn()
    {
        base.StartSpawn();
        timeSample = Time.deltaTime;

    }

    protected override void Spawn()
    {
        base.Spawn();
        if (Time.time - timeSample > GetRandomDelayBetweenSecondsInRange())
        {
            var astroid =  base.GetReadyAstroid();
            astroid.InitlialInitliaze(base.GetRandomPosInRange(), GetRandomSpeedInRange());

        }
    }

    protected override void Update()
    {
        base.Update();
        if(Input.GetKeyDown(KeyCode.X))
            StartSpawn();
    }
}