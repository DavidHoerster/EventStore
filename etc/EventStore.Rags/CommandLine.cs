﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EventStore.Rags
{
    public class CommandLine
    {
        public static IEnumerable<OptionSource> Parse(string[] args)
        {
            var ret = new List<OptionSource>();
            if (args == null || args.Length == 0)
            {
                return ret;
            }
            foreach (var argument in ParseArgs(args))
            {
                ret.Add(OptionSource.String("Command Line", argument.Item1, argument.Item2));
            }
            return ret;
        }

        internal static IEnumerable<Tuple<string, string>> ParseArgs(string[] args)
        {
            var result = new List<Tuple<string, string>>();
            for (int i = 0; i < args.Length; i++)
            {
                var token = args[i];

                if (token.StartsWith("-"))
                {
                    string key = token.Substring(1);

                    if (key.Length == 0) throw new ArgException("Missing argument value after '-'");

                    string value;

                    // Handles a special case --arg-name- where we have a trailing -
                    // it's a shortcut way of disabling an option
                    if (key.StartsWith("-") && key.EndsWith("-") ||
                        key.StartsWith("-") && key.EndsWith("+"))
                    {
                        value = key.Substring(key.Length - 1, 1);
                        key = key.Substring(1, key.Length - 2);
                    }
                    // Handles long form syntax --argName=argValue.
                    else if (key.StartsWith("-") && key.Contains("="))
                    {
                        var index = key.IndexOf("=");
                        value = key.Substring(index + 1);
                        key = key.Substring(1, index - 1);
                    }
                    else
                    {
                        if (key.StartsWith("-"))
                        {
                            key = key.Substring(1);
                        }
                        if (i == args.Length - 1)
                        {
                            value = "";
                        }
                        else
                        {
                            i++;
                            value = args[i];
                        }
                    }

                    yield return new Tuple<string, string>(key.TrimStart(new char[]{'-'}), value);
                }
            }

            yield break;
        }
    }
}