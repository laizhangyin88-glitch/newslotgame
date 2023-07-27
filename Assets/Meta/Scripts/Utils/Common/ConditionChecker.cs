using System.Collections.Generic;
using System.Linq;
using System;

namespace BagelCode
{
    public enum TrueCase
    {
        ANY_TRUE,
        ANY_FALSE,
        ALL_TRUE,
        ALL_FALSE,
    }

    // Manages 'State' by integrating multiple states(keyStateDict)
    public class ConditionChecker<T>
    {
        private bool state;
        public bool State
        {
            get
            {
                UpdateAll();
                return state;
            }
        }

        public TrueCase trueCase = TrueCase.ANY_TRUE;

        private Dictionary<T, Func<bool>> keyStateDict = new Dictionary<T, Func<bool>>();
        private List<Action> onChangeActionList = new List<Action>();

        public ConditionChecker(TrueCase _trueCase)
        {
            trueCase = _trueCase;
        }

        public bool SetOrAddState(T key, bool s)
        {
            return SetOrAddState(key, () => { return s; });
        }

        public bool SetOrAddState(T key, Func<bool> s)
        {
            if (s == null) return false;

            if (keyStateDict.ContainsKey(key))
                keyStateDict[key] = s;
            else
                keyStateDict.Add(key, s);

            UpdateAll();

            return true;
        }

        public bool RemoveState(T key)
        {
            return keyStateDict.Remove(key);
        }

        public void ClearState()
        {
            keyStateDict.Clear();
            UpdateAll();
        }

        public void RegisterOnChangeAction(System.Action action)
        {
            onChangeActionList.Remove(action);
            if (action != null) onChangeActionList.Add(action);
        }

        public bool UnRegisterOnChangeAction(System.Action action)
        {
            return onChangeActionList.Remove(action);
        }

        public void ClearOnChangeAction()
        {
            onChangeActionList.Clear();
        }

        //

        public static bool operator ==(ConditionChecker<T> a, bool b)
        {
            return a.State == b;
        }

        public static bool operator !=(ConditionChecker<T> a, bool b)
        {
            return a.State != b;
        }

        public static bool operator ==(bool a, ConditionChecker<T> b)
        {
            return a == b.State;
        }

        public static bool operator !=(bool a, ConditionChecker<T> b)
        {
            return a != b.State;
        }

        public override bool Equals(object obj)
        {
            if (obj is ConditionChecker<T> checker)
                return State == checker.State;
            else if (obj is bool b)
                return State == b;
            else return false;
        }

        public override int GetHashCode()
        {
            var hashCode = -325116050;
            hashCode = hashCode * -1521134295 + State.GetHashCode();
            return hashCode;
        }

        //

        private void OnChange()
        {
            onChangeActionList.ForEach(a => a?.Invoke());
        }

        private void UpdateAll()
        {
            bool prev = state;
            if (trueCase == TrueCase.ALL_TRUE)
                state = keyStateDict.Values.All(s => (bool)(s?.Invoke()));
            else if (trueCase == TrueCase.ALL_FALSE)
                state = keyStateDict.Values.All(s => !(bool)(s?.Invoke()));
            else if (trueCase == TrueCase.ANY_TRUE)
                state = keyStateDict.Values.Any(s => (bool)(s?.Invoke()));
            else if (trueCase == TrueCase.ANY_FALSE)
                state = keyStateDict.Values.Any(s => !(bool)(s?.Invoke()));

            if (state != prev) OnChange();
        }
    }
}