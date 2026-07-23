cd /srv/project/

docker exec -it port-nginx nginx -t

docker exec -it port-nginx nginx -s reload
