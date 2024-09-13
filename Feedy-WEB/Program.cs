// define IP-Port in: Project->Properties->launchSettings.json  --> http.applicationUrl .... (line 10)
//

using Feedy_WEB.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.Add(new ServiceDescriptor(typeof(MachineInterface), new MachineInterface()));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

FDGlobals.WebSocket.Start();

Thread timer = new Thread(() =>
{
    int ticks=0;
    while(true)
    {
        Thread.Sleep(1000);
        string msg=string.Format("ticks={0}\ntest={0}", ++ticks);
        FDGlobals.WebSocket.Send(msg);
    }
});
timer.Start();

app.Run();
