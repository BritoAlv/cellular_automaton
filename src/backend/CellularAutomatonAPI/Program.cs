var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var api = new ApiMethods();

app.MapPost("/Start", ( PostStartApi x) => api.HandleStartRequest(x));
app.MapGet("/UpdateCurrentState", () => api.handleUpdateRequest());
app.Run();