using UnityEngine;

[SerializeField]
public class DiagnalAstroidSpawner : AstroidSpawner
{
    public enum DiagnalDirection { TopToBottom,BottomToTop}



    private float timeSample;
    [SerializeField] private DiagnalDirection SpawnDirection;
    private float stepHeight;

    public override void StartSpawn()
    {
        base.StartSpawn();
        timeSample = Time.deltaTime;
        stepHeight = _spawnCount / (base._distanceFromPos.y);
    }

    protected override bool Spawn()
    {
        if (!base.Spawn()) return false;
        if (Time.time - timeSample > GetRandomDelayBetweenSecondsInRange())
        {
            timeSample = Time.time;
            var astroid = GetReadyAstroid();
            var transform = astroid.Instance.transform;


            if (SpawnDirection == DiagnalDirection.TopToBottom)
                transform.position = new Vector3(_spawnPositionTransform.position.x,
                    _spawnPositionTransform.position.y+ _distanceFromPos.y - stepHeight * (_currentSpawnCount - 1),
                    _spawnPositionTransform.position.z);


            if (SpawnDirection == DiagnalDirection.BottomToTop)
                transform.position = new Vector3(_spawnPositionTransform.position.x,
                    _spawnPositionTransform.position.y + stepHeight * (_currentSpawnCount - 1),
                    _spawnPositionTransform.position.z);



            astroid.Instance.SetActive(true);
            astroid.InitlialInitliaze(Vector3.left, GetRandomSpeedInRange()); //todo get random Direction

        }
        return true;
    }

    protected override void Update()
    {
        base.Update();


    }


}