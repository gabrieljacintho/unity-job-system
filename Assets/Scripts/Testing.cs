using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;
using static Testing;

public class Testing : MonoBehaviour
{
    public class Zombie
    {
        public Transform Transform;
        public float MoveY;
    }

    [SerializeField] private bool _useJobs;
    [SerializeField] private bool _useJobsTransform;
    [SerializeField] private Transform _zombiePrefab;

    private List<Zombie> _zombieList = new List<Zombie>();


    private void Start()
    {
        for (int i = 0; i < 1000; i++)
        {
            Transform zombieTransform = Instantiate(_zombiePrefab,
                new Vector3(UnityEngine.Random.Range(-8f, 8f), UnityEngine.Random.Range(-5f, 5f)), Quaternion.identity);
            _zombieList.Add(new Zombie
            {
                Transform = zombieTransform,
                MoveY = UnityEngine.Random.Range(1f, 2f)
            });
        }
    }

    private void Update()
    {
        float startTime = Time.realtimeSinceStartup;

        if (_useJobs)
        {
            NativeArray<float> moveYArray = new NativeArray<float>(_zombieList.Count, Allocator.TempJob);
            TransformAccessArray transformAccessArray = new TransformAccessArray(_zombieList.Count);

            if (_useJobsTransform)
            {
                for (int i = 0; i < _zombieList.Count; i++)
                {
                    moveYArray[i] = _zombieList[i].MoveY;
                    transformAccessArray.Add(_zombieList[i].Transform);
                }

                ReallyToughParallelJobTransforms job = new ReallyToughParallelJobTransforms
                {
                    MoveYArray = moveYArray,
                    DeltaTime = Time.deltaTime
                };

                job.Schedule(transformAccessArray).Complete();

                for (int i = 0; i < _zombieList.Count; i++)
                {
                    _zombieList[i].MoveY = moveYArray[i];
                }

                transformAccessArray.Dispose();
            }
            else
            {
                NativeArray<float3> positionArray = new NativeArray<float3>(_zombieList.Count, Allocator.TempJob);

                for (int i = 0; i < _zombieList.Count; i++)
                {
                    positionArray[i] = _zombieList[i].Transform.position;
                    moveYArray[i] = _zombieList[i].MoveY;
                }

                ReallyToughParallelJob job = new ReallyToughParallelJob
                {
                    PositionArray = positionArray,
                    MoveYArray = moveYArray,
                    DeltaTime = Time.deltaTime
                };

                job.Schedule(_zombieList.Count, 100).Complete();

                for (int i = 0; i < _zombieList.Count; i++)
                {
                    _zombieList[i].Transform.position = positionArray[i];
                    _zombieList[i].MoveY = moveYArray[i];
                }

                positionArray.Dispose();
            }

            moveYArray.Dispose();
        }
        else
        {
            foreach (Zombie zombie in _zombieList)
            {
                zombie.Transform.position += new Vector3(0f, zombie.MoveY * Time.deltaTime, 0f);

                if (zombie.Transform.position.y > 5f)
                {
                    zombie.MoveY = -math.abs(zombie.MoveY);
                }
                else if (zombie.Transform.position.y < -5f)
                {
                    zombie.MoveY = math.abs(zombie.MoveY);
                }

                float value = 0f;
                for (int i = 0; i < 1000; i++)
                {
                    value += math.exp10(math.sqrt(value));
                }
            }
        }

        /*if (_useJobs)
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
        }*/

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