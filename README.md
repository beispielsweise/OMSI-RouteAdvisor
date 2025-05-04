# OMSI RouteAdvisor
A custom navigator app for OMSI 2 Patch 2.3.004 (latest as of 28.04.2025)
No more updates.

## Features
- Open any OMSI 2 map and display it with Bus stops on it!
- Bus position tracking
- Updating the next stop on the route

Currently working on maps: 
- Grundorf
- Berlin-Spandau
- Hafencity 
- Should work on any map, that has a generatable roadmap and does not have Latitude value. Latutide can still work, but is inconsistent

## Installation
1. Download the latest release
2. Unpack .rar file, run the .exe and enjoy!

## Usage
- IMPORTANT: Before loading a map for the FIRST time, go into OMSI Editor >> On the right side of the editor window click the "Tiles" submenu >> Click "Create Roadmap" at the end of the list.
After that the app will pick up the map automatically
- Check if the map has Latitude and Longitude listed in the red debug line (first top line). If it does, memorize the latitude value WITH A FLOATING POINT! Or at least the hours value.
As of now, you need to unfortunately enter the latitude every time, sorry
- Choose the folder of the map you want to play. Directory example: YourOMSIInstallation/maps/MapName
- If your map had latitude value, you will be prompted to enter it
- Now you are in the navigator itself! Zoom the map in and out to suit your needs
- Resize the app as you want and move it around freely. Once you have the position sorted out, tick the Fixed checkbox and enjoy your navigator always on top of OMSI 2
- Finally, tick the Inject checkbox to start tracking your bus. IMPORTANT: It is advised to load into the map fully first!
- When you enter the Bus line you want to drive in-game, the red arrow will help you check the direction you need to drive in, if the next stop is off screen. The next bus stop will change its color to red

## WARNING
Maps, that are not allowing for Roadmap to be generated (Insufficient RAM error) ARE NOT SUPPORTED. Here's a list of maps, which currently don't work.
- GLADBECK. If you try to load it, BEWARE that the map will take up 3.6 GB of your RAM. Do this at your own risk.
- ...
You can try running any map without whole.roadmap generated. The navigator will try to get pieces of map and put them together. It should work, if they were generated correctly, however do this at your own risk. In this case, I assume it should work, as I have tested it. But beware of high RAM usage.

## Technologies Used
- C# WPF

## License
MIT License
