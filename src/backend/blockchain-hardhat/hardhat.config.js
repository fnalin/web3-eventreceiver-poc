require("@nomicfoundation/hardhat-toolbox");

/** @type import('hardhat/config').HardhatUserConfig */
module.exports = {
  solidity: "0.8.24", // ou a versão do seu contrato
  networks: {
    localhost: {
      url: "http://127.0.0.1:8545",
    },
  },
};