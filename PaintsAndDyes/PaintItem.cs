// Project:         Paints And Dyes mod for Daggerfall Unity (http://www.dfworkshop.net)
// Copyright:       Copyright (C) 2021 Ralzar
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Author:          Ralzar

using UnityEngine;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using System.Collections.Generic;
using DaggerfallWorkshop.Game.UserInterface;

namespace PaintsAndDyes
{
    public class Paint: DaggerfallUnityItem
    {
        static PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
        static UserInterfaceManager uiManager = DaggerfallUI.Instance.UserInterfaceManager;
        List<DaggerfallUnityItem> validPaintItems = new List<DaggerfallUnityItem>();

        public const int templateIndex = 555;
        public static DyeColors color = DyeColors.Blue;

        static DaggerfallUnityItem paintBeingUsed = null;

        public Paint() : base(ItemGroups.UselessItems2, templateIndex)
        {
            //Armor Dyes
            //Iron = 15,
            //Steel = 16,
            //Chain = 18, // This enum kept for compatibility with older saves
            //Unchanged = 18,
            //SilverOrElven = 18, // This enum kept for compatibility with older saves
            //Silver = 18,
            //Elven = 19,
            //Dwarven = 20,
            //Mithril = 21,
            //Adamantium = 22,
            //Ebony = 23,
            //Orcish = 24,
            //Daedric = 25,
            int roll = Random.Range(15, 26);


            if (color != DyeColors.Blue)
                dyeColor = color;
            else if (roll == 17)
                dyeColor = DyeColors.Silver;
            else
                dyeColor = (DyeColors)roll;
            color = DyeColors.Blue;

            shortName = PaintName(dyeColor) + " Paint";
        }

        public override string ItemName
        {
            get { return PaintName(dyeColor) + " Paint"; }
        }

        public override string LongName
        {
            get { return PaintName(dyeColor) + " Paint"; }
        }

        public override bool UseItem(ItemCollection collection)
        {
            //code for listpicker of worn plate
            if (GameManager.Instance.AreEnemiesNearby())
            {
                DaggerfallUI.MessageBox("Can't use that with enemies around.");
                return true;
            }

            paintBeingUsed = this;
            DaggerfallListPickerWindow validItemPicker = new DaggerfallListPickerWindow(uiManager, uiManager.TopWindow);
            validItemPicker.OnItemPicked += Paint_OnItemPicked;
            validPaintItems.Clear();

            for (int i = 0; i < playerEntity.Items.Count; i++)
            {
                DaggerfallUnityItem item = playerEntity.Items.GetItem(i);
                if (item.ItemGroup == ItemGroups.Armor && item.nativeMaterialValue != 0)
                {
                    validPaintItems.Add(item);
                    string validItemName = item.LongName;
                    if (item.IsEquipped)
                        validItemPicker.ListBox.AddItem(validItemName + " (equipped)");
                    else
                        validItemPicker.ListBox.AddItem(validItemName);
                }
            }

            if (validItemPicker.ListBox.Count > 0)
                uiManager.PushWindow(validItemPicker);
            else
                DaggerfallUI.MessageBox("You have no armor or weapons to paint.");

            return true;
        }

        public void Paint_OnItemPicked(int index, string itemName)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            DaggerfallUI.UIManager.PopWindow();

            validPaintItems[index].dyeColor = paintBeingUsed.dyeColor;
            playerEntity.Items.RemoveItem(paintBeingUsed);

            DaggerfallUI.Instance.InventoryWindow.Refresh();
        }

        public string PaintName(DyeColors color)
        {
            if (color == DyeColors.SilverOrElven)
                return "Silver";
            else if (color == DyeColors.Chain)
                return "Silver";
            else
                return color.ToString();
        }

        public override ItemData_v1 GetSaveData()
        {
            ItemData_v1 data = base.GetSaveData();
            data.className = typeof(Paint).ToString();
            return data;
        }



    }
}
