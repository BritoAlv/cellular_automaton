#!/bin/bash

# Run the API in a new xterm window
xterm -e "cd ./src/backend && dotnet run --project CellularAutomatonAPI; exec bash" &

# Run the GUI server in another new xterm window
xterm -e "cd ./src/frontend && python3 server.py exec bash" &

# Wait a few seconds to ensure the servers are up
sleep 20

# Open the application in the default web browser
xdg-open http://127.0.0.1:5050/
