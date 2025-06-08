// SPDX-License-Identifier: MIT
pragma solidity ^0.8.20;

import "@openzeppelin/contracts/token/ERC721/extensions/ERC721URIStorage.sol";
import "@openzeppelin/contracts/access/Ownable.sol";

contract EventNFT is ERC721URIStorage, Ownable {
    uint256 private _tokenIdCounter;
    mapping(uint256 => address) public originalCreators;
    mapping(bytes32 => bool) public registeredEvents;
    mapping(uint256 => mapping(address => bool)) public allowed;

    // 🔔 Eventos adicionados para rastreamento
    event RegisteredNFT(uint256 indexed tokenId, string uri);
    event GrantedAccessNFT(uint256 indexed tokenId, address indexed wallet);

    constructor(address initialOwner) ERC721("EventNFT", "EVT") Ownable(initialOwner) {}

    function mintFor(
        address creator,
        string memory tokenURI,
        bytes32 eventHash
    ) external onlyOwner returns (uint256) {
        require(creator != address(0), "Invalid creator address");
        require(!registeredEvents[eventHash], "Event already tokenized");

        uint256 tokenId = ++_tokenIdCounter;

        _safeMint(creator, tokenId);
        _setTokenURI(tokenId, tokenURI);
        originalCreators[tokenId] = creator;
        registeredEvents[eventHash] = true;

        // ✅ Garante acesso ao dono
        allowed[tokenId][creator] = true;

        // 🔔 Emissão de eventos adicionados
        emit RegisteredNFT(tokenId, tokenURI);
        emit GrantedAccessNFT(tokenId, creator);

        return tokenId;
    }

    // Versão anterior permitia apenas o owner do token, que é a carteira do
    // Digital Twin, conceder acesso --> Verificar com o JP sobre isso
    // Substituído para permitir também o owner do contrato (backend), enquanto não confirmo
    // function grantAccess(uint256 tokenId, address user) public {
    //     require(msg.sender == ownerOf(tokenId), "Only owner can grant access");
    //     allowed[tokenId][user] = true;
    // }

    function grantAccess(uint256 tokenId, address user) public {
        require(
            msg.sender == ownerOf(tokenId) || msg.sender == owner(),
            "Only owner or token holder can grant access"
        );
        allowed[tokenId][user] = true;

        // 🔔 Emissão do evento de acesso concedido
        emit GrantedAccessNFT(tokenId, user);
    }

    function hasAccess(uint256 tokenId, address user) public view returns (bool) {
        return allowed[tokenId][user] || user == ownerOf(tokenId);
    }

    function totalMinted() external view returns (uint256) {
        return _tokenIdCounter;
    }

    function isEventRegistered(bytes32 eventHash) external view returns (bool) {
        return registeredEvents[eventHash];
    }

    function isTokenized(bytes32 eventHash) public view returns (bool) {
        return registeredEvents[eventHash];
    }
}