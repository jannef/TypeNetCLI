# TypeNetCLI
Generating Typescript class definitions from dotnet classes with CLI.

## Inspiration
I've worked with [TypeWriter](https://frhagn.github.io/Typewriter/) before, and it was amazing. TypeWriter is a product of its own time however, and it's time to move on.

## Description
TypeNetCLI aims to automate generating typescript definitions for you. Write data contracts once in dotnet, and enjoy the show.

I'm looking to create an easy, somewhat configurable, and perhaps extendable way of generating typescript classes with no dependencies and no hassle.

## Plans
* Generate typescript classes and enums
* Publish as dotnet tool
* Upgrage to .NET 6

## Out of scope (for now)
* Any code compilation, just read dll's produced with `dotnet publish`.
