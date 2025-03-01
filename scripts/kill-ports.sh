#!/bin/bash
# Kill all processes listening on the specified ports

ports=(3000 3030 3031 5000 5050 5051 7000 7070 7071)

echo "Killing processes listening on ports: ${ports[@]}"

for port in "${ports[@]}";
do
    lsof -i :$port | grep LISTEN | awk '{print $2}' | xargs kill -9
done

# Run one more time just in case
for port in "${ports[@]}";
do
    lsof -i :$port | grep LISTEN | awk '{print $2}' | xargs kill -9
done
