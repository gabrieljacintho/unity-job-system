# Unity Job System

## Links
[Documentation](https://docs.unity3d.com/Manual/job-system.html)

[Tutorial](https://www.youtube.com/watch?v=C56bbgtPr_w)

## How to create a Job?
[Testing.cs](https://github.com/gabrieljacintho/unity-job-system/blob/47beb9463b4f14416fc559b5b29867dd65025520/Assets/Scripts/Testing.cs)
```
public struct ReallyToughJob : IJob
{
    public void Execute()
    {
        float value = 0f;
        for (int i = 0; i < 50000; i++)
        {
            value += math.exp10(math.sqrt(value));
        }
    }
}
```
```
public struct ReallyToughParallelJob : IJobParallelFor
{
    public NativeArray<float3> PositionArray;
    public NativeArray<float> MoveYArray;
    public float DeltaTime;

    public void Execute(int index)
    {
        PositionArray[index] += new float3(0f, MoveYArray[index] * DeltaTime, 0f);

        if (PositionArray[index].y > 5f)
        {
            MoveYArray[index] = -math.abs(MoveYArray[index]);
        }
        else if (PositionArray[index].y < -5f)
        {
            MoveYArray[index] = math.abs(MoveYArray[index]);
        }
    }
}
```
```
public struct ReallyToughParallelJobTransforms : IJobParallelForTransform
{
    public NativeArray<float> MoveYArray;
    public float DeltaTime;

    public void Execute(int index, TransformAccess transform)
    {
        transform.position += new Vector3(0f, MoveYArray[index] * DeltaTime, 0f);

        if (transform.position.y > 5f)
        {
            MoveYArray[index] = -math.abs(MoveYArray[index]);
        }
        else if (transform.position.y < -5f)
        {
            MoveYArray[index] = math.abs(MoveYArray[index]);
        }
    }
}
```

## How to schedule a Job?
```
NativeArray<JobHandle> jobHandleArray = new NativeArray<JobHandle>(10, Allocator.Temp);

for (int i = 0; i < 10; i++)
{
    JobHandle jobHandle = ReallyToughTaskJob();
    jobHandleArray[i] = jobHandle;
}

JobHandle.CompleteAll(jobHandleArray);
jobHandleArray.Dispose();
```
```
NativeArray<float> moveYArray = new NativeArray<float>(_zombieList.Count, Allocator.TempJob);
NativeArray<float3> positionArray = new NativeArray<float3>(_zombieList.Count, Allocator.TempJob);

ReallyToughParallelJob job = new ReallyToughParallelJob
{
    PositionArray = positionArray,
    MoveYArray = moveYArray,
    DeltaTime = Time.deltaTime
};

job.Schedule(_zombieList.Count, 100).Complete();
moveYArray.Dispose();
positionArray.Dispose();
```
```
NativeArray<float> moveYArray = new NativeArray<float>(_zombieList.Count, Allocator.TempJob);
TransformAccessArray transformAccessArray = new TransformAccessArray(_zombieList.Count);

ReallyToughParallelJobTransforms job = new ReallyToughParallelJobTransforms
{
    MoveYArray = moveYArray,
    DeltaTime = Time.deltaTime
};

job.Schedule(transformAccessArray).Complete();
moveYArray.Dispose();
transformAccessArray.Dispose();
```

## How to use Burst Compiler?
Jobs > Burst > Enable Compilation
```
[BurstCompile]
public struct ReallyToughJob : IJob
```
