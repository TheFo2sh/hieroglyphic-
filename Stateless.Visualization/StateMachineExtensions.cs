using System;
using System.Collections.Generic;
using System.Text;

namespace Stateless.Visualization
{
    public static class StateMachineExtensions
    {
        public static string ToGraph<S,T>(this StateMachine<S,T> stateMachine)
        {
            StringBuilder result=new StringBuilder();
            result.AppendLine("digraph");
            result.AppendLine("\t {");
            foreach (var stateInfo in stateMachine.GetInfo().States)
            {
                result.AppendLine($"\t\t {stateInfo.UnderlyingState}");
            }

            foreach (var stateInfo in stateMachine.GetInfo().States)
            {
                foreach (var stateInfoTransition in stateInfo.FixedTransitions)
                {
                    result.AppendLine(
                        $"\t\t {stateInfo.UnderlyingState} -> {stateInfoTransition.DestinationState.UnderlyingState} [label=\"{stateInfoTransition.Trigger}\" ]");
                }
            }
            result.AppendLine("\t }");

            return result.ToString();

        }
    }
}
