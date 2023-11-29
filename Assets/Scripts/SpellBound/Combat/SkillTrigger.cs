using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MessagePipe;
using SpellBound.Core;
using UnityEngine;

namespace SpellBound.Combat
{
    public class SkillTrigger<T> : IDisposable
    {
        public SkillTriggerSetting Setting => this.setting;
        private readonly SkillTriggerSetting setting;
        private readonly Character owner;

        private IDisposablePublisher<T> publisher;
        private ISubscriber<T> subscriber;

        private T skillArg;
        private bool hasSkillArg;
        private float nextClearTime;

        public float TriggerTimer { get; private set; }

        public SkillTrigger(SkillTriggerSetting setting, Character owner)
        {
            this.setting = setting;
            this.owner = owner;
            (this.publisher, this.subscriber) = GlobalMessagePipe.CreateEvent<T>();
            this.hasSkillArg = false;
            this.nextClearTime = float.MaxValue;
            this.TriggerTimer = this.Setting.CooldownSeconds;
        }

        public void Start(CancellationToken ct)
        {
            this.refreshSkillArg(ct).Forget();
            this.triggerTask(ct).Forget();
        }

        private async UniTaskVoid refreshSkillArg(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                if (Time.realtimeSinceStartup > this.nextClearTime)
                {
                    this.hasSkillArg = false;
                }
                await UniTask.Yield();
            }
        }

        private async UniTaskVoid triggerTask(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                if (this.TriggerTimer > 0f)
                {
                    this.TriggerTimer = Mathf.Max(this.TriggerTimer - Time.deltaTime, 0f);
                    await UniTask.Yield();
                    continue;
                }

                if (this.owner.MP < this.Setting.Cost)
                {
                    continue;
                }

                if (this.hasSkillArg)
                {
                    this.publisher.Publish(this.skillArg);
                    this.hasSkillArg = false;
                    this.TriggerTimer = this.Setting.CooldownSeconds;
                    this.owner.Cast(this.Setting.Cost);
                }
                await UniTask.Yield();
            }
        }

        public void Trigger(T arg)
        {
            this.hasSkillArg = true;
            this.skillArg = arg;
            // TODO: configurable clear interval
            this.nextClearTime = Time.realtimeSinceStartup + 0.05f;
        }

        public IDisposable Subscribe(Action<T> handler)
        {
            return this.subscriber.Subscribe(handler);
        }

        public void Dispose()
        {
            this.publisher?.Dispose();
        }
    }
}

