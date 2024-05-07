FROM node:latest

WORKDIR /app

COPY package*.json ./

RUN npm install

# Copy of Directory in Directory
COPY . .

RUN npm run build

EXPOSE 8080

CMD ["npm", "run", "serve"]
