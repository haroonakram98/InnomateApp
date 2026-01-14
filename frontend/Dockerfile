# =========================
# BUILD STAGE
# =========================
FROM node:20-alpine AS build
WORKDIR /app

# Copy package files and install dependencies
COPY package*.json ./
RUN npm install

# Copy source and build
COPY . .
ARG VITE_API_URL
RUN echo "VITE_API_URL=${VITE_API_URL}" > .env
RUN npm run build

# =========================
# RUNTIME STAGE
# =========================
FROM nginx:stable-alpine
WORKDIR /usr/share/nginx/html

# Copy build output to Nginx default folder
COPY --from=build /app/dist .

# Copy custom nginx config
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
