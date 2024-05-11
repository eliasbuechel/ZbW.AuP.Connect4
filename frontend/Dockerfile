# Use Node.js LTS version for building Vue.js app
FROM node:lts as builder

# Set working directory
WORKDIR /app

# Copy package.json and package-lock.json
COPY package*.json ./

# Install dependencies
RUN npm install

# Copy the rest of the application code
COPY . .

# Build the Vue.js application for production
RUN npm run build

# Use a lightweight nginx image for serving the built Vue.js application
FROM nginx:stable-alpine

# Copy built Vue.js application to nginx directory
COPY --from=builder /app/dist /usr/share/nginx/html

# Copy nginx configuration to handle subdirectory routing
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Expose port 8080
EXPOSE 8080

# Start nginx server
CMD ["nginx", "-g", "daemon off;"]