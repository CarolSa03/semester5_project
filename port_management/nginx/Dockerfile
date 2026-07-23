FROM nginx:1.25-alpine

# Remove default nginx config
RUN rm -f /etc/nginx/conf.d/default.conf

# Copy custom config(s)
COPY default.conf /etc/nginx/conf.d/default.conf

# Create directories for the mounted volumes
RUN mkdir -p /public \
    && mkdir -p /etc/nginx/allowed_ips \
    && mkdir -p /var/log/nginx

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
