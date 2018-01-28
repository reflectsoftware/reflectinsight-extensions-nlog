# ReflectInsight NLog Extension Sample

This sample demonstrates how to use the ReflectInsight NLog Extension. The purpose of this sample is to show you how to leverage ReflectInsight with an pre-existing logging framework, like NLog.

This sample also uses satellite configuration files, which are a practice that we recommend doing. For more information about satellite configuration files, please
see the satellite configuration sample and/or our online resource.

## Getting Started

To install the NLog extension for ReflectInsight, run the following command in the Package Manager Console:

```powershell
Install-Package ReflectSoftware.Insight.Extensions.NLog
```

### Configuration

Start by adding an application configuration file (app.config) to your project. Then open app.config file and add a new configuration
section as shown here for "insightSettings":

```xml
<configSections>
    <section name="nlog" type="NLog.Config.ConfigSectionHandler, NLog" />
    <section name="insightSettings" type="ReflectSoftware.Insight.ConfigurationHandler,ReflectSoftware.Insight"/>
</configSections>
```

Now add a new section called <insightSettings></insightSettings> and add the **externalConfigSource** to point to your external ReflectInsight.config file which we will create next as shown here:

```xml
<insightSettings externalConfigSource="ReflectInsight.config" />
```

Now let's create a new configuration file for ReflectInsight. Add a new file to your project and call it **ReflectInsight.config**. 
Update the file property 'Copy to Output Directory' value to 'Copy always'. We will now add in basic configuration to log to the 
Live Viewer as well as to a Binary log file.

Here is the ReflectInsight configuration for the ReflectInsight.config file:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <insightSettings>
    <baseSettings>
      <configChange enabled="true"/>
      <propagateException enabled="false"/>
      <exceptionEventTracker time="20"/>
      <debugMessageProcess enabled="true"/>
    </baseSettings>

    <files default="">
      <autoSave name="save1" onNewDay="true" onMsgLimit="1000000" onSize="0" recycleFilesEvery="30" />
    </files>

    <listenerGroups active="Debug">
      <group name="Debug" enabled="true" maskIdentities="false">
        <destinations>
          <destination name="Viewer" enabled="true" filter="" details="Viewer"/>
          <destination name="BinaryFile" enabled="true" filter="" details="BinaryFile[path=$(workingdir)\Log\ReflectInsightLog.rlg; autoSave=save1]" />
        </destinations>
      </group>
    </listenerGroups>

    <logManager>
      <instance name="nlogInstance1" category="NLog" bkColor=""/>
    </logManager>
  </insightSettings>
</configuration>
```
  
Taking a closer look at the ReflectInsight configuration, you can see we have added two destinations defined for the **Viewer** and **BinaryFile** and both are enabled.
The BinaryFile has a default **autoSave** configuration which will create a new log file each day and/or when a message limit of 1 million is reached and the log files 
will be kept for 30 days befpre being cleaned up (deleted).

Finally, we have configured the ReflectInsight.config file to hookup to the NLog extension by defining the <logManager> and <instance> name of **nlogInstance1**. 

All that is left to do is update the NLog configuration sections  <extensions>, <targets> and <rules> for the ReflectInsight NLog extension details as shown here:
```xml
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
    <!-- In order to receive location information, you must ensure the layout has the parameter ${callsite} and all 
    its properties set accordantly. -->
    <extensions>
        <add assembly="ReflectSoftware.Insight.Extensions.NLog"/>
    </extensions>
    <targets>
        <target name="ReflectInsight" xsi:type="ReflectInsight" instanceName="nlogInstance1" displayLevel="true" displayLocation="true" layout="${callsite:className=true:fileName=true:includeSourcePath=true:methodName=true}"/>
    </targets>
    <rules>
        <logger name="*" minlevel="TRACE" writeTo="ReflectInsight"/>
    </rules>
</nlog>
```




## Resources

Please refer to the ReflectInsight [Documentation](https://reflectsoftware.atlassian.net/wiki/display/RI5/ReflectInsight+5+documentation) for details on configuring ReflectInsight.
       
Further configuration details on getting started with the [NLog Extension for ReflectInsight](https://reflectsoftware.atlassian.net/wiki/display/RI5/NLog+Extension).

Feedback is always welcome on our [UserVoice](http://reflectsoftware.uservoice.com/forums/158277-reflectinsight-feedback).

Contact [support](support@reflectsoftware.com) for any help!