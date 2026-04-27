# ─── Stage 1: Build ───────────────────────────────────────────────────────────
FROM node:20-alpine AS build

WORKDIR /app

# Kopiraj dependency fajlove i instaliraj
COPY package*.json ./
RUN npm install

# Kopiraj ostatak koda i buildi
COPY . .
RUN npx vite build

# ─── Stage 2: Serve ───────────────────────────────────────────────────────────
FROM nginx:alpine

# Kopiraj build output u Nginx
COPY --from=build /app/dist /usr/share/nginx/html

# Kopiraj našu Nginx konfiguraciju
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]