Carby JobManager for Azure Functions
=

Abstract
-

Carby JobManager for Azure Functions is a collection of triggers to 
enable integration of smaller function components in order to create 
custom workflows without hardcoding the flow descriptor. 

Motivation
-

I've been working with Azure Durable functions on a project where we 
had to replace Apache Flink and move to an Azure native version. 
We have faced many issues and realised that Durable Functions framework 
is not a direct replacement for the problem we have. While it can handle 
long running functions with monitoring capabilities, it fails on being 
performant when the task is to handle thousands of smaller activities in 
parallel.

Issues we identified were related to:

- End to End tracing capability with Application Insights not being to 
able to persist custom properties to ease with searching and aggregation.
- It uses one queue for all activities, so having multiple orchestrations
with thousands of activities are blocking each other especially if the flow
is to send the message through multiple stages.
- When the function runtime is under heavy load, the processing of 
orchestrator functions becomes very slow.
- Sending and extracting the argument of an activity (there can be only one)
and the actual call of activities happen through a context object by referring
to the activity only by name, so no compile time checks are made if the call 
is correct or not.

We have managed to overcome the above issues by either finding workarounds 
or simply deciding to live with limitations.

I as a side project decided though to investigate if a better framework 
can be constructed using the WebJobs SDK.

How to use
-

Look at the project in the Carby.JobManager.Functions.Tests folder for examples.

License
-

[MIT](http://opensource.org/licenses/MIT)




