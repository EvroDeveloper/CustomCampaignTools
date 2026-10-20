using System;
using System.Collections;
using System.Threading.Tasks;
using Il2CppSystem.Threading.Tasks;
using Il2CppCysharp.Threading.Tasks;
using SysTask = System.Threading.Tasks.Task;
using Il2Task = Il2CppSystem.Threading.Tasks.Task;

namespace CustomCampaignTools.Utilities;

public class AwaitCompletionBase : IEnumerator
{
    protected bool isCompleted = false;
    protected virtual void Complete()
    {
        isCompleted = true;
    }

    public bool MoveNext()
    {
        return !isCompleted;
    }

    public void Reset()
    {
        throw new NotImplementedException();
    }

    public object Current => null;
}

public class AwaitTask : AwaitCompletionBase
{
    protected readonly UniTask task;
    protected readonly SysTask systemTask;
    protected readonly Il2Task il2SystemTask;

    public AwaitTask() { }

    public AwaitTask(UniTask task)
    {
        this.task = task;
        isCompleted = task.GetAwaiter().IsCompleted;
        if(!isCompleted)
        {
            task.GetAwaiter().UnsafeOnCompleted((Action)Complete);
        }
        else
        {
            Complete();
        }
    }

    public AwaitTask(SysTask systemTask)
    {
        this.systemTask = systemTask;
        isCompleted = systemTask.GetAwaiter().IsCompleted;
        if(!isCompleted)
        {
            systemTask.GetAwaiter().UnsafeOnCompleted(Complete);
        }
        else
        {
            Complete();
        }
    }

    public AwaitTask(Il2Task il2SystemTask)
    {
        this.il2SystemTask = il2SystemTask;
        isCompleted = il2SystemTask.GetAwaiter().IsCompleted;
        if(!isCompleted)
        {
            il2SystemTask.GetAwaiter().UnsafeOnCompleted((Action)Complete);
        }
        else
        {
            Complete();
        }
    }
}

public class AwaitTask<T> : AwaitTask
{
    public class ResultHolder<R>
    {
        public R Value { get; set; }
    }

    private readonly UniTask<T> typedTask;
    private readonly System.Threading.Tasks.Task<T> typedTaskSys;
    private readonly Il2CppSystem.Threading.Tasks.Task<T> typedTaskIl2;

    public T Result { get; private set; }

    private readonly ResultHolder<T> resultHolder = new();

    public AwaitTask(UniTask<T> task) : base()
    {
        typedTask = task;
        isCompleted = task.GetAwaiter().IsCompleted;
        if(!isCompleted)
        {
            task.GetAwaiter().UnsafeOnCompleted((Action)Complete);
        }
        else
        {
            Complete();
        }
    }
    
    public AwaitTask(UniTask<T> task, out ResultHolder<T> result) : this(task)
    {
        result = resultHolder;
    }

    public AwaitTask(System.Threading.Tasks.Task<T> sysTask)
    {
        typedTaskSys = sysTask;
        isCompleted = sysTask.GetAwaiter().IsCompleted;

        if(!isCompleted)
        {
            sysTask.GetAwaiter().UnsafeOnCompleted((Action)Complete);
        }
        else
        {
            Complete();
        }
    }

    public AwaitTask(System.Threading.Tasks.Task<T> sysTask, out ResultHolder<T> result) : this(sysTask)
    {
        result = resultHolder;
    }

    public AwaitTask(Il2CppSystem.Threading.Tasks.Task<T> il2Task)
    {
        typedTaskIl2 = il2Task;
        isCompleted = il2Task.GetAwaiter().IsCompleted;
        if(!isCompleted)
        {
            il2Task.GetAwaiter().UnsafeOnCompleted((Action)Complete);
        }
        else
        {
            Complete();
        }
    }

    public AwaitTask(Il2CppSystem.Threading.Tasks.Task<T> il2Task, out ResultHolder<T> result) : this(il2Task)
    {
        result = resultHolder;
    }

    protected override void Complete()
    {
        base.Complete();

        if(typedTask != null)
            Result = typedTask.GetAwaiter().GetResult();
        else if(typedTaskSys != null)
            Result = typedTaskSys.GetAwaiter().GetResult();
        else if (typedTaskIl2 != null)
            Result = typedTaskIl2.GetAwaiter().GetResult();
        
        resultHolder.Value = Result;
    }
}
