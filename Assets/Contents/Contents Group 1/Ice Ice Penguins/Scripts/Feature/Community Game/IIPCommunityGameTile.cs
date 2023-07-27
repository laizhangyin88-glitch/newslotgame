using System.Collections;
using System.Collections.Generic;
using BagelCode.ClientModels;
using TMPro;
using UnityEngine;


namespace GameStudio.Slot.IIP.Feature
{
    public enum TileKind
    {
        NONE = -1,
        SEA_WATER = 0,
        ICE = 1,
        PENGUIN_SEA_WATER = 2,
        PENGUIN_ICE = 3,
        PENGUIN_BOSS = 4,
        TREASURE_SEA_WATER = 5,
        TREASURE_ICE = 6,
    }

    public enum TileFloorStatus
    {
        UNABLE = 0,
        DARK = 1,
        WATER = 2,
        ICE = 3,
    }

    public enum TileObjectStatus
    {
        NONE = 0,
        PENGUIN = 1,
        TREASURE = 2,
    }

    public class IIPCommunityGameTile : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        //public TileKind tileKind { get; private set; }

        // level 1 is Normal Tile
        // level 2 is Dark Tile
        // level 3 is Unable Tile
        public int tileLevel;

        public TileFloorStatus tileFloorStatus { get; private set; } = TileFloorStatus.UNABLE;
        public TileObjectStatus tileObjectStatus { get; private set; } = TileObjectStatus.NONE;
        public IIPCommunityGamePenguin penguin;
        public Transform penguinAnchor;
        public bool isObjectOpened = false;
        private int glowCount = 0;
        [SerializeField] private TextMeshProUGUI valueText;

        public void Initialize(TileObjectStatus tileObject, int level)
        {
            tileLevel = level;
            isObjectOpened = false;
            glowCount = 0;
            if (level == 1) UpdateFloor(TileFloorStatus.WATER, true);
            else if (level == 2) UpdateFloor(TileFloorStatus.DARK, true);
            else if (level == 3) UpdateFloor(TileFloorStatus.UNABLE, true);

            tileObjectStatus = tileObject;
            if (penguin != null)
                penguin.transform.localPosition = Vector3.zero;
            if (tileObject == TileObjectStatus.PENGUIN)
                EnableObject(tileObject);
        }


        public void AddGlowStatus(int amount)
        {
            glowCount += amount;
            if (glowCount > 0)
                animator.SetBool("Glow", true);
            else animator.SetBool("Glow", false);

        }
        public void EnableTileObject(TileObjectStatus status)
        {
            Debug.Assert(tileObjectStatus == TileObjectStatus.NONE);

            tileObjectStatus = status;
        }

        public void EnableObject(TileObjectStatus status)
        {
            if (status == TileObjectStatus.PENGUIN)
            {
                animator.SetBool("Penguin", true);
                isObjectOpened = true;
            }
            else if (status == TileObjectStatus.TREASURE)
            {
                animator.SetBool("Treasure", true);
                isObjectOpened = true;
            }
        }

        public void DisableObject(TileObjectStatus status)
        {
            if (status == TileObjectStatus.PENGUIN)
            {
                animator.SetBool("Penguin", false);
            }
            else if (status == TileObjectStatus.TREASURE)
            {
                animator.SetBool("Treasure", false);
            }
        }
        public void SetValueEnable(bool isActive) => animator.SetBool("Value", isActive);

        public void SetValueText(string text) => valueText.text = text;

        public void UpdateFloor(TileFloorStatus status, bool force)
        {
            if (force == true)
            {
                if (status == TileFloorStatus.DARK)
                    animator.SetTrigger("ForceDarkWater");
                else if (status == TileFloorStatus.UNABLE)
                    animator.SetTrigger("ForceUnableWater");
                else if (status == TileFloorStatus.ICE)
                    animator.SetTrigger("ForceIceTile");
                else if (status == TileFloorStatus.WATER)
                    animator.SetTrigger("ForceTileWater");

                tileFloorStatus = status;
            }
            else
            {
                if (tileFloorStatus == TileFloorStatus.UNABLE && status == TileFloorStatus.DARK)
                {
                    animator.SetTrigger("UnableToDark");
                    tileFloorStatus = status;
                }
                else if (tileFloorStatus == TileFloorStatus.DARK && status == TileFloorStatus.WATER)
                {
                    animator.SetTrigger("DarkToWater");
                    tileFloorStatus = status;
                }
                else if (tileFloorStatus == TileFloorStatus.WATER && status == TileFloorStatus.ICE)
                {
                    animator.SetTrigger("WaterToIce");
                    tileFloorStatus = status;
                }
                else Debug.LogError("Error Transition");
            }
        }
    }
}