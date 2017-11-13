using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Graphviz4Net.Dot.AntlrParser;
using Graphviz4Net.Graphs;
using Stateless;
using Stateless.Graph;
using Stateless.Visualization;

namespace ConsoleApp2
{
    enum UserStates{Empty,Active,New,LoggedIn, LoggedOut }
    enum Trigger { RegisterAccount, VerifyEmail, LogIn,LogOut,Deactivate }

    class Program
    {
        static void Main(string[] args)
        {
            var userRegistrationMachine= new StateMachine<UserStates, Trigger>(UserStates.Empty);

            userRegistrationMachine.Configure(UserStates.Empty)
                .Permit(Trigger.RegisterAccount, UserStates.New);

            userRegistrationMachine.Configure(UserStates.New)
                .Permit(Trigger.VerifyEmail, UserStates.Active);

            userRegistrationMachine.Configure(UserStates.Active)
                .Permit(Trigger.LogIn, UserStates.LoggedIn)
                .Permit(Trigger.Deactivate, UserStates.New);

            userRegistrationMachine.Configure(UserStates.LoggedIn)
                .SubstateOf(UserStates.Active)
                .Permit(Trigger.LogOut, UserStates.LoggedOut);

            userRegistrationMachine.Configure(UserStates.LoggedOut)
                .SubstateOf(UserStates.Active)
                .Permit(Trigger.LogIn, UserStates.LoggedIn);

            userRegistrationMachine.Activate();
            using (var sw = new StreamWriter("graph1.dot"))
            {
               sw.Write( userRegistrationMachine.ToGraph());
                sw.Flush();
            }
            return;
            var graph=new Graph<UserStates>();

            foreach (var stateInfo in userRegistrationMachine.GetInfo().States)
            {
                graph.AddVertex((UserStates)stateInfo.UnderlyingState);

            }
            foreach (var stateInfo in userRegistrationMachine.GetInfo().States)
            {
                foreach (var stateInfoTransition in stateInfo.FixedTransitions)
                {
                    var sourceState = (UserStates)stateInfo.UnderlyingState;
                    var destinationState = (UserStates)stateInfoTransition.DestinationState.UnderlyingState;
                    graph.AddEdge(new Edge<UserStates>(sourceState, destinationState){Label = stateInfoTransition.Trigger.ToString()});

                }
            }
            graph.Ratio = 100;
            var value = graph.ToString();
            using (var sw=new StreamWriter("graph.dot"))
            {
                value= value.Replace(":", "{");
                value = value.Replace("graph", "digraph");
                value = value.Replace("--", "->");
                value = value + "}";
                sw.Write(value);
                sw.Flush();
            }
            Console.WriteLine(value);
            //Console.WriteLine(userRegistrationMachine.GetInfo().States.FirstOrDefault().Transitions.FirstOrDefault().Trigger.ToString());
            Console.WriteLine(userRegistrationMachine.CanFire(Trigger.LogIn));
        }
    }
}
