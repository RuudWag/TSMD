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
    public class Node
    {
        private int next = 0;
        private int prev = 0;
        private bool inSolution = false;

        public int Next { get { return next; } set { next = value; } }
        public int Prev { get { return prev; } set { prev = value;  } }
        public bool InSolution { get { return inSolution; } set { inSolution = value; } }
    }
    class SolutionModifiers {
        public void InsertNodeAt(List<Node> io_route, int i_insertAfterNode, int i_insertNode)
        {
            if (io_route[i_insertAfterNode].InSolution && !io_route[i_insertNode].InSolution)
            {
                int NodeAfterInstertedNode = io_route[i_insertAfterNode].Next;
                io_route[i_insertAfterNode].Next = i_insertNode;
                io_route[NodeAfterInstertedNode].Prev = i_insertNode;

                io_route[i_insertNode].Prev = i_insertAfterNode;
                io_route[i_insertNode].Next = NodeAfterInstertedNode;

                io_route[i_insertNode].InSolution = true;
            }
            else
            {
                if (!io_route[i_insertAfterNode].InSolution)
                {
                    Console.Write(string.Format("tried to insert after node {0},but it is not in the solution ", i_insertAfterNode));
                }
                else
                {
                    Console.Write(string.Format("tried to insert after node {0},but it is already in the solution ", i_insertNode));
                }
            }
        }

        public void RemoveNode(List<Node> io_route, int i_removeNode )
        {
	        if (io_route[i_removeNode].InSolution )
	        {
		        int prevNode = io_route[i_removeNode].Prev;
                int nextNode = io_route[i_removeNode].Next;
                io_route[prevNode].Next = nextNode;
		        io_route[nextNode].Prev = prevNode;
		        io_route[i_removeNode].InSolution = false;
	        }
	        else
	        {
                Console.Write(string.Format("tried to remove node {0} ,but it is not in the solution ", i_removeNode));
	        }
        }
    }
    // All console commands must be in the sub-namespace Commands: 
    namespace ConsoleApplicationBase.Commands
    {
        // Must be a public static class:
        public static class SolutionCommands
        {
            public static List<Coordinate> _coordinates = new List<Coordinate>();

            private static List<Node> _route1;
            private static List<Node> _route2;
         
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

                _route1 = new List<Node>(_coordinates.Count);
                _route2 = new List<Node>(_coordinates.Count);
                for ( int i = 0; i< _coordinates.Count; i++)
                {
                    _route1.Add(new Node());
                    _route2.Add(new Node());
                }
                
                _route1[0].InSolution = true;
                _route2[0].InSolution = true;

                return string.Format("Reading data was successful. The .csv file consisted of {0} lines.", _coordinates.Count);
            }

           


            public static string CalculateSolution()
            {
                // Sanity check
                if (_coordinates.Count == 0)
                {
                    return "No coordinates have been loaded, you fool.";
                }
                var solutionModifier = new SolutionModifiers();
                // ============================ GO FOR IT===============================

                //initial random insert
                int insertAfterNode = 0;
                List<int> nodesToInsert = new List<int>();
                for (int indexNode = 1; indexNode < _route1.Count; indexNode++)
                {
                    nodesToInsert.Add(indexNode);
                }
                foreach (var node in nodesToInsert)
                {
                    solutionModifier.InsertNodeAt(_route1, insertAfterNode, node);
                    insertAfterNode = node;
                }
                insertAfterNode = 0;
                int middlePoint = (int)Math.Ceiling(((double)(nodesToInsert.Count - 1) / 2.0));
                int left = 0;
                int right = nodesToInsert.Count - 1;
                solutionModifier.InsertNodeAt(_route2, insertAfterNode, nodesToInsert[middlePoint]);
                insertAfterNode = nodesToInsert[middlePoint];
                bool insertLeft = true;

                while (!(left == middlePoint && right == middlePoint))
                {
                    int insertNode;
                    if (insertLeft)
                    {
                        insertNode = nodesToInsert[left];
                        left++;
                    }
                    else
                    {
                        insertNode = nodesToInsert[right];

                        right--;
                    }
                    solutionModifier.InsertNodeAt(_route2, insertAfterNode, insertNode);
                    insertAfterNode = insertNode;
                    insertLeft = !insertLeft;
                }

                // ============================ GO FOR IT===============================
                return "Calculating solution data was successful";
            }

            public static string CheckSolution()
            {
                SortedSet<int> nodesVisited1 = new SortedSet<int>();                
                int currentNode = 0;
                bool feasible = true;
                do
                {
                    if ( nodesVisited1.Contains(currentNode))
                    {                       
                        return String.Format("not feasible: Node {0} is visited twice in solution 1",currentNode);
                    }
                    nodesVisited1.Add(currentNode);

                    int nextNode = _route1[currentNode].Next;
                    if (_route2[currentNode].Prev == nextNode || _route2[currentNode].Next == nextNode)
                    {
                        Console.WriteLine(String.Format("not feasible: Arc from node {0} to node {1} is in both solutions", currentNode, nextNode));
                        feasible = false;
                    }
                    currentNode = _route1[currentNode].Next;
                } while (currentNode != 0);

                if (nodesVisited1.Count != _route1.Count)
                {
                    Console.WriteLine(String.Format("not feasible: The following nodes are not visited in solution 1: "));
                    for (int node = 0; node < _route1.Count; node++)
                    {
                        if ( !nodesVisited1.Contains(node))
                        {
                            Console.WriteLine(String.Format("{0}", node));

                        }
                    }
                    feasible = false;
                }
                SortedSet<int> nodesVisited2 = new SortedSet<int>();
                currentNode = 0;
                do
                {
                    if ( nodesVisited2.Contains(currentNode) )
                    {
                        return String.Format("not feasible: Node {0} is visited twice in solution 2", currentNode);
                    }
                    nodesVisited2.Add(currentNode);
                    currentNode = _route2[currentNode].Next;
                } while (currentNode != 0);

                if (nodesVisited2.Count != _route2.Count)
                {
                    Console.WriteLine(String.Format("not feasible: The following nodes are not visited in solution 3: "));
                    for (int node = 0; node < _route2.Count; node++)
                    {
                        if (!nodesVisited2.Contains(node))
                        {
                            Console.WriteLine(String.Format("{0}", node));
                        }
                    }
                    feasible = false;
                }                
                return String.Format("Is Solution Feasible? {0}", feasible);
            }

            public static string CalculateDistance()
            {
                double distance1 = 0;
                for (int i = 0; i < _route1.Count; i++)
                {
                    double x1 = _coordinates[i].X;
                    double x2 = _coordinates[_route1[i].Next].X;
                    double y1 = _coordinates[i].Y;
                    double y2 = _coordinates[_route1[i].Next].Y;

                    distance1 += Math.Sqrt(Math.Pow(x1 - x2,2) + Math.Pow(y1 - y2,2));
                }

                double distance2 = 0;
                for (int i = 0; i < _route2.Count; i++)
                {
                    double x1 = _coordinates[i].X;
                    double x2 = _coordinates[_route2[i].Next].X;
                    double y1 = _coordinates[i].Y;
                    double y2 = _coordinates[_route2[i].Next].Y;

                    distance2 += Math.Sqrt(((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2)));
                }

                return string.Format("The distance for route 1 is {0} and for route 2 {1}", distance1, distance2);

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
                    var newLine = string.Format("{0},{1}", _route1[i].Next,_route2[i].Next);
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
