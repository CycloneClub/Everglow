using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Everglow.Commons.Physics.PBEngine.Collision.BroadPhase
{
	/// <summary>
	/// 存储碰撞组的碰撞关系的图结构
	/// </summary>
	public class CollisionGraph
    {
        public static CollisionGraph DefualtGraph 
            = new CollisionGraph(new Dictionary<string, List<string>>() { { "Default", new List<string>() } });
        public Dictionary<string, List<string>> Graph
        {
            get;
            set;
        }

        public CollisionGraph()
        {
            Graph = new Dictionary<string, List<string>>();
        }

        public CollisionGraph(Dictionary<string, List<string>> graph)
        {
            Graph = graph;
        }

        public void AddDoubleEdge(string groupA, string groupB)
        {
            if (groupA == groupB)
                return;
            // Ensure only compare once
            if (String.CompareOrdinal(groupA, groupB) < 0)
            {
                AddSingleEdge(groupA, groupB);
            }
            else
            {
                AddSingleEdge(groupB, groupA);
            }
        }

        public void AddSingleEdge(string groupA, string groupB)
        {
            if (groupA == groupB)
                return;
            if (Graph.ContainsKey(groupA))
            {
                Graph[groupA].Add(groupB);
            }
            else
            {
                Graph.Add(groupA, new List<string>()
                {
                    groupB
                });
            }
        }
    }
}
