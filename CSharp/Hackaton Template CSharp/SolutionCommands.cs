using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;

namespace Hackaton_Template_CSharp
{
    // All console commands must be in the sub-namespace Commands: 
    namespace ConsoleApplicationBase.Commands
    {
        // Must be a public static class:
        public static class SolutionCommands
        {
            public static List<Coordinate> _coordinates = new List<Coordinate>();

            private static List<int> _route1 = new List<int>();
            private static List<int> _route2 = new List<int>();
         
            public static string ReadData(string path)
            {
                if (!File.Exists(path)) 
                {
                    return string.Format("{0} File does not exist.", path);
                }
                if (Path.GetExtension(path) != ".csv")
                {
                    return string.Format("{0} File is not a csv file.", path);
                }

                using (var reader = new StreamReader(path))
                {
                    string headerLine = reader.ReadLine();
                    var values = headerLine.Split(',');
                    float temp;
                    if (float.TryParse(values[0], out temp) && float.TryParse(values[1], out temp) && float.TryParse(values[2], out temp))
                    {
                        _coordinates.Add(new Coordinate(double.Parse(values[1]), double.Parse(values[2])));
                    }
                    
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        values = line.Split(',');

                        _coordinates.Add(new Coordinate(double.Parse(values[1]), double.Parse(values[2])));
                    }
                }

                return string.Format("Reading data was successful. The .csv file consisted of {0} lines.", _coordinates.Count);
            }

            public static string CalculateSolution()
            {
                // Sanity check
                if (_coordinates.Count == 0)
                {
                    return "No coordinates have been loaded, you fool.";
                }

                // ============================ GO FOR IT===============================
                _route1.Add(_coordinates.Count - 1);
                for (int i = 0; i < _coordinates.Count-1; i++)
                {
                    _route1.Add(i);
                }

                _route2.Add(_coordinates.Count - 2);
                _route2.Add(_coordinates.Count - 1);
                for (int i = 0; i < _coordinates.Count-2; i++)
                {
                    _route2.Add(i);
                }

                // ============================ GO FOR IT===============================
                return "Calculating solution data was successful";
            }

            public static string CheckSolution()
            {
                // Sanity check
                if (_route1.Count == 0 || _route2.Count == 0)
                {
                    return "No solution have been calculated, you fool.";
                }

                // Check Scrooge criteria
                if(_route1.Count != _coordinates.Count || _route2.Count != _coordinates.Count)
                {
                    return "One or both solution routes are too short.";
                }

                int nofHops = 0;
                for (int i = 0; i < _coordinates.Count; i++)
                {
                    if (_route1[i] == i || _route2[i] == i)
                    {
                        return string.Format("Route node points to itself. Route1: {0}, Route2: {1}", _route1[i], _route2[i]);
                    }

                    if (_route1[i] >= _route2.Count || _route2[i] >= _route1.Count)
                    {
                        return string.Format("ID was out of bound. Route1: {0}, Route2: {1}", _route1[i], _route2[i]);
                    }

                    if (_route1[i] == _route2[i] || _route2[_route2[i]] == i)
                    {
                        return string.Format("Solution contains similar edge. {0} <--> {1}", i, _route1[i]);
                    }
                    nofHops++;
                }
                if (nofHops != _route1.Count)
                {
                    return string.Format("Solution only consists of {0} edges.", nofHops);
                }

                // Calculating score.
                double distance = 0;
                for (int i = 0; i < _route1.Count; i++)
                {
                    double x1 = _coordinates[_route1[i]].X;
                    double x2 = _coordinates[_route1[_route1[i]]].X;
                    double y1 = _coordinates[_route1[i]].Y;
                    double y2 = _coordinates[_route1[_route1[i]]].Y;

                    distance += Math.Sqrt(((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2)));
                }

                for (int i = 0; i < _route2.Count; i++)
                {
                    double x1 = _coordinates[_route2[i]].X;
                    double x2 = _coordinates[_route2[_route2[i]]].X;
                    double y1 = _coordinates[_route2[i]].Y;
                    double y2 = _coordinates[_route2[_route2[i]]].Y;

                    distance += Math.Sqrt(((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2)));
                }

                return string.Format("Checking solution data was successful. The total distance is {0}", distance);
            }

            public static string SaveSolution(string path)
            {
                if (_route1.Count != _route2.Count || _route1.Count != _coordinates.Count)
                {
                    return string.Format("List of route1, route2, and/or coordinates is not the same length. Route1: {0}, Route2: {1}, Coordinates: {2}", _route1.Count, _route2.Count, _coordinates.Count);
                }

                string filename = string.Format(path + "output.csv");
                TextWriter tsw = new StreamWriter(filename, false);

                for (int i = 0; i < _route1.Count; i++)
                {
                    var newLine = string.Format("{0},{1}", _route1[i],_route2[i]);
                    tsw.Write(newLine);
                    tsw.Write(Environment.NewLine);
                }
                tsw.Close();

                return "Saving solution was successful";
            }
        }

        public struct Coordinate
        {
            private readonly double x;
            private readonly double y;

            public double X { get { return x; } }
            public double Y { get { return y; } }

            public Coordinate(double x, double y)
            {
                this.x = x;
                this.y = y;
            }

            public override string ToString()
            {
                return string.Format("{0},{1}", x, y);
            }
        }
    }
}
