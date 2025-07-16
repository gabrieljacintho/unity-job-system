# Unity Job System

## Links
[Unity Job System Documentation](https://docs.unity3d.com/Manual/job-system.html)

[Tutorial](https://www.youtube.com/watch?v=C56bbgtPr_w)

## How to create a Job?
[Testing.cs](https://github.com/gabrieljacintho/unity-job-system/blob/47beb9463b4f14416fc559b5b29867dd65025520/Assets/Scripts/Testing.cs)
```
[BurstCompile]
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
[BurstCompile]
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

        float value = 0f;
        for (int i = 0; i < 1000; i++)
        {
            value += math.exp10(math.sqrt(value));
        }
    }
}
```
```
[BurstCompile]
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

        float value = 0f;
        for (int i = 0; i < 1000; i++)
        {
            value += math.exp10(math.sqrt(value));
        }
    }
}
```

## How to use Burst?
Jobs > Burst > Enable Compilation
```
[BurstCompile]
public struct ReallyToughJob : IJob
```
