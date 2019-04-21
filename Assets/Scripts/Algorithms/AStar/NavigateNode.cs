using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThomasBisson.Algorithms.AStar
{
    public class NavigateNode : IComparable<NavigateNode>
    {
        public enum StateEnum
        {
            WALL,
            NAVIGABLE,
            PATH,
            GOAL,
            START
        };

        private StateEnum mState;

        /**
         * Direct cost
         */
        private double mDirectCost;

        /**
         * Heuristic cost
         */
        private double mHeuristicCost;

        /**
         * Total cost
         */
        private double mTotalCost;

        /**
         * Map coordinate
         */
        private int mX;
        private int mY;

        /**
         * Parent of current node
         */
        private NavigateNode mParent;

        #region
        public StateEnum State
        {
            get
            {
                return mState;
            }
            set
            {
                mState = value;
            }
        }

        public double DirectCost
        {
            get
            {
                return mDirectCost;
            }
            set
            {
                mDirectCost = value;
            }
        }

        public double HeuristicCost
        {
            get
            {
                return mHeuristicCost;
            }
            set
            {
                mHeuristicCost = value;
            }
        }

        public double TotalCost
        {
            get
            {
                return mTotalCost;
            }
            set
            {
                mTotalCost = value;
            }
        }

        public int X
        {
            get
            {
                return mX;
            }
        }

        public int Y
        {
            get
            {
                return mY;
            }
        }

        public NavigateNode Parent
        {
            get
            {
                return mParent;
            }
            set
            {
                mParent = value;
            }
        }

        #endregion

        public NavigateNode(int x, int y, StateEnum state = StateEnum.NAVIGABLE)
        {
            mX = x;
            mY = y;
            mState = state;
            mDirectCost = 0.0;
            mHeuristicCost = 0.0;
            mTotalCost = 0.0;
            mParent = null;
        }

        public bool IsSameLocation(NavigateNode n)
        {
            return (X == n.X && Y == n.Y);
        }

        public int CompareTo(NavigateNode n)
        {
            if (mTotalCost < n.mTotalCost)
                return -1;
            else if (mTotalCost > n.mTotalCost)
                return 1;
            else
                return 0;
        }
    }
}
