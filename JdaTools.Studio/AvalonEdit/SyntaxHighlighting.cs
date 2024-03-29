﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using Caliburn.Micro;
using ICSharpCode.AvalonEdit.Highlighting;
using JdaTools.Studio.Services;

namespace JdaTools.Studio.AvalonEdit
{
    public class MocaHighlightingDefinition : IHighlightingDefinition
    {
        public HighlightingRuleSet GetNamedRuleSet(string name)
        {
            throw new System.NotImplementedException();
        }

        public HighlightingColor GetNamedColor(string name)
        {
            throw new System.NotImplementedException();
        }
        public string Name { get; } = "MOCA";
        public HighlightingRuleSet MainRuleSet { get; private set; }
        public IEnumerable<HighlightingColor> NamedHighlightingColors { get; }
        public IDictionary<string, string> Properties { get; }

        private readonly HighlightingRuleSet _mocaRuleSet;
        private readonly HighlightingRuleSet _commandRuleSet;
        private bool _isCommandFile;

        public bool IsCommandFile
        {
            get => _isCommandFile;
            set
            {
                if (value == _isCommandFile)
                {
                    return;
                }
                MainRuleSet = value ? _commandRuleSet : _mocaRuleSet;
                _isCommandFile = value;
            }
        }


        public MocaHighlightingDefinition(bool isCommandFile = false)
        {
            _commandRuleSet = new HighlightingRuleSet
            {
                Spans =
                {
                    new HighlightingSpan
                    {
                        RuleSet = MocaHighlightResources.MocaRuleSet,
                        SpanColorIncludesEnd = false,
                        SpanColorIncludesStart = false,
                        StartExpression = new Regex("CDATA\\["),
                        EndExpression = new Regex("</local-syntax>")
                    }
                }
            };
            _mocaRuleSet = MocaHighlightResources.MocaRuleSet;
            MainRuleSet = isCommandFile ? _commandRuleSet : _mocaRuleSet;
            var commands = IoC.Get<SchemaExplorer>().Commands?.Select(c => c.CommandName);
            if (commands != null)
            {
                SetCommands(commands);
            }
            
        }
        //Called after connecting
        public void SetCommands(IEnumerable<string> commands) //TODO Manage reconnects and not duplicating
        {
            _mocaRuleSet.Rules.Add(new HighlightingRule
            {
                Color = MocaHighlightResources.CommandColor,
                Regex = MocaHighlightResources.GenerateKeywordRegEx(commands.ToArray())
            });
        }
    }

    public static class MocaHighlightResources
    {
        public static HighlightingColor StringColor { get; } = GetHighlightingColor(Colors.DarkOrange);
        public static HighlightingColor CommentColor { get; } = GetHighlightingColor(Colors.Green);
        public static HighlightingColor KeyWordColor { get; } = GetHighlightingColor(Colors.MediumPurple);
        public static HighlightingColor XmlTagColor { get; } = GetHighlightingColor(Colors.Gray);
        public static HighlightingColor CommandColor { get; } = GetHighlightingColor(Colors.CornflowerBlue);
        public static HighlightingSpan CommentSpan { get; } = BuildSpan("--|//", "$", CommentColor, true, true, "Comments");
        public static HighlightingSpan BlockCommentSpan { get; } = BuildSpan("/\\*", "\\*/", CommentColor, true, true, "Comments");
        public static HighlightingSpan DoubleQuotesSpan { get; } = BuildSpan("\"", "\"", StringColor, true, true, "Strings");
        public static HighlightingSpan SingleQuotesSpan { get; } = BuildSpan("'", "'", StringColor, true, true, "Strings");


        public static HighlightingRule XmlTagRule { get; } = new HighlightingRule
        {
            Regex = new Regex(@"</?(.*?\w)/?>"),
            Color = XmlTagColor
        };

        public static HighlightingRule VariableRule { get; } = new HighlightingRule
        {
            Color = new HighlightingColor
            {
                FontWeight = FontWeights.Bold
            },
            Regex = new Regex("@(.*?\\w)\\b")
        };

        public static HighlightingRuleSet MocaRuleSet { get; } = new HighlightingRuleSet
        {
            Name = "MOCA",
            Rules =
            {
                XmlTagRule,
                VariableRule
            },
            Spans =
            {
                CommentSpan,
                BlockCommentSpan,
                SingleQuotesSpan,
                DoubleQuotesSpan,
                SqlSpan
            }
        };


        public static HighlightingSpan SqlSpan
        {
            get
            {
                var span = new HighlightingSpan()
                {
                    //Sql between [] need but commands are nested in CDATA xml element where we don't want to detect sql
                    StartExpression = new Regex("(?<!CDATA)(?<!<!)\\["), 
                    EndExpression = new Regex("\\]"),
                };
                span.RuleSet = new HighlightingRuleSet
                {
                    Name = "SQL",
                    Spans =
                    {
                        BlockCommentSpan,
                        CommentSpan,
                        SingleQuotesSpan,
                    },
                    Rules =
                    {
                        new HighlightingRule
                        {
                            Color = KeyWordColor,
                            Regex = GenerateKeywordRegEx(Keywords.SqlKeywords)
                        },
                        VariableRule
                    }
                };
                return span;
            }
        }

        

        public static Regex GenerateKeywordRegEx(string[] keywords)
        {
            //Order by desc to avoid partial matches
            return new Regex(string.Join('|',
                    keywords.OrderByDescending(w => w.Length)
                        .Select(w => $"\\b{w}\\b")),
                RegexOptions.IgnoreCase);
        }

        public static HighlightingColor GetHighlightingColor(Color color) => new HighlightingColor
        {
            Foreground = new SimpleHighlightingBrush(color)
        };

        private static HighlightingSpan BuildSpan(string startRegex,
            string endRegex,
            HighlightingColor highlightingColor,
            bool colorStart = true,
            bool colorEnd = true, string name = null
        ) =>
            BuildSpan(new Regex(startRegex), new Regex(endRegex), highlightingColor, colorStart, colorEnd, name);

        private static HighlightingSpan BuildSpan(Regex startRegex,
            Regex endRegex,
            HighlightingColor highlightingColor,
            bool colorStart = true,
            bool colorEnd = true,
            string name = null) =>
            new()
            {
                SpanColor = highlightingColor,
                StartExpression = startRegex,
                EndExpression = endRegex,
                SpanColorIncludesStart = colorStart,
                SpanColorIncludesEnd = colorEnd,
                RuleSet = new HighlightingRuleSet
                {
                    Name = name
                }
            };
    }

    public static class Keywords
    {
        public static readonly string[] SqlKeywords = new string[] { "ABORT", "BETWEEN", "CRASH", "DIGITS", "ACCEPT", "BINARY_INTEGER", "CREATE", "DISPOSE", "ACCESS", "BODY", "CURRENT", "DISTINCT", "ADD", "BOOLEAN", "CURRVAL", "DO", "ALL", "BY", "CURSOR", "DROP", "ALTER", "CASE", "DATABASE", "ELSE", "AND", "CHAR", "DATA_BASE", "ELSIF", "ANY", "CHAR_BASE", "DATE", "END", "ARRAY", "CHECK", "DBA", "ENTRY", "ARRAYLEN", "CLOSE", "DEBUGOFF", "EXCEPTION", "AS", "CLUSTER", "DEBUGON", "EXCEPTION_INIT", "ASC", "CLUSTERS", "DECLARE", "EXISTS", "ASSERT", "COLAUTH", "DECIMAL", "EXIT", "ASSIGN", "COLUMNS", "DEFAULT", "FALSE", "AT", "COMMIT", "DEFINITION", "FETCH", "AUTHORIZATION", "COMPRESS", "DELAY", "FLOAT", "AVG", "CONNECT", "DELETE", "FOR", "BASE_TABLE", "CONSTANT", "DELTA", "FORM", "BEGIN", "COUNT", "DESC", "FROM", "FUNCTION", "NEW", "RELEASE", "SUM", "GENERIC", "NEXTVAL", "REMR", "TABAUTH", "GOTO", "NOCOMPRESS", "RENAME", "TABLE", "GRANT", "NOT", "RESOURCE", "TABLES", "GROUP", "NULL", "RETURN", "TASK", "HAVING", "NUMBER", "REVERSE", "TERMINATE", "IDENTIFIED", "NUMBER_BASE", "REVOKE", "THEN", "IF", "OF", "ROLLBACK", "TO", "IN", "ON", "ROWID", "TRUE", "INDEX", "OPEN", "ROWLABEL", "TYPE", "INDEXES", "OPTION", "ROWNUM", "UNION", "INDICATOR", "OR", "ROWTYPE", "UNIQUE", "INSERT", "ORDER", "RUN", "UPDATE", "INTEGER", "OTHERS", "SAVEPOINT", "USE", "INTERSECT", "OUT", "SCHEMA", "VALUES", "INTO", "PACKAGE", "SELECT", "VARCHAR", "IS", "PARTITION", "SEPARATE", "VARCHAR2", "LEVEL", "PCTFREE", "SET", "VARIANCE", "LIKE", "POSITIVE", "SIZE", "VIEW", "LIMITED", "PRAGMA", "SMALLINT", "VIEWS", "LOOP", "PRIOR", "SPACE", "WHEN", "MAX", "PRIVATE", "SQL", "WHERE", "MIN", "PROCEDURE", "SQLCODE", "WHILE", "MINUS", "PUBLIC", "SQLERRM", "WITH", "MLSLABEL", "RAISE", "START", "WORK", "MOD", "RANGE", "STATEMENT", "XOR", "MODE", "REAL", "STDDEV", "NATURAL", "RECORD", "SUBTYPE" };
        //public static 
    }
}