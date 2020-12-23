#load "Helper.fsx"
#r "nuget: Akka.FSharp"
#r "nuget: Akka.Remote"
open Helper
open System
open Akka.Actor
open Akka.Configuration
open Akka.FSharp
open Akka.Remote
open System.Collections.Generic

let args = System.Environment.GetCommandLineArgs()
let configuration = 
    ConfigurationFactory.ParseString(
        @"akka {
            log-config-on-start : on
            stdout-loglevel : DEBUG
            loglevel : ERROR
            actor {
                provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
                debug : {
                    receive : on
                    autoreceive : on
                    lifecycle : on
                    event-stream : on
                    unhandled : on
                }
            }
            remote {
                helios.tcp {
                    port = 8778
                    hostname = localhost
                }
            }
        }")

let system = ActorSystem.Create("RemoteFSharp", configuration)

type ServerInitMessage = {
    ServerInit: bool;
} 

type ServerCompletionMessage = {
    ServerCompleted: bool;
}

type Server() =
    inherit Actor()
    let mutable originalSender = null
    let mutable tweetsServed = 0
    let mutable retweetsServed = 0
    let registeredUsers = Dictionary<int, int>()
    let followers = Dictionary<int, List<int>>()
    let mentions = Dictionary<string, List<string>>()
    let hashtags = Dictionary<string, List<string>>()
    override x.OnReceive (message:obj) =   
        match message with
        | :? ServerInitMessage as msg ->
            printfn "Initialized Server"
            originalSender <- x.Sender
        | :? RegisterAccount as msg ->
            registeredUsers.Add(msg.UserId, 1)
            let mutable list = new List<int>()
            followers.Add(msg.UserId, list)
            //x.Sender.Tell { Ack = true }
            //printfn "Registered: %d" msg.UserId
        | :? FollowAccount as msg ->
            let follower = msg.MyId
            let following = msg.UserId
            followers.Item(follower).Add(following)
        | :? DisconnectMessage as msg ->
            registeredUsers.Item(msg.DisconnectMe) <- 0
        | :? TweetMessage as msg ->   
            let tweet = msg.Tweet
            let tweeter = msg.Tweeter
            let rt = msg.RT
            if rt = true then
                retweetsServed <- retweetsServed + 1
            else
                tweetsServed <- tweetsServed + 1
            // printfn "%d" tweetsServed
            if tweet <> "FINISH" then
                if tweet.Contains("@") && tweet.Contains("#") then
                    let hashtag = (tweet.Split "#").[1]
                    if not(hashtags.ContainsKey(hashtag)) then
                        let mutable list = new List<string>()
                        hashtags.Add(hashtag, list)
                    hashtags.Item(hashtag).Add(tweet)
                    let mention = ((tweet.Split "#").[0].Split "@").[1]
                    if not(mentions.ContainsKey(mention)) then
                        let mutable list = new List<string>()
                        mentions.Add(mention, list)
                    mentions.Item(mention).Add(tweet) 
                else if tweet.Contains("@") then
                    let mention = (tweet.Split "@").[1]
                    if not(mentions.ContainsKey(mention)) then
                        let mutable list = new List<string>()
                        mentions.Add(mention, list)
                    mentions.Item(mention).Add(tweet)  
                else if tweet.Contains("#") then
                    let hashtag = (tweet.Split "#").[1]
                    if not(hashtags.ContainsKey(hashtag)) then
                        let mutable list = new List<string>()
                        hashtags.Add(hashtag, list)
                    hashtags.Item(hashtag).Add(tweet)
                for follower in followers.Item(tweeter) do
                    if registeredUsers.Item(follower) = 1 then
                        let followerRef = system.ActorSelection("akka.tcp://RemoteFSharp@localhost:7887/user/"+string(follower))
                        followerRef.Tell { TweeterYouFollow = tweeter; TweetYouGot = tweet; RT = rt }
                x.Sender.Tell { TweetAck = true; RT = rt; }
            else
                x.Sender.Tell { TweetAck = true; RT = rt; }
                originalSender.Tell { ServerCompleted = true }
                printfn "Completed."
        | _ -> printfn "ERROR WHILE PARSING MESSAGE"

let server = system.ActorOf(Props(typedefof<Server>), "Server")
let (task:Async<ServerCompletionMessage>) = ( server <? { ServerInit = true; })
let response = Async.RunSynchronously (task)
server.Tell(PoisonPill.Instance);