#!/bin/bash

echo "⌛ Aguardando Hardhat iniciar..."
sleep 5

echo "🚀 Fazendo deploy do contrato EventNFT..."
npx hardhat run scripts/deploy.js --network localhost

echo "✅ Deploy concluído."