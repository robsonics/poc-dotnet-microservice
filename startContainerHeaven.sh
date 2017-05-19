#!/bin/bash

RED='\033[0;31m'
GREEN='\033[0;32m'
NC='\033[0m' 

ASP_PORT=5000
ASP_VERSION=1

# $1 message to print
function print_msg (){
	echo -e "[#] $1"
}

# $1 container ID/name
function check_pull_image (){
	if docker images | grep $1 | wc -c -gt 1; then
		echo -e "[#] $1 image ${GREEN}OK${NC}"
	else
		echo "[#] Pulling $1 image"
		docker pull $1 && print_msg "$1 pulled: ${GREEN}OK${NC}"
	fi
}

# Return container IP
# $1 container ID/name
function get_container_ip(){
	docker inspect -f '{{range .NetworkSettings.Networks}}{{.IPAddress}}{{end}}' $1
}

function publish_asp(){

}




print_msg "Welcome in docker heaven"
print_msg ""
print_msg ""
# docker stop all containers
print_msg "Stopping all containers"
docker stop $(docker ps -a -q) 

# docker remove all containers
print_msg "Removing all containers"
docker rm $(docker ps -a -q) 

print_msg "Checking for consul image progrium/consul"
check_pull_image progrium/consul

print_msg "Starting node1 - server leader"
docker run -d --name node1 -h node1 progrium/consul -server -bootstrap-expect 3 && print_msg "node1 started ${GREEN}OK ${NC}"

JOIN_IP="$(docker inspect -f '{{.NetworkSettings.IPAddress}}' node1)"
print_msg "node1 start with IP: " $JOIN_IP

print_msg "Starting node2"
docker run -d --name node2 -h node2 progrium/consul -server -join $JOIN_IP && print_msg "node2 started ${GREEN}OK${NC}"
print_msg "Starting node3"
docker run -d --name node3 -h node3 progrium/consul -server -join $JOIN_IP && print_msg "node3 started ${GREEN}OK${NC}"

# verify  members in consul 1
print_msg "Listing node1 members"
docker exec node1 consul members

print_msg "Checking for rabbitmq image"
check_pull_image rabbitmq


print_msg "Starting rabbitmq container"
docker run -d --hostname rabbitmq --name rabbitmq rabbitmq:3 && print_msg "rabbitmq node started ${GREEN}OK${NC}"

if [ -e consul ]; then
	echo -e "[#] consul file exist in current directory: ${GREEN}OK${NC}"
else
	echo -e "[#] consul file exist in current directory: ${RED}FAIL${NC}"
fi

NODE1_IP="$(get_container_ip node1)"
RABBITMQ_IP="$(docker inspect -f '{{range .NetworkSettings.Networks}}{{.IPAddress}}{{end}}' rabbitmq)"

print_msg "RabbitMq IP: $RABBITMQ_IP"

REQ="curl -s  --request PUT --data "$RABBITMQ_IP" $NODE1_IP:8500/v1/kv/rabbitmqip "

print_msg "Adding KV rabbitmqip"
#print_msg "executing on node2 $REQ"
docker exec node2 $REQ

docker exec node2 curl -s $NODE1_IP:8500/v1/kv/rabbitmqip?raw

print_msg "Setting up client"
cd WebApplication1
dotnet restore && dotnet publish -o publish && print_msg "WebApplication1 build: ${GREEN}OK${NC}" && docker build -t asp-test3 . && print_msg "Docker image asp-test3 build: ${GREEN}OK${NC}"
docker run  -d -p 80:80 -h asp-test-cnt --name asp-test-cnt asp-test3 && print_msg "asp-test3 in container asp-test-cnt deploy: ${GREEN}OK${NC}"






