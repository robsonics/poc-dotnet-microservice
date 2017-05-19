#!/bin/bash

if [ -e consul ]; then
	echo "Consul has been found"
else
	echo "Exception: Can't find consul"
	return 123;
fi

chmod +x consul

mkdir /tmp/consul

./consul agent -data-dir=/tmp/consul -config-file=consulDef.json

dotnet WebApplication1.dll