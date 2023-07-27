using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using ParadoxNotion;
using NodeCanvas.Framework;
using System;

namespace GameStudio.Slot.FSF
{
    [RequireComponent(typeof(DirectionalWeightPositionController))]
    public class FSFWinFrameController : MonoBehaviour
    {
        [SerializeField]
        private BaseSlotMachine slotMachine;
        private DirectionalWeightPositionController positionController;
        private Animator animator;
        private GameObject destAnchor;
        private GameObject startAnchor;
        private int frameMultiplier;

        private Frame currentFrameInfo;


        public Vector3 StartAnchoorPosition { get => startAnchor.transform.position; }
        public Vector3 DestAnchoorPosition { get => destAnchor.transform.position; }
        public Frame CurrentFrameInfo { get => currentFrameInfo; }

        public const int ANIMATOR_SCALE_LAYER_INDEX = 1;

        public const string ON_CONTENT_UI_EVENT = "OnContentUIEvent";
        public const string FRAME_MOVE_EVENT = "MoveFrame";
        public const string FRAME_MOVE_ORDER_DONE_EVENT = "MoveFrameOrderDone";
        public const string FRAME_MOVE_SKIP_ORDER = "MoveFrameSkip";
        public const string FRAME_WIN_ORDER_EVENT = "WinFrame";
        public const string FRAME_WIN_ORDER_DONE_EVENT = "WinFrameEnd";
        public const string FRAME_EXPECTATION_EVENT = "FrameExpectation";
        public const string FRAME_EXPECTATION_FINISH_EVENT = "FinishFrameExpectation";

        public const string SCALE_ANIM_STATE_NAME_FORMAT = "{0}X{1}";
        public const int SCALE_ANIM_LAYER = 1;

        public int FrameMultiplier
        {
            get => frameMultiplier;
        }



        public static IEnumerator CallActionAfterDelay(Action func, float delay)
        {
            yield return new WaitForSeconds(delay);
            func();
        }

        void Awake()
        {
            destAnchor = new GameObject("Win Box Dest Anchor");
            startAnchor = new GameObject("Win Box Start Anchor");
            destAnchor.transform.SetParent(transform.parent);
            startAnchor.transform.SetParent(transform.parent);
            positionController = GetComponent<DirectionalWeightPositionController>();
            animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            frameMultiplier = 0;
            MessageDispatcher.Register(ON_CONTENT_UI_EVENT, OnContentUIEventDelegator);
            if (slotMachine.slotIndex != 0) animator.SetBool("IsSubSlot", true);
        }


        private void OnDisable()
        {
            MessageDispatcher.UnRegister(ON_CONTENT_UI_EVENT, OnContentUIEventDelegator);
        }

        private void OnContentUIEventDelegator(EventData eventData)
        {
            if (eventData.name == FRAME_MOVE_EVENT && gameObject.activeInHierarchy)
            {
                StartFrameMove(GetFrameFromBlackboard(BlackboardUtils.FindValue<IBlackboard>("./spin/response/frame")));
                StartCoroutine(CallActionAfterDelay(() => { MessageDispatcher.Dispatch(ON_CONTENT_UI_EVENT, new EventData(FRAME_MOVE_ORDER_DONE_EVENT)); }, 0.1f));
            }
            else if (eventData.name == FRAME_MOVE_SKIP_ORDER)
            {
                animator.SetTrigger("Skip Move");
                animator.SetTrigger("Skip Scale Change");
                var stateName = string.Format(SCALE_ANIM_STATE_NAME_FORMAT, currentFrameInfo.height, currentFrameInfo.width);
                animator.Play(stateName, SCALE_ANIM_LAYER);
                StopAllCoroutines();
            }
            else if (eventData.name == FRAME_WIN_ORDER_EVENT)
            {
                animator.ResetTrigger("Skip Effect");
                animator.SetTrigger("Win");
                StartCoroutine(CallActionAfterDelay(() => { MessageDispatcher.Dispatch(ON_CONTENT_UI_EVENT, new EventData(FRAME_WIN_ORDER_DONE_EVENT)); }, 0.1f));
            }
            else if (eventData.name == FRAME_EXPECTATION_EVENT)
            {
                animator.SetBool("Expectation",true);
            }
            else if (eventData.name == FRAME_EXPECTATION_FINISH_EVENT)
            {
                animator.SetBool("Expectation", false);
            }

        }

        public static Frame GetFrameFromBlackboard(IBlackboard bb)
        {
            int width = BlackboardUtils.FindVariable<int>(bb, "width").value;
            int height = BlackboardUtils.FindVariable<int>(bb, "height").value;
            int targetCol = BlackboardUtils.FindVariable<int>(bb, "position/column").value;
            int targetRow = BlackboardUtils.FindVariable<int>(bb, "position/row").value;

            Frame frame;
            frame.width = width;
            frame.height = height;
            frame.column = targetCol;
            frame.row = targetRow;

            return frame;
        }

        public void StartFrameMove(Frame frame, int multiplier = 0)
        {
            currentFrameInfo = frame;
            animator.SetInteger("Multiplier", multiplier);
            animator.ResetTrigger("Skip Move");
            animator.ResetTrigger("Skip Scale Change");
            animator.SetTrigger("Skip Effect");

            var stateName = string.Format(SCALE_ANIM_STATE_NAME_FORMAT, frame.height, frame.width);
            animator.CrossFadeInFixedTime(stateName, 0.5f);

            SetStartDestAnchors(frame.column, frame.row);
            animator.SetTrigger("Move");
            animator.SetInteger("Width", frame.width);
            animator.SetInteger("Height", frame.height);
        }


        private void SetStartDestAnchors(int destCol, int destRow)
        {
            var targetReel = slotMachine.reels[destCol];
            Vector3 targetPosition = targetReel.CalcSymbolPosition(targetReel.beginColumn, targetReel.beginRow, destCol, destRow);
            targetPosition = targetReel.transform.TransformVector(targetPosition);
            targetPosition = targetReel.transform.position + targetPosition;
            destAnchor.transform.position = targetPosition;
            startAnchor.transform.position = transform.position;

            positionController.from = startAnchor.transform;
            positionController.to = destAnchor.transform;
        }
    }

    public struct Frame
    {
        public int column;
        public int row;
        public int width;
        public int height;
    }
}