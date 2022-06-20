using PostSharp.Engineering.BuildTools;
using PostSharp.Engineering.BuildTools.Build.Model;
using PostSharp.Engineering.BuildTools.Build.Solutions;
using PostSharp.Engineering.BuildTools.Dependencies.Model;
using Spectre.Console.Cli;

var product = new Product(Dependencies.MetalamaFrameworkExtensions)
{
    Solutions = new[] { new DotNetSolution("Metalama.Framework.Extensions.sln") { CanFormatCode = true } },
    PublicArtifacts = Pattern.Create(
        "Metalama.Framework.DependencyInjection.$(PackageVersion).nupkg",
        "Metalama.Framework.DependencyInjection.ServiceLocator.$(PackageVersion).nupkg" ),
    Dependencies = new[] { Dependencies.PostSharpEngineering, Dependencies.Metalama },
    MainVersionDependency = Dependencies.Metalama
};

var commandApp = new CommandApp();

commandApp.AddProductCommands(product);

return commandApp.Run(args);