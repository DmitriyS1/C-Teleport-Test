# CTeleport Simple Wheather API

## Technical task

**Task: Develop a Weather Forecasting Service**

<u>*Objective:*</u>
Build a weather forecasting REST-service that fetches weather data from a public API and provides responses to clients.
Requirements
- The service should fetch current weather data from a public API (e.g., OpenWeatherMap, Weatherstack).
- Clients should be able to request weather data for a specific city/date.
- Whatever you find relevant to show you expertise
  
It's allowed to use any 3-rd party components/frameworks or AI code-generation tools. Solution has to be based on .NET Core 8


## Solution Description

1. Because of the only one purpose of the application I decided not to divide it to several projects and instead used Folders to show specific areas of responsibility
2. I used version of Clean Architecture that I like the most for small projects. Core is responsible for app configurations and middlewares to configure application; Infrastructure is responsible for external communications (http and cache); Application is a Service layer where business logic is concentrated; Presentation layer is just Controllers in this case, only because of the folders structure. Technically Validators can be on that layer as well, because Validation works before business logic, but it's quite controversial moment because Validation is based on business rules so any way is acceptable
3. I decided to use Redis to store data about cities to have less requests to the api because requests over the limit are paid. Cities with their Longitude and Lattitude are stable and change rarely so it's a good way of optimization without problems. Redis instead of internal cache because it will allow us to redeploy application but have data in place
4. I decided not to store weather data in the cache because there are too many options for this. Plus, I don't see real optimization here. Amount of variants concerning City and Date is huge. Probably, a good optimization could be using Current day and store weather for today and for every city that was requested and lifetime of such data as 12 hours, because people usually look for a today's weather. But this is just a theory and has to be proved by real data and metrics
5. I added basic unit tests because it's essential to have a confirmation of your contracts and overall assurance that code is woking as expected
6. Added Docker support to make an application ready for cloud deployments and easier testing for new developers. Compose files are different for infrastructural entities aka Redis or database (if we need one in the future). So you can run and debug application from your IDE and with configuration that easily connects to all needed services or you can run everything in Docker to test the whole app
7. Added Metrics and Healthchecks also to be ready for cloud and make sure that we can handle the load from users. Metrics could be extended to measure Request-Response time, time of the http client requests and etc.
8. Added Swagger UI to have a map of the available endpoints
9. Added Rate limiter because of the public nature of the api. So it will prevent the server from the DoS attacks, though it has to be also configured before the server. On the load balancer level or behind Cloudflare. Clouds like AWS provide their solutions to prevent such attacks


## How to run

To run in docker you need to run this command `docker compose -f docker-compose-app.yml -f docker-compose.yml up -d` it will start the app on port 5944 and you can check the swagger by going to `localhost:5944/swagger`

To run in IDE you have to start redis container which can be done by `docker compose up -d`. Then you can debug. App runs on port 5093

## Request to test the app

To test the app, you have to provide your API key. Request it on OpenWheather web page. Add you api key to docker-compose configuration (CTELEPORT_WEATHER_API_WeatherHttpClientConfiguration:ApiKey=YOUR_API_KEY) and to appsettings.development.json file ("WeatherHttpClientConfiguration": {
    "ApiKey" : "your-api-key",
  }).

`localhost:5944/api/v1/weather?zip="35007"&countryCode="ES"&date=2024-10-5&units=standard` this will return you a wheather for Las Palmas de Gran Canaria for 5th of October 2024. You can use different zip code and Country code to get wheather for a different place

## Limitations

You can get Wheather forecast for 4 days ahead or until 1979-01-01.
