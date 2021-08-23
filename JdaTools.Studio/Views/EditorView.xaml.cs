﻿using ICSharpCode.AvalonEdit.CodeCompletion;
using JdaTools.Studio.Models;
using JdaTools.Studio.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Caliburn.Micro;

namespace JdaTools.Studio.Views
{
    /// <summary>
    /// Interaction logic for QueryView.xaml
    /// </summary>
    public partial class EditorView : UserControl
    {
        public EditorView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            using var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("JdaTools.Studio.Resources.sql.xshd");
            using var reader = new System.Xml.XmlTextReader(stream);
            QueryTextBox.SyntaxHighlighting =
                ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader.Load(reader,
                ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance);
            QueryTextBox.TextArea.TextEntered += textEditor_TextArea_TextEntered;
            QueryTextBox.TextArea.TextEntering += textEditor_TextArea_TextEntering;
        }
        CompletionWindow completionWindow;
        private void textEditor_TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            if (completionWindow != null && !completionWindow.CompletionList.CompletionData.Any())
            {
                completionWindow?.Close();
            }
            var fullText = QueryTextBox.TextArea.Document.Text;
            var textToCursor = fullText.ToLower().Substring(0,QueryTextBox.CaretOffset);
            var backBlocks = Regex.Split(fullText.ToLower().Substring(0, QueryTextBox.CaretOffset - 1), @"\s+");
            var forwardBlocks = Regex.Split(fullText.ToLower().Substring(QueryTextBox.CaretOffset), @"\s+");
            var schema = IoC.Get<SchemaExplorer>();
            var lastWord = backBlocks.Last();

            if (textToCursor.LastIndexOf(']') >= textToCursor.LastIndexOf('['))
            {
                return; //TODO hand autocomplete with spaces
                //Autocomplet MOCA
                if (completionWindow?.IsVisible ?? false)
                {
                    return;
                }
                if (lastWord.Length < 3)
                {
                    return;
                }
                if (completionWindow == null)
                {
                    completionWindow = new CompletionWindow(QueryTextBox.TextArea);
                }

                var commands = schema.Commands.Select(c => c.CommandName).Distinct()
                    .Where(c => c.StartsWith(lastWord, StringComparison.InvariantCultureIgnoreCase));
                IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                foreach (var command in commands)
                {
                    if (!data.Select(d => d.Text).Contains(command))
                    {
                        data.Add(new MyCompletionData(command));
                    }
                }
                completionWindow.Show();
                completionWindow.Closed += delegate {
                    completionWindow = null;
                };
            }

            if (e.Text == " ")
            {
                
                

                //Last position of [ or ] lets us know if we are working sql or moca
                
                lastWord = lastWord.Replace("[", "");
                if (lastWord == "select" || lastWord.LastOrDefault() == ',')
                {
                    var fromPosition = Array.IndexOf(forwardBlocks, "from");
                    if (fromPosition < 0)
                    {
                        //No from statement
                        return;
                    }
                    if (forwardBlocks.Length < fromPosition)
                    {
                        //No table to parse
                        return;
                    }
                    //TODO check joins
                    var tableName = forwardBlocks[fromPosition + 1];
                    // Open code completion after the user has pressed dot:
                    var columns = schema.Tables.FirstOrDefault(t => t.TableName.Equals(tableName, StringComparison.InvariantCultureIgnoreCase))?
                        .Columns.Select(c => c.ColumnName);
                    if (columns == default)
                    {
                        return;
                    }
                    completionWindow = new CompletionWindow(QueryTextBox.TextArea);
                    IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                    foreach (var col in columns)
                    {
                        data.Add(new MyCompletionData(col));
                    }
                    completionWindow.Show();
                    completionWindow.Closed += delegate {
                        completionWindow = null;
                    };
                }
                else if (lastWord.Equals("from",StringComparison.InvariantCultureIgnoreCase))
                {
                    completionWindow = new CompletionWindow(QueryTextBox.TextArea);
                    IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                    
                    var completionData = schema.Tables.Select(t => new MyCompletionData(t.TableName)).ToList();
                    foreach (var item in completionData)
                    {
                        data.Add(item);
                    }
                    completionWindow.Show();
                    completionWindow.Closed += delegate {
                        completionWindow = null;
                    };
                }
            }
        }

        void textEditor_TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    // Whenever a non-letter is typed while the completion window is open,
                    // insert the currently selected element.
                    completionWindow.CompletionList.RequestInsertion(e);
                }
            }
            // Do not set e.Handled=true.
            // We still want to insert the character that was typed.
        }

        private void ResultsGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            string header = e.Column.Header.ToString();
            // Replace all underscores with two underscores, to prevent AccessKey handling
            var type = e.PropertyType;
            //header = header + $" ({type.Name})";
            e.Column.Header = header.Replace("_", "__");

        }
        //TODO: Not wo
        private void QueryTextBox_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var editor = (Control)sender;
            if (editor.IsEnabled)
            {
                editor.Focus();
            }            
        }
    }
}