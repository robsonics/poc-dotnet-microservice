#!/bin/bash

function print_msg (){
	echo -e "[#] $1"
}

print_msg "Setting up Satellite v2 microservice"
cd Satellite.v2
dotnet restore && dotnet build &&  dotnet publish -o published && docker build -t asp_satellitev2 . && docker run -d -p 82:80 asp_satellitev2 && print_msg "Satellite v2 microservice started ${GREEN}OK${NC}"


