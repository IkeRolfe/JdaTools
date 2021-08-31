using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace JdaTools.Studio.Controls
{
    public class MocaEditor : MvvmTextEditor
    {
        private List<string> _mocaCommands = new List<string>();
        private CodeSyntax _currentSyntax;

        public MocaEditor()
        {
            TextArea.TextEntered += DoAutoComplete;
            TextArea.Caret.PositionChanged += CaretOnPositionChanged;
        }

        private void CaretOnPositionChanged(object? sender, EventArgs e)
        {
            //Get current syntax
            var syntax = GetSyntaxAt(CaretOffset);
            if (CurrentSyntax != syntax) CurrentSyntax = syntax;
        }

        private void DoAutoComplete(object sender, TextCompositionEventArgs e)
        {
            
        }


        public static readonly DependencyProperty CurrentSyntaxProperty =
            DependencyProperty.Register("CurrentSyntax", typeof(CodeSyntax), typeof(MvvmTextEditor),
                new PropertyMetadata((obj, args) =>
                {
                    MocaEditor target = (MocaEditor)obj;
                    target.SelectionStart = (int)args.NewValue;
                }));
        public CodeSyntax CurrentSyntax
        {
            get => _currentSyntax;
            private set
            {
                _currentSyntax = value;
                SetValue(CurrentSyntaxProperty, value);
            }
        }

        private CodeSyntax GetSyntaxAt(int caretOffset)
        {
            var syntax = CodeSyntax.MOCA;
            var text = TextArea.Document.Text;
            int mocaStart = 0;
            int mocaEnd = 0;
            if (text.Trim().FirstOrDefault() == '<')
            {
                //Look for 
                var searchString = "<![CDATA[";
                mocaStart = text.IndexOf(searchString, StringComparison.InvariantCultureIgnoreCase) + searchString.Length;
                mocaEnd = text.IndexOf("</local-syntax>", StringComparison.InvariantCultureIgnoreCase);
            }
            if (caretOffset <= mocaStart || caretOffset >= mocaEnd)
            {
                return CodeSyntax.UNDEFINED;
            }

            char lastChar = ' ';
            foreach (var currentChar in text.Substring(mocaStart,caretOffset-mocaStart))
            {
                if (currentChar == '[')
                {
                    switch (syntax)
                    {
                        case CodeSyntax.MOCA:
                            syntax = CodeSyntax.SQL;
                            break;
                        case CodeSyntax.SQL:
                            if (lastChar == '[')
                            {
                                syntax = CodeSyntax.GROOVY;
                            }
                            break;
                    }
                }
                //Section Ends
                //Section starts
                if (currentChar == ']')
                {
                    switch (syntax)
                    {
                        case CodeSyntax.SQL:
                            syntax = CodeSyntax.MOCA;
                            break;
                        case CodeSyntax.GROOVY:
                            if (lastChar == ']')
                            {
                                syntax = CodeSyntax.MOCA;
                            }
                            break;
                    }
                }

                lastChar = currentChar;
            }
            return syntax;
        }

        private void AddMocaCommand(string command)
        {
            _mocaCommands.Add(command);
        }
    }

    

    public enum CodeSyntax
    {
        UNDEFINED,
        MOCA,
        SQL,
        GROOVY,
        XML
    }
}