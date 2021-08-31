﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Search;

namespace JdaTools.Studio.Controls
{
    /// <summary>
    /// Class that inherits from the AvalonEdit TextEditor control to 
    /// enable MVVM interaction. 
    /// </summary>
    public class MvvmTextEditor : TextEditor, INotifyPropertyChanged
    {
        /// <summary>
        /// Default constructor to set up event handlers.
        /// </summary>
        public MvvmTextEditor()
        {
            SearchPanel.Install(this); //Enable search with ctrl+f
            TextArea.SelectionChanged += TextArea_SelectionChanged;
        }

        /// <summary>
        /// Event handler to update properties based upon the selection changed event.
        /// </summary>
        void TextArea_SelectionChanged(object sender, EventArgs e)
        {
            this.SelectionStart = SelectionStart;
            this.SelectionLength = SelectionLength;
        }

        #region Text.
        /// <summary>
        /// Dependancy property for the editor text property binding.
        /// </summary>
        public static readonly DependencyProperty TextProperty =
             DependencyProperty.Register("Text", typeof(string), typeof(MvvmTextEditor),
             new PropertyMetadata((obj, args) =>
             {
                 MvvmTextEditor target = (MvvmTextEditor)obj;
                 target.Text = (string)args.NewValue;
             }));

        /// <summary>
        /// Provide access to the Text.
        /// </summary>
        public new string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        /// <summary>
        /// Return the current text length.
        /// </summary>
        public int Length => base.Text.Length;

        /// <summary>
        /// Override of OnTextChanged event.
        /// </summary>
        protected override void OnTextChanged(EventArgs e)
        {
            RaisePropertyChanged(nameof(Length));
            base.OnTextChanged(e);
        }
        #endregion // Text.

        #region Caret Offset.
        /// <summary>
        /// DependencyProperty for the TextEditorCaretOffset binding. 
        /// </summary>
        public static DependencyProperty CaretOffsetProperty =
            DependencyProperty.Register("CaretOffset", typeof(int), typeof(MvvmTextEditor),
            new PropertyMetadata((obj, args) =>
            {
                MvvmTextEditor target = (MvvmTextEditor)obj;
                target.CaretOffset = (int)args.NewValue;
            }));

        /// <summary>
        /// Provide access to the CaretOffset.
        /// </summary>
        public new int CaretOffset
        {
            get => base.CaretOffset;
            set => base.CaretOffset = value;
        }
        #endregion // Caret Offset.

        #region Selection.
        /// <summary>
        /// DependencyProperty for the TextEditor SelectionLength property. 
        /// </summary>
        public static readonly DependencyProperty SelectionLengthProperty =
             DependencyProperty.Register("SelectionLength", typeof(int), typeof(MvvmTextEditor),
             new PropertyMetadata((obj, args) =>
             {
                 MvvmTextEditor target = (MvvmTextEditor)obj;
                 target.SelectionLength = (int)args.NewValue;
             }));

        /// <summary>
        /// Access to the SelectionLength property.
        /// </summary>
        public new int SelectionLength
        {
            get => base.SelectionLength;
            set => SetValue(SelectionLengthProperty, value);
        }

        /// <summary>
        /// DependencyProperty for the TextEditor SelectionStart property. 
        /// </summary>
        public static readonly DependencyProperty SelectionStartProperty =
             DependencyProperty.Register("SelectionStart", typeof(int), typeof(MvvmTextEditor),
             new PropertyMetadata((obj, args) =>
             {
                 MvvmTextEditor target = (MvvmTextEditor)obj;
                 target.SelectionStart = (int)args.NewValue;
             }));

        /// <summary>
        /// Access to the SelectionStart property.
        /// </summary>
        public new int SelectionStart
        {
            get => base.SelectionStart;
            set => SetValue(SelectionStartProperty, value);
        }
        #endregion // Selection.

        /// <summary>
        /// Implement the INotifyPropertyChanged event handler.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName] string caller = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));
        }
    }
}