﻿using STROOP.Structs;
using STROOP.Structs.Configurations;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace STROOP.Forms
{
    public partial class SelectionForm : Form
    {
        public SelectionForm(
            string selectionText,
            string buttonText,
            List<string> items,
            Action<string> selectionAction)
        {
            InitializeComponent();
            textBoxSelect.Text = selectionText;
            buttonSet.Text = buttonText;
            listBoxSelections.DataSource = items;
            buttonSet.Click += (sender, e) =>
            {
                string selection = listBoxSelections.SelectedItem.ToString();
                selectionAction(selection);
                Close();
            };
        }
        
        public static void ShowActionSelectionForm()
        {
            SelectionForm selectionForm = new SelectionForm(
                "Select an Action",
                "Set Action",
                TableConfig.MarioActions.GetActionNameList(),
                actionName =>
                {
                    uint? action = TableConfig.MarioActions.GetActionFromName(actionName);
                    if (action.HasValue)
                        Config.Stream.SetValue(action.Value, MarioConfig.StructAddress + MarioConfig.ActionOffset);
                });
            selectionForm.Show();
        }

        public static void ShowAnimationSelectionForm()
        {
            SelectionForm selectionForm = new SelectionForm(
                "Select an Animation",
                "Set Animation",
                TableConfig.MarioAnimations.GetAnimationNameList(),
                animationName =>
                {
                    int? animation = TableConfig.MarioAnimations.GetAnimationFromName(animationName);
                    if (animation.HasValue)
                    {
                        uint marioObjRef = Config.Stream.GetUInt32(MarioObjectConfig.PointerAddress);
                        Config.Stream.SetValue((short)animation.Value, marioObjRef + MarioObjectConfig.AnimationOffset);
                    }
                });
            selectionForm.Show();
        }
    }
}