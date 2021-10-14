// Project:         Paints And Dyes mod for Daggerfall Unity (http://www.dfworkshop.net)
// Copyright:       Copyright (C) 2021 Ralzar
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Author:          Ralzar

using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using UnityEngine;
using System;
using Wenzil.Console;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings;
using System.Linq;

namespace PaintsAndDyes
{
    public class PaintsAndDyes : MonoBehaviour
    {
        static Mod mod;

        static PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;


        [Invoke(StateManager.StateTypes.Start, 0)]
        public static void Init(InitParams initParams)
        {
            mod = initParams.Mod;
            var go = new GameObject(mod.Title);
            go.AddComponent<PaintsAndDyes>();

            DaggerfallUnity.Instance.ItemHelper.RegisterCustomItem(Paint.templateIndex, ItemGroups.UselessItems2, typeof(Paint));
            DaggerfallUnity.Instance.ItemHelper.RegisterCustomItem(Dye.templateIndex, ItemGroups.UselessItems2, typeof(Dye));
        }

        void Awake()
        {
            mod.IsReady = true;
            Debug.Log("[SkillBooks] Ready");
        }

        // Start is called before the first frame update
        void Start()
        {
            RegisterSBCommands();
        }

        // Update is called once per frame
        void Update()
        {

        }


        public static void RegisterSBCommands()
        {
            Debug.Log("[PaintsAndDyes] Trying to register console commands.");
            try
            {
                ConsoleCommandsDatabase.RegisterCommand(AddPaint.command, AddPaint.description, AddPaint.usage, AddPaint.Execute);
                ConsoleCommandsDatabase.RegisterCommand(AddDye.command, AddDye.description, AddDye.usage, AddDye.Execute);
            }
            catch (Exception e)
            {
                Debug.LogError(string.Format("Error Registering SkillBooks Console commands: {0}", e.Message));
            }
        }

        private static class AddPaint
        {
            public static readonly string command = "add_paint";
            public static readonly string description = "Put basic paint in players inventory";
            public static readonly string usage = "add_paint (color of paint)";

            public static string Execute(params string[] args)
            {
                int value = -1;
                if (args.Length < 1)
                    return usage;
                if (!int.TryParse(args[0], out value))
                    return string.Format("Could not parse argument `{0}` to a number", args[0]);
                if (value < 0 || value > 10)
                    return string.Format("Index {0} is out range. Must be 0-10.", value);
                if (args == null || args.Length < 1 || args.Length > 1)
                    return usage;
                else if (args.Length == 1 && value < 11)
                {
                    if (value < 2)
                        value += 15;
                    else if (value == 3)
                        value = 19;
                    else
                        value += 16;

                    Paint.color = (DyeColors)value;
                    DaggerfallUnityItem paintBottle = ItemBuilder.CreateItem(ItemGroups.UselessItems2, Paint.templateIndex);
                    GameManager.Instance.PlayerEntity.Items.AddItem(paintBottle);
                }

                return ((DyeColors)value).ToString() + " Paint added";
            }
        }

        private static class AddDye
        {
            public static readonly string command = "add_dye";
            public static readonly string description = "Put basic dye in players inventory";
            public static readonly string usage = "add_dye (number of dye)";

            public static string Execute(params string[] args)
            {
                int value = -1;
                if (args.Length < 1)
                    return usage;
                if (!int.TryParse(args[0], out value))
                    return string.Format("Could not parse argument `{0}` to a number", args[0]);
                if (value < 0 || value > 9)
                    return string.Format("Index {0} is out range. Must be 0-9.", value);
                if (args == null || args.Length < 1 || args.Length > 1)
                    return usage;
                else if (args.Length == 1 && value < 10)
                {
                    Dye.color = (DyeColors)value;
                    DaggerfallUnityItem dyeBottle = ItemBuilder.CreateItem(ItemGroups.UselessItems2, Dye.templateIndex);
                    GameManager.Instance.PlayerEntity.Items.AddItem(dyeBottle);
                }

                return ((DyeColors)value).ToString() + " Dye added";
            }
        }
    }
}
