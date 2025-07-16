using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class Testing : MonoBehaviour
{
    [SerializeField] private bool _useJobs;


    private void Update()
    {
        float startTime = Time.realtimeSinceStartup;

        if (_useJobs)
        {
            NativeArray<JobHandle> jobHandleArray = new NativeArray<JobHandle>(10, Allocator.Temp);
            for (int i = 0; i < 10; i++)
            {
                JobHandle jobHandle = ReallyToughTaskJob();
                jobHandleArray[i] = jobHandle;
            }

            JobHandle.CompleteAll(jobHandleArray);
            jobHandleArray.Dispose();
        }
        else
        {
            for (int i = 0; i < 10; i++)
            {
                ReallyToughTask();
            }
        }

        Debug.Log($"Time taken: {(Time.realtimeSinceStartup - startTime) * 1000f} ms");
    }

    private void ReallyToughTask()
    {
        float value = 0f;
        for (int i = 0; i < 50000; i++)
        {
            value += math.exp10(math.sqrt(value));
        }
    }

    private JobHandle ReallyToughTaskJob()
    {
        ReallyToughJob job = new ReallyToughJob();
        return job.Schedule();
    }
}

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
