run:
	cd src/backend
	dotnet run --project CellularAutomatonAPI
	cd ../frontend
	python3 server.py
	xdg open http://127.0.0.1:5050/
