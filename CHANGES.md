## Change Log ##

#### Version 5.7.0 ####
 * Updated to ReflectInsight v5.7.0.0

#### Version 5.6.2 ####
* Updated to support NLog v4.4.5
 
#### Version 5.6.1 ####
* Updated to NLog v4.3.5
* Updated to ReflectInsight v5.6.1.2
* Updated to .NET 4.5.1

#### Version 5.6.0 ####
 * Updated extension to use ReflectInsight 5.6.0 version
 * Dropping support for older .NET versions. As of this release, we're only deploying NuGet package for .NET 4.5. However the source code still supports older framework. 
 
#### Version 5.5.1 ####
 * Bug fixes for packages improperly being downloaded. RabbitMQ is now a nuget dependency.
 
#### Version 5.5.0 ####
 * Moved source code from [CodePlex](http://insightextensions.codeplex.com/ "CodePlex") to GitHub
 * Moved extension documentation to [ReflectInsight wiki](https://reflectsoftware.atlassian.net/wiki/display/RI5/ReflectInsight+5+documentation "ReflectInsight wiki") 
 * Updated extension to use ReflectInsight 5.5.0 version
 	* New HttpRequest message type
 	* New JSON message type
 	* General bug fixes and enhancements
 	* Performance tuning
 * Using [AppVeyor](http://www.appveyor.com/ "AppVeyor") for continue delivery (builds and NuGet publishing)