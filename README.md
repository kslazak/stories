# Stories API

This is a RESTful API implemented in ASP.NET Core that returns the specified number of "best stories" from the Hacker News API.

## Running the API

After building the project go to the output folder of the relevant configuration (either *\bin\Release\net8.0* or *\bin\Debug\net8.0*) and run the *StoriesAPI.exe* executable.

By default, the API uses cache retention of 30 seconds for *Development* environment and 60 seconds for *Production* environment. If you want to use a different retention period, you need to modify the *CacheRetentionSeconds* settings in relevant appsettings.json file.

The following cache retention values are allowed:

- -1 (infinite retention)
- 0 (no caching)
-  1 .. 2147483647 (1 .. 2147483647 seconds)

If an invalid retention period (or no retention period) is specified in a config file, the default value of infinite retention is being assumed.

## Usage

Using any HTTP client (e.g. an internet browser) request the following URL:
> https://localhost:7002/api/stories/best?count={n}

where {n} is the number of stories to be returned (between 1 and 200).
You can also use the following URL to see API definition and call it using *Swagger*:
> https://localhost:7002/

## Assumptions

- The documentation does not explicitly state how many items are being returned by https://hacker-news.firebaseio.com/v0/beststories.json but the actual number returned is 200, so the number of requested stories must be a value between 1 and 200.
- The documentation does not expicitly state that the results returned by https://hacker-news.firebaseio.com/v0/beststories.json are sorted by score but it can be inferred from the context and it has been confirmed by checking the returned results, so we assume the results are sorted.
- The value of *commentCount* is taken from *descendants* property (total comment count) rather than from the *kids* property (direct comments).

## Additional Considerations

If time allows the following changes could be introduced:

- Logging to a file.
- Using different logging levels (e.g. General, Debug, etc.) so that more details can be logged in the Debug mode, while only general information is being logged in Production code. Warnings and errors should always be logged.
- Wrapping the DateTime type in order to be able to mock it in unit tests for more accurate testing of time values.
- Adding behavior tests.
- Adding load tests and/or stress tests.