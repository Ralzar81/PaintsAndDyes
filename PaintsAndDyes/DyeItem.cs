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
    public class Dye : DaggerfallUnityItem
    {
        static PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
        static UserInterfaceManager uiManager = DaggerfallUI.Instance.UserInterfaceManager;
        List<DaggerfallUnityItem> validDyeItems = new List<DaggerfallUnityItem>();


        public const int templateIndex = 556;
        public static DyeColors color = DyeColors.Iron;

        static DaggerfallUnityItem dyeBeingUsed = null;

        public Dye() : base(ItemGroups.UselessItems2, templateIndex)
        {

            // Clothing dyes
            //Blue = 0,
            //Grey = 1,
            //Red = 2,
            //DarkBrown = 3,
            //Purple = 4,
            //LightBrown = 5,
            //White = 6,
            //Aquamarine = 7,
            //Yellow = 8,
            //Green = 9,
            if (color != DyeColors.Iron)
                dyeColor = color;
            else
                dyeColor = (DyeColors)Random.Range(0, 10);
            color = DyeColors.Iron;

            shortName = DyeName(dyeColor) + " Dye";
        }

        public override string ItemName
        {
            get { return DyeName(dyeColor) + " Dye"; }
        }

        public override string LongName
        {
            get { return DyeName(dyeColor) + " Dye"; }
        }

        public override bool UseItem(ItemCollection collection)
        {
            //code for listpicker of worn clothesS
            if (GameManager.Instance.AreEnemiesNearby())
            {
                DaggerfallUI.MessageBox("Can't use that with enemies around.");
                return true;
            }

            dyeBeingUsed = this;
            DaggerfallListPickerWindow validItemPicker = new DaggerfallListPickerWindow(uiManager, uiManager.TopWindow);
            validItemPicker.OnItemPicked += Dye_OnItemPicked;
            validDyeItems.Clear();

            for (int i = 0; i < playerEntity.Items.Count; i++)
            {
                DaggerfallUnityItem item = playerEntity.Items.GetItem(i);
                if (item.IsClothing)
                {
                    validDyeItems.Add(item);
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
                DaggerfallUI.MessageBox("You have no clothing to dye.");

            return true;
        }

        public void Dye_OnItemPicked(int index, string itemName)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            DaggerfallUI.UIManager.PopWindow();

            validDyeItems[index].dyeColor = dyeBeingUsed.dyeColor;
            playerEntity.Items.RemoveItem(dyeBeingUsed);

            DaggerfallUI.Instance.InventoryWindow.Refresh();


        }

        public string DyeName(DyeColors color)
        {
            if (color == DyeColors.DarkBrown)
                return "Dark Brown";
            else if (color == DyeColors.LightBrown)
                return "Light Brown";
            else
            return color.ToString();
        }

        public override ItemData_v1 GetSaveData()
        {
            ItemData_v1 data = base.GetSaveData();
            data.className = typeof(Dye).ToString();
            return data;
        }


    }
}
