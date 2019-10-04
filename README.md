# Springfield Devs Meetup RSVP Tool

This tool was created for 3 reasons.

1. A quick way to get a normalized headcount for capacity and food orders
2. Random member selection for raffles
3. It was a good fit for a presentation on Blazor

As mentioned above, this tools is created in [Blazor](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor). Specifically the [WebAssembly](https://docs.microsoft.com/en-us/aspnet/core/blazor/hosting-models?view=aspnetcore-3.0#blazor-webassembly) hosted model so it can run on a simple server.

To work with this project you will need the [.NET Core SDK 3.0+](https://dotnet.microsoft.com/apps/aspnet). Once installed you should be able to run the project by running the following in the root of the project

`dotnet run`

Optionally you can run this to automatically rebuild the project on file change

`dot watch run`

This tool is hosted on Netlify which runs the compiled assets in the `dist` directory. To compile the solution run the following

`dotnet publish -c release -o dist`

NOTE: Still trying to find a better way to deal with this... If you already have a `dist` folder as a result of publishing. Run this so there isn't any dist-ception.

`rm -rf dist && dotnet publish -c release -o dist`