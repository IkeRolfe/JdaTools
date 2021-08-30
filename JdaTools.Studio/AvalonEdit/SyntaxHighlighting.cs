using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Highlighting;

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

        public MocaHighlightingDefinition()
        {
            MainRuleSet = new HighlightingRuleSet()
            {
                Name = "MOCA Default"
            };
            MainRuleSet.Spans.Add(MocaHighlightResources.CommentSpan);
            MainRuleSet.Spans.Add(MocaHighlightResources.BlockCommentSpan);
            MainRuleSet.Rules.Add(MocaHighlightResources.StringRule);
            MainRuleSet.Rules.Add(MocaHighlightResources.XmlTagRule);
            MainRuleSet.Spans.Add(MocaHighlightResources.SqlSpan);
        }
        //Called after connecting
        public void SetCommands(IEnumerable<string> commands) //TODO Manage reconnects and not duplicating
        {
            MainRuleSet.Rules.Add(new HighlightingRule
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
        public static HighlightingSpan CommentSpan { get; } = BuildSpan("--|//", "$", CommentColor);
        public static HighlightingSpan BlockCommentSpan { get; } = BuildSpan("/\\*", "\\*/", CommentColor);
        //Use rule for strings because start has to match end
        public static HighlightingRule StringRule { get; } = new HighlightingRule
        {
            Regex = new Regex("\"(.*?)\"|'(.*?)'"),
            Color = StringColor
        };
        public static HighlightingRule XmlTagRule { get; } = new HighlightingRule
        {
            Regex = new Regex(@"</?(.*?\w)/?>"),
            Color = XmlTagColor
        };

        public static HighlightingSpan SqlSpan
        {
            get
            {
                var span = new HighlightingSpan()
                {
                    StartExpression = new Regex("\\["),
                    EndExpression = new Regex("\\]"),
                };
                span.RuleSet = new HighlightingRuleSet
                {
                    Spans =
                    {
                        BlockCommentSpan,
                        CommentSpan,
                    },
                    Rules =
                    {
                        StringRule,
                        new HighlightingRule
                        {
                            Color = KeyWordColor,
                            Regex = GenerateKeywordRegEx(Keywords.SqlKeywords)
                        },
                        new HighlightingRule
                        {
                            Color = new HighlightingColor
                            {
                                FontWeight = FontWeights.Bold
                            },
                            Regex = new Regex("@(.*?\\w)\\b")
                        }
                    }
                };
                return span;
            }
        }

        public static Regex GenerateKeywordRegEx(string[] keywords)
        {
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
            bool colorEnd = true) =>
            BuildSpan(new Regex(startRegex), new Regex(endRegex), highlightingColor, colorStart, colorEnd);
        private static HighlightingSpan BuildSpan(Regex startRegex,
            Regex endRegex,
            HighlightingColor highlightingColor,
            bool colorStart = true,
            bool colorEnd = true) =>
            new()
            {
                SpanColor = highlightingColor,
                StartExpression = startRegex,
                EndExpression = endRegex,
                SpanColorIncludesStart = colorStart,
                SpanColorIncludesEnd = colorEnd
            };
    }

    public static class Keywords
    {
        public static readonly string[] SqlKeywords = new string[] { "ABORT", "BETWEEN", "CRASH", "DIGITS", "ACCEPT", "BINARY_INTEGER", "CREATE", "DISPOSE", "ACCESS", "BODY", "CURRENT", "DISTINCT", "ADD", "BOOLEAN", "CURRVAL", "DO", "ALL", "BY", "CURSOR", "DROP", "ALTER", "CASE", "DATABASE", "ELSE", "AND", "CHAR", "DATA_BASE", "ELSIF", "ANY", "CHAR_BASE", "DATE", "END", "ARRAY", "CHECK", "DBA", "ENTRY", "ARRAYLEN", "CLOSE", "DEBUGOFF", "EXCEPTION", "AS", "CLUSTER", "DEBUGON", "EXCEPTION_INIT", "ASC", "CLUSTERS", "DECLARE", "EXISTS", "ASSERT", "COLAUTH", "DECIMAL", "EXIT", "ASSIGN", "COLUMNS", "DEFAULT", "FALSE", "AT", "COMMIT", "DEFINITION", "FETCH", "AUTHORIZATION", "COMPRESS", "DELAY", "FLOAT", "AVG", "CONNECT", "DELETE", "FOR", "BASE_TABLE", "CONSTANT", "DELTA", "FORM", "BEGIN", "COUNT", "DESC", "FROM", "FUNCTION", "NEW", "RELEASE", "SUM", "GENERIC", "NEXTVAL", "REMR", "TABAUTH", "GOTO", "NOCOMPRESS", "RENAME", "TABLE", "GRANT", "NOT", "RESOURCE", "TABLES", "GROUP", "NULL", "RETURN", "TASK", "HAVING", "NUMBER", "REVERSE", "TERMINATE", "IDENTIFIED", "NUMBER_BASE", "REVOKE", "THEN", "IF", "OF", "ROLLBACK", "TO", "IN", "ON", "ROWID", "TRUE", "INDEX", "OPEN", "ROWLABEL", "TYPE", "INDEXES", "OPTION", "ROWNUM", "UNION", "INDICATOR", "OR", "ROWTYPE", "UNIQUE", "INSERT", "ORDER", "RUN", "UPDATE", "INTEGER", "OTHERS", "SAVEPOINT", "USE", "INTERSECT", "OUT", "SCHEMA", "VALUES", "INTO", "PACKAGE", "SELECT", "VARCHAR", "IS", "PARTITION", "SEPARATE", "VARCHAR2", "LEVEL", "PCTFREE", "SET", "VARIANCE", "LIKE", "POSITIVE", "SIZE", "VIEW", "LIMITED", "PRAGMA", "SMALLINT", "VIEWS", "LOOP", "PRIOR", "SPACE", "WHEN", "MAX", "PRIVATE", "SQL", "WHERE", "MIN", "PROCEDURE", "SQLCODE", "WHILE", "MINUS", "PUBLIC", "SQLERRM", "WITH", "MLSLABEL", "RAISE", "START", "WORK", "MOD", "RANGE", "STATEMENT", "XOR", "MODE", "REAL", "STDDEV", "NATURAL", "RECORD", "SUBTYPE" };
        //public static 
    }
}