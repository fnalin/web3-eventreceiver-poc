const hre = require("hardhat");

async function main() {
    const [deployer] = await hre.ethers.getSigners();
    console.log("Deploying with:", deployer.address);

    const EventNFT = await hre.ethers.getContractFactory("EventNFT");
    const contract = await EventNFT.deploy(deployer.address);

    await contract.waitForDeployment(); // <- aqui está a correção

    console.log("EventNFT deployed at:", await contract.getAddress());
}

main().catch((error) => {
    console.error(error);
    process.exit(1);
});