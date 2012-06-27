using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Text.RegularExpressions;
using System.Diagnostics;

namespace Inferables.Internal
{
    internal static class BindingMapParser
    {
        static public List<BindingMap> GetMaps(string paths)
        {
            var scan = new StringScan(paths);


            var ex = new ArgumentException("Invalid path");

            var maps = new List<BindingMap>();
            var state = ParseState.Begin;
            var current = StringScan.EOF;

            //Parsing State Machine
            switch (state)
            {
                case ParseState.Begin:
                case ParseState.BeginMap:
                    var map = ParseMap(scan);
                    if (!maps.Contains(map))
                        maps.Add(map);
                    goto case ParseState.EndMap;
                case ParseState.EndMap:
                    current = scan.Current();
                    if (current == ',')
                    {
                        scan.MoveNext();
                        goto case ParseState.BeginMap;
                    }
                    if (current == StringScan.EOF)
                        goto case ParseState.End;
                    throw scan.CreateException();
                case ParseState.End:
                    break;
            }

            return maps;

        }

        private static BindingMap ParseMap(StringScan scan)
        {
            var state = MapState.Begin;
            var map = new BindingMap();
            var current = StringScan.EOF;

            //Parsing State Machine
            switch (state)
            {
                case MapState.Begin:
                    current = scan.SkipWhitespace();
                    if (current == '*')
                        goto case MapState.WildCardMatch;
                    if (char.IsLetter(current) || current == '_')
                        goto case MapState.AmbiguousNamespace;
                    if (current == '~')
                    {
                        map.MatchPath.IsWildcard = true;
                        goto case MapState.RelativeBinding;
                    }
                    if (current == '-')
                    {
                        map.MatchPath.IsWildcard = true;
                        goto case MapState.RelativeRootBinding;
                    }
                    throw scan.CreateException();

                case MapState.WildCardMatch:
                    map.MatchPath.IsWildcard = true;
                    current = scan.MoveNext();
                    if (current == '.')
                    {
                        scan.MoveNext();
                        goto case MapState.MatchNamespace;
                    }
                    if (char.IsWhiteSpace(current))
                    {
                        current = scan.SkipWhitespace();
                        if (current == '=')
                            goto case MapState.MapOp;
                        throw scan.CreateException();
                    }
                    if (current == '=')
                        goto case MapState.MapOp;
                    throw scan.CreateException();

                case MapState.AmbiguousNamespace:
                    string nsp = ScanNamespace(scan);
                    current = scan.Current();
                    if (char.IsWhiteSpace(current))
                    {
                        current = scan.SkipWhitespace();
                        if (current == '=')
                        {
                            map.MatchPath.Namespace = nsp;
                            goto case MapState.MapOp;
                        }
                        map.MatchPath.IsWildcard = true;
                        map.BindingPath.Namespace = nsp;
                        goto case MapState.End;
                    }
                    if (current == '=')
                    {
                        map.MatchPath.Namespace = nsp;
                        goto case MapState.MapOp;
                    }
                    map.MatchPath.IsWildcard = true;
                    map.BindingPath.Namespace = nsp;
                    goto case MapState.End;

                case MapState.MatchNamespace:
                    map.MatchPath.Namespace = ScanNamespace(scan);
                    current = scan.Current();
                    if (char.IsWhiteSpace(current))
                    {
                        current = scan.SkipWhitespace();
                        if (current == '=')
                            goto case MapState.MapOp;
                        throw scan.CreateException();
                    }
                    if (current == '=')
                        goto case MapState.MapOp;
                    throw scan.CreateException();
                case MapState.MapOp:
                    current = scan.MoveNext();
                    if (current == '>')
                        goto case MapState.Binding;
                    throw scan.CreateException();
                case MapState.Binding:
                    scan.MoveNext();
                    current = scan.SkipWhitespace();
                    if (char.IsLetter(current) || current == '_')
                        goto case MapState.BindingNamespace;
                    if (current == '~')
                        goto case MapState.RelativeBinding;
                    if (current == '-')
                        goto case MapState.RelativeRootBinding;
                    throw scan.CreateException();
                case MapState.BindingNamespace:
                    map.BindingPath.Namespace = ScanNamespace(scan);
                    goto case MapState.End;
                case MapState.RelativeBinding:
                    map.BindingPath.IsRelative = true;
                    current = scan.MoveNext();
                    if (current == '.')
                    {
                        scan.MoveNext();
                        goto case MapState.BindingNamespace;
                    }
                    goto case MapState.End;
                case MapState.RelativeRootBinding:
                    map.BindingPath.IsRelative = true;
                    while (true)
                    {
                        map.BindingPath.RootDepth++;
                        current = scan.MoveNext();
                        if (current == '.')
                        {
                            current = scan.MoveNext();
                            if (current == '-')
                                continue;
                            if (char.IsLetter(current) || current == '_')
                                goto case MapState.BindingNamespace;
                            throw scan.CreateException();
                        }
                        goto case MapState.End;
                    }

                case MapState.End:
                    scan.SkipWhitespace();
                    break;
            }
            return map;
        }


        private static string ScanNamespace(StringScan scan)
        {
            scan.StartBuffer();
            char current = scan.Current();
            while (true)
            {
                if (current != '_' && !char.IsLetter(current))
                    throw scan.CreateException();
                while (true)
                {
                    current = scan.MoveNext();
                    if (current == '_' || char.IsLetter(current) || char.IsDigit(current))
                        continue;
                    if (current == '.')
                    {
                        current = scan.MoveNext();
                        break;
                    }
                    var result = scan.DumpBuffer();
                    return result;
                }
            }
        }


        enum MapState
        {
            Begin,
            WildCardMatch,
            MatchNamespace,
            AmbiguousNamespace,
            Binding,
            RelativeBinding,
            MapOp,
            RelativeRootBinding,
            BindingNamespace,
            End,

        }

        enum ParseState
        {
            Begin,
            BeginMap,
            EndMap,
            End,
        }

        private class StringScan
        {
            public const char EOF = '\0';
            private string str;
            private int pos;
            private int bufferPos = 0;

            public void StartBuffer()
            {
                bufferPos = pos;
            }

            public string DumpBuffer()
            {
                return str.Substring(bufferPos, pos - bufferPos);
            }

            public StringScan(string str)
            {
                this.str = str;
                this.pos = 0;

            }

            public char SkipWhitespace()
            {
                while (CurrentIsWhitespace())
                    MoveNext();
                return Current();
            }

            public bool CurrentIsWhitespace()
            {
                return char.IsWhiteSpace(Current());
            }

            public char Current()
            {
                if (str.Length <= this.pos)
                    return EOF;

                return str[pos];
            }

            public bool Matches(params char[] matches)
            {
                return matches.Contains(Current());
            }

            public char Peek()
            {
                if (str.Length <= this.pos + 1)
                    return EOF;
                return str[pos + 1];
            }

            public bool IsEOF()
            {
                return str.Length <= this.pos;
            }



            public char MoveNext()
            {
                if (!IsEOF())
                    this.pos++;
                return Current();
            }

            public Exception CreateException()
            {
                return new ArgumentException("Invalid Paths");
            }
        }
    }
}
