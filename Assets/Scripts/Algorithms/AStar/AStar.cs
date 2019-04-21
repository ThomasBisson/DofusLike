using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ThomasBisson.Algorithms.AStar
{
    public class AStar
    {

        private NavigateNode[,] m_map;
        private List<NavigateNode> m_path = new List<NavigateNode>();
        private int m_row;
        private int m_column;

        private bool m_allowHorizontal;

        public AStar(int row, int column, bool allowHorizontal = false)
        {
            m_allowHorizontal = allowHorizontal;
            m_row = row;
            m_column = column;
            m_map = new NavigateNode[row, column];


            // initialize all nodes to navigable
            for (int x = 0; x < m_map.GetLength(0); ++x)
            {
                for (int y = 0; y < m_map.GetLength(1); ++y)
                {
                    m_map[x, y] = new NavigateNode(x, y, NavigateNode.StateEnum.NAVIGABLE);
                }
            }
        }

        public AStar(bool[,] array, bool allowHorizontal = false)
        {
            m_allowHorizontal = allowHorizontal;
            m_row = array.GetLength(0);
            m_column = array.GetLength(1);
            m_map = new NavigateNode[m_row, m_column];

            // initialize all nodes to navigable
            for (int x = 0; x < m_map.GetLength(0); ++x)
            {
                for (int y = 0; y < m_map.GetLength(1); ++y)
                {
                    if (array[x, y])
                        m_map[x, y] = new NavigateNode(x, y, NavigateNode.StateEnum.NAVIGABLE);
                    else
                        m_map[x, y] = new NavigateNode(x, y, NavigateNode.StateEnum.WALL);
                }
            }
        }

        public NavigateNode[,] GetMap() { return m_map; }
        public List<NavigateNode> GetPath() { return m_path; }
        public void ResetMap()
        {
            for (int x = 0; x < m_map.GetLength(0); ++x)
            {
                for (int y = 0; y < m_map.GetLength(1); ++y)
                {
                    m_map[x, y] = new NavigateNode(x, y, NavigateNode.StateEnum.NAVIGABLE);
                }
            }
        }
        public void ResetPath()
        {
            m_path.Clear();
        }

        public void ResetMapAndPath()
        {
            ResetMap();
            ResetPath();
        }

        private bool IsInRange(int x, int y)
        {
            return (x >= 0 && x < m_row && y >= 0 && y < m_column);
        }

        private List<NavigateNode> GetNeighbors(NavigateNode n)
        {
            int x = n.X;
            int y = n.Y;
            List<NavigateNode> neighbors = new List<NavigateNode>();
            if (IsInRange(x - 1, y))
            {
                if (m_map[x - 1, y].State != NavigateNode.StateEnum.WALL)
                    neighbors.Add(m_map[x - 1, y]);
            }

            if (IsInRange(x + 1, y))
            {
                if (m_map[x + 1, y].State != NavigateNode.StateEnum.WALL)
                    neighbors.Add(m_map[x + 1, y]);
            }

            if (IsInRange(x, y - 1))
            {
                if (m_map[x, y - 1].State != NavigateNode.StateEnum.WALL)
                    neighbors.Add(m_map[x, y - 1]);
            }

            if (IsInRange(x, y + 1))
            {
                if (m_map[x, y + 1].State != NavigateNode.StateEnum.WALL)
                    neighbors.Add(m_map[x, y + 1]);
            }

            if (m_allowHorizontal)
            {
                if (IsInRange(x - 1, y - 1))
                {
                    if (m_map[x - 1, y - 1].State != NavigateNode.StateEnum.WALL)
                        neighbors.Add(m_map[x - 1, y - 1]);
                }

                if (IsInRange(x + 1, y - 1))
                {
                    if (m_map[x + 1, y - 1].State != NavigateNode.StateEnum.WALL)
                        neighbors.Add(m_map[x + 1, y - 1]);
                }

                if (IsInRange(x - 1, y + 1))
                {
                    if (m_map[x - 1, y + 1].State != NavigateNode.StateEnum.WALL)
                        neighbors.Add(m_map[x - 1, y + 1]);
                }

                if (IsInRange(x + 1, y + 1))
                {
                    if (m_map[x + 1, y + 1].State != NavigateNode.StateEnum.WALL)
                        neighbors.Add(m_map[x + 1, y + 1]);
                }
            }

            return neighbors;
        }

        private double GetDirectCost(NavigateNode n, NavigateNode m)
        {
            int temp = Math.Abs(n.X - m.X) + Math.Abs(n.Y - m.Y);
            if (temp == 2)
                return 14.0;
            else
                return 10.0;
        }

        private double GetHeuristicCost(NavigateNode n, NavigateNode goal)
        {
            return 10.0 * (Math.Abs(n.X - goal.X) + Math.Abs(n.Y - goal.Y));
        }

        public void AstarRun(Vector2 startPoint, Vector2 endPoint)
        {
            //Console.WriteLine("A* starts!");
            NavigateNode start = new NavigateNode((int)startPoint.x, (int)startPoint.y, NavigateNode.StateEnum.NAVIGABLE);
            NavigateNode end = new NavigateNode((int)endPoint.x, (int)endPoint.y, NavigateNode.StateEnum.NAVIGABLE);

            PriorityQueue<NavigateNode> openSet = new PriorityQueue<NavigateNode>();
            PriorityQueue<NavigateNode> closeSet = new PriorityQueue<NavigateNode>();

            openSet.Add(start);

            while (!openSet.Empty)
            {
                // get from open set
                NavigateNode current = openSet.Pop();

                // add to close set
                closeSet.Add(current);

                // goal found
                if (current.IsSameLocation(end))
                {
                    while (current.Parent != null)
                    {
                        m_map[current.X, current.Y].State = NavigateNode.StateEnum.PATH;
                        m_path.Add(current);
                        current = current.Parent;
                    }
                    return;
                }
                else
                {
                    List<NavigateNode> neighbors = GetNeighbors(current);

                    foreach (NavigateNode n in neighbors)
                    {
                        if (closeSet.IsMember(n))
                        {
                            continue;
                        }
                        else
                        {
                            if (!openSet.IsMember(n))
                            {
                                n.Parent = current;
                                n.DirectCost = current.DirectCost + GetDirectCost(current, n);
                                n.HeuristicCost = GetHeuristicCost(n, end);
                                n.TotalCost = n.DirectCost + n.HeuristicCost;

                                // add to open set
                                openSet.Add(n);
                            }
                            else
                            {
                                double costFromThisPathToM = current.DirectCost + GetDirectCost(current, n);
                                // we found a better path
                                if (costFromThisPathToM < n.DirectCost)
                                {
                                    n.Parent = current;                                   // change parent to n
                                    n.DirectCost = costFromThisPathToM;             // recalculate direct cost
                                    n.TotalCost = n.HeuristicCost + n.DirectCost;   // recalculate total cost
                                }
                            }
                        }
                    }
                }
            }

            //Console.WriteLine("end here?");
        }
    }
}
