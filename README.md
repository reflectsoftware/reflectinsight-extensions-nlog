# ReflectInsight-Extensions-NLog

[![Build status](https://ci.appveyor.com/api/projects/status/github/reflectsoftware/reflectinsight-extensions-nlog?svg=true)](https://ci.appveyor.com/project/reflectsoftware/reflectinsight-extensions-nlog)
[![License](https://img.shields.io/:license-MS--PL-blue.svg)](https://github.com/reflectsoftware/reflectinsight-extensions-nlog/license.md)
[![Release](https://img.shields.io/github/release/reflectsoftware/reflectinsight-extensions-nlog.svg)](https://github.com/reflectsoftware/reflectinsight-extensions-nlog/releases/latest)
[![NuGet Version](http://img.shields.io/nuget/v/reflectsoftware.insight.extensions.nlog.svg?style=flat)](http://www.nuget.org/packages/ReflectSoftware.Insight.Extensions.NLog/)
[![Stars](https://img.shields.io/github/stars/reflectsoftware/reflectinsight-extensions-nlog.svg)](https://github.com/reflectsoftware/reflectinsight-extensions-nlog/stargazers)

**Package** - [ReflectSoftware.Insight.Extensions.NLog](http://www.nuget.org/packages/ReflectSoftware.Insight.Extensions.NLog/) | **Platforms** - .NET 4.5.1 and above

## Overview ##

We've added support for the NLog appender. This allows you to leverage your current investment in NLog, but leverage the power and flexibility that comes with the ReflectInsight viewer. You can view your NLog messages in real-time, in a rich viewer that allows you to filter out and search for what really matters to you.

The NLog extension supports v4.4.9. However if you need to support an older version, then you will need to download the ReflectInsight Logging Extensions Library from GitHub. You can then reference and rebuild the extension against your a specific release of the NLog dll. (Note: if you use the Nuget Package, the update to the latest version of NLog will be automatic).

## Benefits of ReflectInsight Extensions ##

The benefits to using the Insight Extensions is that you can easily and quickly add them to your applicable with little effort and then use the ReflectInsight Viewer to view your logging in real-time, allowing you to filter, search, navigate and see the details of your logged messages.

## Getting Started

To install ReflectSoftware.Insight.Extensions.NLog extension, run the following command in the Package Manager Console:

```powershell
Install-Package ReflectSoftware.Insight.Extensions.NLog
```

Then in your app.config or web.config file, add the following configuration sections:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <configSections>
    <section name="insightSettings" type="ReflectSoftware.Insight.ConfigurationHandler,ReflectSoftware.Insight" />
    <section name="nlog" type="NLog.Config.ConfigSectionHandler, NLog" />
  </configSections>

  <insightSettings>
    <baseSettings>
      <configChange enabled="true"/>
      <propagateException enabled="false"/>
      <exceptionEventTracker time="20"/>
      <debugMessageProcess enabled="true"/>
    </baseSettings>

    <listenerGroups active="Debug">
      <group name="Debug" enabled="true" maskIdentities="false">
        <destinations>
          <destination name="Viewer" enabled="true" filter="" details="Viewer"/>
        </destinations>
      </group>
    </listenerGroups>

    <logManager>
      <instance name="nlogInstance1" category="NLog" bkColor=""/>
    </logManager>
  </insightSettings>

  <nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
    <!-- In order to receive location information, you must ensure the layout has the parameter ${callsite} and all 
    its properties set accordantly. -->
    <extensions>
      <add assembly="ReflectSoftware.Insight.Extensions.NLog" />
    </extensions>
    <targets>
      <target name="ReflectInsight" xsi:type="ReflectInsight" instanceName="nlogInstance1" displayLevel="true" displayLocation="true" layout="${callsite:className=true:fileName=true:includeSourcePath=true:methodName=true}" />
    </targets>
    <rules>
      <logger name="*" minlevel="TRACE" writeTo="ReflectInsight" />
    </rules>
  </nlog>
  
  <startup> 
    <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.0" />
  </startup>
</configuration>
```

Additional configuration details for the **ReflectSoftware.Insight.Extensions.NLog** logging extension can be found in our [documentation](https://reflectsoftware.atlassian.net/wiki/display/RI5/NLog+Extension).


## Additional Resources

[Documentation](https://reflectsoftware.atlassian.net/wiki/display/RI5/ReflectInsight+5+documentation)

[Submit User Feedback](http://reflectsoftware.uservoice.com/forums/158277-reflectinsight-feedback)

[Contact Support](support@reflectsoftware.com)

[ReflectSoftware Website](http://reflectsoftware.com)
