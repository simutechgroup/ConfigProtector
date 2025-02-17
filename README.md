# ConfigProtector

ConfigProtector is a simple console application written in C# (.NET 4.x) that encrypts and decrypts sections in a web.config file. This tool supports both top-level and nested configuration sections (such as *appSettings*, *connectionStrings*, and custom nested sections like *system.net/mailSettings/smtp*) using the built-in DataProtectionConfigurationProvider.

## Features

- **Encrypt/Decrypt Web.config Sections:** Easily encrypt or decrypt configuration sections using command-line flags.
- **Supports Nested Sections:** Automatically traverse nested section groups (e.g., `system.net/mailSettings/smtp`).
- **Simple Command-Line Interface:** Process a given configuration file by providing the folder path and desired operation.

## Prerequisites

- **.NET Framework 4.x:** The project targets .NET Framework 4.x.
- **Visual Studio or any C# Compiler:** To build the project.
- **Familiarity with ASP.NET Configuration:** Basic understanding of web.config and its sections is helpful.

## Getting Started

### Clone the Repository

Clone this repository to your local machine using:

```bash
git clone https://github.com/simutechgroup/ConfigProtector.git
