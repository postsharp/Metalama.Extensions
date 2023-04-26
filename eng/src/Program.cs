﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using PostSharp.Engineering.BuildTools;
using PostSharp.Engineering.BuildTools.Build.Model;
using PostSharp.Engineering.BuildTools.Build.Solutions;
using PostSharp.Engineering.BuildTools.Dependencies.Model;
using Spectre.Console.Cli;

var product = new Product( Dependencies.MetalamaExtensions )
{
    Solutions = new Solution[] { new DotNetSolution( "Metalama.Extensions.sln" ) { CanFormatCode = true } },
    PublicArtifacts = Pattern.Create(
        "Metalama.Extensions.DependencyInjection.$(PackageVersion).nupkg",
        "Metalama.Extensions.DependencyInjection.ServiceLocator.$(PackageVersion).nupkg",
        "Metalama.Extensions.Multicast.$(PackageVersion).nupkg",
        "Metalama.Extensions.Metrics.Redist.$(PackageVersion).nupkg",
        "Metalama.Extensions.Metrics.$(PackageVersion).nupkg",
        "Metalama.Extensions.Architecture.$(PackageVersion).nupkg" ),
    Dependencies = new[] { Dependencies.PostSharpEngineering, Dependencies.Metalama },
    MainVersionDependency = Dependencies.Metalama,
    
    // MergePublisher disabled for 2023.1.
    // Configurations = Product.DefaultConfigurations
    //     .WithValue(
    //     BuildConfiguration.Public, Product.DefaultConfigurations.Public with { 
    //         PublicPublishers = Product.DefaultPublicPublishers.Add( new MergePublisher() ).ToArray() } )
};

var commandApp = new CommandApp();

commandApp.AddProductCommands(product);

return commandApp.Run(args);