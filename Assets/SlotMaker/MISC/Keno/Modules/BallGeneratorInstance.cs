using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SlotMaker.Keno
{
    public class BallGeneratorInstance : MonoBehaviour
    {
        [Serializable]
        public class Layout
        {
            public int rowCount;
            public int colCount;
            public float ballStartPosY;
            public Vector2 ballSize = Vector2.zero;
            public Vector2 offset = Vector2.zero;
            public Vector2 spacing = Vector2.zero;
        }

        [Serializable]
        public class QueueData
        {
            private List<BallInfo> balls = new List<BallInfo>();

            public int Count { get { return balls.Count; } }

            public void Clear()
            {
                balls.Clear();
            }

            public BallInfo Dequeue()
            {
                var ball = balls[0];
                balls.RemoveAt(0);
                ball.Reset();
                return ball;
            }

            public void Enqueue(BallInfo info)
            {
                balls.Add(info);
            }

            public void Enqueue(List<BallInfo> infoList)
            {
                balls.AddRange(infoList);
            }
        }

        public int drawCount;
        public float intervalTime;

        public ObjectPool ballPool;
        public RectTransform ballsTransform;

        public Layout layout = new Layout();
        public List<BallInstance> balls = new List<BallInstance>();

        public List<BallInfo> deck = new List<BallInfo>();
        private QueueData queueData = new QueueData();

        [InlineEditor]
        public KenoMediator mediator;

        public bool IsDrawnAll { get { return drawnCount >= drawCount; } }
        public int drawnCount
        {
            get
            {
                int count = 0;
                foreach (var ball in balls)
                {
                    if (ball.drawState == DrawState.Drawn)
                        count++;
                }
                return count;
            }
        }

        public void Clear()
        {
            foreach (var ball in balls)
                ball.Reset();
            balls.Clear();
        }

        public int GetRow(int index) { return layout.rowCount - (index / layout.colCount) - 1; }
        public int GetColum(int index) { return index % layout.colCount; }

        public BallInstance CreateBall()
        {
            var po = ballPool.GetObject();
            return po.GetComponent<BallInstance>();
        }

        public void Initialize()
        {
            for (int i = 0; i < drawCount; ++i)
                deck.Add(new BallInfo());
        }

        public void ChargeNumbers(List<int> numbers)
        {
            for (int i = 0; i < deck.Count; ++i)
                deck[i].number = numbers[i];

            queueData.Enqueue(BallInfo.CloneList1(deck));
        }

        IEnumerator DrawSequence()
        {
            while (queueData.Count > 0)
            {
                int index = balls.Count;

                var ball = CreateBall();
                ball.transform.SetParent(ballsTransform, false);
                ball.transform.localPosition = GetStartPosition(GetColum(index));
                ball.Initialize(queueData.Dequeue());
                ball.target = CalcBallPosition(GetRow(index), GetColum(index));
                balls.Add(ball);

                ball.Draw();

                yield return new WaitForSeconds(intervalTime);
            }
        }

        public void Draw()
        {
            Clear();
            StartCoroutine(DrawSequence());
        }

        public void Stop()
        {
            StopAllCoroutines();

            foreach (var ball in balls)
                if (ball.drawState == DrawState.Drawing)
                    ball.Skip();

            while (queueData.Count > 0)
            {
                int index = balls.Count;

                var ball = CreateBall();
                ball.transform.SetParent(ballsTransform, false);
                ball.transform.localPosition = GetStartPosition(GetColum(index));
                ball.Initialize(queueData.Dequeue());
                ball.target = CalcBallPosition(GetRow(index), GetColum(index));
                balls.Add(ball);

                ball.Draw();
                ball.Skip();
            }
        }

        public Vector3 GetStartPosition(int column)
        {
            float x = (layout.ballSize.x * 0.5f) - (layout.colCount * layout.ballSize.x / 2.0f) - ((layout.colCount - 1) * layout.spacing.x / 2.0f) + layout.offset.x / 2.0f;
            x += column * (layout.ballSize.x + layout.spacing.x);
            return new Vector3(x, layout.ballStartPosY, 0);
        }

        public Vector3 CalcBallPosition(int row, int column)
        {
            float x = (layout.ballSize.x * 0.5f) - (layout.colCount * layout.ballSize.x / 2.0f) - ((layout.colCount - 1) * layout.spacing.x / 2.0f) + layout.offset.x / 2.0f;
            x += column * (layout.ballSize.x + layout.spacing.x);
            float y = (layout.ballSize.y * 0.5f) - (layout.rowCount * layout.ballSize.y / 2.0f) - ((layout.rowCount - 1) * layout.spacing.y / 2.0f) + layout.offset.y / 2.0f;
            y += (layout.rowCount - row - 1) * (layout.ballSize.y + layout.spacing.y);
            return new Vector3(x, y, 0);
        }

        public BallGeneratorInstance Save()
        {
            var go = new GameObject();
    		go.name = "Ball Generator";
    		var snapshot = go.AddComponent<BallGeneratorInstance>();
            snapshot.deck = new List<BallInfo>();

            for (var i = 0; i < balls.Count; i++) {
                snapshot.deck.Add((BallInfo)balls[i].ballInfo.Clone());
            }

    		return snapshot;
        }

        public void Load(BallGeneratorInstance snapshot)
    	{
            for (var i = 0; i < snapshot.deck.Count; i++) {
                deck[i] = (BallInfo)snapshot.deck[i].Clone();
            }

            for (var i = 0; i < balls.Count; i++) {
                balls[i].ballInfo = (BallInfo)deck[i].Clone();
                balls[i].SendEvent("Restore");
                if (balls[i].catchState == CatchState.Catch)
                {
                    balls[i].SendEvent("Hit");
                }
            }
    	}
    }
}
