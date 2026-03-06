FROM node:24-alpine AS build-env
WORKDIR /app
COPY package.json package-lock.json ./
RUN npm install
COPY . .
RUN npm run build

FROM nginx:1.29-alpine
COPY /nginx-custom.conf /etc/nginx/conf.d/
COPY --from=build-env /app/dist/app/ /usr/share/nginx/html
