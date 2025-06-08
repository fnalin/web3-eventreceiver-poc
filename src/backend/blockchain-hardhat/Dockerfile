FROM node:20

WORKDIR /app

COPY . .

RUN npm install
RUN chmod +x scripts/auto-deploy.sh

CMD npx hardhat node &