using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;

namespace LD1_Denas_Kinderis_IF160009
{
    public class MainProgram
    {
        static void Main(string[] args)
        {
            List<NodeState> visitedNodes = new List<NodeState>();
            var state = new NodeState(true, new List<char>{'A', 'a', 'B', 'b', 'C', 'c'}, new List<char>() );
            visitedNodes.Add(state);
            Console.WriteLine(state);
            int count = 0;
            while (!state.IsAnwser())
            {
                var nextStates = state.GetNextStates().FindAll(nodeState => nodeState.IsValid() && !visitedNodes.Contains(nodeState));
                state = nextStates.OrderByDescending(nodeState => nodeState.AStarSearch()).First();
                Console.WriteLine(state);
                visitedNodes.Add(state);
                Console.WriteLine(count++);
            }
            Console.ReadLine();
        }

    }

    public class NodeState
    {
        public override bool Equals(object obj)
        {
            var otherState = obj as NodeState;
            if (IsBoatOnLeftSide != otherState.IsBoatOnLeftSide)
                return false;
            if (!PeopleOnLeftSide.All(c => otherState.PeopleOnLeftSide.Contains(c)))
                return false;
            if (!PeopleOnRightSide.All(c => otherState.PeopleOnRightSide.Contains(c)))
                return false;
            return true;
        }

        public override string ToString()
        {
            string boatSide = IsBoatOnLeftSide ? "left" : "right";
            string peopleOnLeftSide = String.Concat(PeopleOnLeftSide);
            string peopleOnRightSide = String.Concat(PeopleOnRightSide);
            return $"Left : {peopleOnLeftSide}, Right : {peopleOnRightSide}, Boat side : {boatSide}";
        }

        public NodeState(bool isBoatOnLeftSide, List<char> peopleOnLeftSide, List<char> peopleOnRightSide)
        {
            IsBoatOnLeftSide = isBoatOnLeftSide;
            PeopleOnLeftSide = peopleOnLeftSide;
            PeopleOnRightSide = peopleOnRightSide;
        }

        public int AStarSearch()
        {
            return PeopleOnRightSide.Count(Char.IsUpper) + PeopleOnRightSide.Count;
        }

        public bool IsBoatOnLeftSide { get; set; }
        public List<char> PeopleOnLeftSide { get; set; }
        public List<char> PeopleOnRightSide { get; set; }

        public bool IsAnwser()
        {
            return PeopleOnLeftSide.Count == 0;
        }

        public List<NodeState> GetNextStates()
        {
            if (IsBoatOnLeftSide)
            {
               return GetNextStates(PeopleOnLeftSide, PeopleOnRightSide);
            }

            return GetNextStates(PeopleOnRightSide, PeopleOnLeftSide);
        }

        private List<NodeState> GetNextStates(List<char> from, List<char> to)
        {
            List<NodeState> listOfNodes = new List<NodeState>();
            foreach (var person in from)
            {
                var newState = CreateNewNode(person, from, to);
                listOfNodes.Add(newState);
            }

            for (var i = 0; i < from.Count; i++)
            {
                for (int j = i + 1; j < from.Count; j++)
                {
                    var newState = CreateNewNode(from[i], from[j], from, to);
                    listOfNodes.Add(newState);
                }
            }

            return listOfNodes;
        }

        private NodeState CreateNewNode(char firstPerson, char secondPerson, List<char> from, List<char> to)
        {
            List<char> fromCopy = new List<char>(from);
            fromCopy.Remove(firstPerson);
            fromCopy.Remove(secondPerson);
            List<char> toCopy = new List<char>(to);
            toCopy.Add(firstPerson);
            toCopy.Add(secondPerson);
            var newState = new NodeState(!IsBoatOnLeftSide, IsBoatOnLeftSide ? fromCopy : toCopy, IsBoatOnLeftSide ? toCopy : fromCopy);
            return newState;
        }

        private NodeState CreateNewNode(char aPerson, List<char> from, List<char> to)
        {
            List<char> fromCopy = new List<char>(from);
            fromCopy.Remove(aPerson);
            List<char> toCopy = new List<char>(to);
            toCopy.Add(aPerson);
            var newState = new NodeState(!IsBoatOnLeftSide, IsBoatOnLeftSide ? fromCopy : toCopy, IsBoatOnLeftSide ? toCopy : fromCopy);
            return newState;
        }

        public bool IsValid()
        {
            return ValidateSideState(PeopleOnLeftSide) && ValidateSideState(PeopleOnRightSide);
        }

        private bool ValidateSideState(List<char> onLeftSide)
        {
            if (onLeftSide.Any(person => Char.IsUpper(person)))
            {
                var womenMassive = onLeftSide.Where(person => Char.IsLower(person));
                return womenMassive.All(woman => onLeftSide.Contains(Char.ToUpper(woman)));
            }

            return true;
        }
    }
}
