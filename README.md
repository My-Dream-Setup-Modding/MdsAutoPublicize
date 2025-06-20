# This mod got removed.

This mod was originally built with [CabbageCrow.AssemblyPublicizer](https://github.com/CabbageCrow/AssemblyPublicizer) in mind. <br/>
But it turns out, there already exists a very easy way, to publicize library references already with [BepInEx.AssemblyPublicizer](https://github.com/BepInEx/BepInEx.AssemblyPublicizer).<br/>
So, if you want to look at some code, to publicize the game libraries on game start, feel free to look at the files in this [commit](https://github.com/My-Dream-Setup-Modding/MdsAutoPublicize/tree/456350ae6fd4255cf5dfbe760df7b67c8d6a0e2b).<br/>
<br/>
## Create your own My Dream Setup mod, while using publicized game files.
1. Create a new Class Library project <br/>
  Easiest framework to use, is netstandard 2.1, but .NET Framework 4.8 also works. .NET6 and higher will run into problems, since BepInEx 5.4 does not seem to support it.
2. Copy paste, the
[Dependencies.props](https://github.com/My-Dream-Setup-Modding/MdsAutoPublicize/blob/master/Dependencies.props) and
[GameFolder.props](https://github.com/My-Dream-Setup-Modding/MdsAutoPublicize/blob/master/GameFolder.props)
into your project.
4. You might have to change the settings in the GameFolder.props file.
5. Go into your csproj file, and add `<Import Project="Dependencies.props" />` inside the Project object. <br/>
   A csproj example file looks like this:
```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>netstandard2.1</TargetFramework>
    <!--<Nullable>true</Nullable>-->
  </PropertyGroup>

  <Import Project="Dependencies.props" />
</Project>
```
Now you have all needed game file dependencies to create your own BepInEx mod for the game "My Dream Setup". <br/>

<s>
# Automatic Assembly Publicizer

This is a unity mod for other mod creators, to automate the process of publicizing the game files with 
[BepInEx.AssemblyPublicizer](https://github.com/BepInEx/BepInEx.AssemblyPublicizer). 
<br/><br/>
It automatically enumerates through all dll files located in <br/>
`MDS_Data\Managed` <br/>
and creates a publicized version of it here <br/>
`BepInEx\BepInEx.AssemblyPublicizer.Publicized`

A check file is also created, to update the publicized files, when the game gets updated.

## Use cases

It is mainly interesting for other mod creators, to more easily make non public calls.
A good start for your mod is to use 
[Dependencies.props](https://github.com/My-Dream-Setup-Modding/MdsAutoPublicize/blob/master/MdsAutoPublicize/Dependencies.props)
and
[GameFolder.props](https://github.com/My-Dream-Setup-Modding/MdsAutoPublicize/blob/master/MdsAutoPublicize/GameFolder.props)
to import the dependencies from BepInEx, other mods and the My Dream Setup Game libraries.

## Create your own My Dream Setup mod, with the use of publicized game libraries:
1. Create a new Class Library project <br/>
  Easiest framework to use, is netstandard 2.1, but .NET Framework 4.8 also works. .NET6 and higher will run into problems, since BepInEx 5.4 does not seem to support it.
2. Copy paste, the
[Dependencies.props](https://github.com/My-Dream-Setup-Modding/MdsAutoPublicize/blob/master/MdsAutoPublicize/Dependencies.props) and
[GameFolder.props](https://github.com/My-Dream-Setup-Modding/MdsAutoPublicize/blob/master/MdsAutoPublicize/GameFolder.props)
into your project.
4. Go into your csproj file, and add `<Import Project="Dependencies.props" />` inside the Project object. <br/>
   An Example is [this mod](https://github.com/My-Dream-Setup-Modding/MdsAutoPublicize/blob/master/MdsAutoPublicize/MdsAutoPublicize.csproj).
5. Now you have all needed dependencies to create your own BepInEx mod for the game "My Dream Setup". <br/>
  If you've run this mod atleast once, the dependencies automatically switch to publicized libraries and activate unsafe code.
</s>
   
   
