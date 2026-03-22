using System;
using System.Collections.Generic;
using UnityEngine.LowLevel;

namespace ShadersMagic
{
    public interface ILoopSystem
    {
        IDisposable Start(Action updateDelegate);
    }

    internal class LoopSystem<TUpdateGroup> : ILoopSystem
    {
        private UpdatableSlot[] _updatables = new UpdatableSlot[1000];
        private List<int> _updatableToRemove = new List<int>();

        private int _count;

        public IDisposable Start(Action updateDelegate)
        {
            if (_count == _updatables.Length)
            {
                Array.Resize(ref _updatables, (int)(_count * 1.5f));
            }

            var updatableSlot = new UpdatableSlot(this, _count, updateDelegate);
            _updatables[_count] = updatableSlot;

            if (_count == 0)
            {
                PlayerLoopExtensions.ModifyCurrentPlayerLoop((ref PlayerLoopSystem system) =>
                {
                    system.GetSystem<TUpdateGroup>().AddSystem<LoopSystem<TUpdateGroup>>(OnUpdate);
                });
            }

            ++_count;
            return updatableSlot.Registration;
        }

        private void OnUpdate()
        {
            foreach (var index in _updatableToRemove)
            {
                RemoveReal(index);
            }

            _updatableToRemove.Clear();

            for (int i = 0; i < _count; i++)
            {
                _updatables[i].Updatable();
            }
        }

        private void RemoveReal(int index)
        {
            var lastUpdateIndex = _count - 1;
            if (index == lastUpdateIndex)
            {
                _updatables[index] = default;
            }
            else
            {
                ref var lastUpdatable = ref _updatables[lastUpdateIndex];
                lastUpdatable.Registration.Index = index;
                _updatables[index] = lastUpdatable;
                _updatables[lastUpdateIndex] = default;
            }

            --_count;

            if (_count == 0)
            {
                PlayerLoopExtensions.ModifyCurrentPlayerLoop((ref PlayerLoopSystem system) =>
                {
                    system.GetSystem<TUpdateGroup>().RemoveSystem<LoopSystem<TUpdateGroup>>(false);
                });
            }
        }

        private void RemoveAt(int index)
        {
            _updatableToRemove.Add(index);
        }

        private struct UpdatableSlot
        {
            public readonly Action Updatable;
            public readonly UpdatableRegistration Registration;

            public UpdatableSlot(LoopSystem<TUpdateGroup> system, int index, Action updatable)
            {
                Updatable = updatable;
                Registration = new UpdatableRegistration(system, index);
            }
        }

        private class UpdatableRegistration : IDisposable
        {
            private LoopSystem<TUpdateGroup> _system;
            internal int Index;

            public UpdatableRegistration(LoopSystem<TUpdateGroup> system, int index)
            {
                _system = system;
                Index = index;
            }

            public void Dispose()
            {
                if (Index < 0) return;
                _system.RemoveAt(Index);
                Index = -1;
            }
        }
    }
}