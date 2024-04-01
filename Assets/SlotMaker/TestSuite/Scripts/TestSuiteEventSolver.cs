using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.TestSuite
{

    public class TestSuiteEventSolver : MonoWeakSingleton<TestSuiteEventSolver>
    {
        public class WeightedSolvers
        {
            private List<Solver> solverList;
            private List<int> weightList;
            private int totalWeight;

            public int Count { get { return solverList.Count; } }

            public WeightedSolvers()
            {
                solverList = new List<Solver>();
                weightList = new List<int>();
                totalWeight = 0;
            }

            public void Add(Solver solver, int weight)
            {
                solverList.Add(solver);
                weightList.Add(weight);
                totalWeight += weight;
            }

            public void Remove(Solver solver)
            {
                int removeIndex = solverList.IndexOf(solver);
                if (removeIndex < 0)
                    return;

                int weight = weightList[removeIndex];
                totalWeight -= weight;
                solverList.RemoveAt(removeIndex);
                weightList.RemoveAt(removeIndex);
            }

            public void Solve()
            {
                int solverIndex = -1;
                int randomSeed = Random.Range(0, totalWeight);
                for (int i = 0, sum = 0; i < weightList.Count; ++i)
                {
                    sum += weightList[i];
                    if (randomSeed < sum)
                    {
                        solverIndex = i;
                        break;
                    }
                }
                solverIndex = (solverIndex == -1) ? weightList.Count - 1 : solverIndex;
                solverList[solverIndex]();
            }
        }

        public delegate void Solver();
        private Dictionary<string, WeightedSolvers> activeSolvers = new Dictionary<string, WeightedSolvers>();

        public static void Register(string categoryName, Solver solver, int weight)
        {
            WeightedSolvers solvers;
            if (!Instance.activeSolvers.TryGetValue(categoryName, out solvers))
            {
                solvers = new WeightedSolvers();
                Instance.activeSolvers[categoryName] = solvers;
            }
            solvers.Add(solver, weight);
        }

        public static void UnRegister(string categoryName, Solver solver)
        {
            WeightedSolvers solvers;
            if (Instance.activeSolvers.TryGetValue(categoryName, out solvers))
            {
                solvers.Remove(solver);
                if (solvers.Count == 0)
                    Instance.activeSolvers.Remove(categoryName);
            }
        }

        public static bool HasSolver(string categoryName)
        {
            return Instance.activeSolvers.ContainsKey(categoryName);
        }

        public static void Solve(string categoryName)
        {
            WeightedSolvers solvers;
            if (Instance.activeSolvers.TryGetValue(categoryName, out solvers))
            {
                solvers.Solve();
            }
        }
    }
}
