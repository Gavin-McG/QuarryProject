using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace JobsTest
{
    public class PopulateJob : MonoBehaviour
    {
        [SerializeField] private int elements = 10;
        
        private struct ComputeCountsJob : IJobFor
        {
            public NativeArray<int> counts;
            
            public void Execute(int index)
            {
                counts[index] = index;
            }
        }

        private struct PrefixSumJob : IJob
        {
            public int elements;
            [ReadOnly] public NativeArray<int> counts;
            
            public NativeArray<int> sums;
            public NativeReference<int> total;
            
            public void Execute()
            {
                int sum = 0;
                for (int i = 0; i < elements; i++)
                {
                    sums[i] = sum;
                    sum += counts[i];
                }
                total.Value = sum;
            }
        }

        private struct AllocateListJob : IJob
        {
            public NativeList<int> list;
            public NativeReference<int> length;
            
            public void Execute()
            {
                list.ResizeUninitialized(length.Value);
            }
        }
        
        private struct PopulateValuesJob : IJobFor
        {
            [ReadOnly] public NativeArray<int> offsets;
            
            public NativeList<int>.ParallelWriter results;
            
            public void Execute(int index)
            {
                unsafe
                {
                    int offset = offsets[index];
                    var dataArray = (int*)results.Ptr;

                    for (int i = 0; i < index; i++)
                    {
                        dataArray[offset + i] = i;
                    }
                }
            }
        }

        [ContextMenu("Run Populate Job")]
        private void RunPopulateJob()
        {
            // Create Native Containers for jobs
            NativeArray<int> counts = new NativeArray<int>(elements, Allocator.TempJob);
            NativeArray<int> sums = new NativeArray<int>(elements, Allocator.TempJob);
            NativeReference<int> total = new NativeReference<int>(Allocator.TempJob);
            NativeList<int> results = new NativeList<int>(elements, Allocator.TempJob);

            // Create Jobs
            ComputeCountsJob countsJob = new ComputeCountsJob()
            {
                counts = counts,
            };
            PrefixSumJob sumJob = new PrefixSumJob()
            {
                elements = elements,
                counts = counts,
                sums = sums,
                total = total,
            };
            AllocateListJob allocJob = new AllocateListJob()
            {
                list = results,
                length = total,
            };
            PopulateValuesJob populateJob = new PopulateValuesJob()
            {
                offsets = sums,
                results = results.AsParallelWriter(),
            };
            
            // Schedule jobs
            JobHandle countsHandle = countsJob.ScheduleParallel(elements, 5, new JobHandle());
            JobHandle sumHandle = sumJob.Schedule(countsHandle);
            JobHandle allocHandle = allocJob.Schedule(sumHandle);
            JobHandle populateHandle = populateJob.ScheduleParallel(elements, 5, allocHandle);
            
            // Complete job and print result
            populateHandle.Complete();
            
            Debug.Log(counts.Length + " " + sums.Length + " " + total.Value + " " + results.Length);
            
            // Dispose of Native Containers
            counts.Dispose();
            sums.Dispose();
            total.Dispose();
            results.Dispose();
        }
    }
}

