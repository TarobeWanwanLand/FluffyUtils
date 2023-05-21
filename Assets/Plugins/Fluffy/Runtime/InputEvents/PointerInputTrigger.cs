using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Fluffy.InputEvents
{
    public sealed class Pointer
    {
        private readonly struct MultiTapObservableKey : IEquatable<MultiTapObservableKey>
        {
            private readonly int _count;
            private readonly float _intervalSecond;

            public MultiTapObservableKey(int count, float intervalSecond)
            {
                _count = count;
                _intervalSecond = intervalSecond;
            }

            public bool Equals(MultiTapObservableKey other)
            {
                return _count == other._count && _intervalSecond.Equals(other._intervalSecond);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(_count, _intervalSecond);
            }
        }
        
        private readonly CompositeDisposable _disposables = new();
        
        private IConnectableObservable<Unit> _onPress;
        private IConnectableObservable<Unit> _onRelease;
        private IConnectableObservable<Vector2> _onPositionChange;
        
        private Dictionary<MultiTapObservableKey, IConnectableObservable<Unit>> _onMultiPressCaches;
        private Dictionary<float, IConnectableObservable<Unit>> _onHoldPressCaches;

        private InputAction _pressAction;
        private InputAction _positionAction;
        
        ~Pointer()
        {
            _disposables.Dispose();
        }

        private InputAction GetPressAction()
        {
            if (_pressAction == null)
            {
                _pressAction = new InputAction("Press", binding: "<Pointer>/press")
                    .AddTo(_disposables);
                _pressAction.Enable();   
            }
            return _pressAction;
        }
        
        private InputAction GetPositionAction()
        {
            if (_positionAction == null)
            {
                _positionAction = new InputAction("Position", binding: "<Pointer>/position")
                    .AddTo(_disposables);
                _positionAction.Enable();   
            }
            return _positionAction;
        }

        private IObservable<Unit> GetPressEventObservable()
        {
            var action = GetPressAction();

            return Observable.FromEvent<InputAction.CallbackContext>(
                    h => action.started += h,
                    h => action.started -= h)
                .Select(ctx => ctx.started)
                .AsUnitObservable();
        }
        
        private IObservable<Unit> GetReleaseEventObservable()
        {
            var action = GetPressAction();
            
            return Observable.FromEvent<InputAction.CallbackContext>(
                    h => action.canceled += h,
                    h => action.canceled -= h)
                .Select(ctx => ctx.canceled)
                .AsUnitObservable();
        }

        private IObservable<Vector2> GetPositionEventObservable()
        {
            var action = GetPositionAction();
            
            return Observable.FromEvent<InputAction.CallbackContext>(
                    h => action.performed += h,
                    h => action.performed -= h)
                .Select(ctx => ctx.ReadValue<Vector2>());
        }

        public IObservable<Unit> OnPressAsObservable()
        {
            if (_onPress != null) 
                return _onPress;
            
            _onPress = GetPressEventObservable()
                .Publish();
            _onPress.Connect()
                .AddTo(_disposables);
            
            return _onPress;
        }

        public IObservable<Unit> OnMultiPressAsObservable(int count = 2, float intervalSecond = 0.3f)
        {
            if (count < 2)
                throw new ArgumentOutOfRangeException(nameof(count));
            
            var key = new MultiTapObservableKey(count, intervalSecond);
            _onMultiPressCaches ??= new Dictionary<MultiTapObservableKey, IConnectableObservable<Unit>>();
            if (_onMultiPressCaches.TryGetValue(key, out var observable))
                return observable;

            observable = OnPressAsObservable()
                .TimeInterval()
                .Select(ti => ti.Interval <= TimeSpan.FromSeconds(intervalSecond))
                .Buffer(count, 1)
                .Where(results => !results[0] && results.Skip(1).All(ti => ti))
                .AsUnitObservable()
                .Publish();
            observable.Connect()
                .AddTo(_disposables);
            
            _onMultiPressCaches.Add(key, observable);
            
            return observable;
        }
        
        public IObservable<Unit> OnHoldPressAsObservable(float holdTimeSecond = 0.5f)
        {
            if (holdTimeSecond < 0f)
                throw new ArgumentOutOfRangeException(nameof(holdTimeSecond));
            
            _onHoldPressCaches ??= new Dictionary<float, IConnectableObservable<Unit>>();
            if (_onHoldPressCaches.TryGetValue(holdTimeSecond, out var observable))
                return observable;
            
            observable = OnPressAsObservable()
                .SelectMany(_ => Observable.Timer(TimeSpan.FromSeconds(holdTimeSecond)))
                .TakeUntil(OnReleaseAsObservable())
                .Repeat()
                .AsUnitObservable()
                .Publish();
            observable.Connect()
                .AddTo(_disposables);
            
            _onHoldPressCaches.Add(holdTimeSecond, observable);
            
            return observable;
        }

        public IObservable<Unit> OnReleaseAsObservable()
        {
            if (_onRelease != null) 
                return _onRelease;
            
            _onRelease = GetReleaseEventObservable()
                .Publish();
            _onRelease.Connect()
                .AddTo(_disposables);
            
            return _onRelease;
        }

        public IObservable<Vector2> OnPositionChangeAsObservable()
        {
            if (_onPositionChange != null) 
                return _onPositionChange;
            
            _onPositionChange = GetPositionEventObservable()
                .Publish();
            _onPositionChange.Connect()
                .AddTo(_disposables);
            
            return _onPositionChange;
        }
    }
}