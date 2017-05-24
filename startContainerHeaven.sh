#!/bin/bash

RED='\033[0;31m'
GREEN='\033[0;32m'
NC='\033[0m' 

ASP_PORT=5000
ASP_VERSION=0
ASP_PORT=80
ASP_CNT=0;

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

# Add consul key value
# $1 Key
# $2 Value
function add_key_value(){
	REQ="curl -s  --request PUT --data "$1" $NODE1_IP:8500/v1/kv/$2 "

	print_msg "Adding KV $1=$2"
	#print_msg "executing on node2 $REQ"
	docker exec node2 $REQ
}

function increase_asp_version(){
	((ASP_VERSION=ASP_VERSION+1))
	((ASP_PORT=ASP_PORT+1))
	add_key_value "v$ASP_VERSION_inputqueue" "v$ASP_VERSION_inputqueue"
	add_key_value "v$ASP_VERSION_outputqueue" "v$ASP_VERSION_outputqueue"
	print_msg "Current asp services version: v$ASP_VERSION on port $ASP_PORT"
}
function build_asp(){
	dotnet restore && dotnet publish -o publish && print_msg "WebApplication1 build: ${GREEN}OK${NC}" && docker build -t asp-test3 . && print_msg "Docker image asp-test3 build: ${GREEN}OK${NC}"
}

function publish_asp(){	
	((ASP_CNT=ASP_CNT+1))
	print_msg "docker run  -d -p $ASP_PORT:$ASP_PORT -h asp-test-cnt$ASP_CNT  --name asp-test-cnt$ASP_CNT asp-test3 \"version=v$ASP_VERSION\" \"port=$ASP_PORT\""
	docker run  -d -p $ASP_PORT:$ASP_PORT -h asp-test-cnt$ASP_CNT  --name asp-test-cnt$ASP_CNT asp-test3 "version=v$ASP_VERSION" "port=$ASP_PORT" && print_msg "asp-test3 version v$ASP_VERSION in container asp-test-cnt$ASP_CNT deploy on port $ASP_PORT: ${GREEN}OK${NC}"
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

# parametrize number of server node
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

build_asp

increase_asp_version
publish_asp






