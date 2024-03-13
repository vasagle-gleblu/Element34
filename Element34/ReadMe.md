# Element 34

A complementary framework for Selenium WebDriver in C#.

## Table of Contents

- [Project Overview](#project-overview)
- [Features](#features)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
- [Usage](#usage)
- [Contributing](#contributing)
- [License](#license)
- [Acknowledgments](#acknowledgments)

## Project Overview

Selenium WebDriver is a tool used for automating web-based application testing.  However, the module leaves you with a bare bones API with no means of loading in test data or recording basic results in a report.  This project is meant to bridge that gap by providing such a complementary framework to WebDriver in C#.

## Features

With a focus on practicality, and scripting-centric design, this package integrates a range of features to elevate your scripting and deliver results.

- Integrated Test Data Loading Solution including support for:
	- Excel files (XLS/XLSX)
	- Comma Separated Value files (CSV)
	- Access database files (MDB/ACCDB)
	- MS-SQL Server
	- Other databases?
- Integrated Desktop Video Recording
- Integration of WebDrivers for the following browsers:
	- Google Chrome
	- Mozilla Firefox
	- Microsoft Edge
	- Microsoft Internet Explorer
- Something, something...

## Getting Started

To get started you should clone the code from GitHub and download the required NuGet packages.

### Prerequisites
- .NET Framework 4.8.1
- other platforms?

#### NuGet Packages:
- DotNetSeleniumExtras.WaitHelpers by SeleniumExtras.WaitHelpers: This package provides an implementation of the ExpectedConditions class for use with WebDriverWait in .NET, replacing the implementation originally provided by the Selenium project.
- Selenium.Support by Selenium Committers: Provides support classes for Selenium Webdvr.
- Selenium.WebDriver by Selenium Committers: .NET bindings for the Selenium WebDriver API.
- EPPlusFree by Rimland: EPPlusFree is an unofficial EPPlus library, it is a continuation of EPPlus Free Edition 4.5.3.3
- Selenium.WebDriver.ChromeDriver by jsakamoto: Google Selenium.WebDriver.ChromeDriver package.
- Selenium.WebDriver.GeckoDriver by jsakamoto: Firefox Selenium.WebDriver.GeckoDriver package.
- Selenium.WebDriver.IEDriver by jsakamoto: Microsoft Selenium.WebDriver.IEDriver package.
- Selenium.WebDriver.MSEdgeDriver by leandrogomes: Microsoft Edge (Chromium) WebDriver Package.
- SharpAvi by baSSiLL: A simple library for creating video files in the AVI format.
- NUnit by Charlie Poole, Rob Prouse: NUnit is a unit-testing framework for all .NET languages with a strong TDD focus.
- NUnit3TestAdapter by Charlie Poole, Terje Sandstrom:  NUnit3 TestAdapter for Visual Studio and DotNet.
- Newtonsoft.Json by James Newton-King: Json.NET is a popular high-performance JSON framework for .NET.

#### Project References:
- ADODB
- ADOX
- EPPlusFree
- Microsoft.Bcl.AsyncInterfaces
- Microsoft.Bcl.HashCode
- Microsoft.CSharp
- Microsoft.IdentityModel.Abstraction
- Microsoft.IdentityModel.Logging
- Microsoft.IdentityModel.Tokens
- Microsoft.VisualBasic
- Newtonsoft.Json
- NLog
- PresentationCore
- SeleniumExtras.WaitHelpers
- SharpAvi
- System
- System.Buffers
- System.Collections.Immutable
- System.Collections.NonGeneric
- System.configuration
- System.Core
- System.Data
- System.Data.DataSetExtensions
- System.Drawing
- System.Drawing.Common
- System.IO.Compression
- System.Memory
- System.Net.Http
- System.Runtime.CompilerService.Unsafe
- System.Runtime.Serialization
- System.Security
- System.Text.Encodings.Web
- System.Threading.Tasks.Extensions
- System.ValueTuple
- System.Web
- System.Windows
- System.Windows.Forms
- System.Xml
- System.Xml.Linq
- WebDriver
- Webdvr.Support
- WindowsBase

### Installation

Provide step-by-step instructions for installing your project. Include code snippets if necessary.

```shell
# Example installation commands
git clone https://github.com/yourusername/yourproject.git
cd yourproject
npm install
```

## Usage

## Contributing

## License

## Acknowledgements



