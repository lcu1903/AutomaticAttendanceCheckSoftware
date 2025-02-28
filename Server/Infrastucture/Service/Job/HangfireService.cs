using System.Linq.Expressions;
using Hangfire;

namespace Core.Services.Job;

public class HangfireService : IJobService
{
    public bool RemoveJobIdIfExist(string jobId)
    {

        return BackgroundJob.Delete(jobId);
    }
    public void RemoveJobIdsIfExist(List<string> jobIds)
    {
        jobIds.ForEach(e => BackgroundJob.Delete(e));
    }

    public string Enqueue(Expression<Action> methodCall)
    {
        return BackgroundJob.Enqueue(methodCall);
    }

    public string Enqueue(Expression<Func<Task>> methodCall)
    {
        return BackgroundJob.Enqueue(methodCall);
    }


    public string Enqueue<T>(Expression<Action<T>> methodCall)
    {
        return BackgroundJob.Enqueue(methodCall);
    }

    public string Enqueue<T>(Expression<Func<T, Task>> methodCall)
    {
        return BackgroundJob.Enqueue(methodCall);
    }

    public string Schedule(Expression<Action> methodCall, TimeSpan delay)
    {
        return BackgroundJob.Schedule(methodCall, delay);
    }

    public void ScheduleDaily(string recurringJobId, Expression<Func<Task>> methodCall, string cronExpression,
        TimeZoneInfo timeZone = null)
    {
        RecurringJob.AddOrUpdate(recurringJobId, methodCall, cronExpression, timeZone);
    }

    public string Schedule(Expression<Func<Task>> methodCall, TimeSpan delay)
    {
        return BackgroundJob.Schedule(methodCall, delay);
    }

    public string Schedule(Expression<Action> methodCall, DateTimeOffset enqueueAt)
    {
        return BackgroundJob.Schedule(methodCall, enqueueAt);
    }

    public string Schedule(Expression<Func<Task>> methodCall, DateTimeOffset enqueueAt)
    {
        return BackgroundJob.Schedule(methodCall, enqueueAt);
    }

    public string Schedule<T>(Expression<Action<T>> methodCall, TimeSpan delay)
    {
        return BackgroundJob.Schedule(methodCall, delay);
    }

    public string Schedule<T>(Expression<Func<T, Task>> methodCall, TimeSpan delay)
    {
        return BackgroundJob.Schedule(methodCall, delay);
    }

    public string Schedule<T>(Expression<Action<T>> methodCall, DateTimeOffset enqueueAt)
    {
        return BackgroundJob.Schedule(methodCall, enqueueAt);
    }

    public string Schedule<T>(Expression<Func<T, Task>> methodCall, DateTimeOffset enqueueAt)
    {
        return BackgroundJob.Schedule(methodCall, enqueueAt);
    }

    public bool Delete(string jobId)
    {
        return BackgroundJob.Delete(jobId);
    }

    public bool Delete(string jobId, string fromState)
    {
        return BackgroundJob.Delete(jobId, fromState);
    }

    public bool Requeue(string jobId)
    {
        return BackgroundJob.Requeue(jobId);
    }

    public bool Requeue(string jobId, string fromState)
    {
        return BackgroundJob.Requeue(jobId, fromState);
    }

    public void RemoveIfExist(string recurringJobId)
    {
        RecurringJob.RemoveIfExists(recurringJobId);
    }
}