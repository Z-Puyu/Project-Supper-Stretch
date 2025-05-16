using System;
using Project.Scripts.Util.Builder;
using UnityEngine;

namespace Project.Scripts.Util.ChainOfResponsibilities;

public abstract class Processor<T> {
    private static Sentinel SentinelProcessor { get; } = new Sentinel();
    
    private Processor<T> Root { get; set; }
    private Processor<T> Next { get; set; }

    private Processor() {
        this.Root = this;
        this.Next = this;
    }

    protected virtual (ProcessorStatus status, T data) Preprocess(T input) {
        return (ProcessorStatus.Healthy, input);
    }
    
    protected virtual (ProcessorStatus status, T data) InProcess(T data) {
        return (ProcessorStatus.Healthy, data);
    }
    
    protected virtual (ProcessorStatus status, T data, bool done) Postprocess(T output) {
        return (ProcessorStatus.Healthy, output, false);
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
    
    private void Access(ProcessorStatus status, T data) {
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
    
    public virtual T Process(T input) {
        (ProcessorStatus status, T data) = this.Preprocess(input);
        this.Access(status, data);
        (status, data) = this.InProcess(data);
        this.Access(status, data);
        (status, data, bool done) = this.Postprocess(data);
        this.Access(status, data);
        return done ? data : this.Next.Process(data);
    }

    private sealed class Sentinel : Processor<T> {
        public override T Process(T input) {
            return input;
        }
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

        public override Processor<T> Build() {
            this.Template.Next = Processor<T>.SentinelProcessor;
            return this.Template.Root;
        }
    }
}
