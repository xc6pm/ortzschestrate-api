import { HardhatUserConfig } from "hardhat/config"
import "hardhat-deploy"

const alchmeyApiKey = process.env.ALCHEMY_API_KEY
const deployerPrivateKey = process.env.DEPLOYER_PRIVATE_KEY || ""

const config: HardhatUserConfig = {
  defaultNetwork: "localhost",
  networks: {
    hardhat: {},
    sepolia: {
      url: `https://eth-sepolia.g.alchemy.com/v2/${alchmeyApiKey}`,
      accounts: [deployerPrivateKey],
    },
  },
  solidity: {
    version: "0.8.28",
    settings: {
      optimizer: {
        enabled: true,
        runs: 200,
      },
    },
  },
  paths: {
    sources: "./contracts",
    tests: "./test",
    cache: "./cache",
    artifacts: "./artifacts",
  },
  mocha: {
    timeout: 40000,
  },
}

export default config
