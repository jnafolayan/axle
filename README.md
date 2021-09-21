# Axle
A search engine written in C#.

![Axle Homepage](https://user-images.githubusercontent.com/44870530/134175683-e64e5fab-befa-450a-a86e-23e221f7aecd.png)

## How to run
### Prerequisites
- [.NET](https://dotnet.microsoft.com/download)
- [Docker](https://docs.docker.com/get-docker/)

### On Linux or Mac
```bash
# allow the script's execution 
chmod +x ./scripts.sh
# build the server
./scripts.sh build-server
# build the docker images
./scripts.sh build-docker-dev
# spin up the containers
./scripts.sh run-dev
```

### On Windows
```cmd
# build the server
dotnet publish -c Release
# build the docker images
docker-compose -f docker-compose.yml build
# spin up the containers
docker-compose -f docker-compose.yml up
```

The frontend can viewed at [http://localhost:3000](http://localhost:3000). To upload documents, visit [http://localhost:3000/admin](http://localhost:3000/admin). 

Indexing is scheduled to run every 30 minutes but can be triggered manually. To trigger it manually, visit [http://localhost:5000/indexdocument](http://localhost:5000/indexdocument).

## Contributors
- [jnafolayan](https://github.com/jnafolayan)
- [iammadab](https://github.com/iammadab)
- [theTrueBoolean](https://github.com/theTrueBoolean)
- [Praise25](https://github.com/Praise25)
