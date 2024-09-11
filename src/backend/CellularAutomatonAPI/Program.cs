var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("Policy", configurePolicy =>
    {
        // TODO: set defaults origins. 
        configurePolicy.AllowAnyOrigin();
        configurePolicy.AllowAnyHeader();
        configurePolicy.AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("Policy");

var api = new ApiMethods();

app.MapPost("/Start", ( PostStartApi x) => api.HandleStartRequest(x));
app.MapGet("/UpdateCurrentState", () => api.handleUpdateRequest());
app.Run();