module Helper
type RegisterAckMessage = {
    Ack: bool;
}

type RegisterAccount = {
    UserId: int;
}

type DisconnectMessage = {
    DisconnectMe: int;
}

type FollowAccount = {
    MyId: int;
    UserId: int;
}

type TweetMessage = {
    Tweeter: int;
    Tweet: string;
    RT: bool;
}

type TweetAckMessage = {
    TweetAck: bool;
    RT: bool;
}

type UpdatedTimelineMessage = {
    TweeterYouFollow: int;
    TweetYouGot: string;
    RT: bool;
}

type StopWorkMessage = {
    StopWork: bool;
}