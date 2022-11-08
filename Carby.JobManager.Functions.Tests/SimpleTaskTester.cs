using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Carby.JobManager.Functions.Attributes;
using Microsoft.Azure.WebJobs;

namespace Carby.JobManager.Functions.Tests;

public class SimpleTaskTester
{

    [FunctionName(nameof(SimpleTaskStartAsync))]
    public Task<Dictionary<string, object>> SimpleTaskStartAsync(
        [TransformTaskTrigger(nameof(SimpleTaskStartAsync))] TaskRequest request,
        string greeting,
        string name)
    {
        Console.WriteLine("Start task");

        return Task.FromResult(new Dictionary<string, object>
        {
            ["IWasSent"] = $"{greeting} {name}",
            ["itemCount"] = 2
        });

    }

    [FunctionName(nameof(SimpleTaskMiddleAsync))]
    public Task<TaskResponse> SimpleTaskMiddleAsync(
        [SplitterTaskTrigger(nameof(SimpleTaskMiddleAsync))] TaskRequest request,
        string iWasSent
        )
    {
        Console.WriteLine($"Middle task {iWasSent}");
        return Task.FromResult(TaskResponse.Ok());
    }

    [FunctionName(nameof(SimpleTaskEndAsync))]
    public Task<TaskResponse> SimpleTaskEndAsync(
        [TransformTaskTrigger(nameof(SimpleTaskEndAsync))] TaskRequest request
        )
    {
        Console.WriteLine("End task");
        return Task.FromResult(TaskResponse.Ok());
    }

    [FunctionName(nameof(SimpleTaskFailureAsync))]
    public Task<TaskResponse> SimpleTaskFailureAsync(
        [TransformTaskTrigger(nameof(SimpleTaskFailureAsync))] TaskRequest request
        )
    {
        Console.WriteLine("Failure handler task");
        return Task.FromResult(TaskResponse.Ok());
    }

    [FunctionName(nameof(SimpleTaskCleanUpAsync))]
    public Task<TaskResponse> SimpleTaskCleanUpAsync(
        [TransformTaskTrigger(nameof(SimpleTaskCleanUpAsync))] TaskRequest request
        )
    {
        Console.WriteLine("CleanUp task");
        return Task.FromResult(TaskResponse.Ok());
    }
}