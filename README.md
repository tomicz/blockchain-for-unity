# Blockchain for Unity

EVM blockchain SDK for Unity Game Developers. Currently supports WebGL builds with MetaMask wallet integration. Multiple platform support and additional wallet integrations are coming in future updates.

## 🚀 Quick Start

```csharp
// Add BlockchainManager to a GameObject
var manager = gameObject.AddComponent<BlockchainManager>();

// Connect to MetaMask
manager.ConnectWallet();

// Subscribe to events
manager.OnWalletConnected += (result) => Debug.Log($"Connected: {result.address}");
manager.OnBalanceReceived += (result) => Debug.Log($"Balance: {result.formattedBalance} {result.currencySymbol}");
```

## ⚙️ Configuration

Configure your blockchain network through Unity Editor:

1. Go to **Tomicz Engineering > Chain Config**
2. Set your network name, chain ID, RPC URL, and currency symbol
3. Click **Update Config** to save

## ✨ Features

### Core Features

- **Ethereum/EVM Chain Integration**: Full support for Ethereum and EVM-compatible blockchains
- **MetaMask Wallet Integration**: Seamless connection with MetaMask browser extension
- **WebGL Support**: Optimized for Unity WebGL builds
- **Smart Contract Interaction**: Complete Nethereum integration for contract calls and transactions
- **Configuration Management**: Easy blockchain network configuration through Unity Editor

### Technical Capabilities

- **Account Management**: Wallet connection, account switching, and balance checking
- **Transaction Signing**: Support for standard transactions and EIP-712 typed data signing
- **Multi-Chain Support**: Ethereum, Optimism, Besu, Geth, and other EVM chains
- **HD Wallet Support**: Hierarchical deterministic wallet functionality
- **Sign-In with Ethereum (SIWE)**: Web3 authentication capabilities

## 📝 Documentation

- [📖 User Guide](docs/USER_GUIDE.md) - Step-by-step setup and usage
- [🔧 API Reference](docs/API_REFERENCE.md) - Complete API documentation
- [🏗️ Architecture](docs/ARCHITECTURE.md) - Technical architecture overview
- [🧪 Examples](docs/EXAMPLES.md) - Code examples and use cases
- [🔍 Troubleshooting](docs/TROUBLESHOOTING.md) - Common issues and solutions

## 🛠️ Installation

1. Import the plugin into your Unity project
2. Configure your network in **Tomicz Engineering > Chain Config**
3. Add `BlockchainManager` to a GameObject
4. Build for WebGL platform

## 🤝 Contributing

We welcome contributions! Please see our [Contributing Guide](docs/CONTRIBUTING.md) for details.

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENCE.md) file for details.

## 📞 Support

- [GitHub Issues](https://github.com/tomicz/blockchain-for-unity/issues)
- [Documentation](docs/)
- [Discord Community](https://discord.gg/your-community)

---

**Built with ❤️ by [Tomicz Engineering](https://tomiczengineering.com)**
