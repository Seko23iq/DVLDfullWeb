var builder = WebApplication.CreateBuilder(args);

// ✅ مرة وحدة فقط
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ✅ CORS أولاً قبل أي شيء
app.UseCors("AllowAll");

// ✅ Static Files بعد CORS مباشرة
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
        @"D:\imagesDVLDproject\people"
    ),
    RequestPath = "/people-images"
});

app.UseAuthorization();
app.MapControllers();
app.Run();