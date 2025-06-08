# Web3 Event Receiver PoC

[![.NET 8](https://img.shields.io/badge/.NET-8.0-blue)](https://dotnet.microsoft.com/)
[![Hardhat](https://img.shields.io/badge/Hardhat-Ethereum-yellow)](https://hardhat.org)
[![RabbitMQ](https://img.shields.io/badge/RabbitMQ-Message--Queue-orange)](https://www.rabbitmq.com)
[![MongoDB](https://img.shields.io/badge/MongoDB-Database-green)](https://www.mongodb.com)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

> Prova de conceito para integração entre **IoT (Digital Twin)**, **Blockchain (via Hardhat)** e **.NET**, utilizando **RabbitMQ** para comunicação e **MongoDB** para persistência.

---

## 🎯 Objetivo

Simular uma arquitetura distribuída onde eventos gerados por dispositivos (Digital Twins) são:

- Tokenizados em **blockchain**.
- Publicados em uma **fila RabbitMQ**.
- Persistidos em um **banco MongoDB**.
- Notificados para consumidores interessados via **AppProvider**.

---

## 🧱 Estrutura do Projeto
web3-eventreceiver-poc/
├── contracts/              # Projeto Hardhat com contrato e listener
├── src/
│   └── backend/
│       ├── EventReceiver/  # API .NET 8 que persiste eventos da fila
│       ├── AppProvider/    # API que recebe notificação de acesso concedido
│       └── _infra/         # Docker Compose com MongoDB, RabbitMQ, etc.
└── README.md

---

## 📦 Tecnologias

- ✅ [.NET 8](https://dotnet.microsoft.com/en-us/)
- ✅ [Hardhat (Solidity)](https://hardhat.org/)
- ✅ [RabbitMQ](https://www.rabbitmq.com/)
- ✅ [MongoDB](https://www.mongodb.com/)
- ✅ [Docker + Compose](https://docs.docker.com/compose/)

---

## 🔁 Fluxo da Aplicação

> ![Arquitetura](docs/arquitetura.png)

1. 🛰 **Digital Twin** envia evento para a blockchain.
2. ⚙️ **Contrato inteligente** emite um evento (`EventCreated`).
3. 🎧 **Listener (Node.js)** escuta o evento e publica na **RabbitMQ**.
4. 📥 **EventReceiver (.NET)** consome da fila e grava no **MongoDB**.
5. 📤 **AppProvider** é notificado e permite o acesso ao conteúdo/token.

---

## 🚀 Como Rodar Localmente

### 1. Clone o repositório

```bash
git clone https://github.com/fnalin/web3-eventreceiver-poc.git
cd web3-eventreceiver-poc
cd src/backend/_infra
docker-compose up --build

![Arquitetura](docs/arquitetura.png)

🧪 Testes (em construção)
	•	Testar recebimento e persistência dos eventos
	•	Simular aquisição por wallet
	•	Mock de eventos para fluxo fim-a-fim
	•	Automatizar integração com front-end

📄 Licençaweb3-events
Este projeto está licenciado sob a licença MIT.