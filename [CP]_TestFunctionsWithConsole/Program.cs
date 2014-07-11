using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neo4jClient;
using Neo4jClient.Cypher;
using C5;
namespace _CP__TestFunctionsWithConsole
{
    class Program
    {
        public static void Main(string[] args)
        {
            var graphClient = new GraphClient(new Uri("http://54.255.155.78:7474/db/data"));
            graphClient.Connect();
            //Test return order
            //MATCH (user:User {userID : '1'})-[:ego1*]->(friend)
            //RETURN friend
            //CypherQuery query = new CypherQuery("MATCH (user:User {userID : '2'})-[:ego2*]->(friend) RETURN friend", new Dictionary<string, object>(), CypherResultMode.Set);
            //var friends = ((IRawGraphClient)graphClient).ExecuteGetCypherResults<User>(query).ToList();
            //for (int i = 0; i < friends.Count; i++)
            //{
            //    Console.WriteLine(friends[i].userID);
            //}
            var query = graphClient.Cypher.Match("(user:User {userID : '1'})-[:ego1*]->(friend)-[:LATEST_ACTIVITY]->(latest_activity)-[:NEXT*]->(next_activity)").
                    Return((friend, latest_activity, next_activity) => new
                    {
                        friends = friend.As<User>(),
                        latest_activity = latest_activity.As<Activity>(),
                        next_activities = next_activity.CollectAs<Activity>()
                    }).Results;
            List<User> friendList = new List<User>();
            List<List<Activity>> activities = new List<List<Activity>>();
            List<Activity> activity, tmp;
            foreach (var item in query)
            {
                friendList.Add(item.friends);
                Console.Write("UserID: " + item.friends.userID + ". Activities: ");
                activity = new List<Activity>();
                activity.Add(item.latest_activity);
                Console.Write(item.latest_activity.timestamp + " ");
                foreach (var i in item.next_activities)
                {
                    activity.Add(i.Data);
                    Console.Write(i.Data.timestamp + " ");
                }
                activities.Add(activity);
                Console.WriteLine();
            }
            //for (int i = 0; i < friendList.Count; i++)
            //{
            //    Console.Write("UserID: " + friendList[i].userID + ". Activities: ");
            //    tmp = activities[i];
            //    for (int j = 0; j < tmp.Count; j++)
            //    {

            //        Console.Write(tmp.ElementAt(j).timestamp + " ");
            //    }
            //    Console.WriteLine();
            //}
            Console.ReadLine();
        }
    }
}
