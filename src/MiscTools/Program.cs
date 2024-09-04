using MiscTools;
using MiscTools.Services;
using StrongInject;

using Container container = new();
using Owned<App> app = container.Resolve();
app.Value.Run();
