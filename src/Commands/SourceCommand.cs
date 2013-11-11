/*
 * SDB - Mono Soft Debugger Client
 * Copyright 2013 Alex Rønne Petersen
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Mono.Debugger.Client.Commands
{
    sealed class SourceCommand : Command
    {
        public override string[] Names
        {
            get { return new[] { "source", "src" }; }
        }

        public override string Summary
        {
            get { return "Show the source for the current stack frame."; }
        }

        public override string Syntax
        {
            get { return "source|src [lower] [upper]"; }
        }

        public override string Help
        {
            get
            {
                return "Prints the source for the current stack frame and highlights the current\n" +
                       "line.\n" +
                       "\n" +
                       "If arguments are given, they specify how many lines to print before and\n" +
                       "after the current line.";
            }
        }

        public override void Process(string args)
        {
            var f = Debugger.ActiveFrame;

            if (f == null)
            {
                Log.Error("No active stack frame");
                return;
            }

            var lower = 10;
            var upper = 10;

            var lowerStr = args.Split(' ').Where(x => x != string.Empty).FirstOrDefault();

            if (lowerStr != null)
            {
                if (!int.TryParse(lowerStr, out lower))
                {
                    Log.Error("Invalid lower bound value");
                    return;
                }

                lower = System.Math.Abs(lower);

                var upperStr = new string(args.Skip(lowerStr.Length).ToArray()).Trim();

                if (upperStr.Length != 0)
                {
                    if (!int.TryParse(upperStr, out upper))
                    {
                        Log.Error("Invalid upper bound value");
                        return;
                    }
                }
            }

            var loc = f.SourceLocation;
            var file = loc.FileName;
            var line = loc.Line;

            if (file != null && line != -1)
            {
                if (!File.Exists(file))
                {
                    Log.Error("Source file '{0}' not found", file);
                    return;
                }

                StreamReader reader;

                try
                {
                    reader = File.OpenText(file);
                }
                catch (Exception ex)
                {
                    Log.Error("Could not open source file '{0}'", file);
                    Log.Error(ex.ToString());

                    return;
                }

                try
                {
                    var exec = Debugger.CurrentExecutable;

                    if (exec != null && File.GetLastWriteTime(file) > exec.LastWriteTime)
                        Log.Notice("Source file '{0}' is newer than the debuggee executable", file);

                    var cur = 0;

                    while (!reader.EndOfStream)
                    {
                        var str = reader.ReadLine();

                        var i = line - cur;
                        var j = cur - line;

                        if (i > 0 && i < lower + 2 || j >= 0 && j < upper)
                        {
                            if (cur == line - 1)
                                Log.Emphasis(str);
                            else
                                Log.Info(str);
                        }

                        cur++;
                    }
                }
                finally
                {
                    reader.Dispose();
                }
            }
            else
                Log.Error("No source information available");
        }
    }
}
