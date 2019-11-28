using System;
using System.IO;
using System.Collections.Generic;

namespace AStar
{
    class Program
    {
        static void Main(string[] args)
        {
            WeightedGraph weightedGraph = new WeightedGraph();
            weightedGraph.readArcsFromFile("test.txt"); // считываем узлы и веса с файла
            weightedGraph.showSquareGraph(); // вывод весов

            Node start = new Node(0, 0);
            Node finish = new Node(4, 4);
            AStar alg = new AStar();
            List<Node> path = alg.findPath(start, finish, weightedGraph);

            weightedGraph.ShowPath(path, start, finish);

            foreach (var step in path)
            {
                Console.WriteLine("x: " + step.x + "   y: " + step.y);
            }
        }
    }


    class AStar
    {
        public List<Node> findPath(Node startNode, Node finishNode, WeightedGraph graph)
        {
            Dictionary<Node, int> pricesToVisitedNodes = new Dictionary<Node, int>();
            Dictionary<Node, Node> paths = new Dictionary<Node, Node>();
            QueueWithPriority queue = new QueueWithPriority();
            queue.Put(startNode, 0);
            pricesToVisitedNodes.Add(startNode, 0);

            while (!queue.IsEmpty())
            {
                Node current = queue.PopMostPriorityElement();


                if (finishNode.Equals(current))
                {
                    break;
                }

                foreach (var neighbor in graph.GetNeighbors(current))
                {
                    int costToNeighbor = pricesToVisitedNodes[current] + graph.getWeight(current, neighbor);

                    bool condition = false;

                    // если соседний узел еще не посещался или новая цена перехода из старта до него меньше чем была, то 
                    if (!pricesToVisitedNodes.ContainsKey(neighbor))
                        condition = true;
                    else
                        if (pricesToVisitedNodes[neighbor] > costToNeighbor)
                            condition = true;
                    if (condition)
                    {
                        // обновляем цену перехода до соседнего узла
                        pricesToVisitedNodes[neighbor] = costToNeighbor;
                        queue.Put(neighbor, costToNeighbor + heuristic(current, neighbor));
                        if (!paths.ContainsKey(neighbor))
                        {
                            paths.Add(neighbor, current);
                        }
                        else
                        { 
                            paths[neighbor] = current;
                        }
                    }
                }
            }

            // выбираем цепочку , которая привела к цели
            Node next = finishNode;
            List<Node> path = new List<Node>();
            path.Add(next);
            while (!startNode.Equals(next))
            {
                next = paths[next];
                path.Add(next);
            }

            return path;
        }

        public int heuristic(Node from, Node to)
        {
            return Math.Abs(from.x - to.x) + Math.Abs(from.y - to.y);
        }
    }
    struct Node : IEquatable<Node>
    {
        public int x;
        public int y;

        public Node(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public bool Equals(Node other)
        {
            if (this.x == other.x && this.y == other.y)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }

    class Arc
    {
        public Node fromNode { get; set; }
        public Node toNode { get; set; }
        public int weight { get; set; }

        public Arc(Node fromNode, Node toNode, int weight)
        {
            changeArc(fromNode, toNode, weight);
        }

        public void changeArc(Node fromNode, Node toNode, int weight)
        {
            this.fromNode = fromNode;
            this.toNode = toNode;
            this.weight = weight;
        }
    }

    class WeightedGraph
    {
        public List<Arc> arcs;

        public List<Node> nodes;

        int width;
        int height;

        public WeightedGraph()
        {
            arcs = new List<Arc>();
            nodes = new List<Node>();
            width = 0;
            height = 0;
        }

        public List<Node> GetNeighbors(Node node)
        {
            List<Node> neighbors = new List<Node>();
            if(node.x - 1 >= 0)
                neighbors.Add(new Node(node.x - 1, node.y));
            if (node.x + 1 < width)
                neighbors.Add(new Node(node.x + 1, node.y));
            if (node.y - 1 >= 0)
                neighbors.Add(new Node(node.x, node.y - 1));
            if (node.y + 1 < height)
                neighbors.Add(new Node(node.x, node.y + 1));

            return neighbors;
        }

        public void ShowPath(List<Node> path, Node A, Node Z)
        {
            string[,] gridArr = new string[height, width];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < height; x++)
                    gridArr[y, x] = ".     ";
            }

            foreach (var step in path)
            { 
                gridArr[step.y, step.x] = "+     ";
            }

            gridArr[A.y, A.x] = "A     ";
            gridArr[Z.y, Z.x] = "Z     ";

            string grid = "";
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < height; x++)
                    grid += gridArr[y, x];
                grid += "\n";
            }
            Console.WriteLine(grid);
        }
        public void showSquareGraph()
        {
            string graphGrid = "";
            for (int y = 0; y < height; y++)
            {
                string str = ".  ";
                for (int x = 0; x < width-1; x++)
                {
                    int weight = getWeight(new Node(x, y), new Node(x + 1, y));
                    string separator = weight > 9 ? " .  " : "  .  ";
                    str += weight.ToString() + separator;
                }
                graphGrid += str + "\n";

                if (y < height-1)
                {
                    str = "";
                    for (int x = 0; x < width; x++)
                    {
                        int weight = getWeight(new Node(x, y), new Node(x, y + 1));
                        string separator = weight > 9 ? "    " : "     ";
                        str += weight.ToString() + separator;
                    }
                    graphGrid += str + "\n";
                }
            }
            Console.WriteLine(graphGrid);
        }

        public bool haveSuchNode(Node node)
        {
            if (nodes.Contains(node))
                return true;
            else
                return false;
        }

        // Возвращает вес ребра по двум узлам дуги (от которого и в который) или -1, если ничего не нашлось
        public int getWeight(Node fromNode, Node toNode)
        {
            foreach(Arc Arc in this.arcs)
            {
                if (Arc.fromNode.Equals(fromNode) && Arc.toNode.Equals(toNode) || Arc.fromNode.Equals(toNode) && Arc.toNode.Equals(fromNode))
                {
                    return Arc.weight;
                }
            }
            return -1;
        }

        // Считывает данные дуг из файла (временно, чтобы проверить алгоритм)
        // формат: вКоторыйУзел.х вКоторыйУзел.у изКоторогоУзла.х изКоторогоУзла.у вес
        public void readArcsFromFile(string path)
        {
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    string[] words = sr.ReadToEnd().Split("\n");

                    for (int i = 0; i < words.Length - 1; i++)
                    {
                        string[] wordPieces = words[i].Split(' ');
                        if (Int32.Parse(wordPieces[0]) >= 0 && Int32.Parse(wordPieces[1]) >= 0 && Int32.Parse(wordPieces[2]) >= 0 && Int32.Parse(wordPieces[3]) >= 0)
                        {
                            Node fromNode = new Node(Int32.Parse(wordPieces[0]), Int32.Parse(wordPieces[1]));
                            Node toNode = new Node(Int32.Parse(wordPieces[2]), Int32.Parse(wordPieces[3]));
                            int weight = Int32.Parse(wordPieces[4]);
                            this.arcs.Add(new Arc(toNode, fromNode, weight));

                            if(!this.haveSuchNode(fromNode))
                                this.nodes.Add(fromNode);

                            if (!this.haveSuchNode(toNode))
                                this.nodes.Add(toNode);
                        }
                    }

                    foreach (Node node in nodes)
                    {
                        if (node.x > width)
                            width = node.x;
                        if (node.y > height)
                            height = node.y;
                    }

                    width++;
                    height++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }

    struct QueueElement
    {
        public Node node;
        public int priority;

        public QueueElement(Node node, int priority)
        {
            this.node = node;
            this.priority = priority;
        }
    }
    class QueueWithPriority
    {
        private List<QueueElement> elements;

        public QueueWithPriority()
        {
            elements = new List<QueueElement>();
        }

        public void Put(Node node, int proirity)
        {
            elements.Add(new QueueElement(node, proirity));
        }

        public Node PopMostPriorityElement()
        {
            int bestPriority = elements[0].priority;
            int index = 0; 
            for(int i = 0; i < elements.Count; i++)
            {
                if(elements[i].priority < bestPriority)
                {
                    bestPriority = elements[i].priority;
                    index = i;
                }
            }

            Node mostPriorityElement = elements[index].node;
            elements.RemoveAt(index);
            return mostPriorityElement;
        }

        public bool IsEmpty()
        {
            return elements.Count == 0;
        }
    }
}
