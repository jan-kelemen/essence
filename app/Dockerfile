FROM node:20-alpine AS build-env
WORKDIR /app
COPY package.json package-lock.json ./
RUN npm install
COPY . .
RUN npm run build

FROM nginx:1.24-alpine
COPY /nginx-custom.conf /etc/nginx/conf.d/
COPY --from=build-env /app/dist/app/ /usr/share/nginx/html
