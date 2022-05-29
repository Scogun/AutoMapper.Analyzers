<div align="center">
	<img src="package/Logo.ico" alt="Analyzers logo"/>
</div>

# Table of Contents

1. [Overview](#overview)
2. [Structure](#structure)
   1. [Currently Available](#currently-available)
3. [Code Smells](#code-smells)
4. [Installation](#installation)
   1. [NuGet package](#nuget-package)
   2. [VSIX Extention](#vsix-extention)

## Overview
Here is AutoMapper Static Analyzers.

The main reason to write this VS Extension and NuGet package is that developers who is not so familiar with AutoMapper sometimes use it not proper way.

## Structure
##### Currently Available
The project already contains:
1. [AutoMapper.Analyzers.Common](src/AutoMapper.Analyzers.Common/AutoMapper.Analyzers.Common.csproj) - common AutoMapper Analyzers
2. [AutoMapper.Analyzers.Common.CodeFixes](src/AutoMapper.Analyzers.Common.CodeFixes/AutoMapper.Analyzers.Common.CodeFixes.csproj) - common fixes for raised AutoMapper Analyzers diagnostics
3. [AutoMapper.Analyzers.Common.Package](package/AutoMapper.Analyzers.Common.Package/AutoMapper.Analyzers.Common.Package.csproj) - project for building and publishing the NuGet package
4. [AutoMapper.Analyzers.Common.Tests](tests/AutoMapper.Analyzers.Common.Tests/AutoMapper.Analyzers.Common.Tests.csproj) - tests for common AutoMapper Analyzers
5. [AutoMapper.Analyzers.Vsix](package/AutoMapper.Analyzers.Vsix/AutoMapper.Analyzers.Vsix.csproj) - project to create VS Extension package

## Code Smells
<table>
	<tr>
		<th>Smells</th>
		<th>Availability</th>
		<th>Codes</th>
		<th>Level</th>
		<th>Description</th>
		<th>Fix</th>
	</tr>
	<tr>
		<td rowspan="7">Common smells</td>
		<td rowspan="5">Available</td>
		<td><b>AMA0001</b></td>
		<td rowspan="7">Warrning</td>
		<td>Profile doesn't contain maps</td>
		<td>In Progress...</td>
	</tr>
	<tr>
		<td><b>AMA0002</b></td>
		<td>Identical names properties are manual mapped</td>
		<td>Available</td>
	</tr>
	<tr>
		<td><b>AMA0003</b></td>
		<td>Manual checking that src is not null</td>
		<td>Available for next checking: "??", "== null", "!= null"</td>
	</tr>
	<tr>		
		<td><b>AMA0006</b></td>
		<td>Manual flattening of naming similar complex model</td>
		<td>Available</td>
	</tr>
	<tr>		
		<td><b>AMA0007</b></td>
		<td>Useless try-catch/finally covering of CreateMap calls.</td>
		<td>Available</td>
	</tr>
	<tr>
        <td rowspan="2">In Plans</td>
		<td><b>AMA0004</b></td>
		<td>ForMember ignore for all left properties</td>
		<td>...</td>
	</tr>
	<tr>		
		<td><b>AMA0005</b></td>
		<td>Manual flattening of complex model</td>
		<td>...</td>
	</tr>
</table>

## Installation
There are two ways to install analyzers:

### NuGet Package
Just add **AutoMapper.Analyzers** package into a project which you would like to control.

### VSIX Extention
In case of installation **VSIX extention** any project will be checking by **AutoMapper.Analyzers**.
So far, there is only one way to install the extension:
1. Build [Vsix](package/AutoMapper.Analyzers.Vsix/AutoMapper.Analyzers.Vsix.csproj) project
2. Find in `...\AutoMapper.Analyzers\package\AutoMapper.Analyzers.Vsix\bin\Release` folder `AutoMapper.Analyzers.Vsix.vsix` file
3. Run it

In nearest future VSIX extension will be pushed into VSIX repository.
