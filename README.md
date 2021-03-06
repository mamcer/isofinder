# IsoFinder

An ASP .NET MVC and two Winforms applications from 2015

In `original` branch you will find the original source code for this application. In `master` an upgraded, refactored version.

> More details about why I published this project in [this blog post](https://mamcer.github.io/2018-09-02-i-cleaned-up-my-virtual-basement/)

## Description

IsoFinder is one of the solutions with more time invested from my side. It started by 2010 with a POC and I completely rewrite it several times. A WPF application, MVC and even an AngularJS version. This is the latest version. 

IsoFinder is an application that helps the user find information (files, folders) inside volumes compiled as iso files.

Main Functionalities:

* Quick search of information 
* Advanced searches
* Download of files and folders as zip files

Summarized workflow: With IsoFinder.Scanner a user scan different iso files and index the content in a SQL Server database. A web application works as front end, making it easier to search for content and navigate trough the stored volumes. It also allows a user to make request to download files or folders included in the iso volumes. That requests are processed by another application, IsoFinder.Handle. An application which is waiting for request, mount the iso files, copy the requested files or folders making them available to the user as a zip file download in the Web application.

Main Projects:

### IsoFinder Web

A web application. Main front-end that exposes the search and download functionality. Still in progress, some functionality is still missing like the user profile

### IsoFinder IsoScanner

A desktop application that scan iso files and save new information to the database.

### IsoFinder Handler

An application that is waiting for user requests, mount the iso file and return the requested files/folders.

Initially hosted in a private bitbucket repository.

## Screenshot

![screenshot](https://raw.githubusercontent.com/mamcer/isofinder/master/doc/screenshot-01.png)

![screenshot](https://raw.githubusercontent.com/mamcer/isofinder/master/doc/screenshot-02.png)

![screenshot](https://raw.githubusercontent.com/mamcer/isofinder/master/doc/screenshot-03.png)

## Technologies

- ASP NET MVC 5
- SQL Server 2012
- EntityFramework 6.1.3
- NET 4.5

## Requirements

- [Virtual CloneDrive](https://www.elby.ch/en/products/vcd.html)
ISO file mount is a built in functionality on Windows since 8. But this application still relies in the free virtual clone drive utility to control that through command line.
