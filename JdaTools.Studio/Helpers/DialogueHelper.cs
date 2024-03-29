﻿using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.SimpleChildWindow;

namespace JdaTools.Studio.Helpers
{
    public static class DialogueHelper
    {
        public static MetroWindow MainWindow => (MetroWindow) Application.Current.MainWindow;
        public static Task ShowChildWindowAsync(ChildWindow childWindow) =>
            MainWindow.ShowChildWindowAsync(childWindow);

        public static async Task<string> ShowInputDialogue(string title, string message)
        {

            return await MainWindow.ShowInputAsync(title, message);
        }

        public static async Task<bool> ShowDialogueYesNo(string title, string message)
        {
            var result = await MainWindow.ShowMessageAsync(title, message, MessageDialogStyle.AffirmativeAndNegative,
                new MetroDialogSettings
                {
                    AffirmativeButtonText = "YES",
                    NegativeButtonText = "NO",
                    DefaultButtonFocus = MessageDialogResult.Negative
                });
            return result == MessageDialogResult.Affirmative;
        }
    }
}