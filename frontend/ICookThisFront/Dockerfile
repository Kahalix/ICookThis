# 1) Etap budowania statycznych plików Vite
FROM node:18-alpine AS builder

WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run build

# 2) Etap serwowania przez nginx
FROM nginx:alpine

# usuń domyślne configi
RUN rm /etc/nginx/conf.d/default.conf

# skopiuj własny prosty config
COPY nginx.conf /etc/nginx/conf.d/

# skopiuj zbudowane pliki
COPY --from=builder /app/dist /usr/share/nginx/html

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]