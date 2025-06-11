using System;
using Project.Scripts.Util.Builder;
using UnityEngine;

namespace Project.Scripts.Util.ChainOfResponsibilities;

public abstract class Processor<T> : IProcessor<T> {
    private Processor<T> Root { get; init; }
    private Processor<T> Next { get; set; }
    private bool WillTerminateIfProcessed { get; set; }
    private bool IsLastInPipeline { get; set; }

    protected Processor() {
        this.Root = this;
        this.Next = this;
    }
    
    protected virtual (ProcessorStatus status, T data) RunProcess(T data) {
        return (ProcessorStatus.Healthy, data);
    }
    
    protected virtual void PreLogWarning(T data) { }

    protected void OnWarning(T data) {
        this.PreLogWarning(data);
        Debug.Log($"Processed data {data} triggered a warning");
    }
    
    protected virtual void PreLogError(T data) { }
    
    protected void OnError(T data) {
        this.PreLogError(data);
        Debug.LogError($"Processed data {data} triggered an error");
    }
    
    protected virtual void PreLogFatalError(T data) { }
    
    protected void OnFatalError(T data) {
        this.PreLogFatalError(data);
        throw new ArgumentException($"Processed data {data} triggered a fatal error");
    }
    
    private void Assess(ProcessorStatus status, T data) {
        switch (status) {
            case ProcessorStatus.Warning:
                this.OnWarning(data);
                break;
            case ProcessorStatus.Error:
                this.OnError(data);
                break;
            case ProcessorStatus.Fatal:
                this.OnFatalError(data);
                break;
        }
    }
    
    public void Process(T input) {
        (ProcessorStatus status, T data) = this.RunProcess(input);
        this.Assess(status, data);
        if (this.IsLastInPipeline) {
            return;
        }
        
        if (status is ProcessorStatus.Failure or ProcessorStatus.Completed && this.WillTerminateIfProcessed) {
            return;
        }
        
        this.Next.Process(input);
    }

    public sealed class Builder : FluentBuilder<Processor<T>> {
        private Builder(Processor<T> template) : base(template) { }

        public static Builder StartWith<P>() where P : Processor<T>, new() {
            return new Builder(new P());
        }
        
        public Builder Then<P>() where P : Processor<T>, new() {
            this.Template = this.Template.Next = new P { Root = this.Template.Root };
            return this;
        }

        public Builder IfNotDoneThen<P>() where P : Processor<T>, new() {
            this.Template.WillTerminateIfProcessed = true;
            this.Template = this.Template.Next = new P { Root = this.Template.Root };
            return this;
        }

        public override Processor<T> Build() {
            this.Template.IsLastInPipeline = true;
            return this.Template.Root;
        }
    }
}
