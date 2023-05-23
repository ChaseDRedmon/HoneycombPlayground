using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

var honeycombOptions = builder.Configuration.GetHoneycombOptions();

// Setup OpenTelemetry Tracing
builder.Services
       .AddOpenTelemetry()
       .WithTracing(otelBuilder =>
       {
           otelBuilder
               .AddHoneycomb(honeycombOptions)
               .AddCommonInstrumentations();
       });

// Register Tracer so it can be injected into other components (eg Controllers)
builder.Services.AddSingleton(TracerProvider.Default.GetTracer(honeycombOptions.ServiceName));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/", (Tracer tracer) =>
{
    using var span = tracer.StartActiveSpan("app.manual-span");
    span.SetAttribute("app.manual-span.message", "Adding custom spans is also super easy!");
});

app.Run();