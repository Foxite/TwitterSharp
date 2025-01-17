# TwitterSharp
C# wrapper around Twitter API V2

| CI | Code Quality | Coverage |
| -- | ------------ | -------- |
| [![.NET](https://github.com/Xwilarg/TwitterSharp/actions/workflows/ci.yml/badge.svg)](https://github.com/Xwilarg/TwitterSharp/actions/workflows/ci.yml) | [![Codacy Badge](https://app.codacy.com/project/badge/Grade/726fd5c6287644d48807fcf03a18d868)](https://www.codacy.com/gh/Xwilarg/TwitterSharp/dashboard?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=Xwilarg/TwitterSharp&amp;utm_campaign=Badge_Grade) | [![Codacy Badge](https://app.codacy.com/project/badge/Coverage/726fd5c6287644d48807fcf03a18d868)](https://www.codacy.com/gh/Xwilarg/TwitterSharp/dashboard?utm_source=github.com&utm_medium=referral&utm_content=Xwilarg/TwitterSharp&utm_campaign=Badge_Coverage) |

## Download

The package is available as a pre-release on [NuGet](https://www.nuget.org/packages/TwitterSharp/)
```powershell
Install-Package TwitterSharp -Version 0.1.0-alpha
```

## How does it works?

To begin with, please go to the [Twitter Developer Portal](https://developer.twitter.com/) and create a new application\
Then you must instantiate a new client:
```cs
var client = new TwitterSharp.Client.TwitterClient(bearerToken);
```
From there you can access various methods to access tweets and users, however please make note that a basic request only includes:
 - For tweets: its ID and content
 - For users: its ID, name and username

To solve that, most function take an array of UserOption or TweetOption, make sure to add what you need there!

Need more help? You can use the examples below, if you're still lost feel free to open an issue or a discussion!

## Examples
### Get a tweet from its ID
```cs
var client = new TwitterSharp.Client.TwitterClient(bearerToken);
var answer = await client.GetTweetAsync("1389189291582967809");
Console.WriteLine(answer[0].Text); // たのしみ！！\uD83D\uDC93 https://t.co/DgBYVYr9lN
```

### Get an user from its username
```cs
var client = new TwitterSharp.Client.TwitterClient(bearerToken);
var answer = await client.GetUsersAsync("theindra5");
Console.WriteLine(answer[0].Id); // 1022468464513089536
```

### Get latest tweets from an user id
```cs
var client = new TwitterSharp.Client.TwitterClient(bearerToken);
// You can get the id using GetUsersAsync
var answer = await client.GetTweetsFromUserIdAsync("1109748792721432577", new[] { TweetOption.Entities }, null, null);
for (int i = 0; i < answer.Length; i++)
{
    Console.WriteLine($"Tweet n°{i}:");
    Console.WriteLine(answer[i].Text);
    Console.WriteLine("URLs:");
    Console.WriteLine(string.Join("\n", answer[i].Entities.Urls.Select(x => x.DisplayUrl)));
    Console.WriteLine("\n");
}
```

### Follow users and get all new tweets
```cs
var client = new TwitterSharp.Client.TwitterClient(bearerToken);

// Subscribe to 5 Twitter accounts
var request = new TwitterSharp.Request.StreamRequest(
    Expression.Author("moricalliope") // using TwitterSharp.Rule;
        .Or(
            Expression.Author("takanashikiara"),
            Expression.Author("ninomaeinanis"),
            Expression.Author("gawrgura"),
            Expression.Author("watsonameliaEN")
        )
);
await client.AddTweetStreamAsync(request); // Add them to the stream

// We display all the subscriptions we have
var subs = await client.GetInfoTweetStreamAsync();
Console.WriteLine("Subscriptions: " + string.Join("\n", subs.Select(x => x.Value.ToString())));

// NextTweetStreamAsync will continue to run in background
Task.Run(async () =>
{
    // Take in parameter a callback called for each new tweet
    // Since we want to get the basic info of the tweet author, we add an empty array of UserOption
    await client.NextTweetStreamAsync((tweet) =>
    {
        Console.WriteLine($"From {tweet.Author.Name}: {tweet.Text}");
    }, null, Array.Empty<UserOption>());
});
```

## Contributing

If you want to contribute feel free to open a pull request\
You can also see how the project is going in the [project tab](https://github.com/Xwilarg/TwitterSharp/projects/1)
