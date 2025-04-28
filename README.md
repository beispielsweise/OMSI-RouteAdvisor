# OMSI RouteAdvisor
A custom navigator app for OMSI 2 Patch 2.3.004 (latest as of 28.04.2025)

## Features
- Open any OMSI 2 map and display it with Bus stops on it!
- Bus position tracking
- Updating next stop of the route 

## Installation
1. Download the latest release
2. Unpack .rar file, run the .exe and enjoy!

## Usage
- IMPORTANT: Before loading a map for the FIRST time, go into OMSI Editor >> On the right side of the editor window click the "Tiles" submenu >> Click "Create Roadmap" at the end of the list.
This step needs to be repeated only once
- Check if the map has Latitude and Longitude listed in the red debug line (first top line). If it does, memorize the latitude value WITH A FLOATING POINT! Or at least the hours value.
As of now, you need to unfortunately enter the latitude every time, sorry
- Choose the folder of the map you want to play. Directory example: YourOMSIInstallation/maps/MapName
- If your map had latitude value, you will be prompted to enter it
- Now you are in the navigator itself! Zoom the map in and out to suit your needs
- Resize the app as you want and move it around freely. Once you have the position sorted out, tick the Fixed checkbox and enjoy your navigator always on top of OMSI 2
- Finally, tick the Inject checkbox to start tracking your bus. IMPORTANT: It is advised to load into the map fully first! 

## Technologies Used
- C# WPF

## Future plans:
- Showing direction of the route
- Pleasant ui
- WIP

## License
MIT License
