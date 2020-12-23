# DOS (COP-5615) Project 4-1 Report

## Information about files:
- The project consists of 3 files - server.fsx, client.fsx and helper.fsx.
- server.fsx contains server code which runs as a standalone program/process.
- client.fsx contains client + simulator(Client Manager in the code) which runs different users as
multiple actors under the hood of the same program.
- helper.fsx contains helper function/types that are used by both server and client.

## Implemented functionalities:
- Mentions, hashtags.
- Retweets.
- Disconnection/Connection.
- Zipf distribution.

## Steps to run:
- Run command - `dotnet fsi --langversion:preview .\server.fsx` to start the server.
- Wait until “Initialized Server” appears on the screen.
- After “Initialized Server” appears. Open another terminal.
- Run command - `dotnet fsi --langversion:preview .\client.fsx` to start the client/simulator.
- Wait… .
- Done. Check the metrics outputted on the client terminal.
- Note: If you see the server has exited but client hasn’t, please rerun the simulation from
the first step. This shouldn’t happen but has happened once or twice due to issue with
random numbers.

## Settings in program:
- Users divided into 3 categories - high, medium and low profile.

|   | High Profile | Medium Profile | Low Profile |
| ------------- | ------------- | ------------- | ------------- |
| Users (% of total)  | 0.1%  | 5% | rest |
| Followers (% of total)  | 30-50%  | 1-10% | 0.05-0.1% |
| Number of tweets | 50-70 | 15-30 | 0-10 |
- Disconnected users - 0 to 10% of total users.
- Retweets - 0 to 10% of total tweets from followers.

## Performance: (intel i5 8th gen- 4 core - 8GB ram)
- Max users tested - 10,000. More than 10,000 is possible but not tested as it was taking
more than 15 minutes.
- High Profile Medium Profile Low Profile
- Users (% of total) 0.1% 5% rest
- Followers (% of total) 30-50% 1-10% 0.05-0.1%
- Number of tweets 50-70 15-30 0-10

|  Users | Tweets (RT + Tweets) | Time Taken(ms) | Tweets Per Second | Disconnected Users |
| ------------- | ------------- | ------------- | ------------- | ------------- |
| 500 | 345+2625=2970 | 2619 | 1113 | 55 |
| 1000 | 1566+5620=718 | 6 | 4827 | 1488 | 96 |
| 2000 | 5937+10840=16 | 777 | 11918 | 1407 | 198 |
| 5000 | 40336+26999=6 | 7335 | 142732 | 471 | 523 |
| 10000 | 167296+54340=221636 | 698348 | 317 | 1043 |

## Observations:
- Due to random number generators behaving in a non-uniform way, the functions that are
using them for e.g. retweets generation are not very accurate and thus causing retweets
to skyrocket sometimes.
